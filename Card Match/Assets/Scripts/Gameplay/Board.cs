using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    [SerializeField]
    private GameObject cardPrefab;
    
    [SerializeField]
    private List<Sprite> cardSpriteList = new List<Sprite>();

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
        cardIDList.Clear();

        int rowCount = 3;
        int colCount = 5;

        int nonTarotSlots = rowCount * colCount - 1;
        int neededPairs   = nonTarotSlots / 2;

        int availableDesigns = cardSpriteList != null ? cardSpriteList.Count : 0;
        if (availableDesigns <= 0)
        {
            Debug.LogError("cardSpriteList가 비어있습니다. 카드 디자인을 에디터에 넣어주세요.");
            return;
        }

        int pairCount = Mathf.Min(neededPairs, availableDesigns);

        List<int> indices = new List<int>(availableDesigns);
        for (int i = 0; i < availableDesigns; i++)
        {
            indices.Add(i);
        }

        for (int i = 0; i < indices.Count; i++)
        {
            int j = Random.Range(i, indices.Count);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        for (int k = 0; k < pairCount; k++)
        {
            int id = indices[k];
            cardIDList.Add(id);
            cardIDList.Add(id);
        }

        if (neededPairs > availableDesigns)
        {
            Debug.LogWarning($"필요한 쌍 수({neededPairs})가 디자인 수({availableDesigns})보다 많아 {pairCount}쌍만 생성합니다.");
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
                    if (cardIndex < cardIDList.Count)
                    {
                        int cardID = cardIDList[cardIndex++];
                        card.SetCardID(cardID);
                        card.SetFrontSprite(cardSpriteList[cardID]);
                        card.name = $"Card_{cardID}";
                    }
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
