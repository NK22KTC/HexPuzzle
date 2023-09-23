using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkbenchManager : MonoBehaviour
{
    ItemController itemController;
    GameObject workbench, item;
    Transform[] itemChilds;
    Transform[] workbenchChilds;
    List<ItemInfomation> installingItems = new List<ItemInfomation>();
    Button finishButton;

    int itemPieceNum = 0;

    bool completedMixing = false;

    Color initColor;

    void Start()
    {
        GameObject camera = Camera.main.gameObject;
        itemController = camera.GetComponent<ItemController>();

        finishButton = FindObjectOfType<Button>();  //�{�^������������ς���
        finishButton.onClick.AddListener(() => OnFinishMixing());

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

    public void OnFinishMixing()  //�����̃{�^�����������Ƃ��̏���
    {
        completedMixing = true;
        //�������̒ǉ�
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
        itemPieceNum = 0;  //������

        List<ItemInfomation.ItemTaste> puttingItemColors = new List<ItemInfomation.ItemTaste>();

        foreach (ItemInfomation installingItem in installingItems)
        {
            puttingItemColors.Add(installingItem.taste);
            Debug.Log(installingItem.taste);

            var itemPieces = installingItem.transform.GetComponentsInChildren<Transform>();

            for (int i = 1; i < itemPieces.Length; i++)
            {
                itemPieceNum++;
            }
            Destroy(installingItem.gameObject);
        }

        GameManager.instance.score += itemPieceNum * 5;
        Debug.Log(GameManager.instance.score);
        completedMixing = false;

        for (int i = 1; i < workbenchChilds.Length; i++)
        {
            workbenchChilds[i].GetComponent<HexInfomation>().isFitting = false;
        }
    }
}