using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ���� ó���� ����ϴ� Ŭ����.
/// ���Ͱ� ���� Ÿ�ֿ̹� ���� �÷��̾�� �������� �ֵ��� ������.
///
/// <para>��� ����</para>
/// <para>public ManualCollider menualCollider</para>
/// <para>private EnemyReference enemy1</para>
///
/// <para>�޼���</para>
/// <para>void Awake() - ������Ʈ �ʱ�ȭ</para>
/// <para>void AttackTrigger() - ���� ���� �� ������ ó��</para>
/// </summary>
public class Monster_AttackManager : MonoBehaviour, IAttackable
{
    public ManualCollider menualCollider;
    //private Enemy enemy1;
    private EnemyReference enemy1;
    //private PlayerManager playerManager;
    void Awake()
    {
        menualCollider = GetComponentInChildren<ManualCollider>();
        //enemy1 = GetComponentInParent<Enemy>();
        //HashSet<Enemy> hitEnemy = new HashSet<Enemy>();
        //playerManager = GetComponent<PlayerManager>();
    }
    private void Start()
    {
        enemy1 = GetComponentInParent<EnemyReference>();
    }
    /// <summary>
    /// ���� ���� �� ȣ��Ǹ�, ������ �÷��̾�� �������� ����
    /// </summary>
    public void AttackTrigger()
    {        
        //Debug.Log("[Monster_AttackManager] AttackTrigger Called");
        // Debug.Log($"[Monster_AttackManager] AttackTrigger Called at {Time.time}"); 
        //Collider[] colliders = menualCollider.GetColliderObject();
        Collider[] colliders = menualCollider.GetColliderObject();
        //HashSet<PlayerManager> hitPlayer = new HashSet<PlayerManager>();
        HashSet<PlayerManagerReference> hitPlayer = new HashSet<PlayerManagerReference>();
        foreach (var hit in colliders)
        {
            //Enemy enemy = hit.GetComponentInParent<Enemy>();
            PlayerManagerReference playerRef = hit.GetComponentInParent<PlayerManagerReference>();
            if (!playerRef.PlayerManager.IsAlive)
            {
                Debug.Log($"[Monster_AttackManager] �÷��̾� ���");
                return;
            }
            //Enemy enemy = hit.GetComponentInParent<Enemy>();
            /*if(hit.GetComponentInParent<PlayerManager>() != null && hit.GetComponentInParent<Enemy>() !=null)*/
            //Debug.Log($"[AttackTrigger] Hit collider: {hit.name}, Layer: {hit.gameObject.layer}, Tag: {hit.tag}");
            /*Transform current = hit.transform;
            while (current != null)
            {
                //Debug.Log($"[AttackTrigger] -> Parent: {current.name}, has PlayerManager: {current.GetComponent<PlayerManager>() != null}");
                current = current.parent;
            }*/
            //processedPlayers.Add(player);
            if (playerRef != null && !hitPlayer.Contains(playerRef))//enemy1 != null
            {
                hitPlayer.Add(playerRef);
                //Debug.Log($"[Monster_AttackManager] Attack Trigger_ enemy:{enemy}");
                //Debug.Log($"[Monster_AttackManager] Attack Trigger_ enemy.Defence1:{enemy.Defense}");
                float damaged = enemy1.Enemy.AttackPower * (100 - playerRef.PlayerManager.Defense) / 100;
                //Debug.Log($"[Monster_AttackManager] Player {player.name} is hit by {enemy1.name} for {damaged} damage.");
                //Debug.Log($"[AttackTrigger] Ÿ�ݵ� ��: {enemy1.name}, �ݶ��̴�: {hit.name}");
                playerRef.PlayerManager.BeDamaged(damaged, playerRef.transform.position);
                Debug.Log($"[Monster_AttackManager] {enemy1.name} �� {playerRef.name} ������: {damaged}");
                /*if (DamageTextSpawner.Instance != null)
                {
                    DamageTextSpawner.Instance.ShowDamage(player.transform.position, damaged);
                    Debug.Log($"[MonsterAttackManager] �ؽ�Ʈ ��: {player.transform.position}");
                }*/
            }
            /*if (enemy != null && !hitEnemy.Contains(enemy))
            {
                hitEnemy.Add(enemy); // �ߺ� �߰� ����

                float attackPower = playerManager.AttackPower;
                float finalDamaged = attackPower * (100 - enemy.Defense) / 100;

                enemy.BeDamaged(finalDamaged, playerManager.transform.position);
                if (DamageTextSpawner.Instance != null)
                {
                    DamageTextSpawner.Instance.ShowDamage(enemy.transform.position, finalDamaged);
                    Debug.Log($"[PlayerAttackManager] �ؽ�Ʈ ��: {enemy.transform.position}");
                }
            }*/
        }
    }

   
}
