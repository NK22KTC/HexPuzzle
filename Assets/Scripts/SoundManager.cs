using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : SingletonDontDestroy<SoundManager>
{
    OptionManager optionManager;

    [SerializeField]
    Slider[] soundSliders;

    [SerializeField]
    AudioSource[] sources;  //sources[0]‚ªBGM,sources[1]‚ªSE

    [SerializeField]
    AudioClip[] BGM_Clips;

    [SerializeField]
    AudioClip[] SE_Clips;

    void Start()
    {
        optionManager = GetComponent<OptionManager>();
        for(int i = 0; i < soundSliders.Length; i++)
        {
            OnValueChange(i);
        }
    }

    void Update()
    {
        PlayBGM();
    }

    public void OnButtonPush(AudioClip clip)
    {
        sources[1].clip = clip;
        sources[1].Play();
    }

    void PlayBGM()
    {
        if (!sources[0].isPlaying)
        {
            sources[0].Play();
        }
    }

    public void OnValueChange(int i)
    {
        sources[i].volume = soundSliders[i].value;
    }
}
