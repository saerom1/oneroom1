using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Location
{
    public string name;
    public Transform tf_Spawn;
}

public class TransferSpawnManager : MonoBehaviour
{
    [SerializeField] Location[] locations;
    // ���⿡ ����� locationDic�� OnSceneLoaded���� ����ؾ� �մϴ�.
    Dictionary<string, Transform> locationDic = new Dictionary<string, Transform>();

    // �÷��̾� ������
    public static bool spawnTiming = false;
    // �ڵ� �̺�Ʈ Ʈ���ſ�
    public static bool autoEventTiming = false;

    void Awake()
    {
        // 1) ��ųʸ� ����
        foreach (var loc in locations)
            locationDic[loc.name] = loc.tf_Spawn;

        // 2) �� �ε� �ݹ�
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!spawnTiming) return;

        spawnTiming = false;

        // �÷��̾� ���� ó��
        var tm = FindObjectOfType<TransferManager>();
        string locName = tm.GetLocationName();
        if (locationDic.TryGetValue(locName, out Transform tf))
        {
            var pc = PlayerController.instance;
            pc.transform.position = tf.position;
            pc.transform.rotation = tf.rotation;
            Camera.main.transform.localPosition = new Vector3(0, 1, 0);
            Camera.main.transform.localEulerAngles = Vector3.zero;
            pc.Reset();
        }

        // �� �� �������� �ڵ� �̺�Ʈ ���
        autoEventTiming = true;

        // ���̵��� + UI ����
        StartCoroutine(tm.Done());
    }
}