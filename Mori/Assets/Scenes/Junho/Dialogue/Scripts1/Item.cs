using UnityEngine;

public class Item : MonoBehaviour
{

    public string Name; // NPC나 Obect의 이름
    public int id; // NPC나 Obect의 아이디

    public bool isObject;
    public bool isNpc;

    // 대사 문단 번호
    public int dialogueNem; //csv E열의 값 처음은 항상 1; 

    public int preEventNum; // 이벤트 체크
    public int preEndingNum; // 엔딩 체크
    

}
