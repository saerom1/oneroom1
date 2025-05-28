using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceParser : MonoBehaviour
{

    [SerializeField] private string choiceCsvFileName; // Inspector에서 선택지 CSV 파일 이름 입력

    public ChoiceData[] Parse()
    {
        List<ChoiceData> choices = new List<ChoiceData>();
        TextAsset csvData = Resources.Load<TextAsset>(choiceCsvFileName);

        if (csvData == null)
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다: " + choiceCsvFileName);
            return choices.ToArray();
        }

        string[] lines = csvData.text.Split(new char[] { '\n' });
        // 첫 줄은 헤더라고 가정
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;
            string[] row = lines[i].Split(new char[] { ',' });
            if (row.Length < 12) // 예상되는 컬럼 수가 12개 미만이면 무시
                continue;

            // 각 요소에 대해 Trim()을 추가하면 공백 문제가 줄어듭니다.
            for (int j = 0; j < row.Length; j++)
            {
                row[j] = row[j].Trim();
            }

            ChoiceData choice = new ChoiceData();
            int.TryParse(row[0], out choice.eventID);
            int.TryParse(row[1], out choice.choiceNumber);
            choice.choiceText = row[2];
            choice.targetCharacter = row[3];

            int.TryParse(row[4], out choice.affinityEffect);
            int.TryParse(row[5], out choice.mentalPowerEffect);
            int.TryParse(row[6], out choice.affinityCondition);
            int.TryParse(row[7], out choice.mentalPowerCondition);

            //기존 choiceParser에서 수정해서 추가함
            int.TryParse(row[8], out choice.successStartDialogueID);
            int.TryParse(row[9], out choice.successEndDialogueID);
            int.TryParse(row[10], out choice.failureStartDialogueID);
            int.TryParse(row[11], out choice.failureEndDialogueID);

            choices.Add(choice);
        }
        return choices.ToArray();
    }
}