using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexInfomation : MonoBehaviour
{
    [Header("HexVector")]
    public int q = 0;
    public int r = 0;
    public int s = 0;

    public bool canFitting = false, isFitting = false;
    [HideInInspector]
    public GameObject fittingTarget = null;

    private void Update()
    {
        if (isFitting)
        {
            canFitting = false;
        }
    }
}

//r = 0, 180 の角度の軸, 90度方向に正、270度方向に負
//s = 60, 240 の角度の軸, 120度方向に正、300度方向に負
//q = 120, 300 の角度の軸, 60度方向に正、240度方向に負


//参考サイト
//https://qiita.com/41semicolon/items/ad13719222575a6a4940
//https://www.redblobgames.com/grids/hexagons/
