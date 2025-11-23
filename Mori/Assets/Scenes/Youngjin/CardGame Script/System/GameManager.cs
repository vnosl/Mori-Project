using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // UI 연결 (Canvas 쪽에 있는 GameUI)
    [Header("UI")]
    [SerializeField] private GameUI ui;

    // 게임 룰 설정
    [Header("Rules")]
    [SerializeField] private float timeMax = 30f;  // 전체 제한 시간
    [SerializeField] private int need = 7;         // 맞춰야 하는 페어 수 (0이면 자동 계산)
    [SerializeField] private int maxHP = 5;        // 최대 HP

    // 타로 카드 스프라이트 (직접 지정하거나, 아래 리스트에서 고르는 방식)
    [Header("Tarot (Pick one in Inspector)")]
    [SerializeField] private Sprite tarotFront;
    [SerializeField] private Sprite tarotBack;

    [System.Serializable]
    public class TarotVar
    {
        public string name;
        public Sprite front;
        public Sprite back;
    }

    // 여러 타로 카드 중 골라 쓰고 싶을 때
    [Header("Tarot Variants (Optional)")]
    [SerializeField] private List<TarotVar> tarotList = new List<TarotVar>();
    [SerializeField] private int tarotIdx = 0;  // tarotList 중 몇 번째를 쓸지

    // 기믹 설정
    [Header("Gimmicks (Manual)")]
    [SerializeField] private bool gPos = true;    // 처음에 전체 보여준 뒤, 좌우 위치 반전
    [SerializeField] private bool gSpr = false;   // 게임 시작 전에, 한 쌍 중 한 장만 좌우반전
    [SerializeField] private bool gShift = false; // 클릭할 때마다 줄 단위로 카드 이동

    // 기믹 연출 속도
    [Header("Gimmick Timing")]
    [SerializeField] private float mirrorMoveTime = 0.25f;
    [SerializeField] private Ease mirrorEase = Ease.InOutQuad;

    // 카드 뒤집기 타이밍
    [Header("Timing")]
    [SerializeField] private float clickFlipDelay = 0.15f;   // 클릭 후 실제 뒤집기 시작까지의 딜레이

    // 효과음 설정
    [Header("SFX")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip sfxFlip;             // 카드 한 장 또는 전체를 뒤집을 때
    [SerializeField] private AudioClip sfxMatchSuccess;     // 페어 맞췄을 때
    [SerializeField] private AudioClip sfxMismatch;         // 페어 실패했을 때
    [SerializeField] private AudioClip sfxGameOverSuccess;  // 마지막 타로까지 성공했을 때
    [SerializeField] private AudioClip sfxGameOverFail;     // 시간초과 or HP 0으로 실패했을 때

    // 내부 상태
    private List<Card> cards;   // 보드 위의 모든 카드
    private Card tarotCard;     // 중앙 타로 카드
    private Card flippedCard;   // 첫 번째로 선택한 카드

    private bool isFlipping = false; // 카드 뒤집기 연출 중인지
    private bool isShifting = false; // 줄 이동 연출 중인지
    private bool isGameOver = false; // 게임이 끝났는지

    private float timeCur; // 남은 시간
    private int found = 0; // 맞춘 페어 수
    private int hp;        // 현재 HP

    // 유니티 기본 라이프사이클
    private void Awake()
    {
        if (instance == null) instance = this;

        // 씬에서 Board를 찾아서, 여기서 정한 타로 앞/뒷면을 넘겨준다
        var board = FindObjectOfType<Board>();
        if (board != null)
        {
            Sprite f = null, b = null;

            // 1순위: inspector에서 tarotFront / tarotBack 직접 넣은 것
            if (tarotFront != null && tarotBack != null)
            {
                f = tarotFront;
                b = tarotBack;
            }
            // 2순위: tarotList에서 하나 골라 쓰기
            else if (tarotList != null && tarotList.Count > 0)
            {
                tarotIdx = Mathf.Clamp(tarotIdx, 0, tarotList.Count - 1);
                var t = tarotList[tarotIdx];
                if (t != null)
                {
                    f = t.front;
                    b = t.back;
                }
            }

            // 둘 중 하나라도 있으면 보드에 전달
            if (f != null || b != null)
                board.ConfigureTarot(f, b);
        }
    }

    private void Start()
    {
        var board = FindObjectOfType<Board>();
        cards = board.getCards();

        // 1차: 카드 내부 isTarot 플래그로 타로 카드 찾기
        tarotCard = cards.Find(c => c.GetIsTarot());
        // 2차: 혹시 플래그가 안 셋팅되어 있어도, 이름이 "TarotCard"면 타로로 인식
        if (tarotCard == null)
            tarotCard = cards.Find(c => c.name == "TarotCard");

        // need가 0 이하이면, 비타로 카드 수를 보고 자동으로 페어 수 계산
        if (need <= 0)
        {
            int nonTarotCount = 0;
            foreach (var c in cards)
                if (!IsTarot(c))
                    nonTarotCount++;

            need = Mathf.Max(1, nonTarotCount / 2);
            Debug.Log("[GameManager] need 값이 0이라서 자동으로 " + need + " 페어로 설정했습니다.");
        }

        timeCur = timeMax;
        hp = maxHP;

        if (ui != null)
        {
            ui.Init(maxHP, hp, timeMax, gPos, gSpr, gShift);
        }
        else
        {
            Debug.LogWarning("GameManager.ui 가 비어 있습니다. Canvas에 있는 GameUI를 할당해 주세요.");
        }

        // 게임 시작 전에 한 쌍의 카드 중 한 장을 좌우반전시키는 기믹
        if (gSpr) PreMirror();

        // 전체 공개 → 다시 가림 → 위치 반전 → 타이머 시작
        StartCoroutine(FlipAllRoutine());
    }

    // 이 카드가 타로 카드인지 판별하는 헬퍼 함수
    private bool IsTarot(Card c)
    {
        if (c == null) return false;
        if (tarotCard != null && ReferenceEquals(c, tarotCard)) return true;
        return c.GetIsTarot();
    }

    // 효과음 재생 헬퍼 (피치 약간 랜덤)
    private void PlaySfx(AudioClip clip, float pitchMin = 0.98f, float pitchMax = 1.02f)
    {
        if (clip == null || sfxSource == null) return;

        sfxSource.pitch = Random.Range(pitchMin, pitchMax);
        sfxSource.PlayOneShot(clip);
    }

    // 게임 시작 시 한 번만 도는 루틴: 전체 뒤집기 연출
    private IEnumerator FlipAllRoutine()
    {
        isFlipping = true;
        yield return new WaitForSeconds(0.5f);

        // 1) 전체 앞면 공개
        PlaySfx(sfxFlip, 0.97f, 1.03f);
        yield return new WaitForSeconds(clickFlipDelay);
        FlipNonTarot();
        yield return new WaitForSeconds(3f);

        // 2) 다시 전체 뒷면
        PlaySfx(sfxFlip, 0.97f, 1.03f);
        yield return new WaitForSeconds(clickFlipDelay);
        FlipNonTarot();
        yield return new WaitForSeconds(0.45f);

        // 3) 기믹이 켜져 있으면, 뒷면 상태에서 좌우 위치 반전
        if (gPos) yield return StartCoroutine(MirrorPos());

        isFlipping = false;

        // 4) 제한 시간 카운트다운 시작
        StartCoroutine(Timer());
    }

    // 제한 시간 관리
    private IEnumerator Timer()
    {
        while (timeCur > 0f && !isGameOver)
        {
            timeCur -= Time.deltaTime;
            if (ui != null) ui.UpdateTime(timeCur);
            yield return null;
        }

        if (!isGameOver)
        {
            // 시간초과: 보드를 잠깐 보여준 뒤 실패 처리
            yield return new WaitForSeconds(1.0f);
            PlaySfx(sfxGameOverFail, 0.99f, 1.01f);
            GameOver(false);
        }
    }

    // 카드에서 클릭되었을 때 호출
    public void CardClicked(Card c)
    {
        // 연출 중이거나, 게임이 이미 끝났거나, 줄 이동 중이면 입력 무시
        if (isFlipping || isGameOver || isShifting) return;

        // 선택 비주얼 먼저 켜기
        c.SetSelected(true);

        // 클릭 즉시 효과음 재생
        PlaySfx(sfxFlip, 0.97f, 1.03f);

        // 실제 뒤집기는 약간 딜레이 후에 시작
        StartCoroutine(FlipCardAfterDelay(c));

        // 줄 이동 기믹이 켜져 있으면, 다음 프레임에 한 칸씩 이동
        if (gShift) StartCoroutine(ShiftNextFrame());

        // 첫 번째 선택인지 두 번째 선택인지에 따라 분기
        if (flippedCard == null) flippedCard = c;
        else StartCoroutine(Check(flippedCard, c));
    }

    // 클릭 후 약간 기다렸다가 카드 뒤집는 코루틴
    private IEnumerator FlipCardAfterDelay(Card c)
    {
        yield return new WaitForSeconds(clickFlipDelay);
        c.FlipCard();
    }

    private IEnumerator ShiftNextFrame()
    {
        // 한 프레임 기다렸다가 줄 이동 처리
        yield return null;
        ShiftOnce();
    }

    // 두 장의 카드를 비교하는 루틴
    private IEnumerator Check(Card a, Card b)
    {
        isFlipping = true;

        if (a.cardID == b.cardID)
        {
            // 매칭 성공
            a.SetMatched();
            b.SetMatched();
            found++;

            // 두 카드가 열린 상태를 잠깐 보여준 후, 성공 효과음 재생
            yield return new WaitForSeconds(0.4f);
            PlaySfx(sfxMatchSuccess, 0.98f, 1.02f);

            // 앞면 상태를 보장하고, 잠깐 유지 후 자연스럽게 퇴장
            yield return StartCoroutine(ShowPairThenFade(a, b, 0.8f, 0.35f, 0.92f));

            if (found == need)
            {
                // 모든 페어를 다 맞춘 경우, 엔딩 연출

                // 마지막 페어가 사라진 뒤 약간의 정적
                yield return new WaitForSeconds(0.4f);

                // 타로 카드 뒤집기: flip 사운드 먼저 → 딜레이 후 실제 Flip
                if (tarotCard != null)
                {
                    PlaySfx(sfxFlip, 0.97f, 1.03f);
                    yield return new WaitForSeconds(clickFlipDelay);
                    tarotCard.FlipCard();
                }

                // 타로 뒤집기 애니메이션이 끝날 때까지 대기
                yield return new WaitForSeconds(0.45f);

                // 클라이맥스를 조금 늦게 보여주기 위해 여유를 둔다
                yield return new WaitForSeconds(1.25f);

                PlaySfx(sfxGameOverSuccess, 0.99f, 1.01f);
                GameOver(true);
                yield break;
            }
        }
        else
        {
            // 매칭 실패

            // 1) 두 카드가 앞면 상태로 잠깐 더 보이게 한다
            yield return new WaitForSeconds(1.0f);

            // 2) 뒤집기 직전에 선택 하이라이트를 먼저 끈다
            a.SetSelected(false);
            b.SetSelected(false);
            yield return new WaitForSeconds(0.12f);

            // 3) 뒷면으로 다시 뒤집기
            a.FlipCard();
            b.FlipCard();

            // 4) 실패 효과음 재생
            PlaySfx(sfxMismatch, 0.96f, 1.04f);

            // 5) HP 감소 및 UI 갱신
            hp = Mathf.Max(0, hp - 1);
            if (ui != null) ui.UpdateHP(hp);

            if (hp <= 0)
            {
                // HP 0이면 게임오버: 잠깐 보드를 보여주고 실패 처리
                yield return new WaitForSeconds(1.0f);
                PlaySfx(sfxGameOverFail, 0.99f, 1.01f);
                GameOver(false);
                yield break;
            }

            // 6) 카드가 완전히 뒷면으로 돌아갈 때까지 템포 여유를 줌
            yield return new WaitForSeconds(0.45f);
        }

        isFlipping = false;
        flippedCard = null;
    }

    // 카드가 앞면이 되도록 보장해 주는 보조 코루틴
    private IEnumerator EnsureFaceUp(Card c)
    {
        if (!c.IsFaceUp()) c.FlipCard();

        float t = 0f;
        float timeout = 1f;
        while ((c.IsFlipping || !c.IsFaceUp()) && t < timeout)
        {
            t += Time.deltaTime;
            yield return null;
        }
    }

    // 매칭된 두 카드를 잠깐 보여주고, 자연스럽게 사라지게 하는 루틴
    private IEnumerator ShowPairThenFade(Card a, Card b, float hold, float fade, float shrink)
    {
        yield return StartCoroutine(EnsureFaceUp(a));
        yield return StartCoroutine(EnsureFaceUp(b));
        yield return new WaitForSeconds(hold);

        a.FadeOutAndDisable(fade, shrink);
        b.FadeOutAndDisable(fade, shrink);
        yield return new WaitForSeconds(fade);
    }

    // 게임 시작 전 기믹: 같은 카드 쌍 중 한 장만 좌우반전
    private void PreMirror()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            var x = cards[i];
            if (IsTarot(x)) continue;

            for (int j = i + 1; j < cards.Count; j++)
            {
                var y = cards[j];
                if (IsTarot(y)) continue;

                if (x.cardID == y.cardID)
                {
                    int pick = Random.Range(0, 2);
                    x.SetSpriteMirror(pick == 0);
                    y.SetSpriteMirror(pick == 1);
                    break;
                }
            }
        }
    }

    // 공개 후 기믹: 뒷면 상태에서 카드 위치를 좌우 반전
    private IEnumerator MirrorPos()
    {
        float minX = 99999f;
        float maxX = -99999f;

        foreach (var c in cards)
        {
            var p = c.transform.position;
            if (p.x < minX) minX = p.x;
            if (p.x > maxX) maxX = p.x;
        }

        float cx = (minX + maxX) * 0.5f;

        var list = new List<Card>();
        var target = new List<Vector3>();

        foreach (var c in cards)
        {
            if (IsTarot(c)) continue;
            list.Add(c);

            c.ShowBackImmediate();
            var p = c.transform.position;
            p.x = cx - (p.x - cx); // 중심을 기준으로 좌우 반전
            target.Add(p);
        }

        isShifting = true;
        for (int i = 0; i < list.Count; i++)
        {
            list[i].MoveTo(target[i], mirrorMoveTime, mirrorEase);
        }
        yield return new WaitForSeconds(mirrorMoveTime + 0.02f);
        isShifting = false;
    }

    // 클릭 시 줄 단위 카드 이동 기믹
    private void ShiftOnce()
    {
        var movable = new List<Card>();
        foreach (var c in cards)
        {
            if (!IsTarot(c) && !c.IsFlipping)
                movable.Add(c);
        }

        if (movable.Count <= 1) return;

        // y 기준으로 위쪽 줄부터, 같은 줄은 x 기준 왼쪽부터
        movable.Sort((a, b) =>
        {
            float dy = b.transform.position.y - a.transform.position.y;
            if (Mathf.Abs(dy) > 0.01f) return dy > 0 ? 1 : -1;
            float dx = a.transform.position.x - b.transform.position.x;
            return dx > 0 ? 1 : -1;
        });

        // y 값이 비슷한 카드끼리 하나의 줄로 묶기
        var rowList = new List<List<Card>>();
        const float rowThreshold = 0.5f;

        foreach (var c in movable)
        {
            bool assigned = false;
            float y = c.transform.position.y;

            foreach (var row in rowList)
            {
                if (Mathf.Abs(row[0].transform.position.y - y) <= rowThreshold)
                {
                    row.Add(c);
                    assigned = true;
                    break;
                }
            }

            if (!assigned)
                rowList.Add(new List<Card> { c });
        }

        isShifting = true;
        float dur = 0.22f;
        Ease ease = Ease.InOutQuad;

        // 각 줄별로 따로 순환 이동
        for (int r = 0; r < rowList.Count; r++)
        {
            var row = rowList[r];

            // 줄 내에서 x 기준 왼쪽 → 오른쪽 정렬
            row.Sort((a, b) =>
            {
                float dx = a.transform.position.x - b.transform.position.x;
                return dx > 0 ? 1 : -1;
            });

            var pos = new List<Vector3>();
            foreach (var c in row) pos.Add(c.transform.position);

            bool isMiddleRow = (rowList.Count == 3 && r == 1);

            if (isMiddleRow)
            {
                // 가운데 줄: 오른쪽으로 한 칸 (맨 오른쪽 카드가 맨 왼쪽으로)
                Vector3 last = pos[pos.Count - 1];
                for (int i = pos.Count - 1; i >= 1; i--)
                    pos[i] = pos[i - 1];
                pos[0] = last;
            }
            else
            {
                // 나머지 줄: 왼쪽으로 한 칸 (맨 왼쪽 카드가 맨 오른쪽으로)
                Vector3 first = pos[0];
                for (int i = 0; i < pos.Count - 1; i++)
                    pos[i] = pos[i + 1];
                pos[pos.Count - 1] = first;
            }

            for (int i = 0; i < row.Count; i++)
            {
                row[i].MoveTo(pos[i], dur, ease);
            }
        }

        StartCoroutine(UnlockAfter(dur + 0.02f));
    }

    private IEnumerator UnlockAfter(float t)
    {
        yield return new WaitForSeconds(t);
        isShifting = false;
    }

    // 타로 카드 제외 전체 뒤집기
    private void FlipNonTarot()
    {
        foreach (var c in cards)
        {
            if (!IsTarot(c)) c.FlipCard();
        }
    }

    // 게임오버 처리 (성공/실패 공통)
    private void GameOver(bool success)
    {
        if (isGameOver) return;
        isGameOver = true;

        StopAllCoroutines();

        if (ui != null)
        {
            ui.SetGameOver(success);
            ui.ShowGameOverPanel(success);
        }
        else
        {
            Debug.Log(success ? "GAME CLEAR" : "GAME OVER");
        }
    }

    public void OnClickQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
