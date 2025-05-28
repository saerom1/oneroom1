using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static bool onlyView = true; //왜 기본값 true로 해놨음? 시작을 방(이동불가)에서 하잖아, 

    Vector3 originPos;
    Quaternion originRot;

    InteractionController theIC;

    PlayerController thePlayer;

    Coroutine coroutine;

    void Start()
    {
        theIC = FindObjectOfType<InteractionController>();
        thePlayer = FindObjectOfType<PlayerController>();


    }

    public void CamOriginSetting()
    {
        originPos = transform.position;
        if(onlyView) 
        {
            originRot = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            originRot = transform.rotation;
        }
        
    }


    public void CameraTargetting(Transform p_Target, float p_CamSpeed = 0.1f, bool p_isReset = false, bool p_isFinish = false)
    {
        
        if (!p_isReset)
        {
            if (p_Target != null)
            {
                StopAllCoroutines();
                StartCoroutine(CameraTargettingCoroutine(p_Target, p_CamSpeed));
            }
                
        }
        else
        {
            StartCoroutine(CameraResetCoroutine(p_CamSpeed, p_isFinish));
        }
    }



    /*
    public void CameraTargetting(Transform p_Target, float p_CamSpeed = 0.1f, bool p_isReset = false, bool p_isFinish = false)
    {
        StopAllCoroutines();
        if (!p_isReset)
        {
            if (p_Target != null)
                StartCoroutine(CameraTargettingCoroutine(p_Target, p_CamSpeed));
        }
        else
        { 
            StartCoroutine(CameraResetCoroutine(p_CamSpeed, p_isFinish));
        }
    }
    */



    /* 실패본
    public void CameraTargetting(Transform p_Target, float p_CamSpeed = 0.1f, bool p_isReset = false, bool p_isFinish = false)
    {
        
        if (!p_isReset)
        {
            if (p_Target != null)
            { 
                StopAllCoroutines();// 5-2, 5:45
                coroutine = StartCoroutine(CameraTargettingCoroutine(p_Target, p_CamSpeed));
            }
            else
            {
                if(coroutine != null)
                {
                    StopCoroutine(coroutine);
                    
                }
                StartCoroutine(CameraResetCoroutine(p_CamSpeed, p_isFinish));
            }
        } //애가 문제인가?
       
        
    }
    */

    IEnumerator CameraTargettingCoroutine(Transform p_Target, float p_CamSpeed = 0.05f)
    {
        Vector3 t_TargetPos = p_Target.position;
        Vector3 t_TargetFrontPos = t_TargetPos + (p_Target.forward * 1.3f);
        Vector3 t_Direction = (t_TargetPos - t_TargetFrontPos).normalized;  //normalized란? 벡터의 크기를 항상 1로 유지

        while(transform.position != t_TargetFrontPos || Quaternion.Angle(transform.rotation, Quaternion.LookRotation(t_Direction)) >= 0.5f) 
        {
            transform.position = Vector3.MoveTowards(transform.position, t_TargetFrontPos, p_CamSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(t_Direction), p_CamSpeed);

            yield return null;
        }
    }

    IEnumerator CameraResetCoroutine(float p_CamSpeed = 0.1f, bool p_isFinish = false)
    {
        

        yield return new WaitForSeconds(0.5f);

        while (transform.position != originPos || Quaternion.Angle(transform.rotation, originRot) >= 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, originPos, p_CamSpeed); //이거대로면 본래 위치로 가야하는데 왜 안감?
            transform.rotation = Quaternion.Lerp(transform.rotation, originRot, p_CamSpeed);

            yield return null;
        }
        transform.position = originPos;
        

        if (p_isFinish) 
        {
            thePlayer.Reset();
            
            InteractionController.isInteract = false;
            
        }
    }
}
