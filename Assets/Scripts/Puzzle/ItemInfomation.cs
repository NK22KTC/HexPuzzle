using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class ItemInfomation : MonoBehaviour
{
    public enum ItemTaste { Warmly, Icy, Fresh, Mellow, Richness };  //���ɂۂ��ۂ��A�Ђ��Ђ��A����₩�A�����Ƃ�A�̂�����
    public ItemTaste taste;

    public GameObject instantiateObject;

    public bool isFitting = false;
}