using UnityEngine;

[CreateAssetMenu(fileName = "QuestionSO", menuName = "Scriptable Objects/QuestionData")]
public class QuestionSO : ScriptableObject
{
    public string Text;
    public string A;
    public string B;
    public string C;
    public string D;
    public string CorrectAnswer;
    public float Time;
    public int Point;
}