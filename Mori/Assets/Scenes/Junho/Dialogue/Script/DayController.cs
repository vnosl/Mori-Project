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
    [SerializeField] string endOfDaySceneName = "EndOfDay";

    [Header("Day 1 Visitor Order (Ink knots)")]
    [SerializeField]
    List<string> day1Visitors = new()
    {
        "day1_visitor1",
        "day1_visitor2",
        "day1_visitor3",
        "day1_visitor4",
        "day1_end"            // 엔딩 노드
    };

    private Queue<string> queue;
    private Phase phase = Phase.None;

    public enum EventResult { None, Success, Fail }
    public EventResult lastEventResult { get; private set; } = EventResult.None;

    private DialogueManager dialogue;

    // 이번 사이클(END 한 번)만 인터미션 스킵
    bool skipIntermissionOnce = false;

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

    public void BeginDay1FromMenu()
    {
        StartDay1();
    }

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
            return; // 로드 완료 시 OnSceneLoaded에서 시작
        }
        TryAssignDialogue();
        StartCurrentVisitor();
    }

    void StartCurrentVisitor()
    {
        if (queue == null || queue.Count == 0) { EndDay(); return; }
        if (dialogue == null) { Debug.LogError("[DayController] DialogueManager is null."); return; }

        string knot = queue.Peek(); // 성공적으로 끝나면 Dequeue
        phase = Phase.Dialogue;

        var initialVars = new Dictionary<string, object>
        {
            ["event_success"] = (lastEventResult == EventResult.Success),
            ["event_has_result"] = (lastEventResult != EventResult.None),
        };

        // 결과는 주입과 동시에 소비(다음 사이클에서 새 결과로 덮어쓰기)
        lastEventResult = EventResult.None;

        dialogue.StartStoryAtKnot(knot, initialVars);
    }

    public void RequestSkipIntermissionOnce() => skipIntermissionOnce = true;

    void OnDialogueFinished()
    {
        if (queue != null && queue.Count > 0) queue.Dequeue();

        if (queue != null && queue.Count > 0)
        {
            var next = queue.Peek();

            // 엔딩 노드이거나 이번 턴 스킵 요청이 있으면 인터미션 없이 바로 다음으로
            if (skipIntermissionOnce || IsEndNode(next))
            {
                skipIntermissionOnce = false; // 한 번 쓰고 초기화
                phase = Phase.Dialogue;
                EnsureOnDialogueSceneAndStart();
            }
            else
            {
                GoIntermission();
            }
        }
        else
        {
            EndDay();
        }
    }

    bool IsEndNode(string knotName)
    {
        // 네이밍 규칙에 맞춰 판정 (원하면 리스트/해시셋으로 관리)
        return knotName == "day1_end" || knotName.StartsWith("end_");
    }

    void GoIntermission()
    {
        phase = Phase.Intermission;
        SceneManager.LoadScene(intermissionSceneName, LoadSceneMode.Single);
    }

    // 미니게임 씬에서 성공/실패만 보고
    public void NotifyIntermissionDone(bool success)
    {
        lastEventResult = success ? EventResult.Success : EventResult.Fail;
        phase = Phase.Dialogue;
        EnsureOnDialogueSceneAndStart();
    }
    
    public void NotifyIntermissionDone(bool success, int score, string tag)
    => NotifyIntermissionDone(success);

    void EndDay()
    {
        phase = Phase.EndOfDay;
        SceneManager.LoadScene(endOfDaySceneName, LoadSceneMode.Single);
        // 씬 이동 없이 패널로 끝내고 싶다면 위 줄을 주석 처리하고 UI 이벤트를 쏘면 됨.
    }
}