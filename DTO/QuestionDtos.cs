public class QuestionResultDto
{
    public string CorrectAnswer { get; set; }
    public int Point { get; set; }
}

public class QuestionInfoDto
{
    public string Text { get; set; }
    public string A { get; set; }
    public string B { get; set; }
    public string C { get; set; }
    public string D { get; set; }
    public float Time { get; set; }
}