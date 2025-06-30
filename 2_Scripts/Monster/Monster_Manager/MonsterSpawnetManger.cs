using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������ �����ϴ� Ŭ����
/// ������ ���� �����հ� ��Ȱ ��ġ�� ������� ������Ʈ Ǯ���� �����ϰ� �ʱ�ȭ��
///
/// <para>��� ����</para>
/// <para>private List&lt;MonsterSpawnData&gt; spwanList</para>
///
/// <para>�޼���</para>
/// <para>public void SpawnAllMonsters()</para>
/// </summary>
public class MonsterSpawnetManger : MonoBehaviour
{
    [System.Serializable]
    public class MonsterSpawnData 
    {
        public Enemy monsterPrefab;
        public Transform revivePoint;
    }
    [SerializeField]private List<MonsterSpawnData> spwanList;
    
    

    void Start()
    {
        SpawnAllMonsters();
    }
    public void SpawnAllMonsters()
    {
        foreach (var spawnData in spwanList) 
        {
            //Debug.Log($"[SpawnTest] Warrok Prefab Valid? {spawnData.monsterPrefab != null}, Key: {spawnData.monsterPrefab?.PoolKey}, DataKey: {((Enemy_Boss_Warrok)spawnData.monsterPrefab)?.warrokData?.poolKey}");
            //Debug.Log($"[SpawnTest] Prefab: {spawnData.monsterPrefab?.name}, Valid? {spawnData.monsterPrefab != null}, PoolKey: {spawnData.monsterPrefab?.PoolKey}");
            //Debug.Log($"[SpawnTest] Warrok Prefab Valid? {spwanList[0].monsterPrefab != null}, Key: {spwanList[0].monsterPrefab?.PoolKey}");
            if (spawnData.monsterPrefab == null || spawnData.revivePoint == null)
            {
                Debug.Log("[MonsterSpawnManager] monsterPrefab�� revivePoint�� ��ȿ���� �ʽ��ϴ�.");
                continue;
            }

            string poolKey = spawnData.monsterPrefab.PoolKey;

            GameObject mosterObj = ObjectPoolingManager.Instance.SpawnFromPool(poolKey, spawnData.revivePoint.position, Quaternion.identity);

            //Enemy enemyRef = mosterObj.GetComponent<Enemy>();
            EnemyReference enemyRef = mosterObj.GetComponent<EnemyReference>();
            if (enemyRef != null)
            {
                //enemy.AssignWayPoints();
                enemyRef.Enemy.revivePoint = spawnData.revivePoint;
                enemyRef.Enemy.Revive();
            }
            else
            {
                Debug.LogWarning($"[MonsterSpawnManager]������ ������Ʈ�� Enemy ������Ʈ�� �����ϴ�. PoolKey: {poolKey}");
            }
        }
    }
}
