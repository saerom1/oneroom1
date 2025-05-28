using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;  // 싱글톤으로 만들기 위해 자기자신을 인스턴스로 만듬

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    float applySpeed;

    [SerializeField] float fieldSensitivity; //필드에서 고개 움직임 민감도
    [SerializeField] float fieldLookLimitX;

    [SerializeField] Transform tf_Crosshair;

    [SerializeField] Transform tf_Cam;
    [SerializeField] Vector2 camBoundary; // 캠의 가두기 영역.
    [SerializeField] float sightMoveSpeed; // 좌우 움직임 스피드.
    [SerializeField] float sightSensivitity; // 고개의 움직임 속도.
    [SerializeField] float lookLimitX;
    [SerializeField] float lookLimitY;
    float currentAngleX;
    float currentAngleY;

    [SerializeField] GameObject go_NotCamDown;
    [SerializeField] GameObject go_NotCamUp;
    [SerializeField] GameObject go_NotCamLeft;
    [SerializeField] GameObject go_NotCamRight;

    float originPosY;

    public void Reset()
    {
        tf_Crosshair.localPosition = Vector3.zero;
        currentAngleX = 4.57f;
        currentAngleY = 1f;
    }

    public void Start()
    {
        originPosY = tf_Cam.localPosition.y;
    }


    // Update is called once per frame
    void Update()
    {
        if(!InteractionController.isInteract) //조건문 순서는 이후에 다시 변경됩니다
        {
            if (CameraController.onlyView) // InteractionController 스크립트내에 있는 isInteract 값이 false인 경우에만! 이를 위해 isInteract 앞에 public statc 붙였음
            {
                CrosshairMoving();
                ViewMoving();
                KeyViewMoving();
                CameraLimit();
                NotCamUI();
            }
            else // 그렇지 않을경우(필드 돌아다닐때)
            {
                FieldMoving();
                FieldLooking();
            }
        }
        
        
    }

    void FieldMoving()
    {
        if(Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0) // GetAxisRaw(눌렀을때) vertical은? 위아래방향키 혹은 w,s키 눌렀을때 1혹은 -1 이 리턴되는 녀석 horizontal은 좌우 ad
        {
            Vector3 t_Dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

            if(Input.GetKey(KeyCode.LeftShift) ) //뛰는거
            {
                applySpeed = runSpeed;
            }
            else //걷는거
            {
                applySpeed = walkSpeed;
            }

            transform.Translate(t_Dir * applySpeed * Time.deltaTime, Space.Self);
        }
    }

    void FieldLooking()
    {
        if(Input.GetAxisRaw("Mouse X") != 0) //Mouse X는 좌우임 오른쪽은1 리턴 왼쪽은 -1 리턴
        {
            float t_AngleY = Input.GetAxisRaw("Mouse X");
            Vector3 t_Rot = new Vector3(0, t_AngleY * fieldSensitivity, 0);
            transform.rotation = Quaternion.Euler(transform.localEulerAngles + t_Rot);
        }
        if(Input.GetAxisRaw("Mouse Y") != 0)
        {
            float t_AngleX = Input.GetAxisRaw("Mouse Y");
            currentAngleX -= t_AngleX; // 빼주는 이유는 Y축 반전 때문 (ex Part2)
            currentAngleX = Mathf.Clamp(currentAngleX, -fieldLookLimitX, fieldLookLimitX);
            tf_Cam.localEulerAngles = new Vector3(currentAngleX, 0, 0);
        }
    }

    void NotCamUI()  // 시점이 상하좌우 최대좌표에 도달했을때 더이상 이동불가하다는 스프라이트 출력 역할
    {
        go_NotCamDown.SetActive(false);
        go_NotCamUp.SetActive(false);
        go_NotCamLeft.SetActive(false);
        go_NotCamRight.SetActive(false);

        if (currentAngleY >= lookLimitX)
            go_NotCamRight.SetActive(true);
        else if (currentAngleY <= -lookLimitX)
            go_NotCamLeft.SetActive(true);

        if (currentAngleX <= -lookLimitY)
            go_NotCamUp.SetActive(true);
        else if (currentAngleX >= lookLimitY)
            go_NotCamDown.SetActive(true);
    }

    void CameraLimit()
    {
        if (tf_Cam.localPosition.x >= camBoundary.x)
            tf_Cam.localPosition = new Vector3(camBoundary.x, tf_Cam.localPosition.y, tf_Cam.localPosition.z);
       else if (tf_Cam.localPosition.x <= -camBoundary.x)
            tf_Cam.localPosition = new Vector3(-camBoundary.x, tf_Cam.localPosition.y, tf_Cam.localPosition.z);

        if (tf_Cam.localPosition.y >=  camBoundary.y)
            tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x,  camBoundary.y, tf_Cam.localPosition.z);
        else if (tf_Cam.localPosition.y <= 1 - camBoundary.y)
            tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x,  camBoundary.y, tf_Cam.localPosition.z);
    }

    void KeyViewMoving()
    {
        if(Input.GetAxisRaw("Horizontal") !=0 ) 
            //수평 방향키 A,D 혹은 좌우키에 반응하는 Horizontal이 0이 아닐경우 >>> A키와 왼쪽 방향키는 -1, D키와 오른쪽키는 1을 도출
            //그 외에는 0을 도출함
        {
            currentAngleY += sightSensivitity * Input.GetAxisRaw("Horizontal");
            currentAngleY = Mathf.Clamp(currentAngleY, -lookLimitX, lookLimitX);

            tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x + sightMoveSpeed * Input.GetAxisRaw("Horizontal"), tf_Cam.localPosition.y, tf_Cam.localPosition.z);
        }

        if (Input.GetAxisRaw("Vertical") != 0)
        //수직 방향키 W,S 혹은 상하키에 반응하는 Vertical이 0이 아닐경우 >>> S키와 아래쪽 방향키는 -1, W키와 윗쪽키는 1을 도출
        //그 외에는 0을 도출함
        {
            currentAngleX += sightSensivitity * -Input.GetAxisRaw("Vertical"); // -붙인이유? 상하반전시키려고
            currentAngleX = Mathf.Clamp(currentAngleX, -lookLimitY, lookLimitY);

            tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x , tf_Cam.localPosition.y + sightMoveSpeed * Input.GetAxisRaw("Vertical"), tf_Cam.localPosition.z);
        }
        tf_Cam.localEulerAngles = new Vector3(currentAngleX, currentAngleY, tf_Cam.localEulerAngles.z);
    }

    void ViewMoving()
    {
        if (tf_Crosshair.localPosition.x > (Screen.width/2 -260 ) || tf_Crosshair.localPosition.x < (-Screen.width/2 +260))
        {
            currentAngleY += (tf_Crosshair.localPosition.x > 0) ? sightSensivitity : -sightSensivitity; 
            //괄호내의 전제가 참이면 전자 거짓이면 후자가 currentY로 들어감

            currentAngleY = Mathf.Clamp(currentAngleY, -lookLimitX, lookLimitX);

            float t_applySpeed = (tf_Crosshair.localPosition.x > 0) ? sightMoveSpeed : -sightMoveSpeed;
            tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x + t_applySpeed, tf_Cam.localPosition.y, tf_Cam.localPosition.z);
            
        }

        if (tf_Crosshair.localPosition.y > (Screen.height/2 -140 ) || tf_Crosshair.localPosition.y < (-Screen.height/2 +140))
        {
            currentAngleX += (tf_Crosshair.localPosition.y > 0) ? -sightSensivitity : +sightSensivitity;
            //괄호내의 전제가 참이면 전자 거짓이면 후자가 currentY로 들어감

            currentAngleX = Mathf.Clamp(currentAngleX, -lookLimitY, lookLimitY);

            float t_applySpeed = (tf_Crosshair.localPosition.y > 0) ? sightMoveSpeed : -sightMoveSpeed;
            tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x, tf_Cam.localPosition.y + t_applySpeed, tf_Cam.localPosition.z);


        }

        tf_Cam.localEulerAngles = new Vector3(currentAngleX, currentAngleY, tf_Cam.localEulerAngles.z);
        // 순서대로 x,y,z 값이며 z값에는 0 혹은 자기자신 값 넣으면됨

    }

    void CrosshairMoving()
    {
        tf_Crosshair.localPosition = new Vector2(Input.mousePosition.x - (Screen.width / 2),
                                                 Input.mousePosition.y - (Screen.height / 2));

        float t_cursorPosX = tf_Crosshair.localPosition.x;  // local positon > 부모객체의 위치값에 영향받음, 그냥 position의 경우 절대 값이라 영향 안받음
        float t_cursorPosY = tf_Crosshair.localPosition.y;

        t_cursorPosX = Mathf.Clamp(t_cursorPosX, (-Screen.width/2 + 130), (Screen.width/2 - 130));
        t_cursorPosY = Mathf.Clamp(t_cursorPosY, (-Screen.height/2 + 80), (Screen.height/2 - 80));

        tf_Crosshair.localPosition = new Vector2(t_cursorPosX, t_cursorPosY);
    }
}
