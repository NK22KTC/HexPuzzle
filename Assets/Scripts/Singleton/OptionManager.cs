using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : SingletonDontDestroy<OptionManager>
{
    SoundManager soundManager;

    [SerializeField]
    Slider[] soundSliders;

    // Start is called before the first frame update
    void Start()
    {
        soundManager = GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}