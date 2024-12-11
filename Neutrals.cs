using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
enum ProviderType
{
    Mock,
    Llama2
}

public interface IObserver<T>
{
    void GetNotified(T t);
}

public interface IObservable<T>
{
    void RegisterObserver(IObserver<T> t);
    void DeleteObserver(IObserver<T> t);
    void NotifyObservers(T t);
}

public interface IPlayerManager
{
    public Action OnPlayersReady { get; set; }
    public PlayerPanelDTO AddPlayer(PlayerCreateDTO playerCreateDTO);
    public void SetPlayerReady(PlayerPanelDTO playerRacingDTO);
    public void SubmitAnswerToQuestion(int playerId, string answer);
    public void SubmitFireToQuestion(int playerId, string answer);
    public void CheckPlayersShoot(int playerId, string answerPoint);
    public Action<int, int> OnPlayerScoreUpdated { get; set; }
    public Action OnAllPlayersAnswered { get; set; }
    public Action<int> OnPlayerShot { get; set; }
    public PlayerRacingDTO ProvideInfoForPanel(int id);

}

public interface IQuestionProvider
{
    void Init(int questionCount);
    Question ProvideQuestion();
    Question Provide();
    void Reset();
}

public interface IQuestionManager
{
    public Action<QuestionInfoDto> OnQuestionProvided { get; set; }
    public Action<QuestionResultDto> OnQuestionResolved { get; set; }
    public Action<float,float> OnCountdownUpdated { get; set; }
    public Action OnTimeElapsed { get; set; }
}

public abstract class QuestionProviderBase : ScriptableObject, IQuestionProvider
{
    public abstract void Init(int questionCount);

    public abstract Question Provide();

    public abstract Question ProvideQuestion();

    public abstract void Reset();
}

public interface IPlayerInputHandler
{
    public Action OnAnswerInput { get; set; }
    public abstract Action OnShootInput { get; set; }
    public Action<int> OnTrackSelections { get; set; }
    void Init();
    void AddActionCallbacks();
    void RemoveActionCallbacks();
}

public interface ICommand
{
    void Execute();
    void Undo();
}

public interface IState
{
    void Enter();
    void Update();
    void Exit();
}

public interface IDamagable
{
    void TakeCollision(Collider collider);
}