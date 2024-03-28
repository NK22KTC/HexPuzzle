using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleGroupUIManager : MonoBehaviour
{

    [SerializeField] private Button startButton;
    [SerializeField] private Button howtoButton;
    [SerializeField] private Button settingsButton;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        startButton.onClick.AddListener(() =>
        {
            SoundManager.instance.PlaySE(6);
        });
        howtoButton.onClick.AddListener(() =>
        {
            SoundManager.instance.PlaySE(6);
        });
        settingsButton.onClick.AddListener(() =>
        {
            SoundManager.instance.PlaySE(6);
        });
    }
}
