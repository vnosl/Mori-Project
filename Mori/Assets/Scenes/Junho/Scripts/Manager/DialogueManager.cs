using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject go_DialogueBar;

    [SerializeField] Text text_Dialogue;

    bool isDialogue = false;


    public void ShowDialogue()
    {
        text_Dialogue.text = "";

        SettingUI(true);
    }

    void SettingUI(bool p_flag)
    {
        go_DialogueBar.SetActive(p_flag);
    }
}
