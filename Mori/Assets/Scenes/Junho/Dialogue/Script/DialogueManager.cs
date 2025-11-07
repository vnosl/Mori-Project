using System;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Ink")]
    [SerializeField] TextAsset inkJson; // Main.json (Ink Integration이 생성)
    Story story;

    [Header("UI")]
    [SerializeField] CanvasGroup dialoguePanel;  // 하단 대화 패널
    [SerializeField] TMP_Text speakerText;
    [SerializeField] TMP_Text lineText;
    [SerializeField] Transform choicesRoot;       // Vertical Layout Group
    [SerializeField] Button choiceButtonPrefab;

    public event Action<List<string>> OnTagsParsed; // 태그를 외부 리스너로

    public event System.Action DialogueFinished;

    bool waitingForClick;
    bool choicesShowing;

    void Awake()
    {
        dialoguePanel.alpha = 0f;
        dialoguePanel.interactable = false;
        dialoguePanel.blocksRaycasts = false;
    }

    public void StartStoryAtKnot(string knotName, Dictionary<string, object> initialVars = null)
    {
        story = new Story(inkJson.text);

        // 초기 변수 주입(선택)
        if (initialVars != null)
        {
            foreach (var kv in initialVars)
                story.variablesState[kv.Key] = kv.Value;
        }

        story.ChoosePathString(knotName);
        OpenPanel();
        ClearChoices();
        lineText.text = string.Empty;
        speakerText.text = string.Empty;

        ContinueAsync(); // 첫 줄 출력
    }

    void OpenPanel()
    {
        dialoguePanel.alpha = 1f;
        dialoguePanel.interactable = true;
        dialoguePanel.blocksRaycasts = true;
    }

    void ClosePanel()
    {
        dialoguePanel.alpha = 0f;
        dialoguePanel.interactable = false;
        dialoguePanel.blocksRaycasts = false;
    }

    void Update()
    {
        // 마우스 클릭/스페이스로 진행
        if (dialoguePanel.interactable && Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (choicesShowing) return;        // 선택지 뜬 상태에서는 클릭 무시(선택 강제)
            if (waitingForClick) ContinueAsync();
        }
    }

    void ContinueAsync()
    {
        if (story == null) return;

        // 더 진행할 라인이 있으면
        if (story.canContinue)
        {
            string text = story.Continue().Trim();
            HandleTags(story.currentTags);   // 이번 줄의 태그 파싱
            SetSpeakerFromTags(story.currentTags);
            lineText.text = text;

            waitingForClick = true;

            // 줄 끝에 즉시 선택지가 있으면 곧바로 렌더
            if (story.currentChoices.Count > 0)
            {
                waitingForClick = false;
                ShowChoices();
            }
        }
        else
        {
            // 줄이 더 없으면 선택지 확인 → 없으면 종료
            if (story.currentChoices.Count > 0)
            {
                ShowChoices();
            }
            else
            {
                EndDialogue();
            }
        }
    }

    void ShowChoices()
    {
        ClearChoices();
        var choices = story.currentChoices;
        choicesShowing = true;

        for (int i = 0; i < choices.Count; i++)
        {
            var btn = Instantiate(choiceButtonPrefab, choicesRoot);
            var idx = i;
            btn.GetComponentInChildren<TMP_Text>().text = choices[i].text;
            btn.onClick.AddListener(() =>
            {
                story.ChooseChoiceIndex(idx);
                ClearChoices();
                choicesShowing = false;
                ContinueAsync();
            });
        }
    }

    void ClearChoices()
    {
        for (int i = choicesRoot.childCount - 1; i >= 0; i--)
            Destroy(choicesRoot.GetChild(i).gameObject);
    }

    void SetSpeakerFromTags(List<string> tags)
    {
        // 태그 예: "speaker:앨리스", "portrait:alice_happy"
        string speaker = null;
        foreach (var t in tags)
        {
            if (t.StartsWith("speaker:", StringComparison.OrdinalIgnoreCase))
            {
                speaker = t.Substring("speaker:".Length).Trim();
                break;
            }
        }
        speakerText.text = string.IsNullOrEmpty(speaker) ? "" : speaker;
    }

    void HandleTags(List<string> tags)
    {
        // 외부 UI/연출 시스템에 브로드캐스트
        OnTagsParsed?.Invoke(tags);
    }

    void EndDialogue()
    {
        ClosePanel();
        story = null;
        waitingForClick = false;
        choicesShowing = false;
        ClearChoices();
        lineText.text = string.Empty;
        speakerText.text = string.Empty;

        DialogueFinished?.Invoke();               // ★추가: DayController가 구독
    }
}