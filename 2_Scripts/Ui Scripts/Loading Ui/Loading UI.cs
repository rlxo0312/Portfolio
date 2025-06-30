using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// �� ��ȯ �� �ε� ȭ���� �����ϴ� Ŭ����
/// 
/// <para>��� ����</para>
/// <para>static string nextScene : ������ �ε��� �� �̸�</para>
/// <para>Image FillImage : �ε� �� �̹���</para>
/// 
/// <para>��� �޼���</para>
/// <para>public static void LoadScene(string SceneName)</para>
/// <para>IEnumerator LoadSceneProcess()</para>
/// </summary>
public class LoadingUI : MonoBehaviour
{
    static string nextScene;
    [SerializeField] private Image FillImage;
    /// <summary>
    /// �ε� ���� ���۵Ǹ� �񵿱� �ε� �ڷ�ƾ�� �����մϴ�.
    /// </summary>
    private void Start()
    {
        StartCoroutine(LoadSceneProcess());
    }
    /// <summary>
    /// �ε��ϰ��� �ϴ� �� �̸��� ���޹޾� "Loading Scene"���� ���� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="SceneName">�̵��� ��� �� �̸�</param>
    public static void LoadScene(string SceneName)
    {
        nextScene = SceneName;

        SceneManager.LoadScene("Loading Scene");
    }
    /// <summary>
    /// ������ ���� ���� �񵿱������� �ε��ϸ�, �ε� �ٸ� �ð������� ä��ϴ�.
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
