using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void OnClickStartGame()
    {
        Debug.Log("게임 시작!");
        UIManager.Instance.ShowMap();
    }

    public void OnClickSettings()
    {
        UIManager.Instance.ShowSettings();
    }

    public void OnClickQuit()
    {
        Debug.Log("게임 종료!");
        Application.Quit();
    }
}
