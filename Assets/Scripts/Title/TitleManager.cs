using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] displayGroups;

    [SerializeField]
    GameObject fadeImage;

    Color fadeColor;

    public enum DisplayGroup { TitleGroup, OptionGroup, HowToOperateGroup };

    [HideInInspector] 
    public DisplayGroup displayingGroup = DisplayGroup.TitleGroup;
    private DisplayGroup nowDisplayingGroup = DisplayGroup.TitleGroup;

    void Awake()
    {
        UpdateDisplayGroup();
    }

    void Start()
    {
        fadeColor = fadeImage.GetComponent<Image>().color;
    }

    void Update()
    {
        if(displayingGroup != nowDisplayingGroup)
        {
            nowDisplayingGroup = displayingGroup;
            UpdateDisplayGroup();
        }
    }

    void UpdateDisplayGroup()
    {
        foreach (GameObject group in displayGroups)
        {
            if(group.name == displayingGroup.ToString())
            {
                group.SetActive(true);
            }
            else
            {
                group.SetActive(false);
            }
        }
    }

    public IEnumerator LoadingScene(int loadSceneNum)
    {
        fadeImage.SetActive(true);

        Color color = fadeColor;
        color.a += Time.deltaTime;
        fadeColor = color;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(loadSceneNum);
        while (!asyncLoad.isDone && color.a >= 1)
        {
            // シーンの読み込みが終わるまでインジケータを表示し回転させる
            //indicator.transform.Rotate(Vector3.back, 2);
            yield return null;
        }
    }
}
