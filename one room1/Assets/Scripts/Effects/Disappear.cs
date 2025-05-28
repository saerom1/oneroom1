using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappear : MonoBehaviour
{
    [SerializeField] float disappearTime;

    private void OnEnable() // 이 객체가 활성화 되면
    {
        StartCoroutine(DisappearCoroutine()); // 코루틴 실행됨
    }

    IEnumerator DisappearCoroutine() 
    {
        yield return new WaitForSeconds(disappearTime);

        gameObject.SetActive(false);
    }
}
