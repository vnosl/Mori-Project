using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemCard : MonoBehaviour
{
    [Header("Data")]
    public string itemId = "item_001";
    public int price = 500;      // 돈
    public int soulGain = 10;    // 구매 시 얻는 영혼

    [Header("Refs")]
    public Image itemImage;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public Button buyButton;

    void Awake()
    {
        if (buyButton) buyButton.onClick.AddListener(Buy);
        if (priceText) priceText.text = $"{price:n0}$";
    }

    public void Buy()
    {
        string msg;
        bool ok = CurrencyManager.Instance.TryBuy(price, soulGain, out msg);
        if (ShopMessage.Instance) ShopMessage.Instance.Show(msg);

        if (ok)
        {
            // 실제 게임 아이템 지급/해금 처리 자리
            Debug.Log($"[{itemId}] 구매 완료. +Souls {soulGain}");
        }
        else
        {
            Debug.Log($"[{itemId}] 구매 실패: 돈 부족");
        }
    }
}

