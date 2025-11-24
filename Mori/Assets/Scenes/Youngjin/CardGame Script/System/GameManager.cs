using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // UI ���� (Canvas �ʿ� �ִ� GameUI)
    [Header("UI")]
    [SerializeField] private GameUI ui;

    // ���� �� ����
    [Header("Rules")]
    [SerializeField] private float timeMax = 30f;  // ��ü ���� �ð�
    [SerializeField] private int need = 7;         // ����� �ϴ� ��� �� (0�̸� �ڵ� ���)
    [SerializeField] private int maxHP = 5;        // �ִ� HP

    // Ÿ�� ī�� ��������Ʈ (���� �����ϰų�, �Ʒ� ����Ʈ���� ������ ���)
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

    // ���� Ÿ�� ī�� �� ��� ���� ���� ��
    [Header("Tarot Variants (Optional)")]
    [SerializeField] private List<TarotVar> tarotList = new List<TarotVar>();
    [SerializeField] private int tarotIdx = 0;  // tarotList �� �� ��°�� ����

    // ��� ����
    [Header("Gimmicks (Manual)")]
    [SerializeField] private bool gPos = true;    // ó���� ��ü ������ ��, �¿� ��ġ ����
    [SerializeField] private bool gSpr = false;   // ���� ���� ����, �� �� �� �� �常 �¿����
    [SerializeField] private bool gShift = false; // Ŭ���� ������ �� ������ ī�� �̵�

    // ��� ���� �ӵ�
    [Header("Gimmick Timing")]
    [SerializeField] private float mirrorMoveTime = 0.25f;
    [SerializeField] private Ease mirrorEase = Ease.InOutQuad;

    // ī�� ������ Ÿ�̹�
    [Header("Timing")]
    [SerializeField] private float clickFlipDelay = 0.15f;   // Ŭ�� �� ���� ������ ���۱����� ������

    // ȿ���� ����
    [Header("SFX")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip sfxFlip;             // ī�� �� �� �Ǵ� ��ü�� ������ ��
    [SerializeField] private AudioClip sfxMatchSuccess;     // ��� ������ ��
    [SerializeField] private AudioClip sfxMismatch;         // ��� �������� ��
    [SerializeField] private AudioClip sfxGameOverSuccess;  // ������ Ÿ�α��� �������� ��
    [SerializeField] private AudioClip sfxGameOverFail;     // �ð��ʰ� or HP 0���� �������� ��

    // ���� ����
    private List<Card> cards;   // ���� ���� ��� ī��
    private Card tarotCard;     // �߾� Ÿ�� ī��
    private Card flippedCard;   // ù ��°�� ������ ī��

    private bool isFlipping = false; // ī�� ������ ���� ������
    private bool isShifting = false; // �� �̵� ���� ������
    private bool isGameOver = false; // ������ ��������

    private float timeCur; // ���� �ð�
    private int found = 0; // ���� ��� ��
    private int hp;        // ���� HP

    // ����Ƽ �⺻ ����������Ŭ
    private void Awake()
    {
        if (instance == null) instance = this;

        // ������ Board�� ã�Ƽ�, ���⼭ ���� Ÿ�� ��/�޸��� �Ѱ��ش�
        var board = FindObjectOfType<Board>();
        if (board != null)
        {
            Sprite f = null, b = null;

            // 1����: inspector���� tarotFront / tarotBack ���� ���� ��
            if (tarotFront != null && tarotBack != null)
            {
                f = tarotFront;
                b = tarotBack;
            }
            // 2����: tarotList���� �ϳ� ��� ����
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

            // �� �� �ϳ��� ������ ���忡 ����
            if (f != null || b != null)
                board.ConfigureTarot(f, b);
        }
    }

    private void Start()
    {
        var board = FindObjectOfType<Board>();
        cards = board.getCards();

        // 1��: ī�� ���� isTarot �÷��׷� Ÿ�� ī�� ã��
        tarotCard = cards.Find(c => c.GetIsTarot());
        // 2��: Ȥ�� �÷��װ� �� ���õǾ� �־, �̸��� "TarotCard"�� Ÿ�η� �ν�
        if (tarotCard == null)
            tarotCard = cards.Find(c => c.name == "TarotCard");

        // need�� 0 �����̸�, ��Ÿ�� ī�� ���� ���� �ڵ����� ��� �� ���
        if (need <= 0)
        {
            int nonTarotCount = 0;
            foreach (var c in cards)
                if (!IsTarot(c))
                    nonTarotCount++;

            need = Mathf.Max(1, nonTarotCount / 2);
            Debug.Log("[GameManager] need ���� 0�̶� �ڵ����� " + need + " ���� �����߽��ϴ�.");
        }

        timeCur = timeMax;
        hp = maxHP;

        if (ui != null)
        {
            ui.Init(maxHP, hp, timeMax, gPos, gSpr, gShift);
        }
        else
        {
            Debug.LogWarning("GameManager.ui �� ��� �ֽ��ϴ�. Canvas�� �ִ� GameUI�� �Ҵ��� �ּ���.");
        }

        // ���� ���� ���� �� ���� ī�� �� �� ���� �¿������Ű�� ���
        if (gSpr) PreMirror();

        // ��ü ���� �� �ٽ� ���� �� ��ġ ���� �� Ÿ�̸� ����
        StartCoroutine(FlipAllRoutine());
    }

    // �� ī�尡 Ÿ�� ī������ �Ǻ��ϴ� ���� �Լ�
    private bool IsTarot(Card c)
    {
        if (c == null) return false;
        if (tarotCard != null && ReferenceEquals(c, tarotCard)) return true;
        return c.GetIsTarot();
    }

    // ȿ���� ��� ���� (��ġ �ణ ����)
    private void PlaySfx(AudioClip clip, float pitchMin = 0.98f, float pitchMax = 1.02f)
    {
        if (clip == null || sfxSource == null) return;

        sfxSource.pitch = Random.Range(pitchMin, pitchMax);
        sfxSource.PlayOneShot(clip);
    }

    // ���� ���� �� �� ���� ���� ��ƾ: ��ü ������ ����
    private IEnumerator FlipAllRoutine()
    {
        isFlipping = true;
        yield return new WaitForSeconds(0.5f);

        // 1) ��ü �ո� ����
        PlaySfx(sfxFlip, 0.97f, 1.03f);
        yield return new WaitForSeconds(clickFlipDelay);
        FlipNonTarot();
        yield return new WaitForSeconds(3f);

        // 2) �ٽ� ��ü �޸�
        PlaySfx(sfxFlip, 0.97f, 1.03f);
        yield return new WaitForSeconds(clickFlipDelay);
        FlipNonTarot();
        yield return new WaitForSeconds(0.45f);

        // 3) ����� ���� ������, �޸� ���¿��� �¿� ��ġ ����
        if (gPos) yield return StartCoroutine(MirrorPos());

        isFlipping = false;

        // 4) ���� �ð� ī��Ʈ�ٿ� ����
        StartCoroutine(Timer());
    }

    // ���� �ð� ����
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
            // �ð��ʰ�: ���带 ��� ������ �� ���� ó��
            yield return new WaitForSeconds(1.0f);
            PlaySfx(sfxGameOverFail, 0.99f, 1.01f);
            GameOver(false);
        }
    }

    // ī�忡�� Ŭ���Ǿ��� �� ȣ��
    public void CardClicked(Card c)
    {
        // ���� ���̰ų�, ������ �̹� �����ų�, �� �̵� ���̸� �Է� ����
        if (isFlipping || isGameOver || isShifting) return;

        // ���� ���־� ���� �ѱ�
        c.SetSelected(true);

        // Ŭ�� ��� ȿ���� ���
        PlaySfx(sfxFlip, 0.97f, 1.03f);

        // ���� ������� �ణ ������ �Ŀ� ����
        StartCoroutine(FlipCardAfterDelay(c));

        // �� �̵� ����� ���� ������, ���� �����ӿ� �� ĭ�� �̵�
        if (gShift) StartCoroutine(ShiftNextFrame());

        // ù ��° �������� �� ��° ���������� ���� �б�
        if (flippedCard == null) flippedCard = c;
        else StartCoroutine(Check(flippedCard, c));
    }

    // Ŭ�� �� �ణ ��ٷȴٰ� ī�� ������ �ڷ�ƾ
    private IEnumerator FlipCardAfterDelay(Card c)
    {
        yield return new WaitForSeconds(clickFlipDelay);
        c.FlipCard();
    }

    private IEnumerator ShiftNextFrame()
    {
        // �� ������ ��ٷȴٰ� �� �̵� ó��
        yield return null;
        ShiftOnce();
    }

    // �� ���� ī�带 ���ϴ� ��ƾ
    private IEnumerator Check(Card a, Card b)
    {
        isFlipping = true;

        if (a.cardID == b.cardID)
        {
            // ��Ī ����
            a.SetMatched();
            b.SetMatched();
            found++;

            // �� ī�尡 ���� ���¸� ��� ������ ��, ���� ȿ���� ���
            yield return new WaitForSeconds(0.4f);
            PlaySfx(sfxMatchSuccess, 0.98f, 1.02f);

            // �ո� ���¸� �����ϰ�, ��� ���� �� �ڿ������� ����
            yield return StartCoroutine(ShowPairThenFade(a, b, 0.8f, 0.35f, 0.92f));

            if (found == need)
            {
                // ��� �� �� ���� ���, ���� ����

                // ������ �� ����� �� �ణ�� ����
                yield return new WaitForSeconds(0.4f);

                // Ÿ�� ī�� ������: flip ���� ���� �� ������ �� ���� Flip
                if (tarotCard != null)
                {
                    PlaySfx(sfxFlip, 0.97f, 1.03f);
                    yield return new WaitForSeconds(clickFlipDelay);
                    tarotCard.FlipCard();
                }

                // Ÿ�� ������ �ִϸ��̼��� ���� ������ ���
                yield return new WaitForSeconds(0.45f);

                // Ŭ���̸ƽ��� ���� �ʰ� �����ֱ� ���� ������ �д�
                yield return new WaitForSeconds(1.25f);

                PlaySfx(sfxGameOverSuccess, 0.99f, 1.01f);
                GameOver(true);
                yield break;
            }
        }
        else
        {
            // ��Ī ����

            // 1) �� ī�尡 �ո� ���·� ��� �� ���̰� �Ѵ�
            yield return new WaitForSeconds(1.0f);

            // 2) ������ ������ ���� ���̶���Ʈ�� ���� ����
            a.SetSelected(false);
            b.SetSelected(false);
            yield return new WaitForSeconds(0.12f);

            // 3) �޸����� �ٽ� ������
            a.FlipCard();
            b.FlipCard();

            // 4) ���� ȿ���� ���
            PlaySfx(sfxMismatch, 0.96f, 1.04f);

            // 5) HP ���� �� UI ����
            hp = Mathf.Max(0, hp - 1);
            if (ui != null) ui.UpdateHP(hp);

            if (hp <= 0)
            {
                // HP 0�̸� ���ӿ���: ��� ���带 �����ְ� ���� ó��
                yield return new WaitForSeconds(1.0f);
                PlaySfx(sfxGameOverFail, 0.99f, 1.01f);
                GameOver(false);
                yield break;
            }

            // 6) ī�尡 ������ �޸����� ���ư� ������ ���� ������ ��
            yield return new WaitForSeconds(0.45f);
        }

        isFlipping = false;
        flippedCard = null;
    }

    // ī�尡 �ո��� �ǵ��� ������ �ִ� ���� �ڷ�ƾ
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

    // ��Ī�� �� ī�带 ��� �����ְ�, �ڿ������� ������� �ϴ� ��ƾ
    private IEnumerator ShowPairThenFade(Card a, Card b, float hold, float fade, float shrink)
    {
        yield return StartCoroutine(EnsureFaceUp(a));
        yield return StartCoroutine(EnsureFaceUp(b));
        yield return new WaitForSeconds(hold);

        a.FadeOutAndDisable(fade, shrink);
        b.FadeOutAndDisable(fade, shrink);
        yield return new WaitForSeconds(fade);
    }

    // ���� ���� �� ���: ���� ī�� �� �� �� �常 �¿����
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

    // ���� �� ���: �޸� ���¿��� ī�� ��ġ�� �¿� ����
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
            p.x = cx - (p.x - cx); // �߽��� �������� �¿� ����
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

    // Ŭ�� �� �� ���� ī�� �̵� ���
    private void ShiftOnce()
    {
        var movable = new List<Card>();
        foreach (var c in cards)
        {
            if (!IsTarot(c) && !c.IsFlipping)
                movable.Add(c);
        }

        if (movable.Count <= 1) return;

        // y �������� ���� �ٺ���, ���� ���� x ���� ���ʺ���
        movable.Sort((a, b) =>
        {
            float dy = b.transform.position.y - a.transform.position.y;
            if (Mathf.Abs(dy) > 0.01f) return dy > 0 ? 1 : -1;
            float dx = a.transform.position.x - b.transform.position.x;
            return dx > 0 ? 1 : -1;
        });

        // y ���� ����� ī�峢�� �ϳ��� �ٷ� ����
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

        // �� �ٺ��� ���� ��ȯ �̵�
        for (int r = 0; r < rowList.Count; r++)
        {
            var row = rowList[r];

            // �� ������ x ���� ���� �� ������ ����
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
                // ��� ��: ���������� �� ĭ (�� ������ ī�尡 �� ��������)
                Vector3 last = pos[pos.Count - 1];
                for (int i = pos.Count - 1; i >= 1; i--)
                    pos[i] = pos[i - 1];
                pos[0] = last;
            }
            else
            {
                // ������ ��: �������� �� ĭ (�� ���� ī�尡 �� ����������)
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

    // Ÿ�� ī�� ���� ��ü ������
    private void FlipNonTarot()
    {
        foreach (var c in cards)
        {
            if (!IsTarot(c)) c.FlipCard();
        }
    }

    // ���ӿ��� ó�� (����/���� ����)
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

        // 성공/실패만 보고하고 DayController가 대화 씬으로 복귀시킴
        if (DayController.Instance != null)
        {
            DayController.Instance.NotifyIntermissionDone(success);
        }
        else
        {
            Debug.LogWarning("[MiniGame] DayController not found.");
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
