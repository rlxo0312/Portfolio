using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ���Ͱ� ����(Patrol) ������ ���� ������ �����ϴ� Ŭ����
/// Ÿ�� Ž��, ��������Ʈ �̵� ���� ó����
/// </summary>
public class Enemy_Patrol : Enemy_Monster_State
{
    /// <summary>
    /// Patrol ���� ������: �ִϸ��̼� �̸��� ���� �ӽ� ����
    /// </summary>
    /// <param name="_enemy">�� ��ü</param>
    /// <param name="_monster_StateMachine">���� �ӽ�</param>
    /// <param name="_animationName">�ִϸ��̼� �̸�</param>
    public Enemy_Patrol(Enemy _enemy, Enemy_Monster_StateMachine _monster_StateMachine, string _animationName) : base(_enemy, _monster_StateMachine, _animationName)
    {
       
    }
    // ���� ���� �� ��������Ʈ�� �����ϰ� �������� �̵���
    public override void Enter()
    {
        base.Enter();      
       if(enemy.targetWayPoint == null)
        {
            enemy.FindnextWayPoint();
        }
       if(enemy.navMeshAgent != null && enemy.navMeshAgent.isActiveAndEnabled && enemy.navMeshAgent.isOnNavMesh)
        {
            enemy.navMeshAgent.SetDestination(enemy.targetWayPoint.position);
        }
        else
        {
            Debug.Log("[Enemy_Patrol] NavMeshAgent not ready for SetDestination.");
        }
        if (enemy.targetWayPoint)
        {
            enemy.navMeshAgent.SetDestination(enemy.targetWayPoint.position);
        }
    }
    public override void Exit() 
    {
        base.Exit();
    }
    // ���� ���� �� Ÿ�� Ž��, �Ÿ� üũ �� ���� ��������Ʈ ���� ó��
    public override void Update()
    {
        base.Update();
        Transform target = enemy.SearchTarget();
        if (target)
        {
            if (enemy.IsAvailableAttack)
            {
                enemy.animator.CrossFade(enemy.AttackAniName, 0.2f);
                enemy.ChangeState(EnemyStateType.Attack);
            }
            else
            {
                enemy.ChangeState(EnemyStateType.Move);
            }
        }
        else
        {
            if (!enemy.CheckRemainDistance())
            {
                Transform nextDistance = enemy.FindnextWayPoint();
                if (nextDistance != null && enemy.navMeshAgent != null &&
                    enemy.navMeshAgent.isActiveAndEnabled && enemy.navMeshAgent.isOnNavMesh)
                {
                    enemy.navMeshAgent.SetDestination(nextDistance.position);
                }
                else
                {
                    Debug.LogWarning("[Enemy_Patrol] Can't set destination during patrol update.");
                }
            }
        }

    }
}
