using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���Ͱ� ����� �� ���� �ð�(delay) �ڿ� ������Ʈ Ǯ���� �ٽ� ��ȯ(��Ȱ)�ϵ��� ó���ϴ� Ŭ����
/// ������Ʈ Ǯ�� ����� ���� ��Ȱ�� ������ �߽����� �۵��ϸ�, �̱��� �������� �����Ǿ� ����
///
/// <para>��� ����</para>
/// <para>public static MonsterReviveManager instance</para>
///
/// <para>�޼���</para>
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
    /// ������ Ű�� ��ġ, ���� �ð�(delay)�� ���� ���� ��Ȱ �����ٸ�
    /// </summary>
    /// <param name="key">������Ʈ Ǯ Ű</param>
    /// <param name="revivePosition">��Ȱ ��ġ</param>
    /// <param name="delay">��Ȱ���� ��� �ð�</param>
    public void ScheduleRevie(string key,Vector3 revivePosition, float delay)
    {
        StartCoroutine(ReviveCouroutine(key, revivePosition, delay));
    }
    /// <summary>
    /// ���� ��Ȱ ó���� �ڷ�ƾ���� ����. ���� �ð� �� ������Ʈ Ǯ���� ������ ��ġ ���ġ �� ���� �ʱ�ȭ
    /// </summary>
    /// <param name="key">������Ʈ Ǯ Ű</param>
    /// <param name="revivePosition">��Ȱ ��ġ</param>
    /// <param name="delay">��� �ð�</param>
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
            Debug.LogError($"[ReviveManager] Enemy ������Ʈ�� {key} ������Ʈ�� �����ϴ�.");
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
            Debug.LogError($"[ReviveManager] '{key}' Ǯ�� ã�� �� �����ϴ�.");
            yield break;
        }

        GameObject newEnemyObj = pool.GetFromPool();
        //Enemy enemyObj = newEnemyObj.GetComponent<Enemy>();
        EnemyReference enemyObj = newEnemyObj.GetComponent<EnemyReference>();
        if (newEnemyObj == null)
        {
            Debug.LogError($"[ReviveManager] '{key}' Ǯ���� ������Ʈ�� ������ �� �����ϴ�.");
            yield break;
        }

        newEnemyObj.transform.position = revivePosition;
        //yield return new WaitForEndOfFrame();
        enemyObj.Enemy.Revive();
        enemyObj.Enemy.InitializeRevivedState();        
        enemyObj.Enemy.ReviveDefaultState();
    }
}
