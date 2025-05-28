using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject choicePanel;           // ������ ��ü �г� (Canvas ����)
    [SerializeField] private GameObject choiceButtonPrefab;      // ������ ��ư ������ (Button + TextMeshProUGUI ����)

    private ChoiceData[] choices;                                // CSV���� �Ľ��� ������ ������ �迭

    // DialogueManager�� �̱����� �ƴ� �Ϲ� ������� ����
    private DialogueManager dialogueManager;

    private void Awake()
    {
        // ������ �г� ��Ȱ��ȭ
        choicePanel.SetActive(false);

        // �� ���� DialogueManager�� ã���ϴ�.
        dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager�� ã�� �� �����ϴ�.");
        }
    }

    public void ShowChoices()
    {
        ChoiceParser parser = GetComponent<ChoiceParser>();
        if (parser == null)
        {
            Debug.LogError("ChoiceParser ������Ʈ�� ã�� �� �����ϴ�.");
            return;
        }
        // �μ��� �������� �ʰ� Parse()�� ȣ���մϴ�.
        choices = parser.Parse();
        DisplayChoices();
    }

    /// <summary>
    /// �Ľ̵� ������ �����͸� ������� ������ ��ư���� �����Ͽ� UI�� ǥ���մϴ�.
    /// </summary>
    private void DisplayChoices()
    {
        // ������ ������ ������ ��ư ����
        foreach (Transform child in choicePanel.transform)
        {
            Destroy(child.gameObject);
        }

        // �Ľ̵� ������ ������ �迭 ��ȸ�ϸ� ��ư ����
        foreach (ChoiceData choice in choices)
        {
            GameObject btnObj = Instantiate(choiceButtonPrefab, choicePanel.transform);
            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                btnText.text = choice.choiceText;
            }
            else
            {
                Debug.LogWarning("������ ��ư�� TextMeshProUGUI ������Ʈ�� ã�� �� �����ϴ�.");
            }

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnChoiceSelected(choice));
            }
        }

        // ������ �г� Ȱ��ȭ
        choicePanel.SetActive(true);
    }

    /// <summary>
    /// �� ������ targetCharacter�� CharacterStatus ������Ʈ�� ã�� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="characterID">CSV�� TargetCharacter �� (��: "nomookoon")</param>
    /// <returns>�ش� ĳ������ CharacterStatus, ������ null</returns>
    private CharacterStatus GetCharacterStatus(string characterID)
    {
        CharacterStatus[] statuses = FindObjectsOfType<CharacterStatus>();
        foreach (CharacterStatus status in statuses)
        {
            if (status.stats.characterID == characterID)
                return status;
        }
        Debug.LogError("�ش� ID�� CharacterStatus�� ã�� �� �����ϴ�: " + characterID);
        return null;
    }

    public void ShowChoicesForEvent(int eventID)
    {
        // ChoiceParser ������Ʈ�� ������ ��ü ������ �����͸� �Ľ��մϴ�.
        ChoiceParser parser = GetComponent<ChoiceParser>();
        if (parser == null)
        {
            Debug.LogError("ChoiceParser ������Ʈ�� ã�� �� �����ϴ�.");
            return;
        }

        // ��� �������� �Ľ��մϴ�.
        ChoiceData[] allChoices = parser.Parse();
        if (allChoices == null || allChoices.Length == 0)
        {
            Debug.LogWarning("������ CSV ���Ͽ� �����Ͱ� �����ϴ�.");
            return;
        }

        // ������ eventID�� �ش��ϴ� �������� ���͸��մϴ�.
        List<ChoiceData> filtered = new List<ChoiceData>();
        foreach (ChoiceData choice in allChoices)
        {
            if (choice.eventID == eventID)
            {
                filtered.Add(choice);
            }
        }

        if (filtered.Count == 0)
        {
            Debug.LogWarning("�̺�Ʈ ��ȣ " + eventID + "�� �ش��ϴ� �������� �����ϴ�.");
            return;
        }

        // ���͸��� ����� choices �迭�� �����ϰ� UI�� ǥ���մϴ�.
        choices = filtered.ToArray();
        DisplayChoices();
    }


    /// <summary>
    /// �������� ���õǾ��� �� ȣ��Ǵ� �Լ��Դϴ�.
    /// ���� �˻� �� ȿ�� ���� �� ����/���� �б� ������ �����մϴ�.
    /// </summary>
    /// <param name="choice">���õ� ������ ������</param>
    private void OnChoiceSelected(ChoiceData choice)
    {
        // ��� ĳ������ ����(Component) ��������
        CharacterStatus targetStatus = GetCharacterStatus(choice.targetCharacter);
        if (targetStatus == null)
        {
            Debug.LogError("��� ĳ���� ���¸� ã�� �� �����ϴ�: " + choice.targetCharacter);
            return;
        }

        bool isSuccess = true;
        if (targetStatus.stats.affinity < choice.affinityCondition)
            isSuccess = false;
        if (targetStatus.stats.mentalPower < choice.mentalPowerCondition)
            isSuccess = false;

        if (isSuccess)
        {
            Debug.Log("���� ����: " + choice.choiceText);
            targetStatus.stats.ModifyAffinity(choice.affinityEffect);
            targetStatus.stats.ModifyMentalPower(choice.mentalPowerEffect);
            // ���� �б�: ���� �б��� ��ȭ ��� ������ ��ȯ
            if (FindObjectOfType<DialogueManager>() is DialogueManager dialogueManager)
            {
                dialogueManager.JumpToDialogueBlock(choice.successStartDialogueID, choice.successEndDialogueID);
            }
            else
            {
                Debug.LogError("DialogueManager�� ã�� �� �����ϴ�.");
            }
        }
        else
        {
            Debug.Log("���� ����: " + choice.choiceText);
            // ���� �б�: ���� �б��� ��ȭ ��� ������ ��ȯ
            if (FindObjectOfType<DialogueManager>() is DialogueManager dialogueManager)
            {
                dialogueManager.JumpToDialogueBlock(choice.failureStartDialogueID, choice.failureEndDialogueID);
            }
            else
            {
                Debug.LogError("DialogueManager�� ã�� �� �����ϴ�.");
            }
        }

        // ������ UI ����
        choicePanel.SetActive(false);
    }
}
