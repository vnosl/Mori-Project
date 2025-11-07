using UnityEngine;

public class IntermissionController : MonoBehaviour
{
    // 예: "다음 손님" 버튼 OnClick에 연결
    public void OnContinueButton()
    {
        DayController.Instance.NotifyIntermissionDone();
    }

    // 미니게임이 성공했을 때 호출하면 동일하게 동작
    public void OnMinigameClear()
    {
        DayController.Instance.NotifyIntermissionDone();
    }
}