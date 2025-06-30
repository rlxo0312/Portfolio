using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 해상도 설정 및 전체 화면 전환 기능을 제공하는 클래스
/// 
/// <para>사용 변수</para>
/// <para>screenMode : 현재 전체화면 모드</para>
/// <para>dropdown : 해상도 선택 드롭다운</para>
/// <para>fullScreenBtn : 전체 화면 여부를 제어하는 토글 버튼</para>
/// <para>resolution : 시스템에서 지원하는 해상도 목록</para>
/// <para>currentResolutionNum : 선택된 해상도 인덱스</para>
/// 
/// <para>사용 메서드</para>
/// <para>LoadComponents() : 드롭다운과 토글 초기화</para>
/// <para>DropboxOptionChange() : 해상도 선택 변경시 호출</para>
/// <para>FullScreenButton() : 전체화면 버튼 상태에 따라 모드 설정</para>
/// <para>ChangeResolution() : 실제 화면 해상도 및 모드 적용</para>
/// </summary>
public class SetResolution : MonoBehaviour
{
    FullScreenMode screenMode;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private Toggle fullScreenBtn; 
    [Header("해상도 저장")]
    List<Resolution> resolution = new List<Resolution>(0);
    int currentResolutionNum;
    private void Start()
    {
        LoadComponents();
    }
    /// <summary>
    /// 드롭다운 옵션을 현재 시스템 해상도 기준으로 채우고 UI 초기화
    /// </summary>
    private void LoadComponents()
    {
        for(int i =0; i< Screen.resolutions.Length; i++)
        {
            resolution.Add(Screen.resolutions[i]);
        }
        dropdown.options.Clear();
        int optionNum = 0; 

        foreach(var res in resolution)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = res.width + "x" + res.height + " " + res.refreshRateRatio + "HZ";

            dropdown.options.Add(option);    

            if(res.width == Screen.width && res.height == Screen.height)
            {
                dropdown.value = optionNum;
            }
            optionNum++;
        }

        dropdown.RefreshShownValue(); 

        fullScreenBtn.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;  
    }
    /// <summary>
    /// 드롭다운에서 해상도 선택이 바뀌었을 때 호출
    /// </summary>
    public void DropboxOptionChange()
    {
        currentResolutionNum = dropdown.value;
    }
    /// <summary>
    /// 전체화면 버튼을 눌렀을 때 설정할 모드를 결정
    /// </summary>
    public void FullScreenButton()
    {
        screenMode = fullScreenBtn.isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }
    /// <summary>
    /// 선택된 해상도와 화면 모드를 적용
    /// </summary>
    public void ChangeResolution()
    {
        Screen.SetResolution(resolution[currentResolutionNum].width,
                             resolution[currentResolutionNum].height,
                             screenMode);                    
    }
}
