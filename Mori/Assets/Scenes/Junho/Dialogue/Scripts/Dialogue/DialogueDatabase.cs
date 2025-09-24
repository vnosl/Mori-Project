using System.Collections.Generic;
using UnityEngine;

public class DialogueDatabase : MonoBehaviour
{

    [Header("Scenes/Junho/Resources/Dialogue/*.csv")]
    public TextAsset linesCsv;    // dialogue_lines.csv
    public TextAsset choicesCsv;  // dialogue_choices.csv
    [Tooltip("ko 또는 en (열 이름과 매칭)")]
    public string language = "ko";

    public static DialogueDatabase Instance { get; private set; }

    // 인덱스
    private readonly Dictionary<string, DialogueLine> _lineById = new();
    private readonly Dictionary<string, List<DialogueLine>> _linesByEvent = new();
    private readonly Dictionary<string, List<DialogueChoice>> _choicesByGroup = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 매 씬마다 CSV파일을 다시 읽지 않도록 함

        // Resources 자동 로드(필드 비어있으면)
        if (linesCsv == null) linesCsv = Resources.Load<TextAsset>("Dialogue/dialogue_lines");
        if (choicesCsv == null) choicesCsv = Resources.Load<TextAsset>("Dialogue/dialogue_choices");

        if (linesCsv == null) Debug.LogError("linesCsv 파일을 찾지 못했습니다.");
        if (choicesCsv == null) Debug.LogError("choicesCsv 파일을 찾지 못했습니다.");

        if(linesCsv != null) LoadLines(linesCsv);
        if (choicesCsv != null) LoadChoices(choicesCsv);
    }

    void LoadLines(TextAsset csv)
    {
        var rows = CsvUtil.Parse(csv.text);
        foreach (var r in rows)
        {
            string get(string k) => r.ContainsKey(k) ? r[k] : string.Empty;
            int? parseInt(string s) => int.TryParse(s, out var v) ? v : null;

            var line = new DialogueLine
            {
                id = get("id"),
                eventId = get("event_id"),
                day = parseInt(get("day")),
                type = get("type"),
                speaker = get("speaker"),
                //portrait = get("portrait"),
                emotion = get("emotion"),
                text = get($"text_{language}"),
                nextId = get("next_id"),
                //choiceGroup = get("choice_group"),
            };

            if (string.IsNullOrEmpty(line.id)) continue; // ID가 없는 행을 만나면 다시 반복
            _lineById[line.id] = line;

            if (!string.IsNullOrEmpty(line.eventId))
            {
                if (!_linesByEvent.TryGetValue(line.eventId, out var list))
                    _linesByEvent[line.eventId] = list = new List<DialogueLine>();
                list.Add(line);
            }
        }
        // 이벤트별 첫 등장 순서 보장(원하는 기준으로 정렬 가능)
        foreach (var kv in _linesByEvent) kv.Value.Sort((a, b) => string.Compare(a.id, b.id));
    }

    void LoadChoices(TextAsset csv)
    {
        var rows = CsvUtil.Parse(csv.text);
        foreach (var r in rows)
        {
            string get(string k) => r.ContainsKey(k) ? r[k] : string.Empty;
            int parseInt(string s) => int.TryParse(s, out var v) ? v : 0;

            var choice = new DialogueChoice
            {
                groupId = get("group_id"),
                choiceId = parseInt(get("choice_id")),
                text = get($"text_{language}"),
                nextId = get("next_id"),
            };
            if (string.IsNullOrEmpty(choice.groupId)) continue;

            if (!_choicesByGroup.TryGetValue(choice.groupId, out var list))
                _choicesByGroup[choice.groupId] = list = new List<DialogueChoice>();
            list.Add(choice);
        }
        // 보기 정렬
        foreach (var kv in _choicesByGroup) kv.Value.Sort((a, b) => a.choiceId.CompareTo(b.choiceId));
    }

    // === 조회 API ===

    public DialogueLine GetLineById(string id) =>
        _lineById.TryGetValue(id, out var l) ? l : null;

    public List<DialogueChoice> GetChoices(string groupId) =>
        _choicesByGroup.TryGetValue(groupId, out var list) ? list : null;

    // 이벤트+일차로 시작 라인 찾기: 같은 event_id 내 "첫 라인"을 시작점으로 사용
    public DialogueLine GetStartLine(string eventId, int day)
    {
        if (!_linesByEvent.TryGetValue(eventId, out var list)) return null;
        // day가 같거나 day가 비어있는(공통) 것 중 첫 라인
        foreach (var l in list)
        {
            if (l.type != "line") continue;
            if (l.day == null || l.day == day) return l;
        }
        return null;
    }
}