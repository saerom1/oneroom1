using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceParser : MonoBehaviour
{

    [SerializeField] private string choiceCsvFileName; // Inspector���� ������ CSV ���� �̸� �Է�

    public ChoiceData[] Parse()
    {
        List<ChoiceData> choices = new List<ChoiceData>();
        TextAsset csvData = Resources.Load<TextAsset>(choiceCsvFileName);

        if (csvData == null)
        {
            Debug.LogError("CSV ������ ã�� �� �����ϴ�: " + choiceCsvFileName);
            return choices.ToArray();
        }

        string[] lines = csvData.text.Split(new char[] { '\n' });
        // ù ���� ������ ����
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;
            string[] row = lines[i].Split(new char[] { ',' });
            if (row.Length < 12) // ����Ǵ� �÷� ���� 12�� �̸��̸� ����
                continue;

            // �� ��ҿ� ���� Trim()�� �߰��ϸ� ���� ������ �پ��ϴ�.
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

            //���� choiceParser���� �����ؼ� �߰���
            int.TryParse(row[8], out choice.successStartDialogueID);
            int.TryParse(row[9], out choice.successEndDialogueID);
            int.TryParse(row[10], out choice.failureStartDialogueID);
            int.TryParse(row[11], out choice.failureEndDialogueID);

            choices.Add(choice);
        }
        return choices.ToArray();
    }
}