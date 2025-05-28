using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro 클래스를 가져오기 위해 선언해야함
using UnityEngine.UI;

public class InteractionController : MonoBehaviour
{


    [SerializeField] Camera cam;

    RaycastHit hitInfo;

    [SerializeField] GameObject go_NormalCrosshair;
    [SerializeField] GameObject go_InteractiveCrosshair;
    [SerializeField] GameObject go_Crosshair;
    [SerializeField] GameObject go_Cursor;
    [SerializeField] GameObject go_FieldCursor;
    [SerializeField] GameObject go_TargetNameBar;
    [SerializeField] TextMeshProUGUI txt_TargetName;

    bool isContact = false;
    public static bool isInteract = false;

    [SerializeField] ParticleSystem ps_QuestionEffect;

    [SerializeField] Image img_Interaction;
    [SerializeField] Image img_InteractionEffect;



    DialogueManager theDM;

   /* public void HideUI()
    {
        go_Crosshair.SetActive(false);
        go_Cursor.SetActive(false);
        go_TargetNameBar.SetActive(false);
    } */

    public void SettingUI(bool p_flag)
    {
      
        go_Crosshair.SetActive(p_flag);
       
       

        if (!p_flag) //ui사라질때
        {
            StopCoroutine("Interaction");

            Color color = img_Interaction.color;
            color.a = 0;
            img_Interaction.color = color;
            go_TargetNameBar.SetActive(false);
            go_Cursor.SetActive(false);
            go_FieldCursor.SetActive(false);
        }
        else //ui등장시킬때
        {
            if(CameraController.onlyView) 
            {
                go_Cursor.SetActive(true);
            }
            else
            {
                go_FieldCursor.SetActive(true) ;
            }
            go_NormalCrosshair.SetActive(true);
            go_InteractiveCrosshair.SetActive(false);
        }

        isInteract = !p_flag; //isInteract는 UI와 반대로 동작하겠지
    }


    public void Start() // 시작하자마자
    {
        theDM = FindObjectOfType<DialogueManager>(); // 계층구조에서 DialogueManager라는 이름붙은걸 찾아서 여기 대입시킨다. 단점? 리소스
    }



    // Update is called once per frame
    void Update()
    {
        if(!isInteract) //상호작용 중에만 네임택 같은게 떠야할거 아녀? 조건문 달자
        {
            CheckObject(); //객체가 있는지 없는지 계속 확인
            ClickLeftBtn();

        }
        
    }


    void CheckObject()
    {
        Vector3 t_MousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        //mouse는 z값이 없기에 0 넣기, 이렇게 선언한 임ㅇ시변수 Vector3를 활용하여
        //마우스 위치값을 t_MousePos에 저장할 수 있다.
        //이제 조건문을 사용하여 마우스 포지션에서 레이저를 쏘게 만들자

        if (Physics.Raycast(cam.ScreenPointToRay(t_MousePos), out hitInfo, 1000))
            //레이저가 맞았는지 안맞았는지 학인해야지
           
        {
            Contact();
        }
        else
        {
            NotContact();
        }
        
        
    }

    void Contact()  //크로스 헤어가 객체에 닿음
    {
        if(hitInfo.transform.CompareTag("Interaction"))
        {
            go_TargetNameBar.SetActive(true); // 크로스 헤어가 객체에 닿았으니 툴팁 활성화
            txt_TargetName.text = hitInfo.transform.GetComponent<InteractionType>().GetName();
            if (!isContact)
            {
                isContact = true;
                go_InteractiveCrosshair.SetActive(true);
                go_NormalCrosshair.SetActive(false);
                StopCoroutine("Interaction"); // 코루틴 중첩실행 되면 안되잖아?
                StopCoroutine("InteractionEffect"); // 코루틴 중첩실행 되면 안되잖아?
                StartCoroutine("Interaction", true); //마름모 효과 넣으려고 만든 코루틴
                StartCoroutine("InteractionEffect"); /// 애는 파라미터가 없으니 ,(콤마) 는 필요 없다
            }
            
        }
        else
        {
            NotContact();
        }
    }

    void NotContact() //크로스 헤어가 객체에서 벗어남
    {
        if(isContact)
        {
            go_TargetNameBar.SetActive(false);
            isContact= false;
            go_InteractiveCrosshair.SetActive(false);
            go_NormalCrosshair.SetActive(true);
            StopCoroutine("Interaction");
            StartCoroutine("Interaction", false); //마름모 효과 넣으려고 만든 코루틴 애는 사라지게 왜? 크로스헤어가 객체에서 벗어났을때니까 그래서 false인겨
        }
       
    }

   
    IEnumerator Interaction(bool p_Appear) // 상호작용시 마름모 서서히 나타나게 하려고
    {
        Color color = img_Interaction.color; //이미지 인터렉션의 칼라를 여기에 넣는다. 애가 갖고있떤 컬러를 변수 컬러에 넣는다
        if (p_Appear ) 
        {
            color.a = 0; //투명도임 서서히 나타나야 하니까 처음엔 안보여야지
            while (color.a < 1) //컬러의 알파값이 1보다 작을때 계속 반복해라 1은 완전히 불투명 해졌을때
            {
                color.a += 0.1f; // a값이 1이 아니라면 0.1f씩 계속 증가시키겠다.
                img_Interaction.color = color; // 그 증가된 값을 이미지 인터렉션 컬러에 다시반영
                yield return null; // 그리고 잠깐 대기(1프레임 대기 시키는거임)
            } 
        }
        else // 사라지게도 해야지
        {
            while(color.a > 0)  //a값이 0보다 크면 계속반복
            {
                color.a -= 0.1f;
                img_Interaction.color = color; // 그 감소된 값을 이미지 인터렉션 컬러에 다시반영
                yield return null; // 그리고 잠깐 대기(1프레임 대기 시키는거임)
            }
        }
    }

    IEnumerator InteractionEffect()
    {
        while (isContact && !isInteract) //isContact가 트루일경우 동안(즉 크로스헤어가 상호작용 가능한 객체에 머무를때), && 조건문은 좌우 조건이 모두 참일 경우 다음 내용을 실행한다는 뜻 이 경우엔 isContact는 참이면서 isInteract(좌클릭)는 거짓일 경우겠지? 
        {
            Color color = img_InteractionEffect.color;
            color.a = 0.5f;

            img_InteractionEffect.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            
            Vector3 t_scale = img_InteractionEffect.transform.localScale;

            while (color.a > 0)
            {
                color.a -= 0.01f;
                img_InteractionEffect.color = color;
                t_scale.Set(t_scale.x + Time.deltaTime, t_scale.y + Time.deltaTime, t_scale.z + Time.deltaTime);
                img_InteractionEffect.transform.localScale = t_scale; //t_scale변수를 실제 객체에다 적용
                yield return new WaitForSeconds(0.01f); // 0.05초마다 반복
               // yield return null;
            }
             yield return null; //1 프레임 대기, 이후 여전히 크로스헤어가 컨택트이며 상호작용 하지 않았다? 다시 반복
           // yield return new WaitForSeconds(0.05f); // 0.05초마다 반복
        }
    }

    void ClickLeftBtn()
    {
        if (!isInteract) // isInteract가 false일 경우에만 좌클릭 가능하게 하자
        {
            if (Input.GetMouseButtonDown(0)) //마우스 좌클릭 감지 > 좌클릭시 0이 나오기에 그걸 감지한다는것
            {
                if (isContact)
                {
                    Interact();
                }
            }
        }
        
    }

    void Interact()  // 상호작용 이펙트 던지기
    {
        isInteract = true; //상호작용 중이니 isInteract를 트루로 바꿔야지


        StopCoroutine("Interaction");

        Color color = img_Interaction.color;
        color.a = 0;
        img_Interaction.color = color;

        ps_QuestionEffect.gameObject.SetActive(true);
       
        Vector3 t_targetPos = hitInfo.transform.position;
        ps_QuestionEffect.GetComponent<QuestionEffect>().SetTarget(t_targetPos);
        ps_QuestionEffect.transform.position = cam.transform.position; //이펙트의 위치를 캠 절대좌표로 이동

        StartCoroutine(WaitCollision());
    }

    IEnumerator WaitCollision() // 이펙트가 충돌할 때 까지 대기해야하잖아?
    {
        yield return new WaitUntil(() => QuestionEffect.isCollide); // waituntill > 특정 조건을 만족할 때 까지
        QuestionEffect.isCollide = false;  //충돌했으면 다시 기본값으로 바꿔야지

        yield return new WaitForSeconds(0.5f);


        

        InteractionEvent t_Event = hitInfo.transform.GetComponent<InteractionEvent>();


        if(hitInfo.transform.GetComponent<InteractionType>().isObject)
        {
            DialogueCall(t_Event);
        }
        else
        {
            if(t_Event != null && t_Event.GetDialogue() != null)
            {
                DialogueCall(t_Event);
            }
            else
            {
                TransferCall();
            }
            
        }
        


    }

    void TransferCall() 
    {
        string t_SceneName = hitInfo.transform.GetComponent<InteractionDoor>().GetSceneName();
        string t_LocationName = hitInfo.transform.GetComponent<InteractionDoor>().GetLocationName();
        StartCoroutine(FindObjectOfType<TransferManager>().Transfer(t_SceneName, t_LocationName));
    }


    void DialogueCall(InteractionEvent p_event) 
    {
        if (!DatabaseManager.instance.eventFlags[p_event.GetEventNumber()])
        {
            theDM.SetNextEvent(p_event.GetNextEvent());
            if (p_event.GetAppearType() == AppearType.Appear) theDM.SetAppearObjects(p_event.GetTargets());
            else if (p_event.GetAppearType() == AppearType.Disappear) theDM.SetDisappearObjects(p_event.GetTargets());
        }
       
        theDM.ShowDialogue(p_event.GetDialogue()); //theDM을 이용해서 Dialogue 호출
    }
}
