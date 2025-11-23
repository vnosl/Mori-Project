using UnityEngine;
using UnityEngine.EventSystems;

public class ShopKeyboard_old : MonoBehaviour
{
    public KeyCode buyKey = KeyCode.G;

    // 처음 기본으로 쓸 카드 하나
    public ShopItemCard defaultCard;

    void Start()
    {
        // 씬 시작할 때 기본 카드의 Buy 버튼에 포커스 주기
        if (defaultCard != null && defaultCard.buyButton != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(defaultCard.buyButton.gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(buyKey))
        {
            ShopItemCard card = null;

            if (EventSystem.current != null)
            {
                var go = EventSystem.current.currentSelectedGameObject;
                if (go != null)
                {
                    card = go.GetComponentInParent<ShopItemCard>();
                }
            }

            // 선택된 게 없으면 기본 카드로
            if (card == null)
            {
                card = defaultCard;
            }

            if (card != null)
            {
                card.Buy();
            }
        }
    }
}
