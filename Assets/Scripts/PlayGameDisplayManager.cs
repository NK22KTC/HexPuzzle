using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayGameDisplayManager : MonoBehaviour
{
    [SerializeField]
    Text timerText, scoreText, moneyText;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.InitializeVariables();
        GameManager.instance.isStartPlaying = true;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();
    }

    void UpdateText()
    {
        timerText.text = ((int)GameManager.instance.gamePlayingTimer + 1).ToString();
        scoreText.text = "ÉXÉRÉA : " + GameManager.instance.score.ToString();
        moneyText.text = "èäéùã‡ : " + GameManager.instance.haveMoney.ToString();
    }
}
