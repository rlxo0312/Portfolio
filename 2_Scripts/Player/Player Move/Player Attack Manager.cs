using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
/// <summary>
/// �÷��̾��� ���� ó���� ����ϴ� Ŭ����
/// <para>��� ����</para>
/// <para>public PlayerManagerReference playerManagerRef</para>
/// <para>private HashSet&lt;Enemy&gt; damagedEnemy</para>
/// 
/// <para>��� �ż���</para>
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
    /// ���� Ʈ���� ���� �� ȣ��Ǿ�, ���� �� �� Ž�� �� ���� ó��
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
            Debug.Log($"[AttackTrigger] ������ ������Ʈ: {hit.name}");
            //Enemy enemyRef = hit.GetComponentInParent<Enemy>();
            EnemyReference enemyRef = hit.GetComponentInParent<EnemyReference>();
            //Enemy enemy = hit.GetComponentInParent<Enemy>() ?? hit.GetComponentInChildren<Enemy>();
            //PlayerManager playerManager = hit.GetComponentInParent<PlayerManager>();
            if (enemyRef == null || enemyRef.Enemy == null)
            {
                Debug.LogWarning($"[AttackTrigger] Enemy ������Ʈ ã�� �� ����: {hit.name}");
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
                Debug.Log($"[PlayerAttackManager] {enemyRef.name}�� ���� �����̹Ƿ� ������ ���õ�");
                continue;
            }

            if (!enemyRef.Enemy.IsAlive)
            {
                Debug.Log($"[PlayerAttackManager] {enemyRef.name}�� ��� �����̹Ƿ� ������ ���õ�");
                continue;
            }
            damagedEnemy.Add(enemyRef.Enemy);
            //Debug.Log($"[AttackTrigger] Player: {playerManager}, Enemy: {enemy}");           
            Debug.Log($"[AttackTrigger] Ÿ�� ��� Enemy: {enemyRef.name}");
            float attackPower = playerManagerRef.PlayerManager.AttackPower;
            float finalDamaged = attackPower * (100 - enemyRef.Enemy.Defense) / 100;

            enemyRef.Enemy.BeDamaged(finalDamaged, playerManagerRef.transform.position);
            if (DamageTextSpawner.Instance != null)
            {
                DamageTextSpawner.Instance.ShowDamage(enemyRef.transform.position, finalDamaged);
                //Debug.Log($"[PlayerAttackManager] �ؽ�Ʈ ��: {enemy.transform.position}");
            }        
        }
    }
}
