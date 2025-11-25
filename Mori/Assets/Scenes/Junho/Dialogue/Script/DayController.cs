using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayController : MonoBehaviour
{
    public static DayController Instance { get; private set; }

    public enum Phase { None, Dialogue, Intermission, EndOfDay }

    [SerializeField] bool autoStart = false;

    [Header("Scene Names")]
    [SerializeField] string dialogueSceneName = "Dialogue";
    [SerializeField] string intermissionSceneName = "Intermission";

    [Header("Day 1 Visitor Order (Ink knots)")]
    [SerializeField]
    List<string> day1Visitors = new()
    {
        "day1_visitor1",
        "day1_visitor2",
        "day1_visitor3",
        "day1_visitor4",
        "day1_end" // 마지막 노드
    };

    // === 마무리를 Dialogue 씬에서 처리하기 위한 훅 ===
    [Header("End UI (optional)")]
    [SerializeField] GameObject endOverlay; // 마무리 패널(없어도 됨)
    public event Action OnDayEnded;         // 외부(UI)가 구독해서 패널 띄우기

    private Queue<string> queue;
    private Phase phase = Phase.None;

    public enum EventResult { None, Success, Fail }
    public EventResult lastEventResult { get; private set; } = EventResult.None;

    private DialogueManager dialogue;

    bool skipIntermissionOnce = false;
    bool skipNextDialogueOnce = false;

    // 현재 진행 중인 knot 이름(엔드 노드 즉시 종료 판정용)
    string currentKnot = null;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void Start()
    {
        if (autoStart) StartDay1();
    }

    public void BeginDay1FromMenu() => StartDay1();

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == dialogueSceneName)
        {
            TryAssignDialogue();
            if (phase == Phase.Dialogue && queue != null && queue.Count > 0)
                StartCurrentVisitor();
        }
    }

    void TryAssignDialogue()
    {
        if (dialogue == null)
            dialogue = FindObjectOfType<DialogueManager>(includeInactive: true);

        if (dialogue != null)
        {
            dialogue.DialogueFinished -= OnDialogueFinished;
            dialogue.DialogueFinished += OnDialogueFinished;
        }
        else
        {
            Debug.LogError("[DayController] DialogueManager not found in Dialogue scene.");
        }
    }

    public void StartDay1()
    {
        queue = new Queue<string>(day1Visitors);
        phase = Phase.Dialogue;
        EnsureOnDialogueSceneAndStart();
    }

    void EnsureOnDialogueSceneAndStart()
    {
        if (SceneManager.GetActiveScene().name != dialogueSceneName)
        {
            SceneManager.LoadScene(dialogueSceneName, LoadSceneMode.Single);
            return;
        }
        TryAssignDialogue();
        StartCurrentVisitor();
    }

    void StartCurrentVisitor()
    {
        if (queue == null || queue.Count == 0) { EndDay(); return; }
        if (dialogue == null) { Debug.LogError("[DayController] DialogueManager is null."); return; }

        string knot = queue.Peek();     // 성공적으로 끝나면 Dequeue
        currentKnot = knot;
        phase = Phase.Dialogue;

        var initialVars = new Dictionary<string, object>
        {
            ["event_success"] = (lastEventResult == EventResult.Success),
            ["event_has_result"] = (lastEventResult != EventResult.None),
        };

        // 주입과 동시에 소비
        lastEventResult = EventResult.None;

        dialogue.StartStoryAtKnot(knot, initialVars);
    }

    public void RequestSkipIntermissionOnce() => skipIntermissionOnce = true;
    public void RequestSkipNextDialogueOnce() => skipNextDialogueOnce = true;

    void OnDialogueFinished()
    {
        Debug.Log($"[DayController] OnDialogueFinished: currentKnot={currentKnot}, overrideNextKnot='{overrideNextKnot}', queue=[{(queue != null ? string.Join(", ", queue) : "null")}]");
        // 방금 끝난 게 엔드 노드면 여기서 마무리
        if (!string.IsNullOrEmpty(currentKnot) && IsEndNode(currentKnot))
        {
            EndDay();
            return;
        }

        // 현재 노드 소비
        if (queue != null && queue.Count > 0) queue.Dequeue();
        currentKnot = null;

        // 1) goto 오버라이드가 있으면 큐를 덮어쓰고 즉시 대화 재개
        if (!string.IsNullOrEmpty(overrideNextKnot))
        {
            queue = new Queue<string>(new[] { overrideNextKnot });
            overrideNextKnot = null;
            forceNextDialogueImmediate = true;  // 중복 보증
            phase = Phase.Dialogue;
            EnsureOnDialogueSceneAndStart();
            return;
        }

        // 2) “무조건 대화 재개” 플래그가 켜져 있으면 인터미션 체크를 아예 건너뜀
        if (forceNextDialogueImmediate)
        {
            forceNextDialogueImmediate = false;
            phase = Phase.Dialogue;
            EnsureOnDialogueSceneAndStart();
            return;
        }

        // (선택) 다음 노드 1개 스킵
        if (skipNextDialogueOnce && queue != null && queue.Count > 0)
        {
            queue.Dequeue();
            skipNextDialogueOnce = false;
        }

        if (queue != null && queue.Count > 0)
        {
            var next = queue.Peek();

            // 다음이 엔드 노드이거나 인터미션 스킵 요청이면 바로 대화
            if (skipIntermissionOnce || IsEndNode(next))
            {
                skipIntermissionOnce = false;
                phase = Phase.Dialogue;
                EnsureOnDialogueSceneAndStart();
            }
            else
            {
                GoIntermission();    // ← 여기까지 오기 전에 위에서 모두 걸러집니다
            }
        }
        else
        {
            EndDay();
        }
    }

    string overrideNextKnot = null;   // 태그로 강제 점프할 다음 노드
    bool forceNextDialogueImmediate = false;

    public void ForceNextDialogueKnot(string knotName)
    {
        Debug.Log($"[DayController] ForceNextDialogueKnot called with '{knotName}' (phase={phase}, currentKnot={currentKnot})");
        overrideNextKnot = knotName;
        forceNextDialogueImmediate = true;   // 인터미션 완전 금지
        skipIntermissionOnce = true;         // 안전망 (겹쳐서 방지)
        skipNextDialogueOnce = false;
    }

    bool IsEndNode(string knotName)
    {
        return knotName == "day1_end" || knotName.StartsWith("end_");
    }

    void GoIntermission()
    {
        phase = Phase.Intermission;
        SceneManager.LoadScene(intermissionSceneName, LoadSceneMode.Single);
    }

    public void NotifyIntermissionDone(bool success)
    {
        lastEventResult = success ? EventResult.Success : EventResult.Fail;
        phase = Phase.Dialogue;
        EnsureOnDialogueSceneAndStart();
    }
    public void NotifyIntermissionDone(bool success, int score, string tag)
        => NotifyIntermissionDone(success);

    public void ForceEndDay()
    {
        // 방어용: 더 이상 인터미션/다음 대화 안 돌도록 상태 정리
        phase = Phase.EndOfDay;
        queue = null;
        currentKnot = null;

        EndDay();
    }

    // === 여기서 마무리를 “Dialogue 씬 내부”에서 처리 ===
    void EndDay()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}