using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueParser : MonoBehaviour
{
    int endingNum_in = 0;
    int townNum_in = 0;
    int npcNum_in = 0;
    int eventNum_in = 0;
    int dialogueNum_in = 0;

    string name_in;

    int choice_OneTwo = 0;

    bool startChoice = false;

    bool finishBreak = false;

    void Initialization()
    {
        endingNum_in = 0;
        townNum_in = 0;
        npcNum_in = 0;
        eventNum_in = 0;
        dialogueNum_in = 0;

        name_in = "";

        choice_OneTwo = 0;

        startChoice = false;

        finishBreak = false;
    }

    public Dialogue[] DialogueParse(string csvFileName)
    {
        Initialization(); // 변수 초기화 작업

        List<Dialogue> dialogues = new List<Dialogue>(); // Dialogue값을 리턴하기 위해서 List로 만듦

        TextAsset csvData = Resources.Load<TextAsset>(csvFileName);//csv파일 가져오기
        string[] data = csvData.text.Split(new char[] { '\n' });

        for (int i = 1; i < (data.Length);)
        {
            //i가 1부터 시작하는 이유 data[0]에는 헤더값이 들어있기 때문이다.
            //data를 , 로 나누어준다.
            string[] row = data[i].Split(new char[] { ',' });

            Dialogue dialogue = new Dialogue();
            List<List<Line>> lines = new List<List<Line>>();

            if (row[3].ToString() == "")
            {
                dialogue.endingNum = endingNum_in; //엔딩번호
                dialogue.townNum = townNum_in;     //마을번호
                dialogue.npcNum = npcNum_in;       //npc or object 번호
                dialogue.eventNum = eventNum_in;   //현재 진행회고 있는 이벤트
            }
            else
            {
                endingNum_in = int.Parse(row[0].ToString());
                townNum_in = int.Parse(row[1].ToString());
                npcNum_in = int.Parse(row[2].ToString());
                eventNum_in = int.Parse(row[3].ToString());

                dialogue.endingNum = endingNum_in; //엔딩번호
                dialogue.townNum = townNum_in;     //마을번호
                dialogue.npcNum = npcNum_in;       //npc or object 번호
                dialogue.eventNum = eventNum_in;   //현재 진행회고 있는 이벤트
            }

            if (row[4].ToString() == "")
            {
                dialogue.dialogueNum = dialogueNum_in;
            }
            else
            {
                dialogueNum_in = int.Parse(row[4].ToString());
                dialogue.dialogueNum = dialogueNum_in;
            }

            do
            {
                List<Line> LineList = new List<Line>();
                List<string> contextList = new List<string>();  //Line.cs의 context[]에 넣기 위함

                startChoice = false;
                choice_OneTwo = 0;

                do
                {
                    Line line = new Line();// 반복 돌때마다 새로운 Line()

                    line.haveChoice = false; // 라인 초기화

                    //대화가 끝났는지 여부
                    line.isFinishLine = false;
                    line.nextDialogueNum = dialogueNum_in;

                    do
                    {
                        if (!startChoice)// 선택지가 없을 경우
                        {
                            if (row[6].ToString() != "")
                            {
                                name_in = row[6].ToString();
                            }
                            line.Name = name_in;

                            contextList.Add(row[7].ToString());
                        }
                        else if (startChoice)// 선택지가 있을 경우
                        {
                            choice_OneTwo++;

                            line.haveChoice = true;
                            if (choice_OneTwo == 1)
                            {
                                //첫번째 질문인가
                                line.choice = new Choice();
                                line.choice.firstOption = row[9].ToString();
                                line.choice.firstOptDialogNum = int.Parse(row[10].ToString());

                                if (row[11].ToString() != "")
                                {
                                    line.choice.firstOptOpenObject = true;
                                    line.choice.firstOptObjectName = row[11].ToString();
                                }
                                else
                                {
                                    line.choice.firstOptOpenObject = false;
                                    line.choice.firstOptObjectName = "";
                                }
                            }
                            else if (choice_OneTwo == 2)
                            {
                                //두번째 질문인가
                                line.choice.secondOption = row[9].ToString();
                                line.choice.secondOptDialogNum = int.Parse(row[10].ToString());

                                //씬 이동이 있는지 확인
                                if (row[11].ToString() != "")
                                {
                                    line.choice.secondOptOpenObject = true;
                                    line.choice.secondOptOptObjectName = row[11].ToString();
                                }
                                else
                                {
                                    line.choice.secondOptOpenObject = false;
                                    line.choice.secondOptOptObjectName = "";
                                }
                            }
                            else if (choice_OneTwo == 3)
                            {
                                //세번째 질문인가
                                line.choice.thirdOption = row[9].ToString();
                                line.choice.thirdOptDialogNum = int.Parse(row[10].ToString());

                                //씬 이동이 있는지 확인
                                if (row[11].ToString() != "")
                                {
                                    line.choice.thirdOptOpenObject = true;
                                    line.choice.thirdOptOptObjectName = row[11].ToString();
                                }
                                else
                                {
                                    line.choice.thirdOptOpenObject = false;
                                    line.choice.secondOptOptObjectName = "";
                                }

                            }

                        }

                        if (row[8].ToString() != "")//선택지가 존재하냐
                        {
                            if (int.Parse(row[8].ToString()) == 1)
                            {
                                //선택지가 존재하는 경우
                                startChoice = true;
                                line.haveChoice = true;
                            }
                            if (int.Parse(row[8].ToString()) == 0)
                            {
                                //선택지가 없이 그냥 대사가 끝나는 경우
                                //다음 대사를 정해야함

                                line.isFinishLine = true;

                                //다음 대사의 순서
                                int nextDialogueNum = 0;
                                bool isNumeric = int.TryParse(row[12].ToString(), out nextDialogueNum);

                                if (isNumeric)
                                {
                                    line.nextDialogueNum = nextDialogueNum;
                                }
                                if (!isNumeric && row[12].ToString() == "-")
                                {
                                    line.nextDialogueNum = dialogueNum_in;
                                }

                                //대사가 끝나고 Evnet의 변화가 있는지 확인

                                if (!line.changeEndingID)//false일때만 변경 한번
                                {
                                    int changeEvnetID = 0;
                                    isNumeric = int.TryParse(row[13].ToString(), out changeEvnetID);

                                    if (isNumeric) //만약 숫자 변환이 가능하다면
                                    {
                                        line.changeEvnetID = true;
                                        line.eventIDToBeChange = changeEvnetID;
                                    }
                                }

                                //대사가 끝나고 Ending의 변화가 있는지 확인
                                if (!line.changeEndingID)
                                {
                                    int changeEndingID = 0;
                                    isNumeric = int.TryParse(row[14].ToString(), out changeEndingID);
                                    if (isNumeric)
                                    {
                                        line.changeEndingID = true;
                                        line.endingIDToBeChange = changeEndingID;
                                    }
                                }
                            }
                        }

                        if (++i < (data.Length))
                        {
                            row = data[i].Split(new char[] { ',' });
                        }
                        else
                        {
                            finishBreak = true;
                            break;
                        }
                    } while (row[6].ToString() == "");//csv G구역 이름이 비어있으면 계속 대사가 이어진다는 뜻

                    line.context = contextList.ToArray();
                    LineList.Add(line);

                    contextList.Clear();
                    if (finishBreak)
                    {
                        break;
                    }
                } while (row[5].ToString() == ""); //csv F구역

                lines.Add(LineList);

                if (finishBreak)
                {
                    break;
                }
            } while (row[4].ToString() == ""); // csv E구역
            dialogue.lines = lines;
            dialogues.Add(dialogue);
        }
        return dialogues.ToArray();
    }

    public Event[] EventParse(string csvFileName)
    {
        List<Event> events = new List<Event>();

        TextAsset csvData = Resources.Load<TextAsset>(csvFileName);
        string[] data = csvData.text.Split(new char[] { '\n' });

        for (int i = 1; i < data.Length;)
        {
            string[] row = data[i].Split(new char[] { ',' });
            Event event_ = new Event();
            event_.eventId = int.Parse(row[0].ToString());
            event_.eventContent = new List<string>();
            do
            {
                event_.eventContent.Add(row[1].ToString());

                if (++i < data.Length)
                {
                    row = data[i].Split(new char[] { ',' });
                }
                else
                {
                    finishBreak = true;
                    break;
                }
            } while (row[0].ToString() == "");
            events.Add(event_);
        }
        return events.ToArray();
    }

    
}
