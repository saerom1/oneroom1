using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferManager : MonoBehaviour
{
    // 다음 씬에서 플레이어를 스폰할 위치 이름
    private string locationName;

    SplashManager theSplash;
    InteractionController theIC;
    public static bool isFinished = true;

    void Start()
    {
        theSplash = FindObjectOfType<SplashManager>();
        theIC = FindObjectOfType<InteractionController>();
    }

    public void SaveObjectsState()
    {
        GameObject[] interactionObjects = GameObject.FindGameObjectsWithTag("Interaction");
        foreach (GameObject obj in interactionObjects)
        {
            // “자동 이벤트” 라벨이 붙은 오브젝트면 저장하지 않고 건너뛴다
            var ie = obj.GetComponent<InteractionEvent>();
            if (ie != null && ie.isAutoEvent)
                continue;

            bool state = obj.activeSelf;
            GameStateManager.instance.SetObjectActiveState(obj.name, state);
            Debug.Log($"[SaveObjectsState] {obj.name}.activeSelf = {state}");
        }
    }

    public IEnumerator Transfer(string sceneName, string locName)
    {
        Debug.Log($"[Transfer] 시작 frame={Time.frameCount}");

        // 1) 상태 저장
        SaveObjectsState();

        // 2) OnDisable 저장 방지 플래그
        GameStateManager.instance.isSceneChanging = true;

        // 3) UI 숨기기
        isFinished = false;
        theIC.SettingUI(false);

        // 4) 페이드 아웃
        SplashManager.isfinished = false;
        yield return StartCoroutine(theSplash.FadeOut(false, true));

        // 5) 기록: 플레이어 스폰 + 자동 이벤트 실행
        locationName = locName;
        TransferSpawnManager.spawnTiming = true;
        TransferSpawnManager.autoEventTiming = true;

        // 6) 씬 로드
        SceneManager.LoadScene(sceneName);
    }

    public IEnumerator Done()
    {
        SplashManager.isfinished = false;
        StartCoroutine(theSplash.FadeIn(false, true));
        yield return new WaitUntil(() => SplashManager.isfinished);

        isFinished = true;
        yield return new WaitForSeconds(0.3f);

        if (!DialogueManager.isWating)
            theIC.SettingUI(true);
    }

    public string GetLocationName() => locationName;
}
