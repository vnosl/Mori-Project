using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        // ���� ����: ���θ� �ѱ� (���� �ʱ�ȭ)
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

    // ��ư���� ȣ���� ���� �Լ���
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
            Debug.Log("�ڷ� �� �г��� �����ϴ�.");
        }
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("CardGame"); // 씬 이름 or build index
    }
}
