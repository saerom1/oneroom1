using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(DialogueEvent))]
public class InteractionEvent : MonoBehaviour
{
    // --------------------------------------------------
    // 1) 모든 InteractionEvent 인스턴스를 한 곳에서 관리하기 위한 static 리스트
    // --------------------------------------------------
    public static List<InteractionEvent> allEvents { get; private set; } = new List<InteractionEvent>();

    [Header("== 자동 이벤트 여부 ==")]
    public bool isAutoEvent = false;

    [Header("== 대사 이벤트 배열 ==")]
    [SerializeField] private DialogueEvent[] dialogueEvent;

    // 현재 검사 중인 이벤트 인덱스
    private int currentCount;

    // 한 씬에서 자동 이벤트가 이미 실행되었는지 여부 (중복 실행 방지)
    private bool _autoExecutedThisScene = false;

    // --------------------------------------------------
    // Awake: 모든 인스턴스를 allEvents에 등록, 기존 Awake 로직 수행
    // --------------------------------------------------
    void Awake()
    {
        // A) static 리스트에 자신 등록 (중복 방지)
        if (!allEvents.Contains(this))
            allEvents.Add(this);

        // B) 이전에 저장된 활성 상태 복원 (자동 이벤트면 무조건 false)
        bool saved = GameStateManager.instance?.GetObjectActiveState(gameObject.name) ?? true;
        Debug.Log($"[InteractionEvent:Awake] {gameObject.name} savedState={saved}");

        // C) 등장/퇴장 조건 검사
        bool allowed = CheckEvent();

        // D) 자동 이벤트라면 이미 실행된 건 다시 허용하지 않음
        if (allowed && isAutoEvent)
        {
            int evtID = dialogueEvent[currentCount].eventTiming.eventNum;
            if (GameStateManager.instance.eventExecuted.TryGetValue(evtID, out bool done) && done)
            {
                allowed = false;
                Debug.Log($"[InteractionEvent:Awake] auto 이벤트 {evtID} 이미 실행됨 → 비활성화");
            }
        }

        // E) 최종 활성화 여부 결정
        gameObject.SetActive(saved && allowed);

        // F) 자동 이벤트라면 씬 로드 콜백 구독
        if (isAutoEvent)
            SceneManager.sceneLoaded += OnSceneLoaded_Auto;
    }

    // --------------------------------------------------
    // OnDisable/OnDestroy: 이벤트 제거 및 콜백 해제
    // --------------------------------------------------
    void OnDisable()
    {
        // allEvents.Remove(this);
        if (isAutoEvent)
            SceneManager.sceneLoaded -= OnSceneLoaded_Auto;
    }

    void OnDestroy()
    {
        allEvents.Remove(this);

        if (isAutoEvent)
            SceneManager.sceneLoaded -= OnSceneLoaded_Auto;
    }

    // --------------------------------------------------
    // 도메인 리로드나 새 씬 로드 전 리스트 초기화
    // --------------------------------------------------
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitAllEvents() => allEvents.Clear();

    /// <summary>
    /// 씬 로드 직후 한 번만 자동 이벤트 트리거. 실제 실행은 코루틴에서 DB·페이드 완료 대기 후 수행.
    /// </summary>
    private void OnSceneLoaded_Auto(Scene scene, LoadSceneMode mode)
    {
        if (!isAutoEvent || _autoExecutedThisScene)
            return;  // 자동 이벤트 아니거나 이미 실행된 경우 무시

        if (!TransferSpawnManager.autoEventTiming)
            return;  // 씬 전환 타이밍이 아닐 때 무시

        StartCoroutine(AutoEventAfterLoad());  // 코루틴으로 안전하게 실행
    }

    private IEnumerator AutoEventAfterLoad()
    {
        // 1) DB 준비와 페이드/이동 완료 대기
        yield return new WaitUntil(() => DatabaseManager.isFinish && TransferManager.isFinished);

        // 2) 등장/퇴장 조건 재검사
        if (!CheckEvent())
            yield break;

        // 3) 중복 실행 방지 및 Transfer 타이밍 초기화
        _autoExecutedThisScene = true;
        TransferSpawnManager.autoEventTiming = false;

        // 4) 실제 자동 이벤트 실행
        TriggerAutoEvent();
    }

    /// <summary>
    /// 대화 종료 시 자동 이벤트 즉시 체크 및 실행
    /// </summary>
    public void TryTriggerAutoOnDialogueEnd()
    {
        // 진입 확인
        Debug.Log($"[TryTrigger] {gameObject.name} 진입, isAutoEvent={isAutoEvent}, _autoExecutedThisScene={_autoExecutedThisScene}");

        // (1) 자동 이벤트가 아니거나 이미 실행된 경우 바로 리턴
        if (!isAutoEvent || _autoExecutedThisScene)
            return;

        // (2) 조건 플래그 값 확인용 로그
        var timing = dialogueEvent[currentCount].eventTiming;
        if (timing.eventConditions != null && timing.eventConditions.Length > 0)
        {
            int condNum = timing.eventConditions[0];
            Debug.Log($"[TryTrigger] 검사할 플래그 eventFlags[{condNum}] = {DatabaseManager.instance.eventFlags[condNum]}");
        }

        // (3) 실제 조건 검사
        bool ok = CheckEvent();
        Debug.Log($"[TryTrigger] CheckEvent 결과: {ok}");
        if (!ok)
            return;

        // (4) 조건 만족 시 실행
        _autoExecutedThisScene = true;  // 중복 방지
        TriggerAutoEvent();
    }

    /// <summary>
    /// 자동 이벤트 실행 로직 (공통)
    /// </summary>
    private void TriggerAutoEvent()
    {
        var dm = FindObjectOfType<DialogueManager>();
        DialogueManager.isWating = true;

        // 1) 대화 배열 가져오기
        Dialogue[] dlg = GetDialogue();

        // 2) null 체크
        if (dlg == null || dlg.Length == 0)
        {
            Debug.LogWarning($"[TriggerAutoEvent] 이벤트 {GetEventNumber()} 대화가 없습니다. 스킵합니다.");
            return;
        }

        // 3) 등장/퇴장 세팅
        if (GetAppearType() == AppearType.Appear)
            dm.SetAppearObjects(GetTargets());
        else
            dm.SetDisappearObjects(GetTargets());

        dm.SetNextEvent(GetNextEvent());

        int evtID = dialogueEvent[currentCount].eventTiming.eventNum;
        Debug.Log($"[InteractionEvent:TriggerAutoEvent] 자동 이벤트 {evtID} 실행");

        // 4) 안전하게 대화 매니저에 넘기기
        dm.ShowDialogue(dlg);
        GameStateManager.instance.SetEventExecuted(evtID, true);

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 대사 이벤트 배열을 돌며 등장/퇴장 조건 검사
    /// </summary>
    private bool CheckEvent()
    {
        if (DatabaseManager.instance == null || DatabaseManager.instance.eventFlags == null)
        {
            Debug.LogWarning($"[InteractionEvent:CheckEvent] DB 미초기화, {gameObject.name} 건너뜀");
            return false;
        }
        if (dialogueEvent == null || dialogueEvent.Length == 0)
        {
            Debug.LogWarning($"[InteractionEvent:CheckEvent] dialogueEvent 비어있음: {gameObject.name}");
            return false;
        }

        for (int i = 0; i < dialogueEvent.Length; i++)
        {
            var evt = dialogueEvent[i];
            if (evt == null || evt.eventTiming == null) continue;
            var t = evt.eventTiming;
            bool ok = true;

            // 이미 실행된 플래그 있으면 스킵
            if (isAutoEvent
                && t.eventNum >= 0 && t.eventNum < DatabaseManager.instance.eventFlags.Length
                && DatabaseManager.instance.eventFlags[t.eventNum])
            {
                ok = false;
            }
            else
            {
                // 등장 조건 검사
                if (t.eventConditions != null)
                {
                    foreach (int cond in t.eventConditions)
                    {
                        if (cond < 0 || cond >= DatabaseManager.instance.eventFlags.Length
                            || DatabaseManager.instance.eventFlags[cond] != t.conditionFlag)
                        {
                            ok = false; break;
                        }
                    }
                }
                // 퇴장(종료) 조건 검사
                if (ok
                    && t.eventEndNum >= 0 && t.eventEndNum < DatabaseManager.instance.eventFlags.Length
                    && DatabaseManager.instance.eventFlags[t.eventEndNum])
                {
                    ok = false;
                }
            }

            if (ok)
            {
                currentCount = i;
                return true;
            }
        }
        return false;
    }

    // 이하 대사 반환 및 헬퍼 메서드들 (기존 로직 그대로)
    public Dialogue[] GetDialogue()
    {
        if (DatabaseManager.instance.eventFlags[dialogueEvent[currentCount].eventTiming.eventEndNum])
            return null;

        if (isAutoEvent)
            DatabaseManager.instance.eventFlags[dialogueEvent[currentCount].eventTiming.eventNum] = true;

        if (!DatabaseManager.instance.eventFlags[dialogueEvent[currentCount].eventTiming.eventNum]
            || dialogueEvent[currentCount].isSame)
        {
            DatabaseManager.instance.eventFlags[dialogueEvent[currentCount].eventTiming.eventNum] = true;
            dialogueEvent[currentCount].dialogues = SettingDialogue(
                dialogueEvent[currentCount].dialogues,
                (int)dialogueEvent[currentCount].line.x,
                (int)dialogueEvent[currentCount].line.y);
            return dialogueEvent[currentCount].dialogues;
        }
        else
        {
            dialogueEvent[currentCount].dialoguesB = SettingDialogue(
                dialogueEvent[currentCount].dialoguesB,
                (int)dialogueEvent[currentCount].lineB.x,
                (int)dialogueEvent[currentCount].lineB.y);
            return dialogueEvent[currentCount].dialoguesB;
        }
    }

    private Dialogue[] SettingDialogue(Dialogue[] source, int x, int y)
    {
        Dialogue[] copy = DatabaseManager.instance.GetDialogue(x, y);
        for (int i = 0; i < dialogueEvent[currentCount].dialogues.Length; i++)
        {
            copy[i].tf_Target = source[i].tf_Target;
            copy[i].cameraType = source[i].cameraType;
        }
        return copy;
    }

    public AppearType GetAppearType() => dialogueEvent[currentCount].appearType;
    public GameObject[] GetTargets() => dialogueEvent[currentCount].go_Targets;
    public GameObject GetNextEvent() => dialogueEvent[currentCount].go_NextEvent;
    public int GetEventNumber() { CheckEvent(); return dialogueEvent[currentCount].eventTiming.eventNum; }
}