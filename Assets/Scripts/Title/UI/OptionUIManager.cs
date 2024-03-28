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
            UnityEditor.EditorApplication.isPlaying = false;//�Q�[���v���C�I��
#else
    Application.Quit();//�Q�[���v���C�I��
#endif
        });
    }



    private void OnBGMSliderValueChanged(float f)
    {
        SoundManager.instance.ChangeBGMVol(f);  //BGM���ʕύX
        BGMField.text = f.ToString("F");  //BGM��InputField�̃e�L�X�g��ύX
    }

    private void OnBGMFieldValueChanged(float f)
    {
        if(f > MaxVol)  //���͒l��1���傫�����̐���
        {
            f = MaxVol;
            BGMField.text = f.ToString("F");  //BGM��InputField�̃e�L�X�g��ύX
        }
        if(f < MinVol)   //���͒l��0��菬�������̐���
        {
            f = MinVol;
            BGMField.text = f.ToString("F");  //BGM��InputField�̃e�L�X�g��ύX
        }

        SoundManager.instance.ChangeBGMVol(f);  //BGM���ʕύX
        BGMSlier.value = f;  //BGMSlider�̒l�ύX
    }

    private void OnSESliderValueChanged(float f)
    {
        SoundManager.instance.ChangeSEVol(f);  //SE���ʕύX
        SEField.text = f.ToString("F");  //SE��InputField�̃e�L�X�g��ύX

        SoundManager.instance.PlaySE(6);  //���ʊm�F
    }

    private void OnSEFieldValueChanged(float f)
    {
        if (f > MaxVol)  //���͒l��1���傫�����̐���
        {
            f = MaxVol;
            SEField.text = f.ToString("F");  //BGM��InputField�̃e�L�X�g��ύX
        }
        if (f < MinVol)   //���͒l��0��菬�������̐���
        {
            f = MinVol;
            SEField.text = f.ToString("F");  //BGM��InputField�̃e�L�X�g��ύX
        }

        SoundManager.instance.ChangeSEVol(f);  //SE���ʕύX
        SESlier.value = f;    //SESlider�̒l�ύX

        SoundManager.instance.PlaySE(6);  //���ʊm�F
    }


    private void SetBGMFieldText() => BGMField.text = BGMSlier.value.ToString("F");
    private void SetSEFieldText() => SEField.text = SESlier.value.ToString("F");
}
