using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void OnClickStartGame()
    {
        UIManager.Instance.ShowMap();
    }

    public void OnClickSettings()
    {
        UIManager.Instance.ShowSettings();
    }

    public void OnClickQuit()
    {
        Application.Quit();
        Debug.Log("Quit called (에디터에서는 종료 안됨)");
    }
}
