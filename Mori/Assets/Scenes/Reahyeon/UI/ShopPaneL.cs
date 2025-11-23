using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    public TMP_Text shopTitle;
    public Button closeButton;

    void OnEnable()
    {
        shopTitle.text = "상점";
        closeButton.onClick.AddListener(CloseShop);
    }

    public void CloseShop()
    {
        UIManager.Instance.ShowMap();
    }

    public void BuyItem(string itemName)
    {
        Debug.Log($"{itemName} 구매!");
        // TODO: 인벤토리와 연결
    }
}
