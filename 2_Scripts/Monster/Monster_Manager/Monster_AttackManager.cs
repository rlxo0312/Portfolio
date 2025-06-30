using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터의 공격 처리를 담당하는 클래스.
/// 몬스터가 공격 타이밍에 맞춰 플레이어에게 데미지를 주도록 구현됨.
///
/// <para>사용 변수</para>
/// <para>public ManualCollider menualCollider</para>
/// <para>private EnemyReference enemy1</para>
///
/// <para>메서드</para>
/// <para>void Awake() - 컴포넌트 초기화</para>
/// <para>void AttackTrigger() - 공격 판정 및 데미지 처리</para>
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
    /// 공격 실행 시 호출되며, 감지된 플레이어에게 데미지를 적용
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
                Debug.Log($"[Monster_AttackManager] 플레이어 사망");
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
                //Debug.Log($"[AttackTrigger] 타격된 적: {enemy1.name}, 콜라이더: {hit.name}");
                playerRef.PlayerManager.BeDamaged(damaged, playerRef.transform.position);
                Debug.Log($"[Monster_AttackManager] {enemy1.name} → {playerRef.name} 데미지: {damaged}");
                /*if (DamageTextSpawner.Instance != null)
                {
                    DamageTextSpawner.Instance.ShowDamage(player.transform.position, damaged);
                    Debug.Log($"[MonsterAttackManager] 텍스트 값: {player.transform.position}");
                }*/
            }
            /*if (enemy != null && !hitEnemy.Contains(enemy))
            {
                hitEnemy.Add(enemy); // 중복 추가 방지

                float attackPower = playerManager.AttackPower;
                float finalDamaged = attackPower * (100 - enemy.Defense) / 100;

                enemy.BeDamaged(finalDamaged, playerManager.transform.position);
                if (DamageTextSpawner.Instance != null)
                {
                    DamageTextSpawner.Instance.ShowDamage(enemy.transform.position, finalDamaged);
                    Debug.Log($"[PlayerAttackManager] 텍스트 값: {enemy.transform.position}");
                }
            }*/
        }
    }

   
}
