using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    [SerializeField]
    Text orderText;

    List<ItemInfomation.ItemTaste> tasteList;
    List<int> pieceNumList;

    private bool isFinishing = false;

    void Start()
    {
        MakeOrder();
    }

    private void FixedUpdate()
    {
        if(GameManager.instance.gamePlayingTimer <= 0)
        {
            SetFinish();
        }
    }

    public void MakeOrder()
    {
        if (GameManager.instance.gamePlayingTimer <= 0) { return; }
        int nextTasteTotal = UnityEngine.Random.Range(2, 4);  //注文の色を2種から3種までにする
        List<ItemInfomation.ItemTaste> tasteInfos = new List<ItemInfomation.ItemTaste>()
        {
            ItemInfomation.ItemTaste.Warmly,
            ItemInfomation.ItemTaste.Icy,
            ItemInfomation.ItemTaste.Fresh,
            ItemInfomation.ItemTaste.Mellow,
            ItemInfomation.ItemTaste.Richness
        };
        List<int> pieceNumInfos = new List<int>(){ 2, 4, 5, 7 };

        tasteList = new List<ItemInfomation.ItemTaste>();
        pieceNumList = new List<int>();


        for (int i = 0; i < nextTasteTotal; i++)
        {
            int tasteIndex = UnityEngine.Random.Range(0, tasteInfos.Count);
            tasteList.Add(tasteInfos[tasteIndex]);
            tasteInfos.RemoveAt(tasteIndex);

            int pieceNumindex = UnityEngine.Random.Range(0, pieceNumInfos.Count);
            pieceNumList.Add(pieceNumInfos[pieceNumindex]);
            pieceNumInfos.RemoveAt(pieceNumindex);
        }

        UpdateOrderText();
    }

    void UpdateOrderText()  //注文UI更新
    {
        if (GameManager.instance.gamePlayingTimer <= 0) { return; }

        orderText.text = "";
        for (int i = 0; i < tasteList.Count; ++i)
        {
            switch (tasteList[i])
            {
                case ItemInfomation.ItemTaste.Warmly:
                    orderText.text += "赤色 x " + pieceNumList[i].ToString();
                    break;
                case ItemInfomation.ItemTaste.Icy:
                    orderText.text += "水色 x " + pieceNumList[i].ToString();
                    break;
                case ItemInfomation.ItemTaste.Fresh:
                    orderText.text += "青色 x " + pieceNumList[i].ToString();
                    break;
                case ItemInfomation.ItemTaste.Mellow:
                    orderText.text += "薄茶色 x " + pieceNumList[i].ToString();
                    break;
                case ItemInfomation.ItemTaste.Richness:
                    orderText.text += "黄色 x " + pieceNumList[i].ToString();
                    break;
            }

            if (i != tasteList.Count - 1)
            {
                orderText.text += ",  ";
            }
        }
    }

    public void SetFinish()
    {
        if(isFinishing) { return; }
        orderText.text = "--閉店--";
        isFinishing = true;
    }

    public void ComparisonOrderAndItem(List<ItemInfomation> infos)  //注文に対してまっている
    {
        //それぞれのピースの数を計算
        int warmlyNum = 0;
        int icyNum = 0;
        int freshNum = 0;
        int mellowNum = 0;
        int richnessNum = 0;

        for (int i  = 0; i < infos.Count; i++)  //はまっているピースの確認
        {
            switch(infos[i].taste)
            {
                case ItemInfomation.ItemTaste.Warmly:
                    warmlyNum += infos[i].gameObject.GetComponentsInChildren<Transform>().Length - 1;
                    break;
                case ItemInfomation.ItemTaste.Icy:
                    icyNum += infos[i].gameObject.GetComponentsInChildren<Transform>().Length - 1;
                    break;
                case ItemInfomation.ItemTaste.Fresh:
                    freshNum += infos[i].gameObject.GetComponentsInChildren<Transform>().Length - 1;
                    break;
                case ItemInfomation.ItemTaste.Mellow:
                    mellowNum += infos[i].gameObject.GetComponentsInChildren<Transform>().Length - 1;
                    break;
                case ItemInfomation.ItemTaste.Richness:
                    richnessNum += infos[i].gameObject.GetComponentsInChildren<Transform>().Length - 1;
                    break;
            }
        }

        int calcNum = 0;
        int totalPieceNum = 0; 
        int orderPieceNum = 0;

        for (int j = 0;  j < tasteList.Count; j++)  //注文と比較
        {
            switch (tasteList[j])
            {
                case ItemInfomation.ItemTaste.Warmly:
                    calcNum += Calc(warmlyNum, pieceNumList[tasteList.IndexOf(ItemInfomation.ItemTaste.Warmly)]);
                    totalPieceNum += warmlyNum;
                    orderPieceNum += pieceNumList[tasteList.IndexOf(ItemInfomation.ItemTaste.Warmly)];
                    break;
                case ItemInfomation.ItemTaste.Icy:
                    calcNum = Calc(icyNum, pieceNumList[tasteList.IndexOf(ItemInfomation.ItemTaste.Icy)]);
                    totalPieceNum += icyNum;
                    orderPieceNum += pieceNumList[tasteList.IndexOf(ItemInfomation.ItemTaste.Icy)];
                    break;
                case ItemInfomation.ItemTaste.Fresh:
                    calcNum += Calc(freshNum, pieceNumList[tasteList.IndexOf(ItemInfomation.ItemTaste.Fresh)]);
                    totalPieceNum += freshNum;
                    orderPieceNum += pieceNumList[tasteList.IndexOf(ItemInfomation.ItemTaste.Fresh)];
                    break;
                case ItemInfomation.ItemTaste.Mellow:
                    calcNum += Calc(mellowNum, pieceNumList[tasteList.IndexOf(ItemInfomation.ItemTaste.Mellow)]);
                    totalPieceNum += mellowNum;
                    orderPieceNum += pieceNumList[tasteList.IndexOf(ItemInfomation.ItemTaste.Mellow)];
                    break;
                case ItemInfomation.ItemTaste.Richness:
                    calcNum += Calc(richnessNum, pieceNumList[tasteList.IndexOf(ItemInfomation.ItemTaste.Richness)]);
                    totalPieceNum += richnessNum;
                    orderPieceNum += pieceNumList[tasteList.IndexOf(ItemInfomation.ItemTaste.Richness)];
                    break;
            }
        }
        GameManager.instance.haveMoney += calcNum;
        GameManager.instance.score += calcNum;

        JudgePlaySE(totalPieceNum, orderPieceNum);
    }

    int Calc(int pieceNum, int orderNum)  //得点、過不足なしで最高得点
    {
        return pieceNum * 7 + (pieceNum - orderNum) * 3;
    }

    void JudgePlaySE(int totalPieceNum, int orderPieceNum)
    {
        if(totalPieceNum >= orderPieceNum / 2)
        {
            SoundManager.instance.PlaySE(3);
        }
        else
        {
            SoundManager.instance.PlaySE(5);
        }
    }
}
