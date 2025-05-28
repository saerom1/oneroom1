using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject choicePanel;           // 선택지 전체 패널 (Canvas 하위)
    [SerializeField] private GameObject choiceButtonPrefab;      // 선택지 버튼 프리팹 (Button + TextMeshProUGUI 포함)

    private ChoiceData[] choices;                                // CSV에서 파싱한 선택지 데이터 배열

    // DialogueManager를 싱글톤이 아닌 일반 방식으로 참조
    private DialogueManager dialogueManager;

    private void Awake()
    {
        // 선택지 패널 비활성화
        choicePanel.SetActive(false);

        // 씬 내의 DialogueManager를 찾습니다.
        dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager를 찾을 수 없습니다.");
        }
    }

    public void ShowChoices()
    {
        ChoiceParser parser = GetComponent<ChoiceParser>();
        if (parser == null)
        {
            Debug.LogError("ChoiceParser 컴포넌트를 찾을 수 없습니다.");
            return;
        }
        // 인수를 전달하지 않고 Parse()를 호출합니다.
        choices = parser.Parse();
        DisplayChoices();
    }

    /// <summary>
    /// 파싱된 선택지 데이터를 기반으로 선택지 버튼들을 생성하여 UI에 표시합니다.
    /// </summary>
    private void DisplayChoices()
    {
        // 기존에 생성된 선택지 버튼 삭제
        foreach (Transform child in choicePanel.transform)
        {
            Destroy(child.gameObject);
        }

        // 파싱된 선택지 데이터 배열 순회하며 버튼 생성
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
                Debug.LogWarning("선택지 버튼에 TextMeshProUGUI 컴포넌트를 찾을 수 없습니다.");
            }

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnChoiceSelected(choice));
            }
        }

        // 선택지 패널 활성화
        choicePanel.SetActive(true);
    }

    /// <summary>
    /// 씬 내에서 targetCharacter의 CharacterStatus 컴포넌트를 찾아 반환합니다.
    /// </summary>
    /// <param name="characterID">CSV의 TargetCharacter 값 (예: "nomookoon")</param>
    /// <returns>해당 캐릭터의 CharacterStatus, 없으면 null</returns>
    private CharacterStatus GetCharacterStatus(string characterID)
    {
        CharacterStatus[] statuses = FindObjectsOfType<CharacterStatus>();
        foreach (CharacterStatus status in statuses)
        {
            if (status.stats.characterID == characterID)
                return status;
        }
        Debug.LogError("해당 ID의 CharacterStatus를 찾을 수 없습니다: " + characterID);
        return null;
    }

    public void ShowChoicesForEvent(int eventID)
    {
        // ChoiceParser 컴포넌트를 가져와 전체 선택지 데이터를 파싱합니다.
        ChoiceParser parser = GetComponent<ChoiceParser>();
        if (parser == null)
        {
            Debug.LogError("ChoiceParser 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        // 모든 선택지를 파싱합니다.
        ChoiceData[] allChoices = parser.Parse();
        if (allChoices == null || allChoices.Length == 0)
        {
            Debug.LogWarning("선택지 CSV 파일에 데이터가 없습니다.");
            return;
        }

        // 지정된 eventID에 해당하는 선택지만 필터링합니다.
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
            Debug.LogWarning("이벤트 번호 " + eventID + "에 해당하는 선택지가 없습니다.");
            return;
        }

        // 필터링된 결과를 choices 배열에 저장하고 UI를 표시합니다.
        choices = filtered.ToArray();
        DisplayChoices();
    }


    /// <summary>
    /// 선택지가 선택되었을 때 호출되는 함수입니다.
    /// 조건 검사 및 효과 적용 후 성공/실패 분기 로직을 실행합니다.
    /// </summary>
    /// <param name="choice">선택된 선택지 데이터</param>
    private void OnChoiceSelected(ChoiceData choice)
    {
        // 대상 캐릭터의 상태(Component) 가져오기
        CharacterStatus targetStatus = GetCharacterStatus(choice.targetCharacter);
        if (targetStatus == null)
        {
            Debug.LogError("대상 캐릭터 상태를 찾을 수 없습니다: " + choice.targetCharacter);
            return;
        }

        bool isSuccess = true;
        if (targetStatus.stats.affinity < choice.affinityCondition)
            isSuccess = false;
        if (targetStatus.stats.mentalPower < choice.mentalPowerCondition)
            isSuccess = false;

        if (isSuccess)
        {
            Debug.Log("선택 성공: " + choice.choiceText);
            targetStatus.stats.ModifyAffinity(choice.affinityEffect);
            targetStatus.stats.ModifyMentalPower(choice.mentalPowerEffect);
            // 성공 분기: 성공 분기의 대화 블록 범위로 전환
            if (FindObjectOfType<DialogueManager>() is DialogueManager dialogueManager)
            {
                dialogueManager.JumpToDialogueBlock(choice.successStartDialogueID, choice.successEndDialogueID);
            }
            else
            {
                Debug.LogError("DialogueManager를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.Log("선택 실패: " + choice.choiceText);
            // 실패 분기: 실패 분기의 대화 블록 범위로 전환
            if (FindObjectOfType<DialogueManager>() is DialogueManager dialogueManager)
            {
                dialogueManager.JumpToDialogueBlock(choice.failureStartDialogueID, choice.failureEndDialogueID);
            }
            else
            {
                Debug.LogError("DialogueManager를 찾을 수 없습니다.");
            }
        }

        // 선택지 UI 숨김
        choicePanel.SetActive(false);
    }
}
