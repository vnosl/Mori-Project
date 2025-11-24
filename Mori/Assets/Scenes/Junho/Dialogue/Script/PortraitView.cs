using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PortraitEntry
{
    public string key;     // e.g., "alice_happy"
    public Sprite sprite;  // 대응 스프라이트
}

public class PortraitView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image portraitImage;
    [SerializeField] CanvasGroup group;          // 페이드용(없으면 Inspector에서 GetComponent로 채움)
    [Header("Data")]
    [SerializeField] List<PortraitEntry> entries = new();

    Dictionary<string, Sprite> map;

    void Awake()
    {
        map = new Dictionary<string, Sprite>();
        foreach (var e in entries)
        {
            if (!string.IsNullOrWhiteSpace(e.key) && e.sprite && !map.ContainsKey(e.key))
                map.Add(e.key, e.sprite);
        }
        if (!group) group = GetComponent<CanvasGroup>();
        if (group) group.alpha = 0f; // 시작 시 숨김
    }

    public void SetPortrait(string key, bool instant = false)
    {
        if (string.IsNullOrEmpty(key) || key == "none")
        {
            Hide(instant);
            return;
        }

        if (map != null && map.TryGetValue(key, out var sprite) && sprite)
        {
            portraitImage.sprite = sprite;
            Show(instant);
        }
        else
        {
            Debug.LogWarning($"[PortraitView] Sprite not found for key: {key}");
            Hide(instant);
        }
    }

    public void Show(bool instant = false) => FadeTo(1f, instant ? 0f : 0.18f);
    public void Hide(bool instant = false) => FadeTo(0f, instant ? 0f : 0.18f);

    void FadeTo(float target, float duration)
    {
        if (!group) { if (portraitImage) portraitImage.enabled = target > 0.5f; return; }
        StopAllCoroutines();
        if (duration <= 0f) { group.alpha = target; portraitImage.enabled = target > 0.01f; return; }
        StartCoroutine(FadeRoutine(target, duration));
    }

    System.Collections.IEnumerator FadeRoutine(float target, float duration)
    {
        portraitImage.enabled = true;
        float start = group.alpha, t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }
        group.alpha = target;
        portraitImage.enabled = target > 0.01f;
    }
}