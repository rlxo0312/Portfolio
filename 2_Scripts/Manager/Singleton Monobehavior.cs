using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// MonoBehaviour�� ����� ���׸� �̱��� Ŭ����
/// Ư�� ������Ʈ Ÿ�� T�� ���� �� �ϳ��� �ν��Ͻ��� �����ϸ�, ���� ��� �ڵ����� ����
///
/// <para>��� ����</para>
/// <para>private static T instance</para>
/// <para>public static T Instance { get; }</para>
///
/// <para>�޼���</para>
/// <para>public static T GetOrCreateInstance()</para>
/// <para>protected virtual void Awake()</para>
///
/// <para>���׸� ����</para>
/// <para>where T : Component</para>
/// </summary>

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : Component
{
    private static T instance;
    public static T Instance => instance;

    public static T GetOrCreateInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType(typeof(T)) as T; // Get

            if (instance == null)
            {
                GameObject newGameObject = new GameObject(typeof(T).Name, typeof(T));
                instance = newGameObject.GetComponent<T>();
            }
            return instance;
        }
        return instance;
    }

    protected virtual void Awake()
    {
        instance = this as T;

        if (Application.isPlaying == true)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
