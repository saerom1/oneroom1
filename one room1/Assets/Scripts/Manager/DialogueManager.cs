using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro 클래스를 가져오기 위해 선언해야함

public class DialogueManager : MonoBehaviour
{
    public static bool isWating = false;

    [SerializeField] GameObject go_DialogueBar;
    [SerializeField] GameObject go_DialogueNameBar;

    [SerializeField] TextMeshProUGUI txt_Dialogue;
    [SerializeField] TextMeshProUGUI txt_Name;

    Dialogue[] dialogues;
    bool isDialogue = false;
    bool isNext = false;

    [Header("TextDelay")]
    [SerializeField] float textDelay;

    public int lineCount = 0;
    public int contextCount = 0;

    GameObject go_NextEvent;
    public void SetNextEvent(GameObject p_NextEvent) => go_NextEvent = p_NextEvent;

    GameObject[] go_Objects;
    byte appearTypeNumber;
    const byte NONE = 0, APPEAR = 1, DISAPPEAR = 2;

    public void SetAppearObjects(GameObject[] p_Targets)
    {
        go_Objects = p_Targets;
        appearTypeNumber = APPEAR;
    }

    public void SetDisappearObjects(GameObject[] p_Targets)
    {
        go_Objects = p_Targets;
        appearTypeNumber = DISAPPEAR;
    }

    InteractionController theIC;
    CameraController theCam;
    SplashManager theSplashManager;
    SpriteManager theSpriteManager;
    CutSceneManager theCutSceneManager;
    SlideManager theSlideManager;

    void Start()
    {
        theIC = FindObjectOfType<InteractionController>();
        theCam = FindObjectOfType<CameraController>();
        theSpriteManager = FindObjectOfType<SpriteManager>();
        theSplashManager = FindObjectOfType<SplashManager>();
        theCutSceneManager = FindObjectOfType<CutSceneManager>();
        theSlideManager = FindObjectOfType<SlideManager>();
    }

    void Update()
    {
        if (!isDialogue || !isNext) return;
        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.KeypadEnter) ||
            Input.GetMouseButtonDown(0))
        {
            isNext = false;
            txt_Dialogue.text = "";

            // 선택지 체크
            if (dialogues[lineCount].number != null &&
                dialogues[lineCount].number.Length > 0 &&
                !string.IsNullOrEmpty(dialogues[lineCount].number[0]))
            {
                if (int.TryParse(dialogues[lineCount].number[0], out int eventID))
                {
                    var cm = FindObjectOfType<ChoiceManager>();
                    if (cm != null) cm.ShowChoicesForEvent(eventID);
                    else Debug.LogError("ChoiceManager를 찾을 수 없습니다.");
                    return;
                }
                else Debug.LogWarning("이벤트 번호 파싱 실패: " + dialogues[lineCount].number[0]);
            }

            // 같은 행 내 다음 문장
            if (++contextCount < dialogues[lineCount].contexts.Length)
            {
                StartCoroutine(TypeWriter());
            }
            else
            {
                contextCount = 0;
                if (++lineCount < dialogues.Length)
                    StartCoroutine(CameraTargettingType());
                else
                    StartCoroutine(EndDialogue());
            }
        }
    }

    public void ShowDialogue(Dialogue[] p_dialogues)
    {
        isDialogue = true;
        txt_Dialogue.text = "";
        txt_Name.text = "";
        theIC.SettingUI(false);
        dialogues = p_dialogues;
        StartCoroutine(StartDialogue());
    }

    IEnumerator StartDialogue()
    {
        if (isWating) yield return new WaitForSeconds(0.5f);
        isWating = false;
        theCam.CamOriginSetting();
        StartCoroutine(CameraTargettingType());
    }

    IEnumerator CameraTargettingType()
    {
        switch (dialogues[lineCount].cameraType)
        {
            case CameraType.FadeIn:
                SettingUI(false);
                SplashManager.isfinished = false;
                StartCoroutine(theSplashManager.FadeIn(false, true));
                yield return new WaitUntil(() => SplashManager.isfinished);
                break;
            case CameraType.FadeOut:
                SettingUI(false);
                SplashManager.isfinished = false;
                StartCoroutine(theSplashManager.FadeOut(false, true));
                yield return new WaitUntil(() => SplashManager.isfinished);
                break;
            case CameraType.FlashIn:
                SettingUI(false);
                SplashManager.isfinished = false;
                StartCoroutine(theSplashManager.FadeIn(true, true));
                yield return new WaitUntil(() => SplashManager.isfinished);
                break;
            case CameraType.FlashOut:
                SettingUI(false);
                SplashManager.isfinished = false;
                StartCoroutine(theSplashManager.FadeOut(true, true));
                yield return new WaitUntil(() => SplashManager.isfinished);
                break;
            case CameraType.ObjectFront:
                theCam.CameraTargetting(dialogues[lineCount].tf_Target);
                break;
            case CameraType.Reset:
                theCam.CameraTargetting(null, 0.05f, true, false);
                break;
            case CameraType.ShowCutScene:
                SettingUI(false);
                CutSceneManager.isFinished = false;
                StartCoroutine(theCutSceneManager.CutSceneCoroutine(
                    dialogues[lineCount].spriteName[contextCount], true));
                yield return new WaitUntil(() => CutSceneManager.isFinished);
                break;
            case CameraType.HideCutScene:
                SettingUI(false);
                CutSceneManager.isFinished = false;
                StartCoroutine(theCutSceneManager.CutSceneCoroutine(null, false));
                yield return new WaitUntil(() => CutSceneManager.isFinished);
                theCam.CameraTargetting(dialogues[lineCount].tf_Target);
                break;
            case CameraType.AppearSlideCG:
                SlideManager.isFinished = false;
                StartCoroutine(theSlideManager.AppearSlide(
                    dialogues[lineCount].spriteName[contextCount].Split('/')[1]));
                yield return new WaitUntil(() => SlideManager.isFinished);
                theCam.CameraTargetting(dialogues[lineCount].tf_Target);
                break;
            case CameraType.DisappearSlideCG:
                SlideManager.isFinished = false;
                StartCoroutine(theSlideManager.DisappearSlide());
                yield return new WaitUntil(() => SlideManager.isFinished);
                theCam.CameraTargetting(dialogues[lineCount].tf_Target);
                break;
            case CameraType.ChangeSlideCG:
                SlideManager.isChanged = false;
                StartCoroutine(theSlideManager.ChangeSlide(
                    dialogues[lineCount].spriteName[contextCount].Split('/')[1]));
                yield return new WaitUntil(() => SlideManager.isChanged);
                theCam.CameraTargetting(dialogues[lineCount].tf_Target);
                break;
        }
        StartCoroutine(TypeWriter());
    }

    IEnumerator EndDialogue()
    {
        SettingUI(false);

        if (dialogues == null || dialogues.Length == 0)
        {
            Debug.LogWarning("대화 배열이 비어있습니다.");
            yield break;
        }
        int lastIndex = Mathf.Clamp(lineCount, 0, dialogues.Length - 1);
        if (dialogues[lastIndex].end == 1)
        {
            isDialogue = false;
            yield break;
        }

        if (theCutSceneManager.CheckCutScene())
        {
            CutSceneManager.isFinished = false;
            StartCoroutine(theCutSceneManager.CutSceneCoroutine(null, false));
            yield return new WaitUntil(() => CutSceneManager.isFinished);
        }

        AppearOrDisappearObjects();
        yield return new WaitUntil(() => Spin2.isFinished);

        isDialogue = false;
        contextCount = 0;
        lineCount = 0;
        dialogues = null;
        isNext = false;
        theCam.CameraTargetting(null, 0.05f, true, true);
        yield return new WaitUntil(() => !InteractionController.isInteract);

        if (go_NextEvent != null)
        {
            go_NextEvent.SetActive(true);
            go_NextEvent = null;
        }
        else
        {
            theIC.SettingUI(true);
        }

        // --- 대화 종료 시마다 자동 이벤트 즉시 체크 호출 ---
        foreach (var ie in FindObjectsOfType<InteractionEvent>())
            ie.TryTriggerAutoOnDialogueEnd();
    }

    void AppearOrDisappearObjects()
    {
        if (go_Objects == null) return;
        Spin2.isFinished = true;
        foreach (var obj in go_Objects)
        {
            if (appearTypeNumber == APPEAR)
            {
                obj.SetActive(true);
                StartCoroutine(obj.GetComponent<Spin2>().SetAppearOrDisappear(true));
            }
            else
                StartCoroutine(obj.GetComponent<Spin2>().SetAppearOrDisappear(false));
        }
        go_Objects = null;
        appearTypeNumber = NONE;
    }

    void ChangeSprite()
    {
        if (dialogues[lineCount].tf_Target == null) return;
        string spriteName = dialogues[lineCount].spriteName[contextCount];
        Debug.Log($"현재 tf_Target: {dialogues[lineCount].tf_Target.name}");
        if (!string.IsNullOrEmpty(spriteName))
            StartCoroutine(theSpriteManager.SpriteChangeCoroutine(
                dialogues[lineCount].tf_Target,
                spriteName.Split('/')[0]
            ));
    }

    void PlaySound()
    {
        string vn = dialogues[lineCount].VoiceName[contextCount];
        if (!string.IsNullOrEmpty(vn))
            SoundManager.instance.PlaySound(vn, 2);
    }

    IEnumerator TypeWriter()
    {
        if (dialogues == null || dialogues.Length == 0)
        {
            Debug.LogError("대화 데이터가 비어 있습니다.");
            yield break;
        }
        if (lineCount >= dialogues.Length)
        {
            Debug.LogWarning("lineCount가 배열 범위를 벗어났습니다.");
            yield break;
        }

        SettingUI(true);
        ChangeSprite();
        PlaySound();

        string txt = dialogues[lineCount].contexts[contextCount]
            .Replace("'", ",")
            .Replace("\\n", "\n");

        bool white = false, yellow = false, cyan = false, red = false, ignore = false;
        foreach (char c in txt)
        {
            switch (c)
            {
                case 'ⓦ': white = true; yellow = cyan = red = false; ignore = true; break;
                case 'ⓨ': yellow = true; white = cyan = red = false; ignore = true; break;
                case 'ⓒ': cyan = true; white = yellow = red = false; ignore = true; break;
                case 'ⓡ': red = true; white = yellow = cyan = false; ignore = true; break;
                case '①': StartCoroutine(theSplashManager.Splash()); SoundManager.instance.PlaySound("Emotion1", 1); ignore = true; break;
                case '②': StartCoroutine(theSplashManager.Splash()); SoundManager.instance.PlaySound("Emotion2", 1); ignore = true; break;
                case '③': StartCoroutine(theSplashManager.Splash()); SoundManager.instance.PlaySound("Emotion3", 1); ignore = true; break;
                case '④': StartCoroutine(theSplashManager.Splash()); SoundManager.instance.PlaySound("Emotion4", 1); ignore = true; break;
                case '⑤': StartCoroutine(theSplashManager.Splash()); SoundManager.instance.PlaySound("Emotion5", 1); ignore = true; break;
            }

            if (!ignore)
            {
                string letter = c.ToString();
                if (white) letter = $"<color=#FFFFFF>{letter}</color>";
                if (yellow) letter = $"<color=#FFFF00>{letter}</color>";
                if (cyan) letter = $"<color=#42DEE3>{letter}</color>";
                if (red) letter = $"<color=#FF0000>{letter}</color>";
                txt_Dialogue.text += letter;
            }
            ignore = false;
            yield return new WaitForSeconds(textDelay);
        }

        isNext = true;
    }

    void SettingUI(bool flag)
    {
        go_DialogueBar.SetActive(flag);
        if (flag && !string.IsNullOrEmpty(dialogues[lineCount].name))
        {
            go_DialogueNameBar.SetActive(true);
            txt_Name.text = dialogues[lineCount].name;
        }
        else
        {
            go_DialogueNameBar.SetActive(false);
        }
    }

    public void JumpToDialogueBlock(int startID, int endID)
    {
        var newD = DatabaseManager.instance.GetDialogue(startID, endID);
        if (newD == null || newD.Length == 0)
        {
            Debug.LogError($"JumpToDialogueBlock 실패: {startID}~{endID}");
            return;
        }
        dialogues = newD;
        lineCount = contextCount = 0;
        Debug.Log($"JumpToDialogueBlock: {startID}→{endID}");
        StopAllCoroutines();
        StartCoroutine(TypeWriter());
    }
}
