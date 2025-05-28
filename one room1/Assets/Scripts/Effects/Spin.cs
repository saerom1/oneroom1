using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{

    [SerializeField] float spinSpeed;
    [SerializeField] Vector3 spinDir; //회전방향 즉, 스핀 다이렉션
   



    // Update is called once per frame
    void Update()
    {
        transform.Rotate(spinDir * spinSpeed * Time.deltaTime);
    }

    // undate함수는 60프레임이니 1초에 약 60번 실행
    // Time.deltaTime은 60분의 1이 리턴
    // spinSpeed가 2면? 2*(1/60) * 60 >>> 1초에 2번 회전
}
