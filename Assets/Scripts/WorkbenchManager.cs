using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkbenchManager : MonoBehaviour
{
    public GameObject puzzleMolds;
    Transform[] puzzleMold;


    void Start()
    {
        puzzleMold = puzzleMolds.GetComponentsInChildren<Transform>();
        //for (int i = 0; i < puzzleMold.Length; i++)
        //{
        //    Debug.Log(puzzleMold[i].name);
        //}
    }

    void Update()
    {
        
    }
}