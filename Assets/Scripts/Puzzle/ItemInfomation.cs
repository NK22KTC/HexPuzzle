using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfomation : MonoBehaviour
{
    public enum ItemTaste { Warmly, Icy, Fresh, Mellow, Richness };  //順にぽかぽか、ひえひえ、さわやか、しっとり、のうこう
    public ItemTaste taste;

    public GameObject instantiateObject;

    public bool isFitting = false;
}