using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// �پ��� Ÿ���� ������Ʈ Ǯ�� ����ϰ� �����ϴ� �̱��� �Ŵ��� Ŭ����
/// GameObject ��� �Ǵ� Component ��� Ǯ�� �����ϸ�, Ű�� ���� Ǯ���� ��ü�� �������� ��ȯ�� �� ����
///
/// <para>��� ����</para>
/// <para>public List&lt;PoolEntry&gt; intilizePool</para>
/// <para>private Dictionary&lt;string, IPooling&gt; pools</para>
///
/// <para>�޼���</para>
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
                Debug.LogWarning($"[ObjectPoolingManager] '{entry.key}' �������� ����ֽ��ϴ�.");
            }
        }
    }
    /// <summary>
    /// GameObject ��� Ǯ�� �����ϰ� Dictionary�� ���
    /// </summary>
    public void CreateGameObjectPooling(string key, GameObject prefab, int size = 10)
    {
        if (string.IsNullOrWhiteSpace(key) || prefab == null)
        {
            Debug.LogError("[ObjectPoolingManager] �߸��� Ű �Ǵ� �������Դϴ�.");
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
    /// Component ��� Ǯ�� �����ϰ� Dictionary�� ���
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
    /// Ű�� ���� Ǯ�� ������
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
    /// ������ Ű�� ������ ��� �� Ǯ ����
    /// </summary>
    public void RegisterPrefab(string key, GameObject prefab)
    {
        if (!pools.ContainsKey(key))
        {
            CreateGameObjectPooling(key, prefab);
        }
    }
    /// <summary>
    /// ��ġ �������� ������Ʈ�� ������ ��ȯ
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
     /// ������Ʈ�� �ش� Ű�� �´� Ǯ�� ��ȯ
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
    /// ������Ʈ�� Ǯ���� �����ϸ� ��ġ�� ȸ�� ����
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
