using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC ������ �⺻ ���̽� Ŭ����
/// 
/// <para>��� ����</para>
/// <para>protected: npc, NPC_StateMachine, stateTimer</para>
/// <para>private: animName</para>
/// 
/// <para>��� �޼���</para>
/// <para>Enter(), Exit(), Update()</para>
/// </summary>
public class NPC_State 
{
    protected NPC npc;
    protected NPC_StateMachine NPC_StateMachine;

    private string animName;

    protected float stateTimer;

    /// <summary>
    /// ������ - NPC ���� ������ �ʱ�ȭ
    /// </summary>
    /// <param name="_npc">NPC �ν��Ͻ�</param>
    /// <param name="_NPC_StateMachine">���� �ӽ�</param>
    /// <param name="_animName">�ִϸ��̼� �̸�</param>
    public NPC_State(NPC _npc, NPC_StateMachine _NPC_StateMachine, string _animName)
    {
        npc = _npc;
        NPC_StateMachine = _NPC_StateMachine;
        animName = _animName;
    }
    // ���� ���� �� ����Ǵ� ���� - �ִϸ��̼� ��� ����
    public virtual void Enter()
    {
        npc.animator.CrossFade(animName, 0.2f);
    }
    public virtual void Exit() 
    {

    }
    // ���� ������Ʈ - Ÿ�̸� ���� ó��
    public virtual void Update() 
    {
        stateTimer -=Time.deltaTime;
    }
}
