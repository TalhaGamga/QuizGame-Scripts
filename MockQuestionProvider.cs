using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MockQuestionProvider", menuName = "Scriptable Objects/MockQuestionProvider")]
public class MockQuestionProvider : QuestionProviderBase
{
    [SerializeField] List<QuestionSO> questionList;
    [SerializeField] int currentIndex = 0;

    public override void Init(int questionCount)
    {
    }

    public override Question Provide()
    {
        if (currentIndex < questionList.Count)
        {
            if (questionList.Count > 0)
            {
                Question question = QuestionAutoMapper.MapQuestionSOToQuestion(questionList[currentIndex]);
                currentIndex++;
                return question;
            }
        }

        return null;
    }

    public override Question ProvideQuestion()
    {
        if (currentIndex < questionList.Count)
        {
            if (questionList.Count > 0)
            {
                Question question = QuestionAutoMapper.MapQuestionSOToQuestion(questionList[currentIndex]);
                currentIndex++;
                return question;
            }
        }

        return null;
    }

    public override void Reset()
    {
        currentIndex = 0;
    }
}