using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.FlowStateWidget;

public class DialogueParser : MonoBehaviour
{
   public Dialogue[] Parse(string _CSVFileName)
    {
        List<Dialogue> dialogueList = new List<Dialogue>(); // 대사 리스트 생성
        TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName); //csv파일 가져옴

        string[] data = csvData.text.Split(new char[] { '\n' }); // 엔터 기준으로 쪼갬.
        
        for(int i = 1; i < data.Length;) 
        {
            string[] row = data[i].Split(new char[] {','}); // ,(콤마) 단위로 쪼개져서 row에 들어감, 그럼 데이터의 i번째 줄이 row에 싹싹 들어가게 되겠지 id,캐릭터 이름, 대사 3개의 내용이 배열에 들어감
 
            Dialogue dialogue = new Dialogue(); // 대사 리스트 생성

            dialogue.name = row[1];
            
            
            List<string> contextList = new List<string>();
            List<string> spriteList = new List<string>();
            List<string> voicelist = new List<string>();

            List<string> EventList = new List<string>(); // 이벤트 넘버 생성
            List<string> SkipList = new List<string>(); // 엑셀 맨끝줄 비고 추가 안하면 오류남


            do
            {
                contextList.Add(row[2]);
                spriteList.Add(row[3]);
                voicelist.Add(row[4]);

                EventList.Add(row[5]);
                SkipList.Add(row[6]);

                // End 열은 스킵라인 열 바로 다음 열이라고 가정
                if (row.Length > 7)
                {
                    int.TryParse(row[7], out dialogue.end);
                }
                else
                {
                    dialogue.end = 0; // 기본값 0 (대화 계속)
                }

                if (++i < data.Length) // i가 미리 증가한 상태에서 비교해준다 dataLentg보다 작다면
                {
                    row = data[i].Split(new char[] { ',' });
                }
                else 
                {
                    break;
                }
            } while (row[0].ToString() == "");      // 최초 1회 조건 비교 없이 한 차례 실행시키고 조건문을 비교
                                                    // row 0번째 줄에는 ID가 들어가 있고 Tostring으로 빈 공간인지 비교해줌
            dialogue.contexts = contextList.ToArray();
            dialogue.spriteName = spriteList.ToArray();
            dialogue.VoiceName = voicelist.ToArray();

            dialogue.number = EventList.ToArray();
            dialogue.skipnum = SkipList.ToArray();

            dialogueList.Add(dialogue);

            //GameObject obj = GameObject.Find("DialgoueManager");
            //obj.GetComponent<interactionEvent>().lineY = dialgoueList.Count; //이건 넣어도 되는지 확신안됨


        }
        return dialogueList.ToArray();
    }

   
}
