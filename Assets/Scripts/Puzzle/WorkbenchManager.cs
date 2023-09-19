using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkbenchManager : MonoBehaviour
{

    ItemController itemController;
    GameObject workbench, item;
    Transform[] itemChilds;
    Transform[] workbenchChilds;
    List<ItemInfomation> installingItems = new List<ItemInfomation>();
    int itemPieceNum = 0;

    bool completedMixing = false;

    Color initColor;

    void Start()
    {
        GameObject camera = Camera.main.gameObject;
        itemController = camera.GetComponent<ItemController>();

        SetWorkbench();
    }

    void Update()
    {
        if(workbench == null)
        {
            SetWorkbench();
        }

        ReadInstallingItem();

        if (!completedMixing)
        {
            ChangeColor();
        }
        else
        {
            CheckOnWorkbench();
        }

    }

    public void OnFinishMixing() => completedMixing = true;  //í≤çáÇÃÉ{É^ÉìÇâüÇµÇΩÇ∆Ç´ÇÃèàóù

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

    void ReadInstallingItem()
    {
        installingItems = new List<ItemInfomation>();
        foreach (ItemInfomation iInfo in FindObjectsOfType<ItemInfomation>())
        {
            if (iInfo.isFitting)
            {
                installingItems.Add(iInfo);
            }
        }
    }

    void CheckOnWorkbench()
    {
        itemPieceNum = 0;  //èâä˙âª

        List<ItemInfomation.ItemColor> puttingItemColors = new List<ItemInfomation.ItemColor>();

        foreach (ItemInfomation installingItem in installingItems)
        {
            puttingItemColors.Add(installingItem.Color);
            Debug.Log(installingItem.Color);

            var itemPieces = installingItem.transform.GetComponentsInChildren<Transform>();

            for (int i = 1; i < itemPieces.Length; i++)
            {
                itemPieceNum++;
            }
        }
        
        ScoreManager.instance.score = itemPieceNum;
        Debug.Log(ScoreManager.instance.score);
        completedMixing = false;
    }
}