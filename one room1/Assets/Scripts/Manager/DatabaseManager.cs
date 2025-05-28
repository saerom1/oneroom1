using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
   public static DatabaseManager instance;

    [SerializeField] string csv_FileName;

    Dictionary<int, Dialogue> dialogueDic = new Dictionary<int, Dialogue>(); // index를 입력하면 그 index에 맞는 dialogue가 꺼내진다.

    public bool[] eventFlags = new bool[100]; //변수개수

    public static bool isFinish = false;

   void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 전환 시 유지.
            DialogueParser theParser = GetComponent<DialogueParser>();
            Dialogue[] dialogues = theParser.Parse(csv_FileName);

            for(int i = 0; i < dialogues.Length; i++) 
            {
                dialogueDic.Add(i+1, dialogues[i]);
            }
            isFinish = true;
        }
        else
        {
            Destroy(gameObject);  // 이미 존재하면 중복 제거
        }
    }

    public Dialogue[] GetDialogue(int _StartNum, int _EndNum)
    {
        List<Dialogue> dialogueList = new List<Dialogue>();

        for(int i = 0;i <= _EndNum - _StartNum;i++) 
        {
            dialogueList.Add(dialogueDic[_StartNum + i]);
        }

        return dialogueList.ToArray();
    }
}
