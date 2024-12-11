using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    [Header("Panel Obejcts")]
    [SerializeField] private GameObject initialPanel;
    [SerializeField] private GameObject gameStartedPanel;

    [Header("Initial Panel")]
    [SerializeField] private TMP_InputField nicknameInputField;
    [SerializeField] private TextMeshProUGUI initialNicknameText;
    [SerializeField] private Button submitInfoButton;
    [SerializeField] private Button readyButton;

    [Header("Infos")]
    [SerializeField] private TextMeshProUGUI gameStartedNicknameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject timer;

    [Header("Managers")]
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private QuestionManager _questionManager;

    [Header("Answer Buttons")]
    [SerializeField] private Button butA;
    [SerializeField] private Button butB;
    [SerializeField] private Button butC;
    [SerializeField] private Button butD;

    [Header("Answer Texts")]
    [SerializeField] private TextMeshProUGUI A;
    [SerializeField] private TextMeshProUGUI B;
    [SerializeField] private TextMeshProUGUI C;
    [SerializeField] private TextMeshProUGUI D;

    private Dictionary<string, TextMeshProUGUI> answerTexts;
    private Dictionary<string, Button> answerButtons;

    private IQuestionManager questionManager => _questionManager as IQuestionManager;
    private IPlayerManager playerManager => _playerManager as IPlayerManager;

    private PlayerPanelDTO playerPanelDTO;

    private string givenAnswer;

    [SerializeField] private TextMeshProUGUI leaderboardNicknameText;
    [SerializeField] private TextMeshProUGUI leaderboardScoreText;

    private void OnEnable()
    {
        questionManager.OnQuestionProvided += updatePanelOnQuestionProvided;
        questionManager.OnQuestionResolved += updatePanelOnQuestionResolved;
        playerManager.OnPlayerScoreUpdated += updateTotalPoint;

        QuizzManager.Instance.OnQuizzStarting += manageTimerActiveness;

        QuizzManager.Instance.OnReplayRequested += resetPanel;
        QuizzManager.Instance.OnQuizzFinished += setLeaderboardOnEnd;

        questionManager.OnTimeElapsed += disableAnswerInteractivity;
    }

    private void OnDisable()
    {
        questionManager.OnQuestionProvided -= updatePanelOnQuestionProvided;
        questionManager.OnQuestionResolved -= updatePanelOnQuestionResolved;
        playerManager.OnPlayerScoreUpdated -= updateTotalPoint;

        QuizzManager.Instance.OnQuizzStarting -= manageTimerActiveness;

        QuizzManager.Instance.OnReplayRequested -= resetPanel;
        QuizzManager.Instance.OnQuizzFinished -= setLeaderboardOnEnd;

        questionManager.OnTimeElapsed -= disableAnswerInteractivity;
    }

    private void Start()
    {
        answerTexts = new Dictionary<string, TextMeshProUGUI>
        {
        { "A", A },
        { "B", B },
        { "C", C },
        { "D", D }
        };

        answerButtons = new Dictionary<string, Button>
        {
        { "A", butA },
        { "B", butB },
        { "C", butC },
        { "D", butD }
        };
    }

    public void SubmitNickname()
    {
        string playerNickname = nicknameInputField.text.Trim();

        if (!string.IsNullOrEmpty(playerNickname))
        {
            PlayerCreateDTO playerCreateDTO = new PlayerCreateDTO { Nickname = playerNickname };

            playerPanelDTO = playerManager?.AddPlayer(playerCreateDTO);

            gameStartedNicknameText.text = playerNickname.Trim();
            scoreText.text = playerPanelDTO.Score.ToString();

            nicknameInputField.interactable = false;
            submitInfoButton.interactable = false;

            submitInfoButton.gameObject.SetActive(false);

            initialNicknameText.text = playerNickname;
            initialNicknameText.gameObject.SetActive(true);
            readyButton.gameObject.SetActive(true);
        }
    }

    private void manageTimerActiveness()
    {
        Debug.Log("manage activeness");

            timer.SetActive(true);
    }


    public void SubmitReady()
    {
        playerManager.SetPlayerReady(playerPanelDTO);
        readyButton.interactable = false;
    }

    public void SubmitAnswerToQuestion(string answer)
    {
        givenAnswer = answer;
        answerTexts[answer].color = Color.yellow;
        playerManager.SubmitAnswerToQuestion(playerPanelDTO.Id, answer);

        disableAnswerInteractivity();
    }

    private void updatePanelOnQuestionProvided(QuestionInfoDto questionInfoDto)
    {
        A.text = questionInfoDto.A;
        B.text = questionInfoDto.B;
        C.text = questionInfoDto.C;
        D.text = questionInfoDto.D;

        initialPanel.SetActive(false);
        gameStartedPanel.SetActive(true);

        foreach (var text in answerTexts.Values)
        {
            text.color = Color.white;
        }

        foreach (var button in answerButtons.Values)
        {
            button.interactable = true;
        }

        givenAnswer = null;
    }

    private void disableAnswerInteractivity()
    {
        foreach (var button in answerButtons.Values)
        {
            button.interactable = false;
        }
    }

    private void enableAnswerInteractivity()
    {
        foreach (var button in answerButtons.Values)
        {
            button.interactable = true;
        }
    }

    private void updatePanelOnQuestionResolved(QuestionResultDto questionResultDto)
    {
        string correctAnswer = questionResultDto.CorrectAnswer;

        if (givenAnswer == null)
        {
            answerTexts[correctAnswer].color = Color.red;
            return;
        }

        if (givenAnswer.Equals(correctAnswer))
        {
            answerTexts[givenAnswer].color = Color.green;
            return;
        }

        answerTexts[givenAnswer].color = Color.red;
        answerTexts[correctAnswer].color = Color.green;
    }

    private void updateTotalPoint(int id, int score)
    {
        if (playerPanelDTO.Id.Equals(id))
        {
            playerPanelDTO.Score = score;
            scoreText.text = score.ToString();
        }
    }

    private void resetPanel()
    {
        A.text = "...";
        B.text = "...";
        C.text = "...";
        D.text = "...";

        updateTotalPoint(playerPanelDTO.Id, 0);

        foreach (var text in answerTexts.Values)
        {
            text.color = Color.black;
        }

        foreach (var button in answerButtons.Values)
        {
            button.interactable = true;
        }
    }

    private void setLeaderboardOnEnd()
    {
        if (playerPanelDTO == null)
        {
            return;
        }

        leaderboardNicknameText.text = playerPanelDTO.Nickname;
        leaderboardScoreText.text = "Score: " + playerPanelDTO.Score.ToString();
        Debug.Log("EnGameInvoked");
    }
}