using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameUI : MonoBehaviour
{
    [Header("Time UI")]
    [SerializeField] private Slider timeoutSlider;
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("Life UI")]
    [SerializeField] private Transform lifeRoot;          // 라이프 아이콘들이 배치될 부모
    [SerializeField] private GameObject lifeIconPrefab;   // 빈 다이아몬드 모양 프리팹
    [SerializeField] private Sprite lifeOnSprite;         // HP가 남아 있을 때 아이콘
    [SerializeField] private Sprite lifeOffSprite;        // HP가 빠졌을 때 아이콘

    [Header("Gimmick UI")]
    [SerializeField] private TextMeshProUGUI gimmickText; // 활성화된 기믹을 표시하는 텍스트

    [Header("GameOver UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private CanvasGroup gameOverCanvasGroup;

    [Header("GameOver Effect")]
    [SerializeField] private float gameOverFadeDuration = 0.6f;   // 패널이 서서히 나타나는 시간
    [SerializeField] private float gameOverScaleDuration = 0.6f;  // 스케일이 정리되는 시간
    [SerializeField] private float gameOverStartScale = 1.12f;    // 약간 크게 시작할 배율
    [SerializeField] private float gameOverStartAngle = 4.5f;     // 처음 기울어진 각도
    [SerializeField] private float gameOverDropOffsetY = 32f;     // 위에서 살짝 떨어지는 느낌을 위한 오프셋

    [SerializeField] private float gameOverHeartbeatScale = 1.05f;    // 마지막에 톡 튀어나오는 스케일
    [SerializeField] private float gameOverHeartbeatDuration = 0.25f; // 그 연출에 걸리는 시간

    // 내부 상태
    private readonly List<Image> lifeImages = new List<Image>();
    private float timeMax = 30f;

    private void Awake()
    {
        // 시작할 때는 게임오버 패널을 안 보이게 세팅해 둔다
        if (gameOverPanel != null)
        {
            if (gameOverCanvasGroup == null)
                gameOverCanvasGroup = gameOverPanel.GetComponent<CanvasGroup>();

            if (gameOverCanvasGroup != null)
                gameOverCanvasGroup.alpha = 0f;

            gameOverPanel.SetActive(false);
        }
    }

    // GameManager에서 게임을 시작할 때 한 번만 호출해서 전체 UI를 초기화
    public void Init(int maxHP, int curHP, float timeMax,
                     bool gPos, bool gSpr, bool gShift)
    {
        this.timeMax = timeMax;

        // 타이머 슬라이더 설정
        if (timeoutSlider != null)
        {
            timeoutSlider.minValue = 0f;
            timeoutSlider.maxValue = 1f;
            timeoutSlider.value = 1f; // 처음에는 가득 찬 상태
        }

        if (timeText != null)
        {
            timeText.SetText(Mathf.CeilToInt(timeMax).ToString());
        }

        // 라이프 아이콘 생성 및 현재 HP 반영
        BuildLifeIcons(maxHP);
        UpdateHP(curHP);

        // 어떤 기믹이 켜져 있는지 텍스트로 정리해서 보여준다
        if (gimmickText != null)
        {
            List<string> parts = new List<string>();
            if (gPos) parts.Add("Mirror");
            if (gSpr) parts.Add("Flip Sprite");
            if (gShift) parts.Add("Row Shift");

            gimmickText.text = parts.Count > 0
                ? string.Join(" / ", parts)
                : "";
        }

        // 게임오버 패널은 초기에는 숨겨 둔다
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            if (gameOverCanvasGroup != null)
                gameOverCanvasGroup.alpha = 0f;
        }
    }

    // 남은 시간 갱신
    public void UpdateTime(float timeCur)
    {
        float t = Mathf.Clamp(timeCur, 0f, timeMax);

        if (timeoutSlider != null)
            timeoutSlider.value = timeMax > 0f ? t / timeMax : 0f;

        if (timeText != null)
            timeText.SetText(Mathf.CeilToInt(t).ToString());
    }

    // HP 갱신
    public void UpdateHP(int hp)
    {
        for (int i = 0; i < lifeImages.Count; i++)
        {
            Image img = lifeImages[i];
            if (img == null) continue;

            bool on = (i < hp);
            img.sprite = on ? lifeOnSprite : lifeOffSprite;
            img.color = Color.white;
        }
    }

    // 라이프 아이콘 처음 구성
    private void BuildLifeIcons(int maxHP)
    {
        lifeImages.Clear();

        if (lifeRoot == null || lifeIconPrefab == null)
        {
            Debug.LogWarning("[GameUI] LifeRoot 또는 LifeIconPrefab이 비어 있습니다.");
            return;
        }

        // 기존 자식 아이콘들은 전부 삭제
        for (int i = lifeRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(lifeRoot.GetChild(i).gameObject);
        }

        // 최대 HP 수만큼 아이콘 생성
        for (int i = 0; i < maxHP; i++)
        {
            GameObject go = Object.Instantiate(lifeIconPrefab, lifeRoot);
            Image img = go.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = lifeOffSprite != null ? lifeOffSprite : img.sprite;
                img.color = Color.white;
                lifeImages.Add(img);
            }

            // RectTransform 기본값을 중앙 기준으로 맞춰 둔다
            RectTransform rtIcon = go.transform as RectTransform;
            if (rtIcon != null)
            {
                rtIcon.anchorMin = new Vector2(0.5f, 0.5f);
                rtIcon.anchorMax = new Vector2(0.5f, 0.5f);
                rtIcon.pivot = new Vector2(0.5f, 0.5f);
                rtIcon.localScale = Vector3.one;
            }
        }

        // 직접 x 좌표를 계산해서 가로로 배치 (전체가 가운데 정렬되도록)
        RectTransform rtRoot = lifeRoot as RectTransform;
        if (rtRoot == null) return;

        float spacing = 40f; // 아이콘 간 간격 (필요하면 여기 숫자만 조절)

        for (int i = 0; i < lifeImages.Count; i++)
        {
            Image img = lifeImages[i];
            if (img == null) continue;

            RectTransform rtIcon = img.transform as RectTransform;
            if (rtIcon == null) continue;

            // 인덱스를 기준으로 -2, -1, 0, 1, 2 이런 식으로 중앙에서 양쪽으로 퍼지게 함
            float offsetX = (i - (lifeImages.Count - 1) * 0.5f) * spacing;
            rtIcon.anchoredPosition = new Vector2(offsetX, 0f);
        }
    }

    // GameManager에서 성공 / 실패 여부만 먼저 알려 줄 때 사용
    public void SetGameOver(bool success)
    {
        if (gameOverText != null)
            gameOverText.SetText(success ? "Success" : "Failure");
    }

    // GameOver 패널을 연출과 함께 보여 주기
    public void ShowGameOverPanel(bool success)
    {
        if (gameOverPanel == null) return;

        gameOverPanel.SetActive(true);

        if (gameOverCanvasGroup == null)
            gameOverCanvasGroup = gameOverPanel.GetComponent<CanvasGroup>();

        // 기존에 돌고 있던 트윈이 있다면 정리
        if (gameOverCanvasGroup != null) gameOverCanvasGroup.DOKill();
        RectTransform rt = gameOverPanel.transform as RectTransform;
        if (rt != null) rt.DOKill();

        // 알파 0에서 시작
        if (gameOverCanvasGroup != null)
            gameOverCanvasGroup.alpha = 0f;

        Vector3 baseScale = Vector3.one;
        Vector3 startScale = baseScale * gameOverStartScale;

        // 씬에서 잡아 둔 기준 위치
        Vector2 basePos = rt != null ? rt.anchoredPosition : Vector2.zero;

        // 실패인 경우에는 살짝 더 위쪽에서 뜨도록 Y 값을 조정
        if (!success)
            basePos += new Vector2(0f, 40f);

        // 시작 위치는 기준 위치보다 조금 더 위에서 시작해서, 떨어지는 느낌을 줌
        Vector2 startPos = basePos + new Vector2(0f, gameOverDropOffsetY);

        if (rt != null)
        {
            rt.localScale = startScale;
            rt.anchoredPosition = startPos;
            rt.localRotation = Quaternion.Euler(0f, 0f, gameOverStartAngle);
        }

        Sequence seq = DOTween.Sequence();

        // 1) 페이드 인
        if (gameOverCanvasGroup != null)
        {
            seq.Join(
                gameOverCanvasGroup
                    .DOFade(1f, gameOverFadeDuration)
                    .SetEase(Ease.OutQuad)
            );
        }

        // 2) 스케일, 위치, 회전을 동시에 정리
        if (rt != null)
        {
            seq.Join(
                rt.DOScale(baseScale, gameOverScaleDuration)
                  .SetEase(Ease.OutCubic)
            );

            seq.Join(
                rt.DOAnchorPos(basePos, gameOverScaleDuration)
                  .SetEase(Ease.OutCubic)
            );

            seq.Join(
                rt.DOLocalRotate(Vector3.zero, gameOverScaleDuration)
                  .SetEase(Ease.OutCubic)
            );

            // 3) 마지막에 살짝 톡 튀어나오게
            seq.Append(
                rt.DOPunchScale(
                    baseScale * (gameOverHeartbeatScale - 1f),
                    gameOverHeartbeatDuration,
                    1,
                    0.15f
                ).SetEase(Ease.OutQuad)
            );
        }

        seq.Play();
    }
}
