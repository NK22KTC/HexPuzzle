using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayGameDisplayManager : MonoBehaviour
{
    [SerializeField]
    Text timerText, scoreText, moneyText;

    [SerializeField]
    public GameObject[] shelves;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.Init();
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

    public void OnChangeShelf(InputAction.CallbackContext context)
    {
        if (!context.started) { return; }
        if (!Keyboard.current.fKey.wasPressedThisFrame) { return; }

        for (int i = 0; i < shelves.Length; i++)
        {
            shelves[i].SetActive(!shelves[i].activeSelf);
        }
    }
}
