using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject mapPanel;
    public GameObject shopPanel;
    public GameObject settingsPanel;
    public GameObject endingPanel;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        ShowMainMenu(); // 처음은 메인메뉴부터
    }

    public void ShowMainMenu() => SwitchPanel(mainMenuPanel);
    public void ShowMap() => SwitchPanel(mapPanel);
    public void ShowShop() => SwitchPanel(shopPanel);
    public void ShowSettings() => SwitchPanel(settingsPanel);
    public void ShowEnding() => SwitchPanel(endingPanel);

    private void SwitchPanel(GameObject target)
    {
        // 모두 끄고
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (mapPanel) mapPanel.SetActive(false);
        if (shopPanel) shopPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (endingPanel) endingPanel.SetActive(false);
        // 하나만 켜기
        if (target) target.SetActive(true);
    }
}
