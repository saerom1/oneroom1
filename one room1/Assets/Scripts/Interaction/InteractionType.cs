using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionType : MonoBehaviour
{
    public bool isDoor; // ���� �������� ��ü�� ������ �ƴ��� �Ǻ��ϴ� ���� ���� �´ٸ� �̰� true����
    public bool isObject; // ���� �������� ��ü�� ������Ʈ���� �ƴ��� �Ǻ��ϴ� ����

    [SerializeField] string interactionName;

    public string GetName()  // getname �Լ��� ȣ��Ǹ�
    { 
        return interactionName; // interactionname�� ���Ͻ�Ű�ڴ�.
    }
}
