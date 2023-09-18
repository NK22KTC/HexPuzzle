using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkbenchManager : MonoBehaviour
{
    ItemController itemController;
    GameObject workbench, item;
    Transform[] itemChilds;
    Transform[] workbenchChilds;

    Color initColor;

    void Start()
    {
        itemController = GetComponent<ItemController>();

        SetWorkbench();
    }

    void Update()
    {
        if(workbench == null)
        {
            SetWorkbench();
        }

        ChangeColor();
    }

    void SetWorkbench()
    {
        workbench = GameObject.FindWithTag("Workbench");
        workbenchChilds = workbench.GetComponentsInChildren<Transform>();
        initColor = workbenchChilds[1].GetComponent<SpriteRenderer>().color;
    }

    void ChangeColor()
    {
        for (int i = 1; i < workbenchChilds.Length; i++)
        {
            HexInfomation info_w = workbenchChilds[i].GetComponent<HexInfomation>();
            Color newColor = info_w.GetComponent<SpriteRenderer>().color;

            if (itemController.movementItemParent != null)
            {
                itemChilds = itemController.movementItemParent.GetComponentsInChildren<Transform>();

                if (itemController.canDoFitting && info_w.canFitting)
                {
                    newColor = (initColor + itemChilds[1].GetComponent<SpriteRenderer>().color) / 2f;
                }
                else
                {
                    newColor = initColor;
                }
                info_w.GetComponent<SpriteRenderer>().color = newColor;
            }
            else
            {
                newColor = initColor;
            }

            info_w.GetComponent<SpriteRenderer>().color = newColor;
        }
    }
}