using UnityEngine;
using UnityEngine.SceneManagement;

public class OnDialogueEndLoadScene : MonoBehaviour
{
    [SerializeField] DialogueManager manager;
    [SerializeField] string nextSceneName = "New Scene"; // 전환할 씬 이름

    void OnEnable()
    {
        if (!manager) manager = FindObjectOfType<DialogueManager>();
        if (manager) manager.DialogueFinished += HandleEnd;
    }
    void OnDisable()
    {
        if (manager) manager.DialogueFinished -= HandleEnd;
    }
    void HandleEnd()
    {
        // 필요하면 페이드/세이브 등 추가 후 로드
        SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
    }

}
