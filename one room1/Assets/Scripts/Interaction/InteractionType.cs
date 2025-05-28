using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionType : MonoBehaviour
{
    public bool isDoor; // 지금 조사중인 객체가 문인지 아닌지 판별하는 변수 문이 맞다면 이게 true겠지
    public bool isObject; // 지금 조사중인 객체가 오브젝트인지 아닌지 판별하는 변수

    [SerializeField] string interactionName;

    public string GetName()  // getname 함수가 호출되면
    { 
        return interactionName; // interactionname을 리턴시키겠다.
    }
}
