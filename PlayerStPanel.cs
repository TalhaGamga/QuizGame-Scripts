using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStPanel : MonoBehaviour
{
    [SerializeField] PlayerInputHandlerBase inputHandler;

    [Header("Initial Panel")]
    [SerializeField] private TMP_InputField nicknameInputField;
    [SerializeField] private Button submitInfoButton;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject timerObj;
    [SerializeField] private Button readyButton;
    [SerializeField] private GameObject initialPanel;

    [Header("Managers")]
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] QuestionManager _questionManager;

    [Header("AnswerPoints")]
    [SerializeField] private Transform a_Point;
    [SerializeField] private Transform b_Point;
    [SerializeField] private Transform c_Point;
    [SerializeField] private Transform d_Point;

    [Header("InformationPanel")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private TextMeshProUGUI lbNickNameText;
    [SerializeField] private TextMeshProUGUI lbScoreText;

    private IQuestionManager questionManager => _questionManager;
    private IPlayerManager playerManager => _playerManager;

    private Action<QuestionInfoDto> onQuestionProvidedWrapper = null;

    private Dictionary<string, string> answerCorresponds;
    private Dictionary<string, Transform> correspondingPoints;
    [SerializeField] Transform[] answerPoints;

    public Action OnShootSelected;

    private bool hasAnswered = false;

    private string _answer;

    [SerializeField] private PlayerController playerController;

    private delegate void ChosenCommand();
    ChosenCommand command;

    private int index = 0;

    private Transform selectedAnswer;
    [SerializeField] private Transform selectionIndicator;

    private PlayerPanelDTO playerPanelDTO;

    private void Awake()
    {
        inputHandler.Init();
        selectedAnswer = answerPoints[0];
        selectionIndicator.position = getSelectedPosition();

        AddActionCallbacks();
    }

    private void Start()
    {
        correspondingPoints = new Dictionary<string, Transform>
        {
            {"A", a_Point},
            {"B", b_Point},
            {"C", c_Point},
            {"D", d_Point},
        };
    }

    private void OnDisable()
    {
        RemoveActionCallbacks();
    }

    private void AddActionCallbacks()
    {
        inputHandler.AddActionCallbacks();

        onQuestionProvidedWrapper += _ => resetVariables();

        questionManager.OnQuestionProvided += onQuestionProvidedWrapper;

        inputHandler.OnAnswerInput += submitAnswer;

        inputHandler.OnShootInput += selectShoot;

        questionManager.OnTimeElapsed += executeCommand;
        questionManager.OnTimeElapsed += lockVariables;

        playerManager.OnAllPlayersAnswered += directCharacter;

        inputHandler.OnTrackSelections += trackPoints;

        playerController.OnPlayerShoot += checkOnPlayerControllerShot;

        playerManager.OnPlayerScoreUpdated += updateScorePanel;

        QuizzManager.Instance.OnQuizzStarted += deactivateInitialPanel;
        QuizzManager.Instance.OnQuizzFinished += updateLeaderboardPanel;
        QuizzManager.Instance.OnQuizzStarting += activateTimer;
    }

    private void RemoveActionCallbacks()
    {
        inputHandler.RemoveActionCallbacks();

        questionManager.OnQuestionProvided -= onQuestionProvidedWrapper;

        onQuestionProvidedWrapper = null;

        inputHandler.OnShootInput -= selectShoot;

        questionManager.OnTimeElapsed -= executeCommand;
        questionManager.OnTimeElapsed -= lockVariables;

        playerManager.OnAllPlayersAnswered -= directCharacter;

        inputHandler.OnTrackSelections -= trackPoints;

        playerController.OnPlayerShoot -= checkOnPlayerControllerShot;

        playerManager.OnPlayerScoreUpdated -= updateScorePanel;

        QuizzManager.Instance.OnQuizzStarted -= deactivateInitialPanel;
        QuizzManager.Instance.OnQuizzFinished -= updateLeaderboardPanel;
        QuizzManager.Instance.OnQuizzStarting -= activateTimer;
    }

    public void SubmitNickname()
    {
        string playerNickname = nicknameInputField.text.Trim();

        if (!string.IsNullOrEmpty(playerNickname))
        {
            PlayerCreateDTO playerCreateDTO = new PlayerCreateDTO { Nickname = playerNickname };

            playerPanelDTO = playerManager?.AddPlayer(playerCreateDTO);

            scoreText.text = playerPanelDTO.Score.ToString();

            nicknameInputField.interactable = false;
            submitInfoButton.interactable = false;

            submitInfoButton.gameObject.SetActive(false);
            readyButton.gameObject.SetActive(true);
        }
    }

    public void SubmitReady()
    {
        string playerNickname = "Test";

        PlayerCreateDTO playerCreateDTO = new PlayerCreateDTO { Nickname = playerNickname };

        playerPanelDTO = _playerManager.AddPlayer(playerCreateDTO);

        _playerManager.SetPlayerReady(playerPanelDTO);
        readyButton.interactable = false;
    }

    private void activateTimer()
    {
        timerObj.SetActive(true);
    }
    private void submitAnswer()
    {
        if (hasAnswered)
        {
            return;
        }

        _answer = selectedAnswer.name;
        playerManager.SubmitAnswerToQuestion(playerPanelDTO.Id, _answer);

        Transform jumpTo = correspondingPoints[_answer];
        command = () => playerController.JumpInto(jumpTo);

        hasAnswered = true;
    }

    private void selectShoot()
    {
        if (hasAnswered || !QuizzManager.Instance.HasBeenStarted)
        {
            return;
        }

        _answer = selectedAnswer.name;
        playerManager.SubmitFireToQuestion(playerPanelDTO.Id, _answer);

        command = () => playerController.Shoot(selectedAnswer);

        hasAnswered = true;
    }

    private void executeCommand()
    {
        if (command == null)
        {
            return;
        }

        command();
    }

    private void getAnswer(string answer)
    {
        _answer = answer;
    }

    private void directCharacter()
    {
        playerController.LookAt(selectedAnswer);
    }

    private void trackPoints(int trackValue)
    {
        if (hasAnswered || !QuizzManager.Instance.HasBeenStarted)
        {
            return;
        }
        index += trackValue;
        index = (index % answerPoints.Count() + answerPoints.Count()) % answerPoints.Count();
        selectedAnswer = answerPoints[index];

        selectionIndicator.position = getSelectedPosition();
    }

    private Vector3 getSelectedPosition()
    {
        Vector3 selectedPos = new Vector3(selectedAnswer.position.x, selectedAnswer.position.y, selectedAnswer.position.z);
        return selectedPos;
    }

    private void resetVariables()
    {
        command = null;
        hasAnswered = false;
        _answer = null;
        selectedAnswer = answerPoints[0];
        selectionIndicator.position = getSelectedPosition();
        playerController.Reset();
    }

    private void lockVariables()
    {
        hasAnswered = true;
    }

    private void checkOnPlayerControllerShot(Transform target)
    {
        string answerPoint = target.gameObject.name;
        playerManager.CheckPlayersShoot(playerPanelDTO.Id, answerPoint);
    }

    private void updateScorePanel(int id, int score)
    {
        if (playerPanelDTO.Id == id)
        {
            playerPanelDTO.Score = score;
            scoreText.text = "Score:" + score.ToString();
        }
    }

    private void updateLeaderboardPanel()
    {
        if (playerPanelDTO == null)
        {
            return;
        }

        lbNickNameText.text = playerPanelDTO.Nickname;
        lbScoreText.text = "Score: " + playerPanelDTO.Score.ToString();
    }

    private void deactivateInitialPanel()
    {
        initialPanel.SetActive(false);
    }
}