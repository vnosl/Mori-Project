using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// TextMeshPro 사용 시
using TMPro;

public class DialogueSimpleUI : MonoBehaviour, IPointerClickHandler
{
    [Header("시작 이벤트 설정")]
    [SerializeField] private string eventId = "DAY1_START";
    [SerializeField] private int day = 1;

    [Header("UI 참조 (TMP 또는 UGUI Text 중 하나만)")]
    [SerializeField] private TMP_Text tmpText;   // TextMeshProUGUI
    [SerializeField] private Text uiText;        // (레거시) Text

    [Header("옵션")]
    [SerializeField] private bool autoPickFirstChoice = true; // 선택지 나오면 첫 번째 자동선택
    [SerializeField] private string endText = "";             // 끝났을 때 표시할 문구

    private DialogueLine _current;

    void Awake()
    {
        // TxT_Dialogue 자동 찾기
        if (tmpText == null && uiText == null)
        {
            var t = transform.Find("TxT_Dialogue");
            if (t != null)
            {
                tmpText = t.GetComponent<TMP_Text>();
                if (tmpText == null) uiText = t.GetComponent<Text>();
            }
        }
    }

    void Start()
    {
        var db = DialogueDatabase.Instance; // DialogueDB 오브젝트가 씬에 있어야 함(또는 부트스트랩)
        _current = db.GetStartLine(eventId, day);
        ShowLine(_current);
    }

    void Update()
    {
        // 스페이스바로 진행
#if ENABLE_INPUT_SYSTEM
        // 새 Input System
        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Advance();
        }
#else
        // 기존 Input Manager
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Advance();
        }
#endif
    }

    // UI(Img_DialogueBar)를 클릭했을 때 진행
    public void OnPointerClick(PointerEventData eventData)
    {
        Advance();
    }

    // === 진행 로직 ===
    public void Advance()
    {
        if (_current == null)
        {
            // 이미 종료
            return;
        }

        // 선택지가 있다면 (간단모드: 첫 번째 자동 선택)
        if (!string.IsNullOrEmpty(_current.choiceGroup))
        {
            var choices = DialogueDatabase.Instance.GetChoices(_current.choiceGroup);
            if (choices != null && choices.Count > 0)
            {
                var picked = autoPickFirstChoice ? choices[0] : choices[0]; // 확장 시 실제 선택 UI로 교체
                _current = DialogueDatabase.Instance.GetLineById(picked.nextId);
                ShowLine(_current);
                return;
            }
        }

        // next_id로 이어가기
        if (!string.IsNullOrEmpty(_current.nextId))
        {
            if (_current.nextId == "END")
            {
                EndDialogue();
                return;
            }

            _current = DialogueDatabase.Instance.GetLineById(_current.nextId);
            ShowLine(_current);
        }
        else
        {
            // 더 이상 진행할 라인이 없음
            EndDialogue();
        }
    }

    void ShowLine(DialogueLine line)
    {
        if (line == null) { SetText(endText); return; }

        // 화자 표시하고 싶으면 아래 주석 해제
        // string content = string.IsNullOrEmpty(line.speaker) ? line.text : $"{line.speaker}: {line.text}";
        string content = line.text;

        SetText(content);
    }

    void EndDialogue()
    {
        _current = null;
        SetText(endText); // 빈 문자열이면 대화바만 남고 텍스트는 사라짐
        // 필요 시 여기서 대화 UI 비활성화 등 처리
        // gameObject.SetActive(false);
    }

    void SetText(string content)
    {
        if (tmpText != null) tmpText.text = content;
        else if (uiText != null) uiText.text = content;
        else Debug.LogWarning("DialogueSimpleUI: 텍스트 컴포넌트가 연결되지 않았습니다.");
    }
}