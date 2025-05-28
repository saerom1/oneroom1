using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
   public static DatabaseManager instance;

    [SerializeField] string csv_FileName;

    Dictionary<int, Dialogue> dialogueDic = new Dictionary<int, Dialogue>(); // index�� �Է��ϸ� �� index�� �´� dialogue�� ��������.

    public bool[] eventFlags = new bool[100]; //��������

    public static bool isFinish = false;

   void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // �� ��ȯ �� ����.
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
            Destroy(gameObject);  // �̹� �����ϸ� �ߺ� ����
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
