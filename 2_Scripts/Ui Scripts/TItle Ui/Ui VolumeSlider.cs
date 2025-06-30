using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

/// <summary>
/// 오디오 믹서의 볼륨을 UI 슬라이더를 통해 조절하는 클래스
/// 
/// <para>사용 변수</para>
/// <para>public Slider slider</para>
/// <para>public string parameter</para>
/// <para>private AudioMixer audioMixer</para>
/// <para>private float sliderValue</para>
/// 
/// <para>사용 메서드</para>
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
            SliderValue(value); // 슬라이더 시작 값 반영
        }
    }*/
    /// <summary>
    /// 슬라이더 입력값을 받아 로그 스케일로 변환 후 AudioMixer 볼륨을 설정합니다.
    /// </summary>
    /// <param name="_value">슬라이더에서 전달된 볼륨 값 (0~1)</param>
    public void SliderValue(float _value)
    {
        //audioMixer.SetFloat(parameter, Mathf.Log10(_value) * sliderValue);
        float volume = Mathf.Log10(Mathf.Clamp(_value, 0.001f, 1f)) * sliderValue;
        audioMixer.SetFloat(parameter, volume);
    }
    /// <summary>
    /// 외부에서 저장된 값을 슬라이더에 로드하여 설정합니다.
    /// </summary>
    /// <param name="_value">불러올 슬라이더 값</param>
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
