using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<FruitController> fruits = new List<FruitController>();
    public int CurrentScore;
    public TextMeshProUGUI scoreTxt;
    public TextMeshProUGUI highscore;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        this.RegisterListener(EventID.GameOver, (sender, param) =>
        {
            GameLose();
        });
        highscore.text = DataAccountPlayer.PlayerInformation.score.ToString();
    }

    private void Start()
    {
        CurrentScore = 0;
    }

    void Update()
    {
        scoreTxt.text = "Score : " + CurrentScore.ToString();
    }
    public void addScore(int score)
    {
        CurrentScore += score;
    }

    private void GameLose()
    {
        DataAccountPlayer.PlayerInformation.SaveScore(CurrentScore);
        Time.timeScale = 0f;
    }
    public void QuitGame()
    {
        Application.Quit();
        FruitManager.instance.SaveFallenFruitsData();
    }
}
