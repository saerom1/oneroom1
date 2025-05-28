using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;

    // �̺�Ʈ ���� ���� ���� (�ʿ��� ���)
    public Dictionary<int, bool> eventExecuted = new Dictionary<int, bool>();

    // ������Ʈ Ȱ��/��Ȱ�� ���� ���� (Ű: ������Ʈ�� ���� ID, ��: Ȱ�� ����)
    public Dictionary<string, bool> objectActiveState = new Dictionary<string, bool>();

    // �� ��ȯ �� ���� �÷���
    public bool isSceneChanging = false;



    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("[GameStateManager] �ʱ�ȭ��. instance�� �����Ǿ����ϴ�.");
            DontDestroyOnLoad(gameObject);

            // �� ��ȯ �̺�Ʈ
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.sceneLoaded += OnSceneLoaded;

            Debug.Log("[GameStateManager] �ʱ�ȭ��. instance�� �����Ǿ����ϴ�.");

        }
        else
        {
            Destroy(gameObject);
        }
    }


    void OnSceneUnloaded(Scene scene)
    {
        isSceneChanging = true;
        Debug.Log($"[GameStateManager] �� ��ε� ����: frame={Time.frameCount}, isSceneChanging={isSceneChanging}");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isSceneChanging = false;
        Debug.Log($"[GameStateManager] �� �ε� �Ϸ�: frame={Time.frameCount}, isSceneChanging={isSceneChanging}");
        // �� �� �κ��� �߰� ��
        // "Interaction" �±װ� ���� ��� ������Ʈ�� ã��,
        // ������ ����� objectActiveState �� ���� SetActive ��ŵ�ϴ�.
        var all = GameObject.FindGameObjectsWithTag("Interaction");
        foreach (var go in all)
        {
            if (objectActiveState.TryGetValue(go.name, out bool shouldBeActive))
            {
                go.SetActive(shouldBeActive);
                Debug.Log($"[GameStateManager] ����: {go.name}.SetActive({shouldBeActive})");
            }
            // else: ���� ����� ������ �� �⺻�� �״�� ���Ӵϴ�.
        }
    }


    // ���� �޼���: �̺�Ʈ ���� ���� ������Ʈ (�߰� �Ǵ� ����)
    public void SetEventExecuted(int eventID, bool executed)
    {
        if (!eventExecuted.ContainsKey(eventID))
        {
            eventExecuted.Add(eventID, executed);
            Debug.Log($"[GameStateManager] �̺�Ʈ {eventID}�� ó�� ����Ǿ����ϴ�. (��: {executed})");
        }
        else
        {
            eventExecuted[eventID] = executed;
            Debug.Log($"[GameStateManager] �̺�Ʈ {eventID}�� ���°� ������Ʈ�Ǿ����ϴ�. (��: {executed})");
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
        Debug.Log($"[GameStateManager] {objectID}�� Ȱ�� ���°� {isActive}�� �����Ǿ����ϴ�.");
    }

    public bool GetObjectActiveState(string objectID)
    {
        if (objectActiveState.TryGetValue(objectID, out bool state))
        {
            return state;
        }
        return true; // �⺻������ Ȱ�� ���¶�� ����
    }
}
