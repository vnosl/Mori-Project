using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopKeyboard : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            var go = EventSystem.current.currentSelectedGameObject;
            if (go == null) return;

            // 선택된 오브젝트에서 위로 타고 올라가 카드 찾기
            var card = go.GetComponentInParent<ShopItemCard>();
            if (card != null) card.Buy();
        }
    }
}
