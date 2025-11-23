using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardHoverOutline : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Outline outline;

    void Awake()
    {
        outline = GetComponent<Outline>();
        if (outline != null)
        {
            var c = outline.effectColor;
            outline.effectColor = new Color(c.r, c.g, c.b, 0f);  // 시작은 안 보이게
        }
        Debug.Log("Awake: " + name);
    }

  

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Enter: " + name);

        if (outline != null)
        {
            var c = outline.effectColor;
            outline.effectColor = new Color(c.r, c.g, c.b, 1f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Exit: " + name);

        if (outline != null)
        {
            var c = outline.effectColor;
            outline.effectColor = new Color(c.r, c.g, c.b, 0f);
        }
    }
}
