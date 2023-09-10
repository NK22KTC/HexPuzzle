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

    // Start is called before the first frame update
    void Start()
    {
        optionManager = GetComponent<OptionManager>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayBGM();
        //ChangeSoundVolume();
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

    //void ChangeSoundVolume()
    //{
    //    for (int i = 0; i < soundSliders.Length; i++)
    //    {
    //        sources[i].volume = soundSliders[i].value;
    //    }
    //}

    public void OnValueChange(int i)
    {
        sources[i].volume = soundSliders[i].value;
    }
}
