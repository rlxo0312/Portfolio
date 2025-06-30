using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 몬스터가 사망한 후 일정 시간(delay) 뒤에 오브젝트 풀에서 다시 소환(부활)하도록 처리하는 클래스
/// 오브젝트 풀링 기반의 몬스터 재활용 로직을 중심으로 작동하며, 싱글톤 패턴으로 구현되어 있음
///
/// <para>사용 변수</para>
/// <para>public static MonsterReviveManager instance</para>
///
/// <para>메서드</para>
/// <para>public void ScheduleRevie(string key, Vector3 revivePosition, float delay)</para>
/// <para>private IEnumerator ReviveCouroutine(string key, Vector3 revivePosition, float delay)</para>
/// </summary>
public class MonsterReviveManager : MonoBehaviour
{
    public static MonsterReviveManager instance;    
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    /// <summary>
    /// 지정된 키와 위치, 지연 시간(delay)에 따라 몬스터 부활 스케줄링
    /// </summary>
    /// <param name="key">오브젝트 풀 키</param>
    /// <param name="revivePosition">부활 위치</param>
    /// <param name="delay">부활까지 대기 시간</param>
    public void ScheduleRevie(string key,Vector3 revivePosition, float delay)
    {
        StartCoroutine(ReviveCouroutine(key, revivePosition, delay));
    }
    /// <summary>
    /// 실제 부활 처리를 코루틴으로 실행. 일정 시간 후 오브젝트 풀에서 가져와 위치 재배치 및 상태 초기화
    /// </summary>
    /// <param name="key">오브젝트 풀 키</param>
    /// <param name="revivePosition">부활 위치</param>
    /// <param name="delay">대기 시간</param>
    /// <returns>IEnumerator</returns>
    private IEnumerator ReviveCouroutine(string key,Vector3 revivePosition, float delay)
    {
        //yield return new WaitForSeconds(delay);
        yield return WaitForSecondsCache.Get(delay);

        //enemy.gameObject.SetActive(true);
        //yield return null;
        //yield return null;
        /*GameObject newEnemyObj = ObjectPooling.instance.GetFromPool(key, revivePosition);
        newEnemyObj.transform.position = revivePosition;
        Enemy enemy = newEnemyObj.GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError($"[ReviveManager] Enemy 컴포넌트가 {key} 오브젝트에 없습니다.");
            yield break;
        }
        if (enemy.animator != null && !enemy.animator.isActiveAndEnabled)
        {
            enemy.animator.enabled = true;
        }

        if (enemy.navMeshAgent != null && !enemy.navMeshAgent.isActiveAndEnabled)
        {
            enemy.navMeshAgent.enabled = true;
        }*/
        var pool = ObjectPoolingManager.Instance.GetPool(key);
        if (pool == null)
        {
            Debug.LogError($"[ReviveManager] '{key}' 풀을 찾을 수 없습니다.");
            yield break;
        }

        GameObject newEnemyObj = pool.GetFromPool();
        //Enemy enemyObj = newEnemyObj.GetComponent<Enemy>();
        EnemyReference enemyObj = newEnemyObj.GetComponent<EnemyReference>();
        if (newEnemyObj == null)
        {
            Debug.LogError($"[ReviveManager] '{key}' 풀에서 오브젝트를 가져올 수 없습니다.");
            yield break;
        }

        newEnemyObj.transform.position = revivePosition;
        //yield return new WaitForEndOfFrame();
        enemyObj.Enemy.Revive();
        enemyObj.Enemy.InitializeRevivedState();        
        enemyObj.Enemy.ReviveDefaultState();
    }
}
