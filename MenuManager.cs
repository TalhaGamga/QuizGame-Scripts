using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject inGameMenuPanel;
    [SerializeField] GameObject endGameMenu;
    private void OnEnable()
    {
        QuizzManager.Instance.OnQuizzFinished += ManageEndGameMenuActiveness;
    }

    private void OnDisable()
    {
        QuizzManager.Instance.OnQuizzFinished -= ManageEndGameMenuActiveness;
    }

    public void Replay()
    {
        if (QuizzManager.Instance.HasBeenStarted)
        {
            QuizzManager.Instance.OnReplayRequested?.Invoke();
            inGameMenuPanel.SetActive(false);
        }
    }

    public void LoadMainMenu()
    {
        QuizzManager.Instance.OnQuizzFinished?.Invoke();
        SceneManager.LoadScene("InitialScene");
    }

    public void Quit()
    {
        QuizzManager.Instance.OnQuitRequested?.Invoke();

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Quit the editor
#endif

    }

    public void ManageEndGameMenuActiveness()
    {
        if (endGameMenu.activeInHierarchy)
        {
            endGameMenu.SetActive(true);
            return;
        }

        endGameMenu.gameObject.SetActive(false);
    }

    public void ManageInGameMenuActiveness()
    {
        if (!inGameMenuPanel.activeInHierarchy)
        {
            inGameMenuPanel.SetActive(true);
            return;
        }

        inGameMenuPanel.gameObject.SetActive(false);
    }
}
