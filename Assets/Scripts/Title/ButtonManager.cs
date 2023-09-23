using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : GameSystems
{
    TitleManager titleManager;
    ResultManager resultManager;

    void Start()
    {
        if(SceneManager.GetActiveScene().name == "_TitleScene")
        {
            titleManager = GetComponent<TitleManager>();
        }
        else if(SceneManager.GetActiveScene().name == "ResultScene")
        {
            resultManager = GetComponent<ResultManager>();
        }
        
    }

    void Update()
    {

    }

    public void OnDisplayNewGroup(GameObject _displayGroup)  //TitleÇ≈ï\é¶Ç∑ÇÈUIÇïœçXÇ∑ÇÈ
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

    public void OnChamgeScene(int _loadSceneNum)
    {
        if(titleManager != null)
        {
            titleManager.startGame = true;
        }
        else if(resultManager != null)
        {
            resultManager.changeScene = true;
        }
        loadSceneNum = _loadSceneNum;
    }
}
