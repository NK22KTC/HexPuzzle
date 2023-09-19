using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    internal int score = 0;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            InitializeVariables();
            Destroy(gameObject);
        }
    }

    private void InitializeVariables()
    {
        instance.score = 0;
    }
}
