public static class QuestionAutoMapper
{
    public static QuestionInfoDto MapQuestionToQuestionInfoDto(Question question)
    {
        return new QuestionInfoDto
        {
            Text = question.Text,
            A = question.A,
            B = question.B,
            C = question.C,
            D = question.D,
            Time = question.Time
        };
    }

    public static QuestionResultDto MapQuestionToQuestionResultDto(Question question)
    {
        return new QuestionResultDto
        {
            CorrectAnswer = question.CorrectAnswer,
            Point = question.Point
        };
    }

    public static Question MapQuestionSOToQuestion(QuestionSO questionSO)
    {
        return new Question
        {
            Text = questionSO.Text,
            A = questionSO.A,
            B = questionSO.B,
            C = questionSO.C,
            D = questionSO.D,
            CorrectAnswer = questionSO.CorrectAnswer,
            Time = questionSO.Time,
            Point = questionSO.Point
        };
    }
}