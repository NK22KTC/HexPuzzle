using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfomation : MonoBehaviour
{
    public enum ItemTaste { Warmly, Icy, Fresh, Mellow, Richness };  //順にぽかぽか、ひえひえ、さわやか、しっとり、のうこう
    public ItemTaste taste;

    [SerializeField] private int pieceNum = 2;

    public GameObject instantiateObject;

    public bool isFitting = false;

    public int PieceNum => pieceNum;
}