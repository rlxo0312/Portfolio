using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 다양한 타입의 오브젝트 풀을 등록하고 관리하는 싱글톤 매니저 클래스
/// GameObject 기반 또는 Component 기반 풀을 생성하며, 키를 통해 풀에서 객체를 가져오고 반환할 수 있음
///
/// <para>사용 변수</para>
/// <para>public List&lt;PoolEntry&gt; intilizePool</para>
/// <para>private Dictionary&lt;string, IPooling&gt; pools</para>
///
/// <para>메서드</para>
/// <para>public void CreateGameObjectPooling(string key, GameObject prefab, int size = 10)</para>
/// <para>public void CreateComponentPool&lt;T&gt;(string key, T prefab, int size = 10) where T : Component</para>
/// <para>public IPooling GetPool(string key)</para>
/// <para>public void RegisterPrefab(string key, GameObject prefab)</para>
/// <para>public GameObject GetFromPool(string key, Vector3 position)</para>
/// <para>public GameObject SpawnFromPool(string key, Vector3 pos, Quaternion rotation)</para>
/// <para>public void ReturnToPool(string key, GameObject obj)</para>
/// </summary>
public class ObjectPoolingManager : SingletonMonoBehaviour<ObjectPoolingManager>
{
    public List<PoolEntry> intilizePool;
    private Dictionary<string, IPooling> pools = new Dictionary<string, IPooling>();

    protected override void Awake()
    {
        base.Awake();
        foreach (var entry in intilizePool)
        {
            if(entry.prefab != null && entry.preload)
            {
                CreateGameObjectPooling(entry.key, entry.prefab, entry.preloadCount);
            }
            else
            {
                Debug.LogWarning($"[ObjectPoolingManager] '{entry.key}' 프리팹이 비어있습니다.");
            }
        }
    }
    /// <summary>
    /// GameObject 기반 풀을 생성하고 Dictionary에 등록
    /// </summary>
    public void CreateGameObjectPooling(string key, GameObject prefab, int size = 10)
    {
        if (string.IsNullOrWhiteSpace(key) || prefab == null)
        {
            Debug.LogError("[ObjectPoolingManager] 잘못된 키 또는 프리팹입니다.");
            return;
        }
        if (pools.ContainsKey(key))
        {
            return;
        }

        var poolObj = new GameObject($"{key}_GameObjectPool");
        poolObj.transform.parent = transform;

        var pool = poolObj.AddComponent<GameObjectPooling>();
        pool.Initialize(prefab, size);
        pools[key] = pool;
    }
    /// <summary>
    /// Component 기반 풀을 생성하고 Dictionary에 등록
    /// </summary>
    public void CreateComponentPool<T>(string key, T prefab, int size = 10) where T : Component
    {

        if (pools.ContainsKey(key))
        {
            return;
        }

        var poolObj = new GameObject($"{key}_ComponentPool");
        poolObj.transform.parent = transform;

        var pool = poolObj.AddComponent<GenericObjectPooling<T>>();
        pool.Initialize(prefab, size);
        pools[key] = pool;
    }
    /// <summary>
    /// 키를 통해 풀을 가져옴
    /// </summary>
    public IPooling GetPool(string key)
    {
        if (pools.TryGetValue(key, out var pool))
        {
            return pool;
        }

        Debug.LogError($"Pool with key '{key}' not found.");
        return null;
    }
    /// <summary>
    /// 지정된 키로 프리팹 등록 및 풀 생성
    /// </summary>
    public void RegisterPrefab(string key, GameObject prefab)
    {
        if (!pools.ContainsKey(key))
        {
            CreateGameObjectPooling(key, prefab);
        }
    }
    /// <summary>
    /// 위치 지정으로 오브젝트를 가져와 반환
    /// </summary>
    public GameObject GetFromPool(string key, Vector3 position)
    {
        if (!pools.ContainsKey(key))
        {
            throw new System.Exception($"[ObjectPoolingManager] No pool registered with key '{key}'");
        }

        var pool = GetPool(key) as GameObjectPooling;
        if (pool == null)
        {
            Debug.LogError($"[ObjectPoolingManager] Pool not found or invalid for key: {key}");
            return null;
        }

        GameObject obj = pool.GetFromPool();
        obj.transform.position = position;
        obj.transform.rotation = Quaternion.identity;
        return obj;
    }
    /// <summary>
     /// 오브젝트를 해당 키에 맞는 풀로 반환
     /// </summary>
    public void ReturnToPool(string key, GameObject obj)
    {
        if (pools.TryGetValue(key, out var pool))
        {
            pool.ReturnToPool(obj);
        }
        else
        {
            Debug.LogWarning($"[ObjectPoolingManager] No pool found with key: {key}");
        }
    }
    /// <summary>
    /// 오브젝트를 풀에서 스폰하며 위치와 회전 적용
    /// </summary>
    public GameObject SpawnFromPool(string key, Vector3 pos, Quaternion rotation)
    {
        if(!pools.ContainsKey(key))
    {
            Debug.LogWarning($"[ObjectPoolingManager] Pool with key '{key}' doesn't exist.");
            return null;
        }

        var pool = GetPool(key) as GameObjectPooling;
        if (pool == null)
        {
            Debug.LogError($"[ObjectPoolingManager] Pool is not a GameObjectPooling for key: {key}");
            return null;
        }

        GameObject obj = pool.GetFromPool();
        obj.transform.position = pos;
        obj.transform.rotation = rotation;  
        obj.SetActive(true);

        return obj;
    }
}
