using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 게임 시작 및 종료를 제어하는 UI 관리 클래스
/// 
/// <para>사용 메서드</para>
/// <para>GameStart() : 인게임 씬으로 전환</para>
/// <para>GameQuit() : 게임 종료</para>
/// </summary>
public class UiManager : MonoBehaviour
{
    public void GameStart()
    {
        LoadingUI.LoadScene("In Game Scene");
    }
    /// <summary>
    /// 게임을 종료합니다.
    /// 에디터 실행 중일 경우, 실행을 멈춥니다.
    /// </summary>
    public void GameQuit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif 
        Application.Quit();
    }
}
