using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ���� ���¸� �����ϴ� Ŭ����
/// ���� ���� �� ���� ���� ���θ� Ȯ���ϰ�, ���ǿ� ���� Idle ���·� ��ȯ��
///</summary>
public class Enemy_AttackState : Enemy_Monster_State
{
    /// <summary>
    /// ������: Enemy, ���¸ӽ�, �ִϸ��̼� �̸��� �ʱ�ȭ
    /// </summary>
    /// <param name="_enemy">�� ��ü</param>
    /// <param name="_monster_StateMachine">���¸ӽ�</param>
    /// <param name="_animationName">�ִϸ��̼� �̸�</param>
    public Enemy_AttackState(Enemy _enemy, Enemy_Monster_StateMachine _monster_StateMachine, string _animationName) : base(_enemy, _monster_StateMachine, _animationName)
    {
        
    }
    // ���� ���¿� ������ �� ȣ��Ǹ�, ���� ���� ���θ� �Ǵ��ϰ� ���� �α׸� ���
    public override void Enter()
    {
        base.Enter();
        //stateTimer = enemy_Monster_Zombie.moveTime;  
        //Debug.Log("���� ����");
        Debug.Log($"[Enemy_AttackState] {enemy.name} ���ݽ��� , {enemy.name}�� ���ݷ�: {enemy.AttackPower}");
        if (!enemy.IsAvailableAttack && enemy.target != null)
        {
            enemy.ChangeState(EnemyStateType.Idle);
            return;
        }
    }
    // ���� ���¿��� ���� �� ȣ��Ǵ� �޼���
    public override void Exit()
    {
        base.Exit();

    }
    // �� ������ ȣ��Ǹ�, ���� ������ ���
    public override void Update()
    {
        base.Update();
        
    }
}
