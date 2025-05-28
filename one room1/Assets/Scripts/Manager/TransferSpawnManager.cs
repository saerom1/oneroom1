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
    // 여기에 선언된 locationDic을 OnSceneLoaded에서 사용해야 합니다.
    Dictionary<string, Transform> locationDic = new Dictionary<string, Transform>();

    // 플레이어 스폰용
    public static bool spawnTiming = false;
    // 자동 이벤트 트리거용
    public static bool autoEventTiming = false;

    void Awake()
    {
        // 1) 딕셔너리 세팅
        foreach (var loc in locations)
            locationDic[loc.name] = loc.tf_Spawn;

        // 2) 씬 로드 콜백
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

        // 플레이어 스폰 처리
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

        // → 이 시점에만 자동 이벤트 허용
        autoEventTiming = true;

        // 페이드인 + UI 복원
        StartCoroutine(tm.Done());
    }
}