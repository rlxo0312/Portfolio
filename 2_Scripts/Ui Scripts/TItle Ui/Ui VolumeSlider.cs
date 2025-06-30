using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

/// <summary>
/// ����� �ͼ��� ������ UI �����̴��� ���� �����ϴ� Ŭ����
/// 
/// <para>��� ����</para>
/// <para>public Slider slider</para>
/// <para>public string parameter</para>
/// <para>private AudioMixer audioMixer</para>
/// <para>private float sliderValue</para>
/// 
/// <para>��� �޼���</para>
/// <para>public void SliderValue(float _value)</para>
/// <para>public void LoadSlider(float _value)</para>
/// </summary>
public class UiVolumSlifer : MonoBehaviour
{
    public Slider slider;
    public string parameter;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float sliderValue;

    /*void Start()
    {
        if (slider != null)
        {
            float value = slider.value;
            SliderValue(value); // �����̴� ���� �� �ݿ�
        }
    }*/
    /// <summary>
    /// �����̴� �Է°��� �޾� �α� �����Ϸ� ��ȯ �� AudioMixer ������ �����մϴ�.
    /// </summary>
    /// <param name="_value">�����̴����� ���޵� ���� �� (0~1)</param>
    public void SliderValue(float _value)
    {
        //audioMixer.SetFloat(parameter, Mathf.Log10(_value) * sliderValue);
        float volume = Mathf.Log10(Mathf.Clamp(_value, 0.001f, 1f)) * sliderValue;
        audioMixer.SetFloat(parameter, volume);
    }
    /// <summary>
    /// �ܺο��� ����� ���� �����̴��� �ε��Ͽ� �����մϴ�.
    /// </summary>
    /// <param name="_value">�ҷ��� �����̴� ��</param>
    public void LoadSlider(float _value)
    {
        slider.value = _value;
        if (_value >= 0.001f)
        {
            slider.value = _value;
        }
        //slider.value = Mathf.Clamp(_value, minSliderValue, 1f);
    }
}
