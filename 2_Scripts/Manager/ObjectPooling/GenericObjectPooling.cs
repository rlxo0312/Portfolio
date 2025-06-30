using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 제네릭 타입 T를 사용하는 유연한 오브젝트 풀링 클래스
/// IPooling 인터페이스를 구현하며, 컴포넌트 타입 T에 맞는 오브젝트를 재사용하도록 설계되어 있음
///
/// <para>사용 변수</para>
/// <para>private T prefab</para>
/// <para>private Queue&lt;T&gt; poolQueue</para>
///
/// <para>제네릭 조건</para>
/// <para>where T : Component</para>
///
/// <para>메서드</para>
/// <para>public void Initialize(T prefab, int size)</para>
/// <para>public GameObject GetFromPool()</para>
/// <para>public void ReturnToPool(GameObject obj)</para>
/// </summary>
public class GenericObjectPooling<T> : MonoBehaviour,IPooling where T : Component
{    
    private T prefab;
    //private int initialSize = 10; 
    private Queue<T> poolQueue;

    public void Initialize(T prefab, int size)
    {
        this.prefab = prefab;
        poolQueue = new Queue<T>(size);    
        //this.initialSize = size;

        for (int i = 0; i < size; i++)
        {
            T obj = Instantiate(prefab, transform);
            obj.gameObject.SetActive(false);
            poolQueue.Enqueue(obj);
        }
    }

    public GameObject GetFromPool()
    {

        T obj = poolQueue.Count > 0 ? poolQueue.Dequeue() : Instantiate(prefab, transform);
        obj.gameObject.SetActive(true);
        return obj.gameObject;
    }


    public void ReturnToPool(GameObject obj)
    {
        T comp = obj.GetComponent<T>();
        if(comp != null)
        {
            obj.gameObject.SetActive(false);
            poolQueue.Enqueue(comp);    
        }
    }
}
