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

        state = new List<ItemInfomation.ItemTaste>();
        pieceNum = new List<int>();

        for (int i = 0; i < UnityEngine.Random.Range(2, 4); i++)  //�����̐F��2�킩��3��܂łɂ���
        {
            while (true)
            {
                ItemInfomation.ItemTaste addTaste = (ItemInfomation.ItemTaste)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ItemInfomation.ItemTaste)).Length);
                int addPieceNum = usePieceNum[UnityEngine.Random.Range(0, usePieceNum.Length)];
                bool canAdd = true;

                for (int j = 0; j < state.Count; j++)  //�����f�ނ��x�ȏ�w�肵�Ȃ�
                {
                    if (state[j] == addTaste)
                    {
                        canAdd = false;
                    }
                    if(pieceNum[j] == addPieceNum)  //�s�[�X�̐��𑼂̒����̗ʂƓ����ɂ��Ȃ�
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

    void UpdateOrderText()  //����UI�X�V
    {
        if (GameManager.instance.gamePlayingTimer <= 0) { return; }

        orderText.text = "";
        for (int i = 0; i < state.Count; ++i)
        {
            switch (state[i])
            {
                case ItemInfomation.ItemTaste.Warmly:
                    orderText.text += "�ԐF x " + pieceNum[i].ToString();
                    break;
                case ItemInfomation.ItemTaste.Icy:
                    orderText.text += "���F x " + pieceNum[i].ToString();
                    break;
                case ItemInfomation.ItemTaste.Fresh:
                    orderText.text += "�F x " + pieceNum[i].ToString();
                    break;
                case ItemInfomation.ItemTaste.Mellow:
                    orderText.text += "�����F x " + pieceNum[i].ToString();
                    break;
                case ItemInfomation.ItemTaste.Richness:
                    orderText.text += "���F x " + pieceNum[i].ToString();
                    break;
            }

            if (i != state.Count - 1)
            {
                orderText.text += ",  ";
            }
        }
    }

    public void SetFinish()
    {
        if(isFinishing) { return; }
        orderText.text = "--�X--";
        isFinishing = true;
    }

    public void ComparisonOrderAndItem(List<ItemInfomation> infos)  //�����ɑ΂��Ă܂��Ă���
    {
        //���ꂼ��̃s�[�X�̐����v�Z
        int warmlyNum = 0;
        int icyNum = 0;
        int freshNum = 0;
        int mellowNum = 0;
        int richnessNum = 0;

        for (int i  = 0; i < infos.Count; i++)  //�͂܂��Ă���s�[�X�̊m�F
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

        for (int j = 0;  j < state.Count; j++)  //�����Ɣ�r
        {
            switch (state[j])
            {
                case ItemInfomation.ItemTaste.Warmly:
                    calcNum += Calc(warmlyNum, pieceNum[state.IndexOf(ItemInfomation.ItemTaste.Warmly)]);
                    totalPieceNum += warmlyNum;
                    orderPieceNum += pieceNum[state.IndexOf(ItemInfomation.ItemTaste.Warmly)];
                    break;
                case ItemInfomation.ItemTaste.Icy:
                    calcNum = Calc(icyNum, pieceNum[state.IndexOf(ItemInfomation.ItemTaste.Icy)]);
                    totalPieceNum += icyNum;
                    orderPieceNum += pieceNum[state.IndexOf(ItemInfomation.ItemTaste.Icy)];
                    break;
                case ItemInfomation.ItemTaste.Fresh:
                    calcNum += Calc(freshNum, pieceNum[state.IndexOf(ItemInfomation.ItemTaste.Fresh)]);
                    totalPieceNum += freshNum;
                    orderPieceNum += pieceNum[state.IndexOf(ItemInfomation.ItemTaste.Fresh)];
                    break;
                case ItemInfomation.ItemTaste.Mellow:
                    calcNum += Calc(mellowNum, pieceNum[state.IndexOf(ItemInfomation.ItemTaste.Mellow)]);
                    totalPieceNum += mellowNum;
                    orderPieceNum += pieceNum[state.IndexOf(ItemInfomation.ItemTaste.Mellow)];
                    break;
                case ItemInfomation.ItemTaste.Richness:
                    calcNum += Calc(richnessNum, pieceNum[state.IndexOf(ItemInfomation.ItemTaste.Richness)]);
                    totalPieceNum += richnessNum;
                    orderPieceNum += pieceNum[state.IndexOf(ItemInfomation.ItemTaste.Richness)];
                    break;
            }
        }
        GameManager.instance.haveMoney += calcNum;
        GameManager.instance.score += calcNum;

        JudgePlaySE(totalPieceNum, orderPieceNum);
    }

    int Calc(int pieceNum, int orderNum)  //���_�A�ߕs���Ȃ��ōō����_
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
