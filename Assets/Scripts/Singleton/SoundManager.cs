using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : SingletonDontDestroy<SoundManager>
{
    OptionManager optionManager;
    GameManager gameManager;

    [SerializeField]
    Slider[] soundSliders;

    [SerializeField]
    AudioSource[] sources;  //sources[0]��BGM,sources[1]��SE

    [SerializeField]
    AudioClip[] BGM_Clips;

    [SerializeField]
    AudioClip[] SE_Clips;

    int nowLoadSceneIndex = 0;

    void Start()
    {
        optionManager = GetComponent<OptionManager>();
        for(int i = 0; i < soundSliders.Length; i++)
        {
            OnValueChange(i);
        }

        nowLoadSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    void Update()
    {
        if (!sources[0].isPlaying)
        {
            PlayBGM();
        }

        if(nowLoadSceneIndex != SceneManager.GetActiveScene().buildIndex)
        {
            ChangeBGM();
        }
    }

    void PlayBGM()  //BGM���~�܂�����Đ�����
    {
        sources[0].Play();
    }

    void ChangeBGM()
    {
        nowLoadSceneIndex = SceneManager.GetActiveScene().buildIndex;
        sources[0].clip = BGM_Clips[nowLoadSceneIndex];
    }

    public void OnButtonPush(AudioClip clip)  //���ʉ���炷�֐�
        {
            sources[1].clip = clip;
            sources[1].Play();
        }

    public void OnValueChange(int i)
    {
        sources[i].volume = soundSliders[i].value;
    }
}
