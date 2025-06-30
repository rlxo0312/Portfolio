using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC�� �̵� ���¸� ����ϴ� Ŭ����
/// ���� �ð� ���� �̵� �� Idle ���·� ��ȯ��
/// </summary>
public class NPC_MoveState : NPC_State
{
    NPC_Client npc_client;
    /// <summary>
    /// �̵� ���� ������
    /// </summary>
    /// <param name="_npc">NPC ��ü</param>
    /// <param name="_NPC_StateMachine">NPC ���� �ӽ�</param>
    /// <param name="_animName">�ִϸ��̼� �̸�</param>
    /// <param name="npc_Client">NPC Ŭ���̾�Ʈ ���� (Idle ���� ��ȯ��)</param>
    public NPC_MoveState(NPC _npc, NPC_StateMachine _NPC_StateMachine, string _animName, NPC_Client npc_Client ) : base(_npc, _NPC_StateMachine, _animName)
    {
       npc_client = npc_Client;
    }
    // ���� ���� �� �̵� �ð� �ʱ�ȭ
    public override void Enter()
    {
        base.Enter();
        stateTimer = npc.moveTime;
    }
    // ���� ���� �� �̵� ��� �ʱ�ȭ
    public override void Exit() 
    {
        base.Exit();
        npc.navMeshAgent.ResetPath();
    }
    // ���� ������Ʈ: �ٷ� Idle ���·� ��ȯ (�ӽ� ������ ���ɼ� ����)
    public override void Update()
    {
        base.Update();
        NPC_StateMachine.NPCChangeState(npc_client.npc_IdleState);
    }
}
