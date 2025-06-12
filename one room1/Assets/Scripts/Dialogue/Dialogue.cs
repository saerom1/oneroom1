using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum CameraType //enum >> ������ �ڷ� ��� ��, ��� �͵��� �����ؼ� ������ó�� �� �� �ְ� �ϴ� �༮
{
   ObjectFront,  // �̵� �������� ���� ����� �����ϴ°� DialogueManager�� �� ��
   Reset,
   FadeOut,
   FadeIn,
   FlashOut,
   FlashIn,
   ShowCutScene,
   HideCutScene,
   AppearSlideCG,
   DisappearSlideCG,
   ChangeSlideCG,
}

public enum AppearType
{
    None,
    Appear,
    Disappear,
}

[System.Serializable] //Ŀ���� Ŭ������ �ν����� â���� �����ϱ� ���� �ʿ�
public class Dialogue //monobehavior�� ��ӹ��� �ʰ� ����������
{

    [Header("camera Targeting Object")]
    public CameraType cameraType;
    public Transform tf_Target;
   

    [HideInInspector]
    public string name;

    [HideInInspector]  
    public string[] contexts;  // ��� ġ�� ĳ���� �̸��� �迭x, ��� ������ �迭 ó�� �߱⿡ sting[]�� ��� ��? �� ĳ���Ͱ� ������ �� �� ���� ���ݾ�

    [HideInInspector]
    public string[] spriteName;

    [HideInInspector]
    public string[] VoiceName;

    [Tooltip("�̺�Ʈ ��ȣ")]
    public string[] number;

    [Tooltip("��ŵ����")]
    public string[] skipnum;

    // ���� �߰�: End �� �� (0 �Ǵ� 1) ���� ������� ����
    public int end;
}

[System.Serializable]
public class EventTiming
{
    public int eventNum;
    public int[] eventConditions;  //Ư������ ex) 3�� �̺�Ʈ�� 5�� �̺�Ʈ�� �ô�? �����Ű�� ������ִµ��� ����
    public bool conditionFlag; // Ư���̺�Ʈ�� �Ⱥ������ ���� �����Ű�°�쿡 �ʿ�
    public int eventEndNum; //Ư���̺�Ʈ�� ������ ������ �����Ű�� ��쿡 �ʿ�
}

[System.Serializable]
public class DialogueEvent
{
    public string name; // �̺�Ʈ �̸�
    public EventTiming eventTiming;

    [Space][Space][Space]
    public Vector2 line; // x,y�� ����
    public Dialogue[] dialogues; //Ŀ���� Ŭ�������� ������ �޾ƿ���, ��ȣ�ۿ��� ���


    [Space]
    public Vector2 lineB; //��ȣ �ۿ��� ���
    public Dialogue[] dialoguesB;

    [Space][Space][Space]
    public AppearType appearType;
    public GameObject[] go_Targets;

    [Space]
    public GameObject go_NextEvent; //�갡 null���̸� ��ȭ�ý��� ���������� null���� �ƴ� ���� �̺�Ʈ�� �ִٸ� ���� �����ؼ� ��ȭ �ý��� ����

    public bool isSame;
   
}
