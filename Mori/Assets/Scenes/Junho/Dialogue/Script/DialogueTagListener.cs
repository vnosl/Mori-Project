using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueTagListener : MonoBehaviour
{
    [SerializeField] DialogueManager manager;
    [SerializeField] PortraitView rightPortrait;

    void Awake()
    {
        // 혹시 인스펙터에 안 넣었으면 찾아오기
        if (manager == null)
            manager = FindObjectOfType<DialogueManager>();
    }

    void OnEnable()
    {
        if (manager != null)
            manager.OnTagsParsed += HandleTags;
        else
            Debug.LogError("[DialogueTagListener] manager reference is null. Tags will not be handled.");
    }

    void OnDisable()
    {
        if (manager != null)
            manager.OnTagsParsed -= HandleTags;
    }

    void HandleTags(List<string> tags)
    {
        Debug.Log("[DialogueTagListener] Tags: " + string.Join(", ", tags));
        foreach (var raw in tags)
        {
            if (string.IsNullOrWhiteSpace(raw)) continue;

            var tokens = raw.Split(' '); // "skip_intermission #skip_next #goto:day1_end" 가능
            foreach (var token in tokens)
            {
                var t = token.Trim();
                if (string.IsNullOrEmpty(t)) continue;
                if (t[0] == '#') t = t.Substring(1);

                var parts = t.Split(':');
                var key = parts[0].Trim().ToLower();
                var val = parts.Length > 1 ? parts[1].Trim() : "";

                switch (key)
                {
                    case "goto":
                        Debug.Log($"[TagListener] goto tag detected, val = '{val}'");
                        DayController.Instance?.ForceNextDialogueKnot(val);
                        break;
                    case "skip_intermission":
                        DayController.Instance?.RequestSkipIntermissionOnce();
                        break;
                    case "skip_next":
                        DayController.Instance?.RequestSkipNextDialogueOnce();
                        break;
                    case "portrait":
                        rightPortrait?.SetPortrait(val);
                        break;
                    case "end_day":
                        DayController.Instance?.ForceEndDay();
                        break;
                }
            }
        }
    }
}