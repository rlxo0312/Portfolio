using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터 스폰을 관리하는 클래스
/// 설정된 몬스터 프리팹과 부활 위치를 기반으로 오브젝트 풀에서 스폰하고 초기화함
///
/// <para>사용 변수</para>
/// <para>private List&lt;MonsterSpawnData&gt; spwanList</para>
///
/// <para>메서드</para>
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
                Debug.Log("[MonsterSpawnManager] monsterPrefab과 revivePoint가 유효하지 않습니다.");
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
                Debug.LogWarning($"[MonsterSpawnManager]스폰된 오브젝트에 Enemy 컴포넌트가 없습니다. PoolKey: {poolKey}");
            }
        }
    }
}
