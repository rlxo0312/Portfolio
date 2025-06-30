using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// ���� ���� �� ���Ḧ �����ϴ� UI ���� Ŭ����
/// 
/// <para>��� �޼���</para>
/// <para>GameStart() : �ΰ��� ������ ��ȯ</para>
/// <para>GameQuit() : ���� ����</para>
/// </summary>
public class UiManager : MonoBehaviour
{
    public void GameStart()
    {
        LoadingUI.LoadScene("In Game Scene");
    }
    /// <summary>
    /// ������ �����մϴ�.
    /// ������ ���� ���� ���, ������ ����ϴ�.
    /// </summary>
    public void GameQuit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif 
        Application.Quit();
    }
}
