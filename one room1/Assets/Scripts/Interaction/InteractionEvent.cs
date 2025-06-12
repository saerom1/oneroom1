using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(DialogueEvent))]
public class InteractionEvent : MonoBehaviour
{
    [Header("== 자동 이벤트 여부 ==")]
    public bool isAutoEvent = false;

    [Header("== 대사 이벤트 배열 ==")]
    [SerializeField] private DialogueEvent[] dialogueEvent;

    // 현재 검사 중인 이벤트 인덱스
    private int currentCount;

    // 한 씬에서 자동 이벤트가 이미 실행되었는지 여부 (중복 실행 방지)
    private bool _autoExecutedThisScene = false;

    void Awake()
    {
        // 1) 이전에 저장된 활성 상태 복원 (자동 이벤트면 무조건 false)
        bool saved = GameStateManager.instance?.GetObjectActiveState(gameObject.name) ?? true;
        Debug.Log($"[InteractionEvent:Awake] {gameObject.name} savedState={saved}");

        // 2) 등장/퇴장 조건 검사
        bool allowed = CheckEvent();

        // → 자동 이벤트라면, 이미 실행된 건 다시 허용하지 않음
        if (allowed && isAutoEvent)
        {
            int evtID = dialogueEvent[currentCount].eventTiming.eventNum;
            if (GameStateManager.instance.eventExecuted.TryGetValue(evtID, out bool done) && done)
            {
                allowed = false;
                Debug.Log($"[InteractionEvent:Awake] auto 이벤트 {evtID} 이미 실행됨 → 비활성화");
            }
        }

        // 3) 최종 활성화 여부 결정
        gameObject.SetActive(saved && allowed);

        // 4) 자동 이벤트라면 씬 로드 콜백 구독
        if (isAutoEvent)
            SceneManager.sceneLoaded += OnSceneLoaded_Auto;
    }

    void OnDisable()
    {
        if (isAutoEvent)
            SceneManager.sceneLoaded -= OnSceneLoaded_Auto;
    }

    void OnDestroy()
    {
        if (isAutoEvent)
            SceneManager.sceneLoaded -= OnSceneLoaded_Auto;
    }

    /// <summary>
    /// 씬 로드 직후 한 번만 자동 이벤트 트리거.
    /// 실제 실행은 코루틴에서 DB·페이드 완료 대기 후 수행.
    /// </summary>
    private void OnSceneLoaded_Auto(Scene scene, LoadSceneMode mode)
    {
        // (A) 이미 실행됐거나 자동 이벤트가 아니면 무시
        if (!isAutoEvent || _autoExecutedThisScene)
            return;

        // (B) TransferManager.Transfer()에서 넘어온 타이밍이 아닐 경우 무시
        if (!TransferSpawnManager.autoEventTiming)
            return;

        // (C) 안전하게 코루틴 시작
        StartCoroutine(AutoEventAfterLoad());
    }

    private IEnumerator AutoEventAfterLoad()
    {
        // 1) DB 준비와 페이드/이동 완료 대기
        yield return new WaitUntil(() =>
            DatabaseManager.isFinish && TransferManager.isFinished);

        // 2) 등장/퇴장 조건 재검사
        if (!CheckEvent())
            yield break;

        // 3) 중복 실행 방지 및 다음 씬 전까지 재트리거 금지
        _autoExecutedThisScene = true;
        TransferSpawnManager.autoEventTiming = false;

        // 4) 실제 자동 이벤트 실행
        TriggerAutoEvent();
    }

    /// <summary>
    /// 대화 종료 직후 즉시 자동 이벤트를 체크하고 실행합니다.
    /// </summary>
    public void TryTriggerAutoOnDialogueEnd()
    {
        if (!isAutoEvent || _autoExecutedThisScene)
            return;

        // 씬 로드시와 달리 TransferTiming 체크 제외하고 즉시 실행
        if (!CheckEvent())
            return;

        _autoExecutedThisScene = true;
        TriggerAutoEvent();
    }

    /// <summary>
    /// 자동 이벤트 실행 로직을 공통으로 처리합니다.
    /// </summary>
    private void TriggerAutoEvent()
    {
        var dm = FindObjectOfType<DialogueManager>();
        DialogueManager.isWating = true;

        // 등장/퇴장 세팅
        if (GetAppearType() == AppearType.Appear)
            dm.SetAppearObjects(GetTargets());
        else
            dm.SetDisappearObjects(GetTargets());

        dm.SetNextEvent(GetNextEvent());

        int evtID = dialogueEvent[currentCount].eventTiming.eventNum;
        Debug.Log($"[InteractionEvent:TriggerAutoEvent] 자동 이벤트 {evtID} 실행");

        dm.ShowDialogue(GetDialogue());
        GameStateManager.instance.SetEventExecuted(evtID, true);

        // 실행 직후 비활성화 (콜백 해제)
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 대사 이벤트 배열을 돌며 등장／퇴장 조건 검사.
    /// 조건 만족 시 currentCount에 인덱스 저장 후 true 반환.
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

            // (1) 자동 이벤트 이미 실행된 플래그 있으면 스킵dd
            if (isAutoEvent
                && t.eventNum >= 0 && t.eventNum < DatabaseManager.instance.eventFlags.Length
                && DatabaseManager.instance.eventFlags[t.eventNum])
            {
                ok = false;
            }
            else
            {
                // (2) 등장 조건 검사
                if (t.eventConditions != null)
                {
                    foreach (int cond in t.eventConditions)
                    {
                        if (cond < 0 || cond >= DatabaseManager.instance.eventFlags.Length
                            || DatabaseManager.instance.eventFlags[cond] != t.conditionFlag)
                        {
                            ok = false;
                            break;
                        }
                    }
                }
                // (3) 퇴장(종료) 조건 검사
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
