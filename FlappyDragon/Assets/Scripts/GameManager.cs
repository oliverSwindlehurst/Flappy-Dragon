using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public delegate void GameDelage();
    public static event GameDelage OnGameStart;
    public static event GameDelage OnGameOverConfirmed;

    public static GameManager Instance;

    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countDownPage;
    public Text scoreText;

    enum PageState
    {
        None,
        Start,
        GameOver,
        Countdown
    }

    int score = 0;
    bool gameOver = true;

    public bool GameOver { get { return gameOver; } }

    private void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        CountdownText.OnCountdownFinished += OnCountdownFinished;
        TapControler.OnPlayerScored += OnPlayerScored;
        TapControler.OnPlayerDied += OnPlayerDied;
    }

    void OnDisable()
    {
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
        TapControler.OnPlayerScored -= OnPlayerScored;
        TapControler.OnPlayerDied -= OnPlayerDied;
    }

    void OnCountdownFinished()
    {
        SetPageState(PageState.None);
        OnGameStart(); //Event sent to tapController
        score = 0;
        gameOver = false;
    }

    void OnPlayerScored()
    {
        score++;
        scoreText.text = score.ToString();
    }

    void OnPlayerDied()
    {
        gameOver = true;
        int savedScore = PlayerPrefs.GetInt("HighScore");
        if (score > savedScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        SetPageState(PageState.GameOver);
    }

    void SetPageState(PageState state)
    {
        switch (state)
        {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countDownPage.SetActive(false);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countDownPage.SetActive(false);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countDownPage.SetActive(false);
                break;
            case PageState.Countdown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countDownPage.SetActive(true);
                break;
        }
    }

    public void ConfirmGameOver()
    {
        //activated when replay button is hit
        OnGameOverConfirmed(); //Event sent to tapController
        scoreText.text = "0";
        SetPageState(PageState.Start);
    }

    public void StartGame()
    {
        //activated when play button is hit
        SetPageState(PageState.Countdown);
    }
}