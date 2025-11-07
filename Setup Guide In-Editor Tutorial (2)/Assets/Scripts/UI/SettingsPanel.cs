using UnityEngine;

public class SettingsUI : MonoBehaviour
{
    public void OnClickBack()
    {
        UIManager.Instance.ShowMainMenu();
    }

    public void ChangeVolume(float value)
    {
        Debug.Log("볼륨 조정: " + value);
        // TODO: 오디오 매니저 연결
    }
}
