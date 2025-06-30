using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���Ͱ� �޸���(Run) ������ ���� ������ �����ϴ� Ŭ����
/// Ư�� �Ÿ� �̻��� �� Move ���� ��� Run ���·� ��ȯ�Ǿ� ������ ������
/// </summary>
public class Enemy_RunState : Enemy_Monster_State
{
    /// <summary>
    /// Run ���� ������: �ִϸ��̼� �̸��� ���� �ӽ� ����
    /// </summary>
    /// <param name="_enemy">�� ��ü</param>
    /// <param name="_monster_StateMachine">���� �ӽ�</param>
    /// <param name="_animationName">�ִϸ��̼� �̸�</param>
    public Enemy_RunState(Enemy _enemy, Enemy_Monster_StateMachine _monster_StateMachine, string _animationName) : base(_enemy, _monster_StateMachine, _animationName)
    {

    }
    // ���� ���� �� �̵� �ӵ� ���� �� �ִϸ��̼� ���
    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.RunDuration;       

        if (enemy.navMeshAgent != null && enemy.navMeshAgent.isOnNavMesh)
        {
            enemy.navMeshAgent.speed = enemy.chaseSpeed;
            enemy.navMeshAgent.isStopped = false;
        }
        enemy.animator?.CrossFade(enemy.RunAniName, 0.2f);
    }
    // ���� ���� �� ���� �� ��� �ʱ�ȭ
    public override void Exit() 
    {
        base.Exit();

        if(enemy.navMeshAgent != null &&enemy.navMeshAgent.isActiveAndEnabled &&
                 enemy.navMeshAgent.isOnNavMesh)
        {
            enemy.navMeshAgent.isStopped = true;
            enemy.navMeshAgent.ResetPath();
        }
    }
    // Ÿ�� ���� �� ���� �Ÿ� �̳� ���� �� Move �Ǵ� Attack ���·� ��ȯ
    public override void Update() 
    {
        base.Update();
        if (enemy.target == null)
        {
            enemy.ChangeState(EnemyStateType.Patrol);
            return;
        }
       /* Transform target = enemy.SearchTarget();
        if (target != null && enemy.navMeshAgent.isActiveAndEnabled && enemy.navMeshAgent.isOnNavMesh)        
        {
            enemy.navMeshAgent.SetDestination(target.position);
        }

        if (enemy.IsAvailableAttack)
        {
            enemy.ChangeState(EnemyStateType.Attack);
            return;
        }

        if (stateTimer <= 0f)
        {
            enemy.ChangeState(EnemyStateType.Move);
        }*/ 

        if(enemy.navMeshAgent != null && enemy.navMeshAgent.isActiveAndEnabled && enemy.navMeshAgent.isOnNavMesh)
        {
            enemy.navMeshAgent.SetDestination(enemy.target.position);
        }

        float dist = (enemy.target.position - enemy.transform.position).sqrMagnitude;
        float hold = enemy.AttackRange + enemy.increasedRunRange; 

        if(dist < hold * hold)
        {
            enemy.ChangeState(EnemyStateType.Move);
        }

        if (enemy.IsAvailableAttack)
        {
            enemy.ChangeState(EnemyStateType.Attack);
        }

        if(stateTimer <= 0f)
        {
            enemy.ChangeState(EnemyStateType.Idle);
        }
    }
}
