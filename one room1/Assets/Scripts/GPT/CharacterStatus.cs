using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    // Inspector���� �� ĳ���Ϳ� �´� SO�� �Ҵ�
    public CharacterStats stats;

    // �ʿ� ��, ���� ��ġ�� UI�� �ݿ��ϰų�, �ٸ� �������� ����մϴ�.
    void Start()
    {
        Debug.Log($"ĳ���� {stats.characterID}�� �ʱ� ȣ����: {stats.affinity}");
        Debug.Log($"ĳ���� {stats.characterID}�� �ʱ� ���ŷ�: {stats.mentalPower}");
    }
}
