using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro Ŭ������ �������� ���� �����ؾ���
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
       
       

        if (!p_flag) //ui�������
        {
            StopCoroutine("Interaction");

            Color color = img_Interaction.color;
            color.a = 0;
            img_Interaction.color = color;
            go_TargetNameBar.SetActive(false);
            go_Cursor.SetActive(false);
            go_FieldCursor.SetActive(false);
        }
        else //ui�����ų��
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

        isInteract = !p_flag; //isInteract�� UI�� �ݴ�� �����ϰ���
    }


    public void Start() // �������ڸ���
    {
        theDM = FindObjectOfType<DialogueManager>(); // ������������ DialogueManager��� �̸������� ã�Ƽ� ���� ���Խ�Ų��. ����? ���ҽ�
    }



    // Update is called once per frame
    void Update()
    {
        if(!isInteract) //��ȣ�ۿ� �߿��� ������ ������ �����Ұ� �Ƴ�? ���ǹ� ����
        {
            CheckObject(); //��ü�� �ִ��� ������ ��� Ȯ��
            ClickLeftBtn();

        }
        
    }


    void CheckObject()
    {
        Vector3 t_MousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        //mouse�� z���� ���⿡ 0 �ֱ�, �̷��� ������ �Ӥ��ú��� Vector3�� Ȱ���Ͽ�
        //���콺 ��ġ���� t_MousePos�� ������ �� �ִ�.
        //���� ���ǹ��� ����Ͽ� ���콺 �����ǿ��� �������� ��� ������

        if (Physics.Raycast(cam.ScreenPointToRay(t_MousePos), out hitInfo, 1000))
            //�������� �¾Ҵ��� �ȸ¾Ҵ��� �����ؾ���
           
        {
            Contact();
        }
        else
        {
            NotContact();
        }
        
        
    }

    void Contact()  //ũ�ν� �� ��ü�� ����
    {
        if(hitInfo.transform.CompareTag("Interaction"))
        {
            go_TargetNameBar.SetActive(true); // ũ�ν� �� ��ü�� ������� ���� Ȱ��ȭ
            txt_TargetName.text = hitInfo.transform.GetComponent<InteractionType>().GetName();
            if (!isContact)
            {
                isContact = true;
                go_InteractiveCrosshair.SetActive(true);
                go_NormalCrosshair.SetActive(false);
                StopCoroutine("Interaction"); // �ڷ�ƾ ��ø���� �Ǹ� �ȵ��ݾ�?
                StopCoroutine("InteractionEffect"); // �ڷ�ƾ ��ø���� �Ǹ� �ȵ��ݾ�?
                StartCoroutine("Interaction", true); //������ ȿ�� �������� ���� �ڷ�ƾ
                StartCoroutine("InteractionEffect"); /// �ִ� �Ķ���Ͱ� ������ ,(�޸�) �� �ʿ� ����
            }
            
        }
        else
        {
            NotContact();
        }
    }

    void NotContact() //ũ�ν� �� ��ü���� ���
    {
        if(isContact)
        {
            go_TargetNameBar.SetActive(false);
            isContact= false;
            go_InteractiveCrosshair.SetActive(false);
            go_NormalCrosshair.SetActive(true);
            StopCoroutine("Interaction");
            StartCoroutine("Interaction", false); //������ ȿ�� �������� ���� �ڷ�ƾ �ִ� ������� ��? ũ�ν��� ��ü���� ��������ϱ� �׷��� false�ΰ�
        }
       
    }

   
    IEnumerator Interaction(bool p_Appear) // ��ȣ�ۿ�� ������ ������ ��Ÿ���� �Ϸ���
    {
        Color color = img_Interaction.color; //�̹��� ���ͷ����� Į�� ���⿡ �ִ´�. �ְ� �����ֶ� �÷��� ���� �÷��� �ִ´�
        if (p_Appear ) 
        {
            color.a = 0; //������ ������ ��Ÿ���� �ϴϱ� ó���� �Ⱥ�������
            while (color.a < 1) //�÷��� ���İ��� 1���� ������ ��� �ݺ��ض� 1�� ������ ������ ��������
            {
                color.a += 0.1f; // a���� 1�� �ƴ϶�� 0.1f�� ��� ������Ű�ڴ�.
                img_Interaction.color = color; // �� ������ ���� �̹��� ���ͷ��� �÷��� �ٽùݿ�
                yield return null; // �׸��� ��� ���(1������ ��� ��Ű�°���)
            } 
        }
        else // ������Ե� �ؾ���
        {
            while(color.a > 0)  //a���� 0���� ũ�� ��ӹݺ�
            {
                color.a -= 0.1f;
                img_Interaction.color = color; // �� ���ҵ� ���� �̹��� ���ͷ��� �÷��� �ٽùݿ�
                yield return null; // �׸��� ��� ���(1������ ��� ��Ű�°���)
            }
        }
    }

    IEnumerator InteractionEffect()
    {
        while (isContact && !isInteract) //isContact�� Ʈ���ϰ�� ����(�� ũ�ν��� ��ȣ�ۿ� ������ ��ü�� �ӹ�����), && ���ǹ��� �¿� ������ ��� ���� ��� ���� ������ �����Ѵٴ� �� �� ��쿣 isContact�� ���̸鼭 isInteract(��Ŭ��)�� ������ ������? 
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
                img_InteractionEffect.transform.localScale = t_scale; //t_scale������ ���� ��ü���� ����
                yield return new WaitForSeconds(0.01f); // 0.05�ʸ��� �ݺ�
               // yield return null;
            }
             yield return null; //1 ������ ���, ���� ������ ũ�ν��� ����Ʈ�̸� ��ȣ�ۿ� ���� �ʾҴ�? �ٽ� �ݺ�
           // yield return new WaitForSeconds(0.05f); // 0.05�ʸ��� �ݺ�
        }
    }

    void ClickLeftBtn()
    {
        if (!isInteract) // isInteract�� false�� ��쿡�� ��Ŭ�� �����ϰ� ����
        {
            if (Input.GetMouseButtonDown(0)) //���콺 ��Ŭ�� ���� > ��Ŭ���� 0�� �����⿡ �װ� �����Ѵٴ°�
            {
                if (isContact)
                {
                    Interact();
                }
            }
        }
        
    }

    void Interact()  // ��ȣ�ۿ� ����Ʈ ������
    {
        isInteract = true; //��ȣ�ۿ� ���̴� isInteract�� Ʈ��� �ٲ����


        StopCoroutine("Interaction");

        Color color = img_Interaction.color;
        color.a = 0;
        img_Interaction.color = color;

        ps_QuestionEffect.gameObject.SetActive(true);
       
        Vector3 t_targetPos = hitInfo.transform.position;
        ps_QuestionEffect.GetComponent<QuestionEffect>().SetTarget(t_targetPos);
        ps_QuestionEffect.transform.position = cam.transform.position; //����Ʈ�� ��ġ�� ķ ������ǥ�� �̵�

        StartCoroutine(WaitCollision());
    }

    IEnumerator WaitCollision() // ����Ʈ�� �浹�� �� ���� ����ؾ����ݾ�?
    {
        yield return new WaitUntil(() => QuestionEffect.isCollide); // waituntill > Ư�� ������ ������ �� ����
        QuestionEffect.isCollide = false;  //�浹������ �ٽ� �⺻������ �ٲ����

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
       
        theDM.ShowDialogue(p_event.GetDialogue()); //theDM�� �̿��ؼ� Dialogue ȣ��
    }
}
