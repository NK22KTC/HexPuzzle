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
        if (!context.started) { return; }
        if (Keyboard.current.fKey.wasPressedThisFrame) { return; }

        for (int i = 0; i < shelves.Length; i++)
        {
            shelves[i].SetActive(!shelves[i].activeSelf);
        }
    }
}
