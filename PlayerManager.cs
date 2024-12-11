using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IPlayerManager
{
    public Action OnPlayersReady { get; set; }

    [SerializeField] private List<Player> players = new List<Player>(); // Db simulation
    [SerializeField] private QuestionManager _questionManager;
    [SerializeField] private int playerCount; // This will be more modular.

    private List<PlayerRacingDTO> racingPlayers = new List<PlayerRacingDTO>(); // Lobi simulation
    private IQuestionManager questionManager => _questionManager as IQuestionManager;

    public Action<int, int> OnPlayerScoreUpdated { get; set; }
    public Action OnAllPlayersAnswered { get; set; }
    public Action<int> OnPlayerShot { get; set; }

    private int currentId = 0;
    private int answeringPlayerCount = 0;

    QuestionResultDto _questionDto;

    private void OnEnable()
    {
        questionManager.OnQuestionResolved += checkPlayerAnswers;
        questionManager.OnQuestionProvided += setPlayerVariablesAfterChecking;
        OnPlayersReady += handleOnPlayersReady;
        QuizzManager.Instance.OnReplayRequested += resetManager;
    }

    private void OnDisable()
    {
        questionManager.OnQuestionResolved -= checkPlayerAnswers;
        OnPlayersReady -= handleOnPlayersReady;
        QuizzManager.Instance.OnReplayRequested -= resetManager;
    }

    public PlayerPanelDTO AddPlayer(PlayerCreateDTO playerCreateDTO)
    {
        Player player = PlayerAutoMapper.MapPlayerCreateDtoToPlayer(playerCreateDTO);
        player.Id = currentId;
        currentId++;
        players.Add(player);

        PlayerRacingDTO playerRacingDTO = PlayerAutoMapper.MapPlayerToPlayerRacingDto(player);
        racingPlayers.Add(playerRacingDTO);

        PlayerPanelDTO playerPanelDTO = PlayerAutoMapper.MapPlayerToPlayerPanelDto(player);

        return playerPanelDTO;
    }

    public void SubmitAnswerToQuestion(int playerId, string answer)
    {
        PlayerRacingDTO racingPlayer = racingPlayers.Find(p => p.Id == playerId);
        racingPlayer.Answer = answer;
        racingPlayer.HasAnswered = true;
        answeringPlayerCount++;

        if (answeringPlayerCount == playerCount)
        {
            OnAllPlayersAnswered?.Invoke();
        }
    }

    public void SubmitFireToQuestion(int playerId, string firedAnswer)
    {
        PlayerRacingDTO racingPlayer = racingPlayers.Find(p => p.Id == playerId);
        racingPlayer.FiredAnswer = firedAnswer;
        racingPlayer.HasAnswered = true;
        answeringPlayerCount++;

        if (answeringPlayerCount == playerCount)
        {
            OnAllPlayersAnswered?.Invoke();
        }
    }

    public void SetPlayerReady(PlayerPanelDTO playerPanelDTO)
    {
        PlayerRacingDTO playerRacingDTO = racingPlayers.Find(p => p.Id == playerPanelDTO.Id);
        playerRacingDTO.IsReady = true;

        int readyPlayerCount = racingPlayers.Count(p => p.IsReady);

        if (readyPlayerCount == playerCount)
        {
            OnPlayersReady?.Invoke();
        }
    }

    private void checkPlayerAnswers(QuestionResultDto questionDto)
    {
        _questionDto = questionDto;

        foreach (var player in racingPlayers)
        {
            if (player.Answer != null && player.Answer.Equals(questionDto.CorrectAnswer))
            {
                rewardPlayer(player.Id, questionDto.Point);
            }

            if (player.Answer != null && !player.Answer.Equals(questionDto.CorrectAnswer))
            {
                punishPlayer(player.Id, questionDto.Point);
            }
        }
    }

    public void CheckPlayersShoot(int playerId, string answerPoint)
    {
        foreach (var player in racingPlayers)
        {
            if (player.Answer == answerPoint)
            {
                punishPlayer(player.Id, _questionDto.Point);
                Debug.Log("Player: " + playerId + " has been shot! and punished!");
            }
        }
    }

    private void rewardPlayer(int id, int point)
    {
        Player player = players.Find(p => p.Id == id);
        player.Score += point;
        Debug.Log("Player " + id + "'s score: " + player.Score);

        OnPlayerScoreUpdated?.Invoke(player.Id, player.Score);
    }

    private void punishPlayer(int id, int point)
    {
        Player player = players.Find(p => p.Id == id);

        int score = player.Score - point;

        if (score < 0)
        {
            score = 0;
        }

        player.Score = score;

        Debug.Log("Player " + id + "'s score: " + player.Score);

        OnPlayerScoreUpdated?.Invoke(player.Id, player.Score);
    }

    private List<Player> getLeaderboard()
    {
        List<Player> leaderBoard = new List<Player>();

        foreach (var player in players.OrderByDescending(p => p.Score))
        {
            leaderBoard.Add(player);
        }

        return leaderBoard;
    }

    private void setPlayerVariablesAfterChecking(QuestionInfoDto infoDto)
    {
        foreach (var player in racingPlayers)
        {
            player.HasAnswered = false;
            player.Answer = null;
            answeringPlayerCount = 0;
        }
    }

    private void resetManager()
    {
        foreach (var player in players)
        {
            answeringPlayerCount = 0;
            player.Score = 0;
            OnPlayerScoreUpdated?.Invoke(player.Id, 0);
        }

        foreach (var player in racingPlayers)
        {
            player.HasAnswered = false;
            player.Answer = null;
            answeringPlayerCount = 0;
            player.Score = 0;
            OnPlayerScoreUpdated?.Invoke(player.Id, 0);
        }
    }

    private void handleOnPlayersReady()
    {
        QuizzManager.Instance.OnQuizzStarting?.Invoke();
    }

    public PlayerRacingDTO ProvideInfoForPanel(int id)
    {
        PlayerRacingDTO racingDTO = racingPlayers.Find(p => p.Id == id);

        if (racingDTO != null)
        {
            return racingDTO;
        }
        return null;
    }

}