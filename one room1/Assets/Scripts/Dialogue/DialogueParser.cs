using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.FlowStateWidget;

public class DialogueParser : MonoBehaviour
{
   public Dialogue[] Parse(string _CSVFileName)
    {
        List<Dialogue> dialogueList = new List<Dialogue>(); // ��� ����Ʈ ����
        TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName); //csv���� ������

        string[] data = csvData.text.Split(new char[] { '\n' }); // ���� �������� �ɰ�.
        
        for(int i = 1; i < data.Length;) 
        {
            string[] row = data[i].Split(new char[] {','}); // ,(�޸�) ������ �ɰ����� row�� ��, �׷� �������� i��° ���� row�� �Ͻ� ���� �ǰ��� id,ĳ���� �̸�, ��� 3���� ������ �迭�� ��
 
            Dialogue dialogue = new Dialogue(); // ��� ����Ʈ ����

            dialogue.name = row[1];
            
            
            List<string> contextList = new List<string>();
            List<string> spriteList = new List<string>();
            List<string> voicelist = new List<string>();

            List<string> EventList = new List<string>(); // �̺�Ʈ �ѹ� ����
            List<string> SkipList = new List<string>(); // ���� �ǳ��� ��� �߰� ���ϸ� ������


            do
            {
                contextList.Add(row[2]);
                spriteList.Add(row[3]);
                voicelist.Add(row[4]);

                EventList.Add(row[5]);
                SkipList.Add(row[6]);

                // End ���� ��ŵ���� �� �ٷ� ���� ���̶�� ����
                if (row.Length > 7)
                {
                    int.TryParse(row[7], out dialogue.end);
                }
                else
                {
                    dialogue.end = 0; // �⺻�� 0 (��ȭ ���)
                }

                if (++i < data.Length) // i�� �̸� ������ ���¿��� �����ش� dataLentg���� �۴ٸ�
                {
                    row = data[i].Split(new char[] { ',' });
                }
                else 
                {
                    break;
                }
            } while (row[0].ToString() == "");      // ���� 1ȸ ���� �� ���� �� ���� �����Ű�� ���ǹ��� ��
                                                    // row 0��° �ٿ��� ID�� �� �ְ� Tostring���� �� �������� ������
            dialogue.contexts = contextList.ToArray();
            dialogue.spriteName = spriteList.ToArray();
            dialogue.VoiceName = voicelist.ToArray();

            dialogue.number = EventList.ToArray();
            dialogue.skipnum = SkipList.ToArray();

            dialogueList.Add(dialogue);

            //GameObject obj = GameObject.Find("DialgoueManager");
            //obj.GetComponent<interactionEvent>().lineY = dialgoueList.Count; //�̰� �־ �Ǵ��� Ȯ�žȵ�


        }
        return dialogueList.ToArray();
    }

   
}
