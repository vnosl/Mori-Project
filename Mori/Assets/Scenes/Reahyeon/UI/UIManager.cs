using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    [Header("UI Panels")]
    public GameObject MainMenuPanel;
    public GameObject MapPanel;
    public GameObject ShopPanel;
    public GameObject SettingPanel;
    public GameObject EndingPanel;

    private GameObject currentPanel;
    private readonly Stack<GameObject> panelHistory = new Stack<GameObject>();

    void Start()
    {
        // 시작 상태: 메인만 켜기 (안전 초기화)
        if (MainMenuPanel) MainMenuPanel.SetActive(false);
        if (MapPanel) MapPanel.SetActive(false);
        if (ShopPanel) ShopPanel.SetActive(false);
        if (SettingPanel) SettingPanel.SetActive(false);
        if (EndingPanel) EndingPanel.SetActive(false);

        ShowPanel(MainMenuPanel, false);
    }

    private void ShowPanel(GameObject panelToShow, bool saveHistory = true)
    {
        if (currentPanel != null)
        {
            currentPanel.SetActive(false);
            if (saveHistory) panelHistory.Push(currentPanel);
        }

        if (panelToShow != null)
        {
            panelToShow.SetActive(true);
            currentPanel = panelToShow;
        }
    }

    // 버튼에서 호출할 공개 함수들
    public void ShowMainMenu() { ShowPanel(MainMenuPanel); }
    public void ShowMap() { ShowPanel(MapPanel); }
    public void ShowShop() { ShowPanel(ShopPanel); }
    public void ShowSettings() { ShowPanel(SettingPanel); }
    public void ShowEnding() { ShowPanel(EndingPanel); }

    public void GoBack()
    {
        if (panelHistory.Count > 0)
        {
            if (currentPanel) currentPanel.SetActive(false);
            currentPanel = panelHistory.Pop();
            if (currentPanel) currentPanel.SetActive(true);
        }
        else
        {
            Debug.Log("뒤로 갈 패널이 없습니다.");
        }
    }
}
