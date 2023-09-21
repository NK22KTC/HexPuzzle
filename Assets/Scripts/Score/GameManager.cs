using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : SingletonDontDestroy<GameManager>
{
    public float onePlayTime = 90f;
    internal float gamePlayingTimer;

    internal int score = 0;

    int initialMoney = 150;
    internal int haveMoney;

    bool isStartPlaying = false;

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
        if(gamePlayingTimer != 0 && isStartPlaying)
        {
            return 0;
        }
        else
        {
            return time -= Time.deltaTime;
        }
    }
}