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
        cullentOpenPage = 0;  //HowToPlay�I�����ɍŏ��̃y�[�W���J��
        SetPageIndex(cullentOpenPage);

        foreach (GameObject page in pages)  //�\���y�[�W���������A
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


        nextButton.onClick.AddListener(() =>  //���̃y�[�W�{�^���ɏ����ǉ�
        {
            SoundManager.instance.PlaySE(6);
        });
        beforeButton.onClick.AddListener(() =>  //�O�̃y�[�W�{�^���ɏ����ǉ�
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

        pages[cullentOpenPage].gameObject.SetActive(false);  //���݊J���Ă���y�[�W���\���ɂ���
        pages[newOpenPage].gameObject.SetActive(true);  //���ɊJ���y�[�W��\������
        cullentOpenPage = newOpenPage;

        SetPageIndex(newOpenPage);
        SetPageButtonVisible();
    }

    private void SetPageButtonVisible()
    {
        beforeButton.gameObject.SetActive(cullentOpenPage != 0);  //�ŏ��̃y�[�W���J���Ă�Ƃ��ȊO�͌�����
        nextButton.gameObject.SetActive(cullentOpenPage != pages.Length - 1);  //�Ō�̃y�[�W���J���Ă�Ƃ��ȊO�͌�����
    }

    private void SetPageIndex(int i)
    {
        i++;  //�z��̓Y�������y�[�W�ԍ��ɕς���
        pageIndexText.text = i.ToString();
    }
}
