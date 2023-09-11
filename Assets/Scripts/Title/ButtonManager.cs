using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ButtonManager : GameSystems
{
    TitleManager titleManager;

    void Start()
    {
        titleManager = GetComponent<TitleManager>();
    }

    void Update()
    {

    }

    public void OnDisplayNewGroup(GameObject _displayGroup)
    {
        for(int i = 0; i < Enum.GetValues(typeof(TitleManager.DisplayGroup)).Length; i++)
        {
            if ( ((TitleManager.DisplayGroup)i).ToString() == _displayGroup.name )
            {
                titleManager.displayingGroup = (TitleManager.DisplayGroup)i;
                return;
            }
        }
    }

    public void OnStartGame(int _loadSceneNum)
    {
        titleManager.startGame = true;
        loadSceneNum = _loadSceneNum;
    }
}
