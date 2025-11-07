using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueTagListener : MonoBehaviour
{
    [SerializeField] DialogueManager manager;
    [SerializeField] PortraitView rightPortrait;   // ← 방금 만든 컴포넌트

    void OnEnable() => manager.OnTagsParsed += HandleTags;
    void OnDisable() => manager.OnTagsParsed -= HandleTags;

    void HandleTags(List<string> tags)
    {
        foreach (var tag in tags)
        {
            var parts = tag.Split(':');
            if (parts.Length < 2) continue;

            var key = parts[0].Trim().ToLower();
            var val = parts[1].Trim();

            switch (key)
            {
                case "portrait":
                    // e.g., "alice_happy", "none"
                    rightPortrait.SetPortrait(val);
                    break;

                case "speaker":
                    // 원하면 화자에 따라 기본 초상 전환 규칙도 가능
                    break;

                case "scene":
                    // 필요 시 씬 전환
                    // SceneManager.LoadScene(val);
                    break;

                case "sfx":
                    // 효과음 재생 등…
                    break;
            }
        }
    }
}