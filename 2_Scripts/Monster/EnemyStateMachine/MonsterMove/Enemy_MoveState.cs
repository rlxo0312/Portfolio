using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���Ͱ� �̵�(Move) ������ ���� ������ �����ϴ� Ŭ����
/// Ÿ���� �����ϰų�, �Ÿ��� ���� Run �Ǵ� Attack ���·� ��ȯ��
/// </summary>
public class Enemy_MoveState : Enemy_Monster_State
{
    /// <summary>
    /// Move ���� ������: �ִϸ��̼� �̸��� ���� �ӽ� ����
    /// </summary>
    /// <param name="_enemy">�� ��ü</param>
    /// <param name="_monster_StateMachine">���� �ӽ�</param>
    /// <param name="_animationName">�ִϸ��̼� �̸�</param>
    public Enemy_MoveState(Enemy _enemy, Enemy_Monster_StateMachine _monster_StateMachine, string _animationName) : base(_enemy, _monster_StateMachine, _animationName)
    {
        
    }
    // ���� ���� �� �̵� Ÿ�̸Ӹ� �ʱ�ȭ��
    public override void Enter()
    {
        base.Enter();        
        stateTimer = enemy.moveTime;
    }
    // ���� ���� �� ��θ� �ʱ�ȭ��
    public override void Exit() 
    {
        base.Exit();          
        enemy.navMeshAgent.ResetPath();
        
    }
    // ���� ���� �� Ÿ�� ���� �� ���� ��ȯ ������ ������
    public override void Update()
    {
        base.Update();        
        if(enemy.target == null)
        {
            enemy.ChangeState(EnemyStateType.Patrol);
            return;
        }

        if (enemy.navMeshAgent != null && enemy.navMeshAgent.isActiveAndEnabled && enemy.navMeshAgent.isOnNavMesh)
        {
            enemy.navMeshAgent.SetDestination(enemy.target.position);
        }

        if (enemy.IsAvailableAttack)
        {
            enemy.ChangeState(EnemyStateType.Attack);
            return;
        }

        float dist = (enemy.target.position - enemy.transform.position).sqrMagnitude;
        float hold = enemy.AttackRange + enemy.increasedRunRange;
        var runState = enemy.GetState(EnemyStateType.Run); 

        if(runState != null)
        {
            if(dist > hold * hold)
            {
                enemy.ChangeState(EnemyStateType.Run);
            }
        }
        else
        {
            enemy.ChangeState(EnemyStateType.Move);
        }
    }
}
