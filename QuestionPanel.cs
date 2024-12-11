using System;
using TMPro;
using UnityEngine;

public class QuestionPanel : MonoBehaviour
{
    [Header("Panel Objects")]
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] TextMeshProUGUI A;
    [SerializeField] TextMeshProUGUI B;
    [SerializeField] TextMeshProUGUI C;
    [SerializeField] TextMeshProUGUI D;

    [Header("QuestionManager")]
    [SerializeField] private QuestionManager _questionManager;

    private IQuestionManager questionManager => _questionManager as IQuestionManager;

    private float questionTime;

    Action<QuestionResultDto> updateUIWrapper;

    public void Start()
    {
        questionManager.OnQuestionProvided += updatePanel;
        QuizzManager.Instance.OnReplayRequested += clearPanelOnReplaying;

        questionManager.OnQuestionResolved += updateUIWrapper;
        questionManager.OnQuestionResolved += updatePanelOnResolved; //
        //updateUIWrapper += updatePanelOnResolved;
    }

    public void OnDisable()
    {
        questionManager.OnQuestionProvided -= updatePanel;
        QuizzManager.Instance.OnReplayRequested -= clearPanelOnReplaying;

        //updateUIWrapper -= updatePanelOnResolved;
        questionManager.OnQuestionResolved -= updateUIWrapper;

        updateUIWrapper = null;
    }

    private void updatePanel(QuestionInfoDto dto)
    {
        A.color = Color.white;
        B.color = Color.white;
        C.color = Color.white;
        D.color = Color.white;

        questionText.text = dto.Text;

        if (A == null)
        {
            return;
        }

        A.text = ("A) " + dto.A);
        B.text = ("B) " + dto.B);
        C.text = ("C) " + dto.C);
        D.text = ("D) " + dto.D);
    }

    private void updatePanelOnResolved(QuestionResultDto resultDTO)
    {
        Debug.Log(resultDTO.CorrectAnswer);
        switch (resultDTO.CorrectAnswer)
        {
            case "A":
                A.color = Color.green;
                break;
            case "B":
                B.color = Color.green;
                break;
            case "C":
                C.color = Color.green;
                break;
            case "D":
                D.color = Color.green;
                break;
            default:
                break;
        }
    }

    private void clearPanelOnReplaying()
    {
        questionText.text = "RESTARTING...";
    }
}
