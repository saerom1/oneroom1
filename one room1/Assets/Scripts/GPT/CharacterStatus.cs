using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    // Inspector에서 각 캐릭터에 맞는 SO를 할당
    public CharacterStats stats;

    // 필요 시, 현재 수치를 UI에 반영하거나, 다른 로직에서 사용합니다.
    void Start()
    {
        Debug.Log($"캐릭터 {stats.characterID}의 초기 호감도: {stats.affinity}");
        Debug.Log($"캐릭터 {stats.characterID}의 초기 정신력: {stats.mentalPower}");
    }
}
