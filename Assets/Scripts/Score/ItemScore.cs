using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemScore", menuName = "ScriptableObjects/ItemScoreAsset")]
public class ItemScore : ScriptableObject
{
    public List<ItemData> ItemDataList = new List<ItemData>();
}

[System.Serializable]
public class ItemData
{
    public GameObject itemName;
    public int score;
}