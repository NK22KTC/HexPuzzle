using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonDontDestroy<GameManager>
{
    public float onePlayTime = 90f;
    internal float gamePlayingTimer;

    public float finishDisplayTime = 2.5f;

    internal int score = 0;

    public int initialMoney = 150;
    internal int haveMoney;

    public bool isStartPlaying = false, isDisplayfinishing = false, fading = false;

    public void InitializeVariables()
    {
        instance.score = 0;
        gamePlayingTimer = onePlayTime;
        haveMoney = initialMoney;

        isStartPlaying = false;
        isDisplayfinishing = false;
        fading = false;
    }

    private void Start()
    {
        InitializeVariables();
    }

    void Update()
    {
        if(SceneManager.GetActiveScene().name == "GameScene")
        {
            if (fading)
            {
                //GameSystems.FadeOut();
                SceneManager.LoadScene("ResultScene");
            }
            else if (isDisplayfinishing)
            {
                finishDisplayTime = CountDown(finishDisplayTime);
            }
            else
            {
                gamePlayingTimer = CountDown(gamePlayingTimer);
            }
        }
    }

    public float CountDown(float time)
    {
        //Debug.Log(gamePlayingTimer);

        if (time <= 0)
        {
            time = -1;
        }
        else
        {
            time -= Time.deltaTime;
        }

        if(finishDisplayTime < 0 && gamePlayingTimer < 0)
        {
            fading = true;
        }
        else if (gamePlayingTimer < 0)
        {
            isStartPlaying = false;
            isDisplayfinishing = true;
        }

        return time;
    }

    public void ScoreAndMoneyUpdate(int calcNum, bool addScore)
    {
        if (addScore)
        {
            score += calcNum;
        }
        haveMoney += calcNum;
    }
}