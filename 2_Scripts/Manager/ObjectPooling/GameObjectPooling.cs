using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// GameObject를 재활용하기 위한 오브젝트 풀링 클래스입니다.
/// IPooling 인터페이스를 구현하며, Queue와 HashSet을 이용해 오브젝트를 효율적으로 관리합니다.
///
/// <para>사용 변수</para>
/// <para>private GameObject prefab</para>
/// <para>private Queue&lt;GameObject&gt; poolQueue</para>
/// <para>private HashSet&lt;GameObject&gt; inPoolSet</para>
/// <para>private int maxSize, currentSize</para>
///
/// <para>메서드</para>
/// <para>public void Initialize(GameObject prefab, int size, int maxSize = 100)</para>
/// <para>public GameObject GetFromPool()</para>
/// <para>public void ReturnToPool(GameObject obj)</para>
///
/// </summary>
public class GameObjectPooling : MonoBehaviour,IPooling
{
    private GameObject prefab;
    private Queue<GameObject> poolQueue;
    private HashSet<GameObject> inPoolSet = new HashSet<GameObject>();
    private int maxSize;
    private int currentSize;    
    public void Initialize(GameObject prefab, int size, int maxSize = 100)
    {
        this.prefab = prefab;
        poolQueue = new Queue<GameObject>(size);
        this.maxSize = maxSize; 
        this.currentSize = size;
        //inPoolSet = new HashSet<GameObject> ();
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            poolQueue.Enqueue(obj);
            //inPoolSet.Add(obj);
            currentSize++;
        }
    }

    public GameObject GetFromPool()
    {
        //GameObject obj = poolQueue.Count > 0 ? poolQueue.Dequeue() : Instantiate(prefab, transform);
        GameObject obj;
        if (poolQueue.Count > 0)
        {
            obj = poolQueue.Dequeue();
            inPoolSet.Remove(obj);
        }
        else if(currentSize < maxSize)
        {
            obj = Instantiate(prefab, transform);
            currentSize++;
            inPoolSet.Add(obj);
        }
        else
        {
            //obj = Instantiate(prefab, transform);
            //inPoolSet.Add(obj);
            Debug.LogWarning($"[GameObjectPooling] Pool has reached max size ({maxSize}). Returning null.");
            return null;
        }
        obj.SetActive(true);
        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        if (inPoolSet.Contains(obj))
        {
            //Debug.LogWarning("[GameObjectPooling] Object is already in the pool.");
            return;
        }
        obj.SetActive(false);
        obj.transform.SetParent(this.transform);
        poolQueue.Enqueue(obj);
        inPoolSet.Add(obj);
    }
}
