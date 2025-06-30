using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��� ���� ���� Ŭ������ �⺻�� �Ǵ� �߻� ���� Ŭ����
/// ���� �� ���� ����(�ִϸ��̼� ���, Ÿ�̸� ���� ��)�� ����
///
/// <para>��� ����</para>
/// <para>protected Enemy_Monster_StateMachine monster_StateMachine, Enemy enemy</para>
/// <para>private string animationName</para>
/// <para>protected float stateTimer</para>
/// </summary>
public class Enemy_Monster_State 
{
    protected Enemy_Monster_StateMachine monster_StateMachine;
    protected Enemy enemy;

    private string animationName;

    protected float stateTimer;
    /// <summary>
    /// ���� ���� ������ - ��, ���¸ӽ�, �ִϸ��̼� �̸� �ʱ�ȭ
    /// </summary>
    public Enemy_Monster_State(Enemy _enemy, Enemy_Monster_StateMachine _monster_StateMachine, string _animationName)
    {
        enemy = _enemy;
        monster_StateMachine = _monster_StateMachine;
        animationName = _animationName;
    }
    // ���� ���� �� �ִϸ��̼� ��� �� �ʱ� ���� ����
    public virtual void Enter()
    {
        if (enemy.animator != null && enemy.animator.isActiveAndEnabled && enemy.animator.gameObject.activeInHierarchy)
        {
            enemy.animator.CrossFade(enemy.DefaultAniName, 0.2f);
        }
        else
        {
            Debug.LogWarning("[Enemy_Monster_State]Animator is not ready for CrossFade.");
        }
        enemy.animator.CrossFade(animationName, 0.2f);
    }
    public virtual void Exit() 
    {

    }
    // �� ������ ȣ��Ǹ�, ���� Ÿ�̸� ���� ���� ���� ����
    public virtual void Update() 
    {
        stateTimer -=Time.deltaTime;
    }
}
