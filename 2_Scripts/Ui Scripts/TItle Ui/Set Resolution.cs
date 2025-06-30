using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ػ� ���� �� ��ü ȭ�� ��ȯ ����� �����ϴ� Ŭ����
/// 
/// <para>��� ����</para>
/// <para>screenMode : ���� ��üȭ�� ���</para>
/// <para>dropdown : �ػ� ���� ��Ӵٿ�</para>
/// <para>fullScreenBtn : ��ü ȭ�� ���θ� �����ϴ� ��� ��ư</para>
/// <para>resolution : �ý��ۿ��� �����ϴ� �ػ� ���</para>
/// <para>currentResolutionNum : ���õ� �ػ� �ε���</para>
/// 
/// <para>��� �޼���</para>
/// <para>LoadComponents() : ��Ӵٿ�� ��� �ʱ�ȭ</para>
/// <para>DropboxOptionChange() : �ػ� ���� ����� ȣ��</para>
/// <para>FullScreenButton() : ��üȭ�� ��ư ���¿� ���� ��� ����</para>
/// <para>ChangeResolution() : ���� ȭ�� �ػ� �� ��� ����</para>
/// </summary>
public class SetResolution : MonoBehaviour
{
    FullScreenMode screenMode;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private Toggle fullScreenBtn; 
    [Header("�ػ� ����")]
    List<Resolution> resolution = new List<Resolution>(0);
    int currentResolutionNum;
    private void Start()
    {
        LoadComponents();
    }
    /// <summary>
    /// ��Ӵٿ� �ɼ��� ���� �ý��� �ػ� �������� ä��� UI �ʱ�ȭ
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
    /// ��Ӵٿ�� �ػ� ������ �ٲ���� �� ȣ��
    /// </summary>
    public void DropboxOptionChange()
    {
        currentResolutionNum = dropdown.value;
    }
    /// <summary>
    /// ��üȭ�� ��ư�� ������ �� ������ ��带 ����
    /// </summary>
    public void FullScreenButton()
    {
        screenMode = fullScreenBtn.isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }
    /// <summary>
    /// ���õ� �ػ󵵿� ȭ�� ��带 ����
    /// </summary>
    public void ChangeResolution()
    {
        Screen.SetResolution(resolution[currentResolutionNum].width,
                             resolution[currentResolutionNum].height,
                             screenMode);                    
    }
}
