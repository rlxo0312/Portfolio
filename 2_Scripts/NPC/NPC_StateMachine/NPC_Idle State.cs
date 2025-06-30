using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC�� Idle ���¸� ��Ÿ���� Ŭ����
/// ���� �ð� ����� �� Patrol ���·� ��ȯ��
/// </summary>
public class NPC_IdleState : NPC_State
{
    NPC_Client npc_client;
    public NPC_IdleState(NPC _npc, NPC_StateMachine _NPC_StateMachine, string _animName, NPC_Client npc_Client) : base(_npc, _NPC_StateMachine, _animName)
    {
        npc_client = npc_Client;
    }
    // ���� ���� �� ��� Ÿ�̸� �ʱ�ȭ
    public override void Enter()
    {
        base.Enter();
        stateTimer = npc.idleTime;
    }

    public override void Exit()
    {
        base.Exit();
       
    }
    // ��� �ð��� ����Ǹ� Patrol ���·� ��ȯ
    public override void Update()
    {
        base.Update();

        if(stateTimer <= 0)
        {           
            NPC_StateMachine.NPCChangeState(npc_client.npc_PatrolState);
        }
    }
}
