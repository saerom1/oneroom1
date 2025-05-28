using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Game/CharacterStats")]
public class CharacterStats : ScriptableObject
{
    public string characterID;   // 예: "Hero", "Villain"
    public int affinity;         // 호감도
    public int mentalPower;      // 정신력

    // 수치 증감 함수
    public void ModifyAffinity(int delta)
    {
        affinity += delta;
        // 필요하면 최소/최대값 clamp 추가
    }

    public void ModifyMentalPower(int delta)
    {
        mentalPower += delta;
    }
}
