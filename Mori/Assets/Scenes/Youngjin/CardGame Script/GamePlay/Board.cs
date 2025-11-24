using UnityEngine;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject cardPrefab;               // 카드 프리팹
    [SerializeField] private List<Sprite> cardSpriteList = new(); // 카드 앞면 스프라이트 목록

    // GameManager에서 선택해 준 타로 앞/뒷면이 들어올 자리
    [SerializeField] private Sprite tarotFrontSprite;
    [SerializeField] private Sprite tarotBackSprite;

    [Header("Layout")]
    [SerializeField] private float spaceX = 3f;      // 카드 사이 가로 간격
    [SerializeField] private float spaceY = 2.7f;    // 카드 사이 세로 간격
    [SerializeField] private float centerY = -0.2f;  // 보드 중앙의 Y 위치(카메라에 맞게 눈으로 조정)

    // 내부 상태
    private readonly List<int> cardIDList = new();   // 카드 ID 배열 (짝 맞추기용)
    private readonly List<Card> cardList = new();    // 씬에 생성된 카드 컴포넌트 목록

    // GameManager에서 타로 스프라이트를 주입할 때 사용
    public void ConfigureTarot(Sprite front, Sprite back)
    {
        tarotFrontSprite = front;
        tarotBackSprite = back;
    }

    // GameManager에서 카드 목록을 가져갈 때 사용
    public List<Card> getCards()
    {
        return cardList;
    }

    // 유니티 생명주기
    private void Start()
    {
        GenerateCardID();
        ShuffleCardID();
        InitBoard();
    }

    // 카드 ID 목록 만들기 (짝 맞추기용으로 0,0,1,1,2,2 이런 식으로 만듦)
    private void GenerateCardID()
    {
        cardIDList.Clear();

        const int rowCount = 3;
        const int colCount = 5;

        int nonTarotSlots = rowCount * colCount - 1; // 중앙 한 칸은 타로
        int neededPairs = nonTarotSlots / 2;

        int availableDesigns = cardSpriteList != null ? cardSpriteList.Count : 0;
        if (availableDesigns <= 0)
        {
            Debug.LogError("cardSpriteList가 비어 있습니다. 카드 앞면 스프라이트를 에디터에서 넣어 주세요.");
            return;
        }

        int pairCount = Mathf.Min(neededPairs, availableDesigns);

        // 사용 가능한 디자인 인덱스를 0,1,2... 로 만들고 섞는다
        List<int> indices = new List<int>(availableDesigns);
        for (int i = 0; i < availableDesigns; i++)
            indices.Add(i);

        for (int i = 0; i < indices.Count; i++)
        {
            int j = Random.Range(i, indices.Count);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        // 필요한 쌍 수만큼 id를 두 번씩 넣어서 짝을 만든다
        for (int k = 0; k < pairCount; k++)
        {
            int id = indices[k];
            cardIDList.Add(id);
            cardIDList.Add(id);
        }

        if (neededPairs > availableDesigns)
        {
            Debug.LogWarning(
                $"필요한 쌍 수({neededPairs})가 카드 디자인 수({availableDesigns})보다 많습니다. " +
                $"{pairCount}쌍만 생성합니다."
            );
        }
    }

    // 위에서 만든 cardIDList 자체를 한 번 더 섞기
    private void ShuffleCardID()
    {
        int count = cardIDList.Count;
        for (int i = 0; i < count; i++)
        {
            int j = Random.Range(i, count);
            int temp = cardIDList[j];
            cardIDList[j] = cardIDList[i];
            cardIDList[i] = temp;
        }
    }

    // 카드 실제로 씬에 배치
    private void InitBoard()
    {
        const int rowCount = 3;
        const int colCount = 5;

        int centerRow = rowCount / 2;
        int centerCol = colCount / 2;

        // (0, centerY)를 보드 중앙으로 보고, 그 기준으로 카드 위치 계산
        float originX = -(colCount - 1) * 0.5f * spaceX;
        float originY = -(rowCount - 1) * 0.5f * spaceY + centerY;

        int cardIndex = 0;

        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                Vector3 pos = new Vector3(
                    originX + col * spaceX,
                    originY + row * spaceY,
                    0f
                );

                GameObject cardObject = Instantiate(cardPrefab, pos, Quaternion.identity);
                Card card = cardObject.GetComponent<Card>();

                // 중앙 한 칸은 타로 카드
                if (row == centerRow && col == centerCol)
                {
                    if (tarotFrontSprite != null || tarotBackSprite != null)
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
}
