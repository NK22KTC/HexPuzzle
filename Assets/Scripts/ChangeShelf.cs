using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChangeShelf : MonoBehaviour
{
    [SerializeField]
    GameObject[] shelves;

    public void OnChangeShelf(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                for (int i = 0; i < shelves.Length; i++)
                {
                    shelves[i].SetActive(!shelves[i].activeSelf);
                }
            }
        }
    }
}
