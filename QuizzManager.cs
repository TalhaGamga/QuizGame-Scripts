using System;
using System.Collections;
using UnityEngine;

public class QuizzManager : MonoBehaviour
{
    public static QuizzManager Instance { get; private set; }

    public Action OnQuizzStarted;
    public Action OnQuizzStarting;
    public Action<float, float> OnCountdownUpdated;
    public Action OnReplayRequested;
    public Action OnQuitRequested;
    public Action OnQuizzFinished;
    public bool HasBeenStarted = false;

    [SerializeField] private float quizzStartingCountdownTime;
    [SerializeField] private GameObject gameEndMenu;
    [SerializeField] GameObject questionCanvas;

    private Coroutine startingGame;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        OnQuizzStarting += callCoroutineToCountdown;
        OnQuizzStarting += deactivateQuizzEndPanel;
        OnQuizzStarting += activateQuestionPanel;

        OnReplayRequested += handleOnReplayRequested;

        OnQuizzFinished += activateQuizzEndPanel;
    }

    private void OnDisable()
    {
        OnQuizzStarting -= callCoroutineToCountdown;
        OnQuizzStarting -= deactivateQuizzEndPanel;
        OnQuizzStarting -= activateQuestionPanel;

        OnReplayRequested -= handleOnReplayRequested;

        OnQuizzFinished -= activateQuizzEndPanel;
    }

    private void callCoroutineToCountdown()
    {
        StartCoroutine(countDownToStartGame());
    }

    private IEnumerator countDownToStartGame()
    {
        IEnumerator delayedExecution = executeAfter(quizzStartingCountdownTime, () => OnQuizzStarted?.Invoke());

        while (delayedExecution.MoveNext())
        {
            if (delayedExecution.Current is float remainingTime)
            {
                OnCountdownUpdated?.Invoke(quizzStartingCountdownTime, remainingTime);
            }

            yield return null;
        }

        HasBeenStarted = true;
    }

    private void handleOnReplayRequested()
    {
        StopAllCoroutines();
        OnQuizzStarting?.Invoke();
    }

    private void activateQuizzEndPanel()
    {
        gameEndMenu.SetActive(true);
    }

    private void deactivateQuizzEndPanel()
    {
        gameEndMenu.SetActive(false);
    }

    private void activateQuestionPanel()
    {
        if (questionCanvas == null)
        {
            return;
        }
        questionCanvas?.SetActive(true);
    }

    private IEnumerator executeAfter(float delay, Action action)
    {
        float remainingTime = delay;

        while (remainingTime > 0)
        {
            yield return remainingTime;
            remainingTime -= Time.deltaTime;
        }

        action?.Invoke();
    }
}
