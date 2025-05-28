using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChoiceData
{
    public int eventID;                  // EventID �÷�
    public int choiceNumber;             // ChoiceNumber �÷�
    public string choiceText;            // ChoiceText �÷�
    public string targetCharacter;       // TargetCharacter �÷�

    public int affinityEffect;           // AffinityEffect �÷� (ȣ���� ȿ��)
    public int mentalPowerEffect;        // MentalPowerEffect �÷� (���ŷ� ȿ��)

    public int affinityCondition;        // AffinityCondition �÷� (ȣ���� �ּ� ����)
    public int mentalPowerCondition;     // MentalPowerCondition �÷� (���ŷ� �ּ� ����)

    // �߰��� �ʵ�: ���� �б�� ���� �б��� ��ȭ ��� ����
    public int successStartDialogueID;
    public int successEndDialogueID;
    public int failureStartDialogueID;
    public int failureEndDialogueID;
}