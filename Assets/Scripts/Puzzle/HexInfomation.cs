using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexInfomation : MonoBehaviour
{
    [Header("HexVector")]
    public int q = 0;
    public int r = 0;
    public int s = 0;
    [Space]
    public int hexRotation = 0;
    [HideInInspector]
    public bool canFitting = false, isFitting = false;
}

public class HexTransform
{
    public int r, s, q;
    //r = 0, 180 �̊p�x�̎�, 90�x�����ɐ��A270�x�����ɕ�
    //s = 60, 240 �̊p�x�̎�, 120�x�����ɐ��A300�x�����ɕ�
    //q = 120, 300 �̊p�x�̎�, 60�x�����ɐ��A240�x�����ɕ�
}

//�Q�l�T�C�g
//https://qiita.com/41semicolon/items/ad13719222575a6a4940
//https://www.redblobgames.com/grids/hexagons/
