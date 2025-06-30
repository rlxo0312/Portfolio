using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// MonoBehaviour를 상속한 제네릭 싱글톤 클래스
/// 특정 컴포넌트 타입 T에 대해 단 하나의 인스턴스를 보장하며, 없을 경우 자동으로 생성
///
/// <para>사용 변수</para>
/// <para>private static T instance</para>
/// <para>public static T Instance { get; }</para>
///
/// <para>메서드</para>
/// <para>public static T GetOrCreateInstance()</para>
/// <para>protected virtual void Awake()</para>
///
/// <para>제네릭 조건</para>
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
