using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� UI �� �ֿ� ��ư�� ������ �����ϴ� Ŭ����
/// 
/// <para>��� ����</para>
/// <para>public Button BackButton, ReturnToLpbbyButton, GameQuitButton</para>
/// <para>private GamePlayUiManager gamePlayUiManager</para>
/// 
/// <para>��� �޼���</para>
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
