using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : SingletonDontDestroy<GameManager>
{
    public float onePlayTime = 90f;
    internal float gamePlayingTimer;

    internal int score = 0;

    public int initialMoney = 150;
    internal int haveMoney;

    public bool isStartPlaying = false;

    public void InitializeVariables()
    {
        instance.score = 0;
        gamePlayingTimer = onePlayTime;
        haveMoney = initialMoney;
    }

    void Update()
    {
        gamePlayingTimer = CountDown(gamePlayingTimer);
    }

    public float CountDown(float time)
    {
        if(gamePlayingTimer <= 0 && isStartPlaying)
        {
            isStartPlaying = false;
            return 0;
        }
        else
        {
            return time -= Time.deltaTime;
        }
    }

    public void ScoreAndMoneyUpdate(int calcNum, bool changeMoneyValue)
    {
        if (changeMoneyValue)
        {
            haveMoney += calcNum;
            score += Mathf.Abs(calcNum);
        }
        else
        {
            score += calcNum;
        }
    }
}