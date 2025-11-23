using UnityEngine;
using TMPro;
using System.Collections;

public class ShopMessage : MonoBehaviour
{
    public static ShopMessage Instance { get; private set; }
    public TMP_Text messageText;
    public float showSeconds = 1.2f;

    CanvasGroup g; Coroutine co;

    void Awake()
    {
        Instance = this;
        g = GetComponent<CanvasGroup>();
        if (!g) g = gameObject.AddComponent<CanvasGroup>();
        g.alpha = 0f;
        if (messageText) messageText.text = "";
    }

    public void Show(string msg)
    {
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(ShowCo(msg));
    }

    IEnumerator ShowCo(string msg)
    {
        messageText.text = msg;
        g.alpha = 1f;
        yield return new WaitForSeconds(showSeconds);
        // 부드러운 페이드아웃
        float t = 0f;
        while (t < .25f) { t += Time.unscaledDeltaTime; g.alpha = 1f - (t / .25f); yield return null; }
        g.alpha = 0f;
        messageText.text = "";
        co = null;
    }
}
