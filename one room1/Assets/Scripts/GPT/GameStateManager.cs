using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;

    // 이벤트 실행 여부 저장 (필요한 경우)
    public Dictionary<int, bool> eventExecuted = new Dictionary<int, bool>();

    // 오브젝트 활성/비활성 상태 저장 (키: 오브젝트의 고유 ID, 값: 활성 상태)
    public Dictionary<string, bool> objectActiveState = new Dictionary<string, bool>();

    // 씬 전환 중 여부 플래그
    public bool isSceneChanging = false;



    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("[GameStateManager] 초기화됨. instance가 설정되었습니다.");
            DontDestroyOnLoad(gameObject);

            // 씬 전환 이벤트
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.sceneLoaded += OnSceneLoaded;

            Debug.Log("[GameStateManager] 초기화됨. instance가 설정되었습니다.");

        }
        else
        {
            Destroy(gameObject);
        }
    }


    void OnSceneUnloaded(Scene scene)
    {
        isSceneChanging = true;
        Debug.Log($"[GameStateManager] 씬 언로드 시작: frame={Time.frameCount}, isSceneChanging={isSceneChanging}");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isSceneChanging = false;
        Debug.Log($"[GameStateManager] 씬 로드 완료: frame={Time.frameCount}, isSceneChanging={isSceneChanging}");
        // ↓ 이 부분을 추가 ↓
        // "Interaction" 태그가 붙은 모든 오브젝트를 찾아,
        // 이전에 저장된 objectActiveState 에 따라 SetActive 시킵니다.
        var all = GameObject.FindGameObjectsWithTag("Interaction");
        foreach (var go in all)
        {
            if (objectActiveState.TryGetValue(go.name, out bool shouldBeActive))
            {
                go.SetActive(shouldBeActive);
                Debug.Log($"[GameStateManager] 복원: {go.name}.SetActive({shouldBeActive})");
            }
            // else: 저장 기록이 없으면 씬 기본값 그대로 놔둡니다.
        }
    }


    // 예시 메서드: 이벤트 실행 상태 업데이트 (추가 또는 갱신)
    public void SetEventExecuted(int eventID, bool executed)
    {
        if (!eventExecuted.ContainsKey(eventID))
        {
            eventExecuted.Add(eventID, executed);
            Debug.Log($"[GameStateManager] 이벤트 {eventID}가 처음 실행되었습니다. (값: {executed})");
        }
        else
        {
            eventExecuted[eventID] = executed;
            Debug.Log($"[GameStateManager] 이벤트 {eventID}의 상태가 업데이트되었습니다. (값: {executed})");
        }
    }

    public void SetObjectActiveState(string objectID, bool isActive)
    {
        if (!objectActiveState.ContainsKey(objectID))
        {
            objectActiveState.Add(objectID, isActive);
        }
        else
        {
            objectActiveState[objectID] = isActive;
        }
        Debug.Log($"[GameStateManager] {objectID}의 활성 상태가 {isActive}로 설정되었습니다.");
    }

    public bool GetObjectActiveState(string objectID)
    {
        if (objectActiveState.TryGetValue(objectID, out bool state))
        {
            return state;
        }
        return true; // 기본적으로 활성 상태라고 가정
    }
}
