using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InitialMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private TMP_Text startQuizButtonText;
    [SerializeField] private TMP_Text exitButtonText;

    private void Awake()
    {
        AudioManager.Instance.Play("InitialSceneMusic");
    }

    public void StartQuiz()
    {
        SceneManager.LoadScene("FastKnowledgeScene");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("InitialScene");
    }

    public void StartQuizWar()
    {
        SceneManager.LoadScene("QuizWar");
    }

    public void Exit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Quit the editor
#endif
    }
}