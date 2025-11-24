using UnityEngine;

public class IntermissionController : MonoBehaviour
{
    // 예: "다음 손님" 버튼 OnClick에 연결
    public void OnMiniGameClear(int score)
    {
        DayController.Instance.NotifyIntermissionDone(success: true, score: score, tag: "tarot_day1");
    }
    public void OnMiniGameFail()
    {
        DayController.Instance.NotifyIntermissionDone(success: false, score: 0, tag: "tarot_day1");
    }
}