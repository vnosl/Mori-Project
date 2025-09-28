using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    [SerializeField]
    private GameObject cardPrefab;
    [SerializeField]
    private Sprite[] cardSprites;

    [SerializeField]
    private Sprite tarotFrontSprite;
    [SerializeField]
    private Sprite tarotBackSprite;

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
        float spaceY = 3f;

        int rowCount = 3;
        int colCount = 5;

        int centerRow = rowCount / 2;
        int centerCol = colCount / 2;

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

                if (row == centerRow && col == centerCol)
                {
                    card.SetTarot(tarotFrontSprite, tarotBackSprite);
                    card.name = "TarotCard";
                }
                else
                {
                    int cardID = cardIDList[cardIndex++];
                    card.SetCardID(cardID);
                    card.SetFrontSprite(cardSprites[cardID]);
                    card.name = $"Card_{cardID}";
                }

                cardList.Add(card);
            }
        }
    }

    public List<Card> getCards()
    {
        return cardList;
    }
}
