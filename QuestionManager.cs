using System;
using System.Collections;
using UnityEngine;

public class QuestionManager : MonoBehaviour, IQuestionManager
{
    public Action<QuestionInfoDto> OnQuestionProvided { get; set; }
    public Action OnTimeElapsed { get; set; }
    public Action<QuestionResultDto> OnQuestionResolved { get; set; }
    public Action<float, float> OnCountdownUpdated { get; set; }
    public Action<float> OnQuestionPreparing { get; set; }

    [Header("Settings")]
    [SerializeField] private ProviderType providerType;
    [SerializeField] private float questionTransitionDelay = 3f;

    [Header("Managers")]
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private QuestionProviderBase questionProvider;

    private IPlayerManager playerManager => _playerManager;

    private Question currentQuestion;

    private float questionTime;
    private float remainingTime;

    private Coroutine countDownCoroutine;
    private Coroutine shortenTransitionCoroutine;

    [SerializeField] int questionCount;
    [SerializeField] int current = 0; // Here Must Be Fixed !

    private void OnEnable()
    {
        QuizzManager.Instance.OnQuizzStarted += startFirstQuestion;
        playerManager.OnAllPlayersAnswered += handleAllPlayersAnswered;
        QuizzManager.Instance.OnReplayRequested += resetManager;

        QuizzManager.Instance.OnQuizzStarting += questionProvider.Reset;
        QuizzManager.Instance.OnReplayRequested += questionProvider.Reset;
    }

    private void OnDisable()
    {
        QuizzManager.Instance.OnQuizzStarted -= startFirstQuestion;
        playerManager.OnAllPlayersAnswered -= handleAllPlayersAnswered;
        QuizzManager.Instance.OnReplayRequested -= resetManager;

        QuizzManager.Instance.OnQuizzStarting += questionProvider.Reset;
        QuizzManager.Instance.OnReplayRequested -= questionProvider.Reset;
    }


    #region Question Workflow

    private void startFirstQuestion()
    {
        questionProvider.Init(questionCount);


        StartCoroutine(loadQuestion());
    }


    private IEnumerator loadQuestion()
    {
        currentQuestion = questionProvider?.ProvideQuestion();
        if (current > questionCount || currentQuestion == null)
        {
            StopAllCoroutines();
            QuizzManager.Instance.OnQuizzFinished?.Invoke();
        }

        else
        {
            QuestionInfoDto questionInfoDto = QuestionAutoMapper.MapQuestionToQuestionInfoDto(currentQuestion);

            questionTime = questionInfoDto.Time;
            OnQuestionProvided?.Invoke(questionInfoDto);
            current++;

            IEnumerator delayedExecution = executeAfter(questionTime, endCurrentQuestion);
            while (delayedExecution.MoveNext())
            {
                if (delayedExecution.Current is float remainingTime)
                {
                    OnCountdownUpdated?.Invoke(questionInfoDto.Time, remainingTime);
                }

                yield return null;
            }
        }
    }

    private void endCurrentQuestion()
    {
        QuestionResultDto resultDto = QuestionAutoMapper.MapQuestionToQuestionResultDto(currentQuestion);
        OnTimeElapsed?.Invoke();
        OnQuestionResolved?.Invoke(resultDto);

        StartCoroutine(prepareNextQuestion(questionTransitionDelay));
    }

    private IEnumerator prepareNextQuestion(float delay)
    {
        float preparingTime = delay;
        IEnumerator delayedExecution = executeAfter(preparingTime, () => { StartCoroutine(loadQuestion()); });
        while (delayedExecution.MoveNext())
        {
            if (delayedExecution.Current is float remainingTime)
            {
                OnCountdownUpdated?.Invoke(preparingTime, remainingTime);
            }

            yield return null;
        }
    }

    private void handleAllPlayersAnswered()
    {
        if (remainingTime > questionTransitionDelay)
        {
            StopAllCoroutines();
            shortenTransitionCoroutine = StartCoroutine(shortenTransitionToNextQuestion());
        }
    }

    private IEnumerator shortenTransitionToNextQuestion()
    {
        IEnumerator delayedExecution = executeAfter(1, endCurrentQuestion);

        while (delayedExecution.MoveNext())
        {
            if (delayedExecution.Current is float remainingTime)
            {
                OnCountdownUpdated?.Invoke(1, remainingTime);
            }

            yield return null;
        }

    }

    private void resetManager()
    {
        current = 0;
        StopAllCoroutines();
    }

    private IEnumerator executeAfter(float delay, Action action)
    {
        remainingTime = delay;

        while (remainingTime > 0)
        {
            yield return remainingTime;
            remainingTime -= Time.deltaTime;
        }

        action?.Invoke();
    }
    #endregion
}