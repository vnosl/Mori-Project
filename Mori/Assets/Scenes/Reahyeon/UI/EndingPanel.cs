using UnityEngine;

public class EndingUI : MonoBehaviour
{
    public void OnClickReturnToMenu()
    {
        UIManager.Instance.ShowMainMenu();
    }
}
