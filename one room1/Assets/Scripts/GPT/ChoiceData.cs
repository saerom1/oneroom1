using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChoiceData
{
    public int eventID;                  // EventID 컬럼
    public int choiceNumber;             // ChoiceNumber 컬럼
    public string choiceText;            // ChoiceText 컬럼
    public string targetCharacter;       // TargetCharacter 컬럼

    public int affinityEffect;           // AffinityEffect 컬럼 (호감도 효과)
    public int mentalPowerEffect;        // MentalPowerEffect 컬럼 (정신력 효과)

    public int affinityCondition;        // AffinityCondition 컬럼 (호감도 최소 조건)
    public int mentalPowerCondition;     // MentalPowerCondition 컬럼 (정신력 최소 조건)

    // 추가된 필드: 성공 분기와 실패 분기의 대화 블록 범위
    public int successStartDialogueID;
    public int successEndDialogueID;
    public int failureStartDialogueID;
    public int failureEndDialogueID;
}