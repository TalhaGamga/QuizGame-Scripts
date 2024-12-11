using TMPro;
using UnityEngine;

public class QuizWarQuestionCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] TextMeshProUGUI A;
    [SerializeField] TextMeshProUGUI B;
    [SerializeField] TextMeshProUGUI C;
    [SerializeField] TextMeshProUGUI D;

    [SerializeField] QuestionManager questionManager;
    //private void OnEnable()
    //{
    //    questionManager.OnQuestionProvided += updatePanel;
    //}

    //private void OnDisable()
    //{
    //    questionManager.OnQuestionProvided -= updatePanel;
    //}


    //private void updatePanel(QuestionInfoDto dto)
    //{
    //    Debug.Log("Updated");
    //    questionText.text = dto.Text;
    //    A.text = ("A)" + dto.Text).ToString();
    //    B.text = ("B)" + dto.Text);
    //    C.text = ("C)" + dto.Text);
    //    D.text = ("D)" + dto.Text);
    //}
}
