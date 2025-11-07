using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필요합니다!

public class SceneLoader1 : MonoBehaviour
{
    // 이 함수는 버튼의 OnClick() 이벤트에 연결할 함수입니다.
    public void LoadSceneByName(string sceneName)
    {
        // SceneManager를 사용하여 지정된 이름의 씬을 로드합니다.
        // 씬 이름이 정확해야 합니다.
        SceneManager.LoadScene("Dialogue");
    }

}