using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;  // �̱������� ����� ���� �ڱ��ڽ��� �ν��Ͻ��� ����

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

    [SerializeField] float fieldSensitivity; //�ʵ忡�� �� ������ �ΰ���
    [SerializeField] float fieldLookLimitX;

    [SerializeField] Transform tf_Crosshair;

    [SerializeField] Transform tf_Cam;
    [SerializeField] Vector2 camBoundary; // ķ�� ���α� ����.
    [SerializeField] float sightMoveSpeed; // �¿� ������ ���ǵ�.
    [SerializeField] float sightSensivitity; // ���� ������ �ӵ�.
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
        if(!InteractionController.isInteract) //���ǹ� ������ ���Ŀ� �ٽ� ����˴ϴ�
        {
            if (CameraController.onlyView) // InteractionController ��ũ��Ʈ���� �ִ� isInteract ���� false�� ��쿡��! �̸� ���� isInteract �տ� public statc �ٿ���
            {
                CrosshairMoving();
                ViewMoving();
                KeyViewMoving();
                CameraLimit();
                NotCamUI();
            }
            else // �׷��� �������(�ʵ� ���ƴٴҶ�)
            {
                FieldMoving();
                FieldLooking();
            }
        }
        
        
    }

    void FieldMoving()
    {
        if(Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0) // GetAxisRaw(��������) vertical��? ���Ʒ�����Ű Ȥ�� w,sŰ �������� 1Ȥ�� -1 �� ���ϵǴ� �༮ horizontal�� �¿� ad
        {
            Vector3 t_Dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

            if(Input.GetKey(KeyCode.LeftShift) ) //�ٴ°�
            {
                applySpeed = runSpeed;
            }
            else //�ȴ°�
            {
                applySpeed = walkSpeed;
            }

            transform.Translate(t_Dir * applySpeed * Time.deltaTime, Space.Self);
        }
    }

    void FieldLooking()
    {
        if(Input.GetAxisRaw("Mouse X") != 0) //Mouse X�� �¿��� ��������1 ���� ������ -1 ����
        {
            float t_AngleY = Input.GetAxisRaw("Mouse X");
            Vector3 t_Rot = new Vector3(0, t_AngleY * fieldSensitivity, 0);
            transform.rotation = Quaternion.Euler(transform.localEulerAngles + t_Rot);
        }
        if(Input.GetAxisRaw("Mouse Y") != 0)
        {
            float t_AngleX = Input.GetAxisRaw("Mouse Y");
            currentAngleX -= t_AngleX; // ���ִ� ������ Y�� ���� ���� (ex Part2)
            currentAngleX = Mathf.Clamp(currentAngleX, -fieldLookLimitX, fieldLookLimitX);
            tf_Cam.localEulerAngles = new Vector3(currentAngleX, 0, 0);
        }
    }

    void NotCamUI()  // ������ �����¿� �ִ���ǥ�� ���������� ���̻� �̵��Ұ��ϴٴ� ��������Ʈ ��� ����
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
            //���� ����Ű A,D Ȥ�� �¿�Ű�� �����ϴ� Horizontal�� 0�� �ƴҰ�� >>> AŰ�� ���� ����Ű�� -1, DŰ�� ������Ű�� 1�� ����
            //�� �ܿ��� 0�� ������
        {
            currentAngleY += sightSensivitity * Input.GetAxisRaw("Horizontal");
            currentAngleY = Mathf.Clamp(currentAngleY, -lookLimitX, lookLimitX);

            tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x + sightMoveSpeed * Input.GetAxisRaw("Horizontal"), tf_Cam.localPosition.y, tf_Cam.localPosition.z);
        }

        if (Input.GetAxisRaw("Vertical") != 0)
        //���� ����Ű W,S Ȥ�� ����Ű�� �����ϴ� Vertical�� 0�� �ƴҰ�� >>> SŰ�� �Ʒ��� ����Ű�� -1, WŰ�� ����Ű�� 1�� ����
        //�� �ܿ��� 0�� ������
        {
            currentAngleX += sightSensivitity * -Input.GetAxisRaw("Vertical"); // -��������? ���Ϲ�����Ű����
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
            //��ȣ���� ������ ���̸� ���� �����̸� ���ڰ� currentY�� ��

            currentAngleY = Mathf.Clamp(currentAngleY, -lookLimitX, lookLimitX);

            float t_applySpeed = (tf_Crosshair.localPosition.x > 0) ? sightMoveSpeed : -sightMoveSpeed;
            tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x + t_applySpeed, tf_Cam.localPosition.y, tf_Cam.localPosition.z);
            
        }

        if (tf_Crosshair.localPosition.y > (Screen.height/2 -140 ) || tf_Crosshair.localPosition.y < (-Screen.height/2 +140))
        {
            currentAngleX += (tf_Crosshair.localPosition.y > 0) ? -sightSensivitity : +sightSensivitity;
            //��ȣ���� ������ ���̸� ���� �����̸� ���ڰ� currentY�� ��

            currentAngleX = Mathf.Clamp(currentAngleX, -lookLimitY, lookLimitY);

            float t_applySpeed = (tf_Crosshair.localPosition.y > 0) ? sightMoveSpeed : -sightMoveSpeed;
            tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x, tf_Cam.localPosition.y + t_applySpeed, tf_Cam.localPosition.z);


        }

        tf_Cam.localEulerAngles = new Vector3(currentAngleX, currentAngleY, tf_Cam.localEulerAngles.z);
        // ������� x,y,z ���̸� z������ 0 Ȥ�� �ڱ��ڽ� �� �������

    }

    void CrosshairMoving()
    {
        tf_Crosshair.localPosition = new Vector2(Input.mousePosition.x - (Screen.width / 2),
                                                 Input.mousePosition.y - (Screen.height / 2));

        float t_cursorPosX = tf_Crosshair.localPosition.x;  // local positon > �θ�ü�� ��ġ���� �������, �׳� position�� ��� ���� ���̶� ���� �ȹ���
        float t_cursorPosY = tf_Crosshair.localPosition.y;

        t_cursorPosX = Mathf.Clamp(t_cursorPosX, (-Screen.width/2 + 130), (Screen.width/2 - 130));
        t_cursorPosY = Mathf.Clamp(t_cursorPosY, (-Screen.height/2 + 80), (Screen.height/2 - 80));

        tf_Crosshair.localPosition = new Vector2(t_cursorPosX, t_cursorPosY);
    }
}
