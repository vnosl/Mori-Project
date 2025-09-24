using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DBManager : MonoBehaviour
{
    static public DBManager instance;

    public string csvFileName_NPC;
    public string csvFileName_Obect;
    public string csvFileName_Evnet_Town01_Ending01;
    public string csvFileName_Evnet_Town01_Ending02;

    public Dictionary<int, Dialogue> NPC_diaglogues_Dictionary = new Dictionary<int, Dialogue>();
    public Dictionary<int, Dialogue> Obect_diaglogues_Dictionary = new Dictionary<int, Dialogue>();

    public Dictionary<int, Event> Evnet_Town01_Ending01 = new Dictionary<int, Event>();
    public Dictionary<int, Event> Evnet_Town02_Ending02 = new Dictionary<int, Event>();

    int eventID;
    //List<int> idList01 = new List<int>(); //Start()에서 Debug할때 사용
    //List<int> idList02 = new List<int>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DialogueParser(csvFileName_Obect, false);
            DialogueParser(csvFileName_NPC, true);
            EventParser(csvFileName_Evnet_Town01_Ending01, 1, 1);
            EventParser(csvFileName_Evnet_Town01_Ending02, 1, 2);

        }
    }

    void Start()
    {
        //데이터 파싱이 잘됐는지  Debug.Log용

        /*
        for (int i = 0; i < idList01.Count; i++)
        {
            int id = idList01[i];
            string name = "";
            Dialogue dialogue = NPC_diaglogues_Dictionary[id];
            Debug.Log(id.ToString());

            List<List<Line>> lines = dialogue.lines;


            for (int j = 0; j < lines.Count; j++)
            {
                for (int k = 0; k < lines[j].Count; k++)
                {

                    for (int l = 0; l < lines[j][k].context.Length; l++)
                    {
                        Debug.Log("[" + j + "] [" + k + "] Context" + l);
                        if (name != lines[j][k].Name)
                        {
                            name = lines[j][k].Name;

                            Debug.Log(name);
                        }

                        Debug.Log(lines[j][k].context[l]);

                        //만약 선택지를 가지고 있다면..
                        if (l == (lines[j][k].context.Length - 1))
                        {
                            if (lines[j][k].haveChoice)
                            {
                                Debug.Log("선택지");
                                Debug.Log(lines[j][k].choice.firstOption);
                                Debug.Log(lines[j][k].choice.firstOptDialogNum.ToString());
                                Debug.Log(lines[j][k].choice.secondOption);
                                Debug.Log(lines[j][k].choice.secondOptDialogNum.ToString());
                            }
                        }
                    }
                }
            }
        }
        Debug.Log("----------------------------------------------------------------------------------------- ");
        //---------------------------------------------------------------------------------------------- 
        for (int i = 0; i < idList02.Count; i++)
        {
            int id = idList02[i];
            string name = "";
            Dialogue dialogue = Obect_diaglogues_Dictionary[id];
            Debug.Log(id.ToString());

            List<List<Line>> lines = dialogue.lines;


            for (int j = 0; j < lines.Count; j++)
            {
                for (int k = 0; k < lines[j].Count; k++)
                {

                    for (int l = 0; l < lines[j][k].context.Length; l++)
                    {
                        Debug.Log("[" + j + "] [" + k + "] Context" + l);
                        if (name != lines[j][k].Name)
                        {
                            name = lines[j][k].Name;

                            Debug.Log(name);
                        }

                        Debug.Log(lines[j][k].context[l]);

                        //만약 선택지를 가지고 있다면..
                        if (l == (lines[j][k].context.Length - 1))
                        {
                            if (lines[j][k].haveChoice)
                            {
                                Debug.Log("선택지");
                                Debug.Log(lines[j][k].choice.firstOption);
                                Debug.Log(lines[j][k].choice.firstOptDialogNum.ToString());
                                Debug.Log(lines[j][k].choice.secondOption);
                                Debug.Log(lines[j][k].choice.secondOptDialogNum.ToString());
                            }
                        }
                    }
                }



            }

        }
        */
    }

    static public DBManager GetInstance()
    {
        return instance;
    }

    public void DialogueParser(string csvFileName, bool isNPC)
    {
        DialogueParser dialogueParser = GetComponent<DialogueParser>();
        Dialogue[] dialogues = dialogueParser.DialogueParse(csvFileName);

        for (int i = 0; i < dialogues.Length; i++)
        {
            int id = 0;
            string id_String = "";

            id_String += dialogues[i].endingNum.ToString();

            id_String += dialogues[i].townNum.ToString();


            if (dialogues[i].npcNum.ToString().Length == 1)
                id_String += "0" + dialogues[i].npcNum.ToString();
            else
                id_String += dialogues[i].npcNum.ToString();

            if (dialogues[i].eventNum.ToString().Length == 1)
                id_String += "000" + dialogues[i].eventNum.ToString();
            else if (dialogues[i].eventNum.ToString().Length == 2)
                id_String += "00" + dialogues[i].eventNum.ToString();
            else if (dialogues[i].eventNum.ToString().Length == 3)
                id_String += "0" + dialogues[i].eventNum.ToString();
            else
                id_String += dialogues[i].eventNum.ToString();

            if (dialogues[i].dialogueNum.ToString().Length == 1)
                id_String += "0" + dialogues[i].dialogueNum.ToString();
            else
                id_String += dialogues[i].dialogueNum.ToString();

            id = int.Parse(id_String);

            if (isNPC)
            {
                //idList01.ADD(id) //Start()에서 Debug용
                NPC_diaglogues_Dictionary[id] = dialogues[i];
            }
            if (!isNPC)
            {
                Obect_diaglogues_Dictionary[id] = dialogues[i];
            }

        }
        Debug.Log(csvFileName + "완료!");

    }

    void EventParser(string csvFileName, int townNum, int endingNum)
    {
        DialogueParser dialogueParser = GetComponent<DialogueParser>();
        Event[] events = dialogueParser.EventParse(csvFileName);

        for (int i = 0; i < events.Length; i++)
        {
            //1 01  17 0001 01 01
            int id = 0;
            id = events[i].eventId;
            switch (townNum)
            {
                case 1: //첫번째 마을
                    if (endingNum == 1)
                    {
                        //첫번째 마을의 Ending이 1인경우..
                        Evnet_Town01_Ending01[id] = events[i];
                    }
                    else if (endingNum == 2)
                    {
                        //첫번째 마을의 Ending이 2인경우..
                        Evnet_Town02_Ending02[id] = events[i];
                    }

                    break;
                default:
                    break;
            }

        }
        Debug.Log(csvFileName + "완료!");
    }


}
