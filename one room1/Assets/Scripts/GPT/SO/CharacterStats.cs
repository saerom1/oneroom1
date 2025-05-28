using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Game/CharacterStats")]
public class CharacterStats : ScriptableObject
{
    public string characterID;   // ��: "Hero", "Villain"
    public int affinity;         // ȣ����
    public int mentalPower;      // ���ŷ�

    // ��ġ ���� �Լ�
    public void ModifyAffinity(int delta)
    {
        affinity += delta;
        // �ʿ��ϸ� �ּ�/�ִ밪 clamp �߰�
    }

    public void ModifyMentalPower(int delta)
    {
        mentalPower += delta;
    }
}
