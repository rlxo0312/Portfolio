using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���� ������ ġ������ ����(����Ż ����) ���¸� �����ϴ� Ŭ����
/// ���� �ð� ���� �ִϸ��̼��� ����ϸ�, �ð��� ������ �̵� ���·� ��ȯ��
/// NavMeshAgent�� �Ͻ������� ���߰� �ٽ� Ȱ��ȭ�ϴ� ������� ������
///
/// <para>��� ����</para>
/// <para>private float FatalAttackDuration</para>
/// </summary>
public class Enemy_FatalSkill : Enemy_Monster_State
{
    private float FatalSkillDuration;

    /// <summary>
    /// ������: ġ���� ���� ���¸� �����ϸ�, ���� �ð��� �ִϸ��̼� �̸� ���� �ʱ�ȭ��
    /// </summary>
    /// <param name="_enemy">�� ��ü</param>
    /// <param name="_monster_StateMachine">���¸ӽ�</param>
    /// <param name="_animationName">����� �ִϸ��̼� �̸�</param>
    /// <param name="_fatalSkillDuration">����Ż ��ų ���� �ð�</param>
    public Enemy_FatalSkill(Enemy _enemy, Enemy_Monster_StateMachine _monster_StateMachine, string _animationName, float _fatalSkillDuration) 
        : base(_enemy, _monster_StateMachine, _animationName)
    {
        FatalSkillDuration = _fatalSkillDuration;
    }
    // ���¿� �������� �� ȣ��Ǹ�, NavMeshAgent�� ���߰� �ִϸ��̼��� �����
    public override void Enter()
    {
        base.Enter();
        enemy.navMeshAgent.isStopped = true;
        enemy.animator?.CrossFade(enemy.FatalSkillAniName, 0.2f);
        //stateTimer = enemy.MonsterSkillDuration;
        stateTimer = FatalSkillDuration;
    }
    // ���°� ����Ǵ� ���� �� ������ ȣ��Ǹ�, �ð��� ����Ǹ� �̵� ���·� ��ȯ��
    public override void Update()
    {
        base.Update();

        if(stateTimer <= 0)
        {
            enemy.ChangeState(EnemyStateType.Move);
        }
    }
    // ���¿��� �������� �� NavMeshAgent�� �ٽ� Ȱ��ȭ�ϰ� ��θ� �ʱ�ȭ��
    public override void Exit()
    {
        base.Exit();
        enemy.navMeshAgent.isStopped=false;
        enemy.navMeshAgent.ResetPath();
    }
}
