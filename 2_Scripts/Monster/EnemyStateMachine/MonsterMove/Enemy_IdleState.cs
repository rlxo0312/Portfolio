using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���Ͱ� ���(Idle) ������ ���� ������ �����ϴ� Ŭ����
/// ���� �ð� ���� ����ϰų� �÷��̾ �߰��ϸ� ���¸� ��ȯ��
/// </summary>
public class Enemy_IdleState : Enemy_Monster_State
{
    /// <summary>
    /// Idle ���� ������: �ִϸ��̼� �̸��� ���� �ӽ� ����
    /// </summary>
    /// <param name="_enemy">�� ��ü</param>
    /// <param name="_monster_StateMachine">���� �ӽ�</param>
    /// <param name="_animationName">�ִϸ��̼� �̸�</param>
    public Enemy_IdleState(Enemy _enemy, Enemy_Monster_StateMachine _monster_StateMachine, string _animationName) : base(_enemy, _monster_StateMachine, _animationName)
    {
        
    }
    // ���� ���� �� Idle Ÿ�̸Ӹ� �ʱ�ȭ��
    public override void Enter()
    {
        base.Enter();
        
        stateTimer = enemy.idleTime;
    }
    // ���� ���� �� ȣ��Ǹ�, ����� Ư���� ó���� ���� ����
    public override void Exit() 
    {
        base.Exit();
    }
    // ���� ���� �� Ÿ�� Ž�� �� ���� ��ȯ ������ ������
    public override void Update()
    {
        base.Update();

        
        Transform target = enemy.SearchTarget();
        if (target != null)
        {
           
            if (enemy.IsAvailableAttack)
            {
                enemy.ChangeState(EnemyStateType.Attack);
                return; //return�� ����ϴ� ���� : �ϳ��� ������ �����ϸ� ��� ����Ǳ� ���� 
            }
           
            enemy.ChangeState(EnemyStateType.Move);
            return;
            
        }
        else
        {            
            if (stateTimer <= 0)
            {
                enemy.ChangeState(EnemyStateType.Patrol);
            }
        }

    }
}
