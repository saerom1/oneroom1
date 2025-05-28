using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappear : MonoBehaviour
{
    [SerializeField] float disappearTime;

    private void OnEnable() // �� ��ü�� Ȱ��ȭ �Ǹ�
    {
        StartCoroutine(DisappearCoroutine()); // �ڷ�ƾ �����
    }

    IEnumerator DisappearCoroutine() 
    {
        yield return new WaitForSeconds(disappearTime);

        gameObject.SetActive(false);
    }
}
