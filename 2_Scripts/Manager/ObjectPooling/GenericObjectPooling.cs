using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���׸� Ÿ�� T�� ����ϴ� ������ ������Ʈ Ǯ�� Ŭ����
/// IPooling �������̽��� �����ϸ�, ������Ʈ Ÿ�� T�� �´� ������Ʈ�� �����ϵ��� ����Ǿ� ����
///
/// <para>��� ����</para>
/// <para>private T prefab</para>
/// <para>private Queue&lt;T&gt; poolQueue</para>
///
/// <para>���׸� ����</para>
/// <para>where T : Component</para>
///
/// <para>�޼���</para>
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
