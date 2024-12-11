using TMPro;
using UnityEngine;

public class AnswerStPanel : MonoBehaviour
{
    [Header("QuestionManager")]
    [SerializeField] private QuestionManager _questionManager;
    private IQuestionManager questionManager => _questionManager as IQuestionManager;

    [Header("Answers")]
    [SerializeField] TextMeshProUGUI A_Text;
    [SerializeField] TextMeshProUGUI B_Text;
    [SerializeField] TextMeshProUGUI C_Text;
    [SerializeField] TextMeshProUGUI D_Text;

    private void OnEnable()
    {
        questionManager.OnQuestionProvided += updateAnswers;
    }

    private void OnDisable()
    {
        questionManager.OnQuestionProvided += updateAnswers;
    }

    private void updateAnswers(QuestionInfoDto questionInfo)
    {
        A_Text.text = questionInfo.A;
        B_Text.text = questionInfo.B;
        C_Text.text = questionInfo.C;
        D_Text.text = questionInfo.D;
    }
}
