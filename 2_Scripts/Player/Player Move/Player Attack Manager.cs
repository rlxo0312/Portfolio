using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
/// <summary>
/// 플레이어의 공격 처리를 담당하는 클래스
/// <para>사용 변수</para>
/// <para>public PlayerManagerReference playerManagerRef</para>
/// <para>private HashSet&lt;Enemy&gt; damagedEnemy</para>
/// 
/// <para>사용 매서드</para>
/// <para>public void AttackTrigger()</para>
/// </summary>
public class PlayerAttackManager : MonoBehaviour,IAttackable
{
    //public PlayerManager playerManager;
    public PlayerManagerReference playerManagerRef;

    private HashSet<Enemy> damagedEnemy;
    private void Awake()
    {
        //playerManagerRef = GetComponent<PlayerManager>();            
        
        /*if(playerManagerRef.PlayerManager.monsterTargetCount > 0)
        {
            damagedEnemy = new HashSet<Enemy>(playerManagerRef.PlayerManager.monsterTargetCount);            
        }
        else
        {
            damagedEnemy = new HashSet<Enemy>();
        }*/
    }
    private void Start()
    {
        playerManagerRef = GetComponent<PlayerManagerReference>();
        if (playerManagerRef.PlayerManager.monsterTargetCount > 0)
        {
            damagedEnemy = new HashSet<Enemy>(playerManagerRef.PlayerManager.monsterTargetCount);
        }
        else
        {
            damagedEnemy = new HashSet<Enemy>();
        }
    }
    /// <summary>
    /// 공격 트리거 실행 시 호출되어, 범위 내 적 탐지 및 피해 처리
    /// </summary>
    public void AttackTrigger()
    {
        /*if (playerManager == null)
        {
            return;
        }*/
        //Debug.Log("[Player_AttackManager] AttackTrigger Called");
        //Debug.Log($"[PlayerAnimationManager] AttackTrigger Called at {Time.time}"); 
        damagedEnemy.Clear();
        Collider[] colliders = playerManagerRef.PlayerManager.manualCollider.GetColliderObject();
        //HashSet<Enemy> damagedEnemy = new HashSet<Enemy>();
        foreach (var hit in colliders)
        {
            Debug.Log($"[AttackTrigger] 감지된 오브젝트: {hit.name}");
            //Enemy enemyRef = hit.GetComponentInParent<Enemy>();
            EnemyReference enemyRef = hit.GetComponentInParent<EnemyReference>();
            //Enemy enemy = hit.GetComponentInParent<Enemy>() ?? hit.GetComponentInChildren<Enemy>();
            //PlayerManager playerManager = hit.GetComponentInParent<PlayerManager>();
            if (enemyRef == null || enemyRef.Enemy == null)
            {
                Debug.LogWarning($"[AttackTrigger] Enemy 컴포넌트 찾을 수 없음: {hit.name}");
                continue;
            }

            if(damagedEnemy.Contains(enemyRef.Enemy))
            {
                continue;
            }

            if (enemyRef.Enemy.IsInvincibility) //enemy.IsInvincibility || !enemy.IsAlive
            {
                if(DamageTextSpawner.Instance != null)
                {
                    DamageTextSpawner.Instance.ShowDamage(transform.position, 0f, false, true);
                }
                Debug.Log($"[PlayerAttackManager] {enemyRef.name}은 무적 상태이므로 데미지 무시됨");
                continue;
            }

            if (!enemyRef.Enemy.IsAlive)
            {
                Debug.Log($"[PlayerAttackManager] {enemyRef.name}은 사망 상태이므로 데미지 무시됨");
                continue;
            }
            damagedEnemy.Add(enemyRef.Enemy);
            //Debug.Log($"[AttackTrigger] Player: {playerManager}, Enemy: {enemy}");           
            Debug.Log($"[AttackTrigger] 타격 대상 Enemy: {enemyRef.name}");
            float attackPower = playerManagerRef.PlayerManager.AttackPower;
            float finalDamaged = attackPower * (100 - enemyRef.Enemy.Defense) / 100;

            enemyRef.Enemy.BeDamaged(finalDamaged, playerManagerRef.transform.position);
            if (DamageTextSpawner.Instance != null)
            {
                DamageTextSpawner.Instance.ShowDamage(enemyRef.transform.position, finalDamaged);
                //Debug.Log($"[PlayerAttackManager] 텍스트 값: {enemy.transform.position}");
            }        
        }
    }
}
