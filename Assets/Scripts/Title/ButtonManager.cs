using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    TitleManager titleManager;

    // Start is called before the first frame update
    void Start()
    {
        titleManager = GetComponent<TitleManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnDisplayNewGroup(GameObject displayGroup)
    {
        for(int i = 0; i < Enum.GetValues(typeof(TitleManager.DisplayGroup)).Length; i++)
        {
            if ( ((TitleManager.DisplayGroup)i).ToString() == displayGroup.name )
            {
                titleManager.displayingGroup = (TitleManager.DisplayGroup)i;
                return;
            }
        }
    }

    public void OnStartGame(int loadSceneNum) => StartCoroutine(titleManager.LoadingScene(loadSceneNum));
}
