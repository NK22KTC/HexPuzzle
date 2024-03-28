using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    [SerializeField]
    Slider[] soundSliders;

    [SerializeField]
    InputField[] inputFields;

    void Start()
    {
        for (int i = 0; i < soundSliders.Length; i++)
        {
            ChangedInSlider(i);
        }
    }

    void Update()
    {
        
    }

    public void ChangedInSlider(int i)
    {
        inputFields[i].text = soundSliders[i].value.ToString("F");
    }

    public void ChangeInInputField(int i)
    {
        soundSliders[i].value = float.Parse(inputFields[i].text);
    }
}