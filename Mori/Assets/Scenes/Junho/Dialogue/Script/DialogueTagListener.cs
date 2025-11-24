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
        foreach (var raw in tags)
        {
            var parts = raw.Split(':');
            var key = parts[0].Trim().ToLower();
            var val = parts.Length > 1 ? parts[1].Trim() : "";

            switch (key)
            {
                case "skip_intermission":
                    // 이번 대화가 END로 끝난 직후, 인터미션을 건너뛰고 다음 knot로 진행
                    DayController.Instance?.RequestSkipIntermissionOnce();
                    break;

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