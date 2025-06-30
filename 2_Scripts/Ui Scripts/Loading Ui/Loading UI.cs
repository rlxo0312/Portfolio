using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 씬 전환 시 로딩 화면을 제어하는 클래스
/// 
/// <para>사용 변수</para>
/// <para>static string nextScene : 다음에 로드할 씬 이름</para>
/// <para>Image FillImage : 로딩 바 이미지</para>
/// 
/// <para>사용 메서드</para>
/// <para>public static void LoadScene(string SceneName)</para>
/// <para>IEnumerator LoadSceneProcess()</para>
/// </summary>
public class LoadingUI : MonoBehaviour
{
    static string nextScene;
    [SerializeField] private Image FillImage;
    /// <summary>
    /// 로딩 씬이 시작되면 비동기 로딩 코루틴을 실행합니다.
    /// </summary>
    private void Start()
    {
        StartCoroutine(LoadSceneProcess());
    }
    /// <summary>
    /// 로딩하고자 하는 씬 이름을 전달받아 "Loading Scene"으로 먼저 전환합니다.
    /// </summary>
    /// <param name="SceneName">이동할 대상 씬 이름</param>
    public static void LoadScene(string SceneName)
    {
        nextScene = SceneName;

        SceneManager.LoadScene("Loading Scene");
    }
    /// <summary>
    /// 실제로 다음 씬을 비동기적으로 로드하며, 로딩 바를 시각적으로 채웁니다.
    /// </summary>
    /// <returns>IEnumerator</returns>
    IEnumerator LoadSceneProcess()
    {
        //yield return new WaitForSeconds(2f);
        yield return WaitForSecondsCache.Get(2f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene);
        operation.allowSceneActivation = false;
        float timer = 0.1f;

        while (!operation.isDone)
        {
            yield return null; 

            if(operation.progress < 0.1f)
            {
                FillImage.fillAmount = operation.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;

                FillImage.fillAmount = Mathf.Lerp(0, 1f, timer);

                if(FillImage.fillAmount >= 1f)
                {
                    //yield return new WaitForSeconds(1f);
                    yield return WaitForSecondsCache.Get(1f);
                    operation.allowSceneActivation = true;
                }
                yield return null;
            }
        }
    }
}
