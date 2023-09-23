using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : GameSystems
{
    [SerializeField]
    GameObject fadeImageObject;

    [SerializeField]
    Text scoreText, moneyText;

    Image fadeImage;

    public bool changeScene = false;

    // Start is called before the first frame update
    void Start()
    {
        fadeImage = fadeImageObject.GetComponent<Image>();

        DisplayScoreMoneyText();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(changeScene)
        {
            GameManager.instance.InitializeVariables();
            FadeOut(fadeImageObject, fadeImage);
            if (fadeImage.color.a >= 1)
            {
                StartCoroutine(LoadingScene());
            }
        }
    }

    void DisplayScoreMoneyText()
    {
        scoreText.text += GameManager.instance.score;
        moneyText.text += GameManager.instance.haveMoney;
    }
}
