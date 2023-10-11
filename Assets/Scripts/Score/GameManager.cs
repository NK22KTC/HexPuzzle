using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonDontDestroy<GameManager>
{
    [SerializeField]
    GameObject[] characters;
    public GameObject nowCharacter;

    public float onePlayTime = 90f;
    internal float gamePlayingTimer;

    public float initialFinishDisplayTime = 2.5f;
    float finishDisplayTime = 2.5f;

    internal int score = 0;

    public int initialMoney = 150;
    internal int haveMoney;

    public bool isStartPlaying = false, isDisplayfinishing = false, isPlayingOnce = false, fading = false;

    public void InitializeVariables()
    {
        instance.score = 0;
        gamePlayingTimer = onePlayTime;
        haveMoney = initialMoney;
        finishDisplayTime = initialFinishDisplayTime;

        isStartPlaying = false;
        isDisplayfinishing = false;
        fading = false;
    }

    private void Start()
    {
        nowCharacter = characters[0];

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
        if (time <= 0)
        {
            time = -1;
        }
        else
        {
            if(gamePlayingTimer < 0 && !isPlayingOnce)
            {
                SoundManager.instance.PlaySE();
            }
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

    public void InstantiateCharacter()
    {
        int newCharaIndex = 0;
        for(int i = 0; i < characters.Length; i++)
        {
            if (characters[i] == nowCharacter)
            {
                if(i == 2)
                {
                    newCharaIndex = 0;
                    break;
                }
                else
                {
                    newCharaIndex = i + 1;
                    break;
                }
            }
        }

        nowCharacter = characters[newCharaIndex];
        Instantiate(nowCharacter, new Vector3(16.5f, 0, 0), Quaternion.identity);
    }
}