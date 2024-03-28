using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUIManager : MonoBehaviour
{
    [SerializeField] private Slider BGMSlier;
    [SerializeField] private InputField BGMField;
    [SerializeField] private Slider SESlier;
    [SerializeField] private InputField SEField;
    [SerializeField] private Button ReturnButton;
    [SerializeField] private Button ExitButton;

    private const float MaxVol = 1f;
    private const float MinVol = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        SetBGMFieldText();
        SetSEFieldText();

        BGMSlier.onValueChanged.AddListener(_ => OnBGMSliderValueChanged(BGMSlier.value));
        BGMField.onValueChanged.AddListener(_ => OnBGMFieldValueChanged(float.Parse(BGMField.text)));

        SESlier.onValueChanged.AddListener(_ => OnSESliderValueChanged(SESlier.value));
        SEField.onEndEdit.AddListener(_ => OnSEFieldValueChanged(float.Parse(SEField.text)));

        ReturnButton.onClick.AddListener(() =>
        {
            SoundManager.instance.PlaySE(7);
        });

        ExitButton.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
        });
    }



    private void OnBGMSliderValueChanged(float f)
    {
        SoundManager.instance.ChangeBGMVol(f);  //BGM音量変更
        BGMField.text = f.ToString("F");  //BGMのInputFieldのテキストを変更
    }

    private void OnBGMFieldValueChanged(float f)
    {
        if(f > MaxVol)  //入力値が1より大きい時の制限
        {
            f = MaxVol;
            BGMField.text = f.ToString("F");  //BGMのInputFieldのテキストを変更
        }
        if(f < MinVol)   //入力値が0より小さい時の制限
        {
            f = MinVol;
            BGMField.text = f.ToString("F");  //BGMのInputFieldのテキストを変更
        }

        SoundManager.instance.ChangeBGMVol(f);  //BGM音量変更
        BGMSlier.value = f;  //BGMSliderの値変更
    }

    private void OnSESliderValueChanged(float f)
    {
        SoundManager.instance.ChangeSEVol(f);  //SE音量変更
        SEField.text = f.ToString("F");  //SEのInputFieldのテキストを変更

        SoundManager.instance.PlaySE(6);  //音量確認
    }

    private void OnSEFieldValueChanged(float f)
    {
        if (f > MaxVol)  //入力値が1より大きい時の制限
        {
            f = MaxVol;
            SEField.text = f.ToString("F");  //BGMのInputFieldのテキストを変更
        }
        if (f < MinVol)   //入力値が0より小さい時の制限
        {
            f = MinVol;
            SEField.text = f.ToString("F");  //BGMのInputFieldのテキストを変更
        }

        SoundManager.instance.ChangeSEVol(f);  //SE音量変更
        SESlier.value = f;    //SESliderの値変更

        SoundManager.instance.PlaySE(6);  //音量確認
    }


    private void SetBGMFieldText() => BGMField.text = BGMSlier.value.ToString("F");
    private void SetSEFieldText() => SEField.text = SESlier.value.ToString("F");
}
