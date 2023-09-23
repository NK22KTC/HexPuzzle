using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    [SerializeField]
    Text orderText;

    int[] usePieceNum = { 2, 4, 5, 7 };

    List<ItemInfomation.ItemTaste> state;
    List<int> pieceNum;

    void Start()
    {
        MakeOrder();
    }

    public void MakeOrder()
    {
        state = new List<ItemInfomation.ItemTaste>();
        pieceNum = new List<int>();

        for (int i = 0; i < UnityEngine.Random.Range(2, 4); i++)
        {
            while (true)
            {
                ItemInfomation.ItemTaste addTaste = (ItemInfomation.ItemTaste)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ItemInfomation.ItemTaste)).Length);
                int addPieceNum = usePieceNum[UnityEngine.Random.Range(0, usePieceNum.Length)];
                bool canAdd = true;

                for (int j = 0; j < state.Count; j++)
                {
                    if (state[j] == addTaste)
                    {
                        canAdd = false;
                    }
                    if(pieceNum[j] == addPieceNum)
                    {
                        canAdd = false;
                    }
                }

                if (canAdd)
                {
                    state.Add(addTaste);
                    pieceNum.Add(addPieceNum);
                    break;
                }
            }
        }
        UpdateOrderText();
    }

    void UpdateOrderText()
    {
        orderText.text = "";
        for (int i = 0; i < state.Count; ++i)
        {
            switch(state[i])
            {
                case ItemInfomation.ItemTaste.Warmly:
                    orderText.text += "赤色 x " + pieceNum[i].ToString();
                    break;
                case ItemInfomation.ItemTaste.Icy:
                    orderText.text += "水色 x " + pieceNum[i].ToString();
                    break;
                case ItemInfomation.ItemTaste.Fresh:
                    orderText.text += "青色 x " + pieceNum[i].ToString();
                    break;
                case ItemInfomation.ItemTaste.Mellow:
                    orderText.text += "薄茶色 x " + pieceNum[i].ToString();
                    break;
                case ItemInfomation.ItemTaste.Richness:
                    orderText.text += "黄色 x " + pieceNum[i].ToString();
                    break;
            }

            if(i != state.Count - 1)
            {
                orderText.text += ",  ";
            }
        }
    }

    public void ComparisonOrderAndItem(List<ItemInfomation> infos)
    {
        int warmlyNum = 0, icyNum = 0, freshNum = 0, mellowNum = 0, richnessNum = 0;

        for (int i  = 0; i < infos.Count; i++)
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

        for(int j = 0;  j < state.Count; j++)
        {
            int calcNum = 0;
            switch (state[j])
            {
                case ItemInfomation.ItemTaste.Warmly:
                    calcNum += Calc(warmlyNum, pieceNum[state.IndexOf(ItemInfomation.ItemTaste.Warmly)]);
                    break;
                case ItemInfomation.ItemTaste.Icy:
                    calcNum = Calc(icyNum, pieceNum[state.IndexOf(ItemInfomation.ItemTaste.Icy)]);
                    break;
                case ItemInfomation.ItemTaste.Fresh:
                    calcNum += Calc(freshNum, pieceNum[state.IndexOf(ItemInfomation.ItemTaste.Fresh)]);
                    break;
                case ItemInfomation.ItemTaste.Mellow:
                    calcNum += Calc(mellowNum, pieceNum[state.IndexOf(ItemInfomation.ItemTaste.Mellow)]);
                    break;
                case ItemInfomation.ItemTaste.Richness:
                    calcNum += Calc(richnessNum, pieceNum[state.IndexOf(ItemInfomation.ItemTaste.Richness)]);
                    break;
            }
            GameManager.instance.haveMoney += calcNum;
            GameManager.instance.score += calcNum;
        }
    }

    int Calc(int num, int orderNum)
    {
        return num * 7 + (num - orderNum) * 3;
    }
}
