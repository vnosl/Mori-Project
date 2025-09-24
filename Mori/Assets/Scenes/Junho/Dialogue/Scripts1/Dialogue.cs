using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
public class Line
{
    // 대사를 치는 인물
    public string Name; // 대사를 치는 사람의 이름
    // 대사 내용
    public string[] context;// 대사들

    // 선택지 여부
    public bool haveChoice;
    public Choice choice;

    // 대화가 끝났는지 여부 확인
    public bool isFinishLine;
    public int nextDialogueNum; // 끝났다면, 이 대사 다음 E열의 값(M열)

    // 대화가 끝나고 이벤트번호의 변경이 있는지 여부 확인
    public bool changeEvnetID;
    public int eventIDToBeChange; // 변경될 이벤트 ID

    // 대화가 끝나고 ending의 번호의 변경이 있는지 여부
    public bool changeEndingID;
    public int endingIDToBeChange; // 변경될 엔딩 ID

}

[System.Serializable]
public class Dialogue
{
    public List<List<Line>> lines;

    //엔딩
    public int endingNum;//A열
    //마을
    public int townNum;//B열
    //상호작용 NPC or Object들 번호
    public int npcNum;//C열
    //이벤트 번호
    public int eventNum;//D열
    //대사 문단의 번호
    public int dialogueNum;//E열
}

[System.Serializable]
public class Choice
{
    public string firstOption;  //1번째 선택지의 Text
    public string secondOption; //2번째 선택지의 Text
    public string thirdOption;  //3번째 선택지의 Text

    public int firstOptDialogNum;  //1번째 선택지를 선택했을 경우, 그다음 E열의 번호(K열)
    public int secondOptDialogNum; //2번째 선택지를 선택했을 경우, 그다음 E열의 번호(K열)
    public int thirdOptDialogNum;  //3번째 선택지를 선택했을 경우, 그다음 E열의 번호(K열)

    public bool firstOptOpenObject;   //선택후 대화를 마치고 오브젝트를 열어야하는지 (L열)
    public string firstOptObjectName; //열어야하는 오브젝트의 이름. //ex.상점

    public bool secondOptOpenObject;      //선택후 대화를 마치고 오브젝트를 열어야하는지 (L열)
    public string secondOptOptObjectName; //열어야하는 오브젝트의 이름. //ex.상점

    public bool thirdOptOpenObject;      //선택후 대화를 마치고 오브젝트를 열어야하는지 (L열)
    public string thirdOptOptObjectName; //열어야하는 오브젝트의 이름. //ex.상점


}