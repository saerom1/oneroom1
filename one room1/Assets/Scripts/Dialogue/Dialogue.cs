using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum CameraType //enum >> 열거형 자료 라는 듯, 어떠한 것들을 열거해서 선택지처럼 고를 수 있게 하는 녀석
{
   ObjectFront,  // 이들 선택지에 따라서 결과를 실행하는건 DialogueManager로 갈 것
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

[System.Serializable] //커스텀 클래스를 인스펙터 창에서 수정하기 위해 필요
public class Dialogue //monobehavior을 상속받지 않게 지워버렸음
{

    [Header("camera Targeting Object")]
    public CameraType cameraType;
    public Transform tf_Target;
   

    [HideInInspector]
    public string name;

    [HideInInspector]  
    public string[] contexts;  // 대사 치는 캐릭터 이름은 배열x, 대사 내용은 배열 처리 했기에 sting[]로 명명 왜? 한 캐릭터가 여러번 말 할 수도 있잖아

    [HideInInspector]
    public string[] spriteName;

    [HideInInspector]
    public string[] VoiceName;

    [Tooltip("이벤트 번호")]
    public string[] number;

    [Tooltip("스킵라인")]
    public string[] skipnum;

    // 새로 추가: End 열 값 (0 또는 1) 현재 사용하지 않음
    public int end;
}

[System.Serializable]
public class EventTiming
{
    public int eventNum;
    public int[] eventConditions;  //특정조건 ex) 3번 이벤트와 5번 이벤트를 봤다? 등장시키게 만들어주는등의 역할
    public bool conditionFlag; // 특정이벤트를 안봤을경우 에도 등장시키는경우에 필요
    public int eventEndNum; //특정이벤트를 봤으면 무조건 퇴장시키는 경우에 필요
}

[System.Serializable]
public class DialogueEvent
{
    public string name; // 이벤트 이름
    public EventTiming eventTiming;

    [Space][Space][Space]
    public Vector2 line; // x,y값 변수
    public Dialogue[] dialogues; //커스텀 클래스에서 변수를 받아왔음, 상호작용전 대사


    [Space]
    public Vector2 lineB; //상호 작용후 대사
    public Dialogue[] dialoguesB;

    [Space][Space][Space]
    public AppearType appearType;
    public GameObject[] go_Targets;

    [Space]
    public GameObject go_NextEvent; //얘가 null값이면 대화시스템 끝내버리고 null값이 아닌 다음 이벤트가 있다면 새로 갱신해서 대화 시스템 유지

    public bool isSame;
   
}
