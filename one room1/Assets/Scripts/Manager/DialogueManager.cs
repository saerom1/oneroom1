using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro Ŭ������ �������� ���� �����ؾ���

public class DialogueManager : MonoBehaviour
{
    public static bool isWating = false;

    [SerializeField] GameObject go_DialogueBar;
    [SerializeField] GameObject go_DialogueNameBar;

    [SerializeField] TextMeshProUGUI txt_Dialogue;
    [SerializeField] TextMeshProUGUI txt_Name;


    Dialogue[] dialogues;

    bool isDialogue = false; //���� ��ȭ������ �ƴ��� ���� �Ǻ����� ����
    bool isNext = false; // Ư�� Ű �Է� ���

    [Header("TextDelay")]
    [SerializeField] float textDelay;

    public int lineCount = 0; // ��ȭ ī��Ʈ
    public int contextCount = 0; // ��� ī��Ʈ.

    // ���� �̺�Ʈ�� ���� ����
    GameObject go_NextEvent;

    public void SetNextEvent(GameObject p_NextEvent)
    {
        go_NextEvent = p_NextEvent;
    }


    // �̺�Ʈ ������ �����ų (Ȥ�� �����ų) ������Ʈ��
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
        if (isDialogue)
        {
            if (isNext)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButtonDown(0))
                {
                    isNext = false;
                    txt_Dialogue.text = "";

                    // ��ȭ ���� ���� ���� ��翡 ������ �̺�Ʈ ��ȣ�� �ִ��� �˻�
                    // ���÷�, Dialogue.number �迭�� ù ��° ��Ҹ� ����մϴ�.
                    if (dialogues[lineCount].number != null &&
                        dialogues[lineCount].number.Length > 0 &&
                        !string.IsNullOrEmpty(dialogues[lineCount].number[0]))
                    {
                        // ������ �̺�Ʈ�� ����: �̺�Ʈ ��ȣ�� ������ ��ȯ
                        int eventID;
                        if (int.TryParse(dialogues[lineCount].number[0], out eventID))
                        {
                            // ChoiceManager�� ������ UI ȣ�� (���⼭�� ShowChoicesForEvent �޼��带 ���)
                            ChoiceManager choiceManager = FindObjectOfType<ChoiceManager>();
                            if (choiceManager != null)
                            {
                                choiceManager.ShowChoicesForEvent(eventID);
                            }
                            else
                            {
                                Debug.LogError("ChoiceManager�� ã�� �� �����ϴ�.");
                            }
                            // ������ UI ȣ�� �� ��ȭ ������ �ߴ��մϴ�.
                            return;
                        }
                        else
                        {
                            Debug.LogWarning("�̺�Ʈ ��ȣ �Ľ̿� �����߽��ϴ�: " + dialogues[lineCount].number[0]);
                        }
                    }

                    // ���� ������ ���� �� ���� ������ ���� �ε��� ����
                    if (++contextCount < dialogues[lineCount].contexts.Length)
                    {
                        StartCoroutine(TypeWriter());
                    }
                    else
                    {
                        contextCount = 0;
                        // ��ŵ���� ó���� �ʿ��� ��� (��: CSV���� ���� ��� ID ����)
                        // ���� ��ŵ���� ������ �ִٸ� �� ������ JumpToDialogue�� ȣ���� ���� ����.
                        if (++lineCount < dialogues.Length)
                        {
                            // CameraTargettingType()�� ���� ī�޶� ���� �� ���� ��縦 ���
                            StartCoroutine(CameraTargettingType());
                        }
                        else//��ȭ�� �������
                        {
                            StartCoroutine(EndDialogue());
                        }
                    }

                }
            }
        }
    }

    public void ShowDialogue(Dialogue[] p_dialogues)
    {
        isDialogue = true; //��ȭ�� �������̸� �������̴ٶ�� ��������� ǥ��
        txt_Dialogue.text = "";
        txt_Name.text = "";   //Ȥ�ø� ���� �ܿ����� Ȯ���� ó���ϱ� ���� �ʱ�ȭ
        theIC.SettingUI(false);
        dialogues = p_dialogues;

        StartCoroutine(StartDialogue());
    }

    IEnumerator StartDialogue()
    {
        if (isWating)
            yield return new WaitForSeconds(0.5f);
        isWating = false;
        theCam.CamOriginSetting();
        StartCoroutine(CameraTargettingType());
    }

    IEnumerator CameraTargettingType() //ī�޶� Ÿ�� �з��ϴ°�
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
                StartCoroutine(theCutSceneManager.CutSceneCoroutine(dialogues[lineCount].spriteName[contextCount], true));
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
                StartCoroutine(theSlideManager.AppearSlide(dialogues[lineCount].spriteName[contextCount].Split(new char[] { '/' })[1]));
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
                StartCoroutine(theSlideManager.ChangeSlide(dialogues[lineCount].spriteName[contextCount].Split(new char[] { '/' })[1]));
                yield return new WaitUntil(() => SlideManager.isChanged);
                theCam.CameraTargetting(dialogues[lineCount].tf_Target);
                break;
        }
        StartCoroutine(TypeWriter());
    }

    IEnumerator EndDialogue()
    {
        SettingUI(false);

        // ������ �ε��� ���: �迭�� ������� �ʴٸ� ������ �ε����� ���
        if (dialogues != null && dialogues.Length > 0)
        {
            int lastIndex = Mathf.Clamp(lineCount, 0, dialogues.Length - 1);
            if (dialogues[lastIndex].end == 1)
            {
                isDialogue = false;
                yield break;
            }
        }
        else
        {
            Debug.LogWarning("��ȭ �迭�� ����ֽ��ϴ�.");
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

        // --- ��ȭ ���� �ø��� �ڵ� �̺�Ʈ ��� üũ ȣ�� ---
        foreach (var ie in FindObjectsOfType<InteractionEvent>())
            ie.TryTriggerAutoOnDialogueEnd();
    }

    void AppearOrDisappearObjects()
    {
        if (go_Objects != null)
        {
            Spin2.isFinished = true;
            for (int i = 0; i < go_Objects.Length; i++)
            {
                if (appearTypeNumber == APPEAR)
                {
                    go_Objects[i].SetActive(true);
                    StartCoroutine(go_Objects[i].GetComponent<Spin2>().SetAppearOrDisappear(true));
                }
                else if (appearTypeNumber == DISAPPEAR)
                    StartCoroutine(go_Objects[i].GetComponent<Spin2>().SetAppearOrDisappear(false));
            }
        }
        go_Objects = null;
        appearTypeNumber = NONE;
    }

    void ChangeSprite()
    {
        if (dialogues[lineCount].tf_Target != null)
        {
            string spriteName = dialogues[lineCount].spriteName[contextCount];
            Debug.Log($"���� tf_Target: {dialogues[lineCount].tf_Target.name}");
            if (dialogues[lineCount].spriteName[contextCount] != "")
            {
                StartCoroutine(theSpriteManager.SpriteChangeCoroutine(
                    dialogues[lineCount].tf_Target,
                    dialogues[lineCount].spriteName[contextCount].Split(new char[] { '/' })[0]
                ));
            }
        }
    }

    void PlaySound()
    {
        if (dialogues[lineCount].VoiceName[contextCount] != "")
        {
            SoundManager.instance.PlaySound(
                dialogues[lineCount].VoiceName[contextCount], 2
            );
        }
    }

    IEnumerator TypeWriter()
    {
        // ���� ���� ���� ��ȭ�� ���� ��� ����
        if (dialogues == null || dialogues.Length == 0)
        {
            Debug.LogError("��ȭ �����Ͱ� ��� �ֽ��ϴ�.");
            yield break;
        }

        // ���� lineCount�� �迭 ������ �ʰ��ϸ� �����ϰ� ����
        if (lineCount >= dialogues.Length)
        {
            Debug.LogWarning("lineCount�� �迭 ������ ������ϴ�. ��ȭ�� �����մϴ�.");
            yield break;
        }

        SettingUI(true);
        ChangeSprite();
        PlaySound();

        string t_ReplaceText = dialogues[lineCount].contexts[contextCount];
        t_ReplaceText = t_ReplaceText.Replace("'", ","); //'�� �޸���
        t_ReplaceText = t_ReplaceText.Replace("\\n", "\n");

        bool t_white = false, t_yellow = false, t_cyan = false, t_red = false;
        bool t_ignore = false;

        for (int i = 0; i < t_ReplaceText.Length; i++)
        {
            switch (t_ReplaceText[i])
            {
                case '��': t_white = true; t_yellow = false; t_cyan = false; t_red = false; t_ignore = true; break;
                case '��': t_white = false; t_yellow = true; t_cyan = false; t_red = false; t_ignore = true; break;
                case '��': t_white = false; t_yellow = false; t_cyan = true; t_red = false; t_ignore = true; break;
                case '��': t_white = false; t_yellow = false; t_cyan = false; t_red = true; t_ignore = true; break;
                case '��': StartCoroutine(theSplashManager.Splash()); SoundManager.instance.PlaySound("Emotion1", 1); t_ignore = true; break;
                case '��': StartCoroutine(theSplashManager.Splash()); SoundManager.instance.PlaySound("Emotion2", 1); t_ignore = true; break;
                case '��': StartCoroutine(theSplashManager.Splash()); SoundManager.instance.PlaySound("Emotion3", 1); t_ignore = true; break;
                case '��': StartCoroutine(theSplashManager.Splash()); SoundManager.instance.PlaySound("Emotion4", 1); t_ignore = true; break;
                case '��': StartCoroutine(theSplashManager.Splash()); SoundManager.instance.PlaySound("Emotion5", 1); t_ignore = true; break;
            }

            string t_letter = t_ReplaceText[i].ToString();

            if (!t_ignore)
            {
                if (t_white)
                    t_letter = "<color=#FFFFFF>" + t_letter + "</color>";
                else if (t_yellow)
                    t_letter = "<color=#FFFF00>" + t_letter + "</color>";
                else if (t_cyan)
                    t_letter = "<color=#42DEE3>" + t_letter + "</color>";
                else if (t_red)
                    t_letter = "<color=#FF0000>" + t_letter + "</color>";

                txt_Dialogue.text += t_letter;
            }
            t_ignore = false;

            yield return new WaitForSeconds(textDelay);
        }

        isNext = true;
    }

    void SettingUI(bool p_flag)  // UI Ȱ��/��Ȱ��
    {
        go_DialogueBar.SetActive(p_flag);

        if (p_flag)
        {
            if (dialogues[lineCount].name == "")
            {
                go_DialogueNameBar.SetActive(false);
            }
            else
            {
                go_DialogueNameBar.SetActive(true);
                txt_Name.text = dialogues[lineCount].name;
            }
        }
        else
        {
            go_DialogueNameBar.SetActive(false);
        }
    }

    public void JumpToDialogueBlock(int startDialogueID, int endDialogueID)
    {
        Dialogue[] newDialogues = DatabaseManager.instance.GetDialogue(startDialogueID, endDialogueID);
        if (newDialogues == null || newDialogues.Length == 0)
        {
            Debug.LogError("JumpToDialogueBlock: ��ȭ ����� ã�� �� �����ϴ�. ����: "
                + startDialogueID + ", ��: " + endDialogueID);
            return;
        }
        dialogues = newDialogues;
        lineCount = 0;
        contextCount = 0;
        Debug.Log("JumpToDialogueBlock: ��ȭ ��� ��ȯ ���� ��ȣ: "
            + startDialogueID + ", �� ��ȣ: " + endDialogueID);
        StopAllCoroutines();
        StartCoroutine(TypeWriter());
    }
}
