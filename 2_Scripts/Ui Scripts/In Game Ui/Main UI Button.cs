using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 설정 UI 내 주요 버튼의 동작을 정의하는 클래스
/// 
/// <para>사용 변수</para>
/// <para>public Button BackButton, ReturnToLpbbyButton, GameQuitButton</para>
/// <para>private GamePlayUiManager gamePlayUiManager</para>
/// 
/// <para>사용 메서드</para>
/// <para>public void Awake()</para>
/// </summary>
public class MainUIButton : MonoBehaviour
{
    public Button BackButton;
    public Button ReturnToLpbbyButton;
    public Button GameQuitButton;

    [SerializeField] private GamePlayUiManager gamePlayUiManager;
    

    public void Awake()
    {
        BackButton.onClick.AddListener(() => gamePlayUiManager.CloseUiMenu());
        ReturnToLpbbyButton.onClick.AddListener(()  => gamePlayUiManager.ReturnToUIScene());
        GameQuitButton.onClick.AddListener(() => gamePlayUiManager.GameQuit());
    }
}
