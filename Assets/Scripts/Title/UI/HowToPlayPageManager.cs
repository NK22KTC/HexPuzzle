using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlayPageManager : MonoBehaviour
{
    [SerializeField] private GameObject[] pages;
    private int cullentOpenPage;

    [SerializeField] private Button nextButton;
    [SerializeField] private Button beforeButton;
    [SerializeField] private Text pageIndexText;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        cullentOpenPage = 0;  //HowToPlay選択時に最初のページを開く
        SetPageIndex(cullentOpenPage);

        foreach (GameObject page in pages)  //表示ページを初期化、
        {
            if (page == pages[cullentOpenPage])
            {
                page.SetActive(true);
            }
            else
            {
                page.SetActive(false);
            }
        }


        nextButton.onClick.AddListener(() =>  //次のページボタンに処理追加
        {
            SoundManager.instance.PlaySE(6);
        });
        beforeButton.onClick.AddListener(() =>  //前のページボタンに処理追加
        {
            SoundManager.instance.PlaySE(6);
        });

        SetPageButtonVisible();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetVisiblePage(int num)
    {
        int newOpenPage = cullentOpenPage + num;

        pages[cullentOpenPage].gameObject.SetActive(false);  //現在開いているページを非表示にする
        pages[newOpenPage].gameObject.SetActive(true);  //次に開くページを表示する
        cullentOpenPage = newOpenPage;

        SetPageIndex(newOpenPage);
        SetPageButtonVisible();
    }

    private void SetPageButtonVisible()
    {
        beforeButton.gameObject.SetActive(cullentOpenPage != 0);  //最初のページを開いてるとき以外は見える
        nextButton.gameObject.SetActive(cullentOpenPage != pages.Length - 1);  //最後のページを開いてるとき以外は見える
    }

    private void SetPageIndex(int i)
    {
        i++;  //配列の添え字をページ番号に変える
        pageIndexText.text = i.ToString();
    }
}
