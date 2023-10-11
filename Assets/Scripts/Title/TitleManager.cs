using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : GameSystems
{
    [SerializeField]
    GameObject[] displayGroups;

    [SerializeField]
    GameObject fadeImageObject;
    [SerializeField]
    Image fadeImage;

    public enum DisplayGroup { TitleGroup, OptionGroup, HowToOperateGroup };
    [HideInInspector] 
    public DisplayGroup displayingGroup = DisplayGroup.TitleGroup;
    private DisplayGroup nowDisplayingGroup = DisplayGroup.TitleGroup;



    [HideInInspector]
    public bool startGame = false;

    void Awake()
    {
        UpdateDisplayGroup();
    }

    void FixedUpdate()
    {
        if(displayingGroup != nowDisplayingGroup)
        {
            nowDisplayingGroup = displayingGroup;
            UpdateDisplayGroup();
        }

        if (startGame)
        {
            FadeOut(fadeImageObject, fadeImage);
            if(fadeImage.color.a >= 1)
            {
                LoadingScene();
            }
        }
    }

    void UpdateDisplayGroup()
    {
        foreach (GameObject group in displayGroups)
        {
            if(group.name == displayingGroup.ToString())
            {
                group.SetActive(true);
            }
            else
            {
                group.SetActive(false);
            }
        }
    }
}
