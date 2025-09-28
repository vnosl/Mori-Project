using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    [SerializeField]
    private GameObject cardPrefab;

    [SerializeField]
    private Sprite[] cardSprites;

    private List<int> cardIDList = new List<int>();
    private List<Card> cardList = new List<Card>();

    void Start()
    {
        GenerateCardID();
        ShuffleCardID();
        InitBoard();
    }

    void GenerateCardID()
    {
        for (int i = 0; i < cardSprites.Length; i++)
        {
            cardIDList.Add(i);
            cardIDList.Add(i);
        }
    }

    void ShuffleCardID()
    {
        int cardCount = cardIDList.Count;
        for (int i = 0; i < cardCount; i++)
        {
            int randomIndex = Random.Range(i, cardCount);
            int temp = cardIDList[randomIndex];
            cardIDList[randomIndex] = cardIDList[i];
            cardIDList[i] = temp;
        }
    }

    void InitBoard()
    {
        float spaceX = 3f;
        // col
        // 0 - 2 = -2 * spaceX = -6
        // 1 - 2 = -1 * spaceX = -3
        // 2 - 2 =  0 * spaceX =  0
        // 3 - 2 =  1 * spaceX =  3
        // 4 - 2 =  2 * spaceX =  6

        // colCount / 2 = ? -> 정수
        // (col - (int)(colCount / 2)) * spaceX

        float spaceY = 3f;
        // row
        // 0 - 1 = -1 * spaceY = -2
        // 1 - 1 =  1 * spaceY =  0
     
        // rowCount / 2 = ? -> 정수
        // (row - (int)(rowCount / 2)) * spaceY

        int rowCount = 3;
        int colCount = 5;

        int cardIndex = 0;

        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                float posX = (col - (int)(colCount / 2)) * spaceX;
                float posY = (row - (int)(rowCount / 2)) * spaceY;
                Vector3 pos = new Vector3(posX, posY, 0f);
                GameObject cardObject = Instantiate(cardPrefab, pos, Quaternion.identity);
                Card card = cardObject.GetComponent<Card>();
                int cardID = cardIDList[cardIndex++];
                card.SetCardID(cardID);
                card.SetFrontSprite(cardSprites[cardID]);
                cardList.Add(card);
            }
        }
    }

    public List<Card> getCards()
    {
        return cardList;
    }
}
