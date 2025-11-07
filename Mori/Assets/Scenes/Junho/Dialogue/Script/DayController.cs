using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayController : MonoBehaviour
{
    public static DayController Instance { get; private set; }

    public enum Phase { None, Dialogue, Intermission, EndOfDay }

    [Header("Refs")]
    [SerializeField] DialogueManager dialogue;  // 대화 씬에서만 존재하므로 null일 수 있음

    [Header("Config")]
    [SerializeField, Range(1, 10)] int visitorsPerDay = 3;
    [SerializeField] List<string> visitorKnotNames = new() { "visitor_alice", "visitor_bob", "visitor_chloe" };
    [SerializeField] string dialogueSceneName = "Game";         // 대화가 있는 씬 이름
    [SerializeField] string intermissionSceneName = "Intermission"; // 중간 UI/미니게임 씬 이름
    [SerializeField] string endOfDaySceneName = "EndOfDay";     // 하루 요약/세이브 씬 이름

    Queue<string> todayQueue;
    int servedToday;
    Phase phase = Phase.None;

    bool intermissionDone; // 미니게임 종료 신호

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartNewDay();
    }

    // 씬이 로드될 때 DialogueManager를 다시 찾아 연결 (씬 간 이동 대비)
    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 대화 씬에 진입하면 DialogueManager를 찾아 이벤트 연결
        if (scene.name == dialogueSceneName)
        {
            dialogue = FindObjectOfType<DialogueManager>();
            if (dialogue != null)
            {
                dialogue.DialogueFinished -= OnDialogueFinished;
                dialogue.DialogueFinished += OnDialogueFinished;
            }
        }
    }

    public void StartNewDay()
    {
        servedToday = 0;
        var shuffled = visitorKnotNames.OrderBy(_ => Random.value).ToList();
        todayQueue = new Queue<string>(shuffled.Take(visitorsPerDay));
        RunNextVisitor();
    }

    void RunNextVisitor()
    {
        if (todayQueue.Count == 0)
        {
            EndDay();
            return;
        }

        // 대화 씬이 아니면 대화 씬으로 이동
        if (SceneManager.GetActiveScene().name != dialogueSceneName)
        {
            //SceneManager.LoadScene(dialogueSceneName, LoadSceneMode.Single);
            // OnSceneLoaded에서 DialogueManager를 찾아 이벤트 연결 → 이후 StartVisitorAtKnot 호출
            StartCoroutine(WaitAndStartDialogue());
            return;
        }

        StartVisitorAtKnot(todayQueue.Peek()); // 우선 peek (끝나면 dequeue)
    }

    System.Collections.IEnumerator WaitAndStartDialogue()
    {
        // 한 프레임 기다렸다가 DialogueManager 연결이 끝나면 시작
        yield return null;
        if (dialogue == null) dialogue = FindObjectOfType<DialogueManager>();
        if (dialogue != null) StartVisitorAtKnot(todayQueue.Peek());
    }

    void StartVisitorAtKnot(string knot)
    {
        phase = Phase.Dialogue;
        // 초기 변수 넣고 싶으면 두 번째 인자에 전달
        dialogue.StartStoryAtKnot(knot, null);
    }

    void OnDialogueFinished()
    {
        if (phase != Phase.Dialogue) return;

        // 이번 손님 처리 완료
        todayQueue.Dequeue();
        servedToday++;

        // 다음 단계: Intermission 씬으로 이동
        GoIntermission();
    }

    void GoIntermission()
    {
        phase = Phase.Intermission;
        intermissionDone = false;
        SceneManager.LoadScene(intermissionSceneName, LoadSceneMode.Single);
        // Intermission 씬에서 미니게임 끝나면 DayController.Instance.NotifyIntermissionDone() 호출
    }

    // IntermissionController(버튼/미니게임 완료 지점)에서 호출
    public void NotifyIntermissionDone()
    {
        if (phase != Phase.Intermission) return;
        intermissionDone = true;

        // 다음 손님으로
        RunNextVisitor();
    }

    void EndDay()
    {
        phase = Phase.EndOfDay;
        SceneManager.LoadScene(endOfDaySceneName, LoadSceneMode.Single);
        // 여기서 요약/보상/세이브 → 다음 날 시작 버튼 등에서 StartNewDay() 호출 가능
    }
}