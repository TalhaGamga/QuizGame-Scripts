using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countDownText;
    [SerializeField] private Image image;
    float time;

    [SerializeField] private QuestionManager questionManager;

    private void OnEnable()
    {
        questionManager.OnCountdownUpdated += updateTimeUI;
        QuizzManager.Instance.OnCountdownUpdated += updateTimeUI;
    }

    private void OnDisable()
    {
        questionManager.OnCountdownUpdated -= updateTimeUI;
        QuizzManager.Instance.OnCountdownUpdated -= updateTimeUI;
    }

    public void updateTimeUI(float fullTime, float remaining)
    {
        if (fullTime <= 0)
        {
            return;
        }

        if (remaining < 0)
        {
            remaining = 0;
        }

        image.fillAmount = remaining / fullTime;

        countDownText.text = ((int)remaining).ToString();
    }
}
