using UnityEngine;
using DG.Tweening;

public class Card : MonoBehaviour
{
    // 인스펙터에서 넣을 것들
    [SerializeField] private SpriteRenderer cardRenderer;
    [SerializeField] private Sprite frontSprite;   // 앞면 스프라이트
    [SerializeField] private Sprite backSprite;    // 뒷면 스프라이트 (프리팹 기본값 권장)

    [Header("Selection Visual")]
    [SerializeField] private Color selOutlineColor = new Color(1f, 0.85f, 0.2f, 1f);
    [SerializeField] private float selOutlineScale = 1.06f; // 윤곽선 확대 비율
    [SerializeField] private float selScale = 0.94f;        // 선택 시 카드 축소 비율
    [SerializeField] private float selTween = 0.12f;        // 선택/해제 애니 시간

    // 상태값
    private bool isFlipped = false;   // true면 앞면, false면 뒷면
    private bool isFlipping = false;  // 지금 뒤집는 중인지
    private bool isMatched = false;   // 이미 맞춘 카드인지
    private bool isTarot = false;     // 타로 카드인지
    private bool isSelected = false;  // 하이라이트 중인지

    private Tween flipTw;             // 뒤집기용 트윈
    private Tween moveTw;             // 이동용 트윈
    private SpriteRenderer outlineSR; // 윤곽선용 렌더러
    private Vector3 baseScale;        // 원래 스케일

    public int cardID;

    public bool IsFlipping => isFlipping;
    public bool IsFaceUp() => isFlipped;
    public bool GetIsTarot() => isTarot;

    private void Awake()
    {
        baseScale = transform.localScale;

        // 윤곽선용 자식 오브젝트 한 번 생성
        var go = new GameObject("Outline");
        go.transform.SetParent(transform, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one * selOutlineScale;

        outlineSR = go.AddComponent<SpriteRenderer>();
        if (cardRenderer != null)
        {
            outlineSR.sprite = cardRenderer.sprite;
            outlineSR.sortingLayerID = cardRenderer.sortingLayerID;
            outlineSR.sortingOrder = cardRenderer.sortingOrder - 1;
        }
        outlineSR.color = selOutlineColor;
        outlineSR.enabled = false;
    }

    // -------- public API (GameManager에서 호출하는 것들) --------

    public void SetCardID(int id) => cardID = id;
    public void SetMatched() => isMatched = true;
    public void SetFrontSprite(Sprite s) => frontSprite = s;
    public void setFrontSprite(Sprite s) => SetFrontSprite(s); // 호환용

    public void SetTarot(Sprite front, Sprite back)
    {
        isTarot = true;
        if (front != null) frontSprite = front;
        if (back != null) backSprite = back;

        isFlipped = false;
        isMatched = false;
        if (cardRenderer != null) cardRenderer.sprite = backSprite;
        SyncOutlineSprite();
    }

    // 연출용: 애니 없이 바로 뒷면으로
    public void ShowBackImmediate()
    {
        isFlipped = false;
        isFlipping = false;
        if (cardRenderer != null) cardRenderer.sprite = backSprite;
        SyncOutlineSprite();
    }

    // 기믹용: 좌우 반전
    public void SetSpriteMirror(bool on)
    {
        if (cardRenderer != null) cardRenderer.flipX = on;
    }

    // 알파 조절 (기믹용)
    public void SetAlpha(float a)
    {
        if (cardRenderer != null)
        {
            var c = cardRenderer.color;
            c.a = a;
            cardRenderer.color = c;
        }

        if (outlineSR != null)
        {
            var oc = outlineSR.color;
            oc.a = a;
            outlineSR.color = oc;
        }
    }

    // 선택/해제 비주얼
    public void SetSelected(bool on)
    {
        if (isSelected == on) return;
        isSelected = on;

        if (outlineSR != null)
        {
            outlineSR.enabled = on;
            SyncOutlineSprite();
            outlineSR.color = selOutlineColor;
        }

        float target = on ? selScale : 1f;
        transform.DOScale(baseScale * target, selTween).SetEase(Ease.OutQuad);
    }

    // 카드 뒤집기 연출 (0.2 + 0.2 초)
    public void FlipCard()
    {
        if (isFlipping) return;
        isFlipping = true;

        if (flipTw != null && flipTw.IsActive()) flipTw.Kill(false);

        Vector3 s0 = transform.localScale;
        Vector3 s1 = new Vector3(0f, s0.y, s0.z);

        var seq = DOTween.Sequence();
        seq.Append(transform.DOScale(s1, 0.2f));
        seq.AppendCallback(() =>
        {
            isFlipped = !isFlipped;
            if (cardRenderer != null)
                cardRenderer.sprite = isFlipped ? frontSprite : backSprite;
            SyncOutlineSprite();
        });
        seq.Append(transform.DOScale(s0, 0.2f));
        seq.OnKill(() => isFlipping = false);
        seq.OnComplete(() => isFlipping = false);

        flipTw = seq;
    }

    // 이동 전용
    public void MoveTo(Vector3 target, float dur, Ease ease)
    {
        if (moveTw != null && moveTw.IsActive()) moveTw.Kill(false);
        moveTw = transform.DOMove(target, dur).SetEase(ease);
    }

    // 매칭 후 페이드 아웃
    public void FadeOutAndDisable(float fadeTime = 0.28f, float shrink = 0.92f)
    {
        isMatched = true;
        var col2D = GetComponent<Collider2D>();
        if (col2D != null) col2D.enabled = false;

        var seq = DOTween.Sequence();
        if (cardRenderer != null) seq.Join(cardRenderer.DOFade(0f, fadeTime));
        if (outlineSR != null) seq.Join(outlineSR.DOFade(0f, fadeTime));
        seq.Join(transform.DOScale(transform.localScale * shrink, fadeTime).SetEase(Ease.InOutQuad));
        seq.OnComplete(() => gameObject.SetActive(false));
    }

    // -------- 내부 보조 함수 --------

    private void SyncOutlineSprite()
    {
        if (outlineSR == null) return;
        outlineSR.sprite = isFlipped ? frontSprite : backSprite;
    }

    // 마우스 클릭 입력
    private void OnMouseDown()
    {
        // 타로 카드, 이미 맞춘 카드, 뒤집는 중, 이미 앞면인 카드는 무시
        if (!isTarot && !isMatched && !isFlipping && !isFlipped)
        {
            GameManager.instance.CardClicked(this);
        }
    }
}
