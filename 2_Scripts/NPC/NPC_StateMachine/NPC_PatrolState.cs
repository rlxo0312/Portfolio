using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC�� ������ ��θ� ���� �����ϴ� ���¸� �����ϴ� Ŭ����
/// Idle Ÿ���� �ƴ� ��� ���� ��������Ʈ�� �̵��ϸ� �̵� ���·� ��ȯ
/// </summary>
public class NPC_PatrolState : NPC_State
{
    NPC_Client npc_client;
    /// <summary>
    /// Patrol ���� ������
    /// </summary>
    /// <param name="_npc">NPC ��ü</param>
    /// <param name="_NPC_StateMachine">NPC ���� �ӽ�</param>
    /// <param name="_animName">�ִϸ��̼� �̸�</param>
    /// <param name="npc_Client">NPC Ŭ���̾�Ʈ ����</param>
    public NPC_PatrolState(NPC _npc, NPC_StateMachine _NPC_StateMachine, string _animName, NPC_Client npc_Client) : base(_npc, _NPC_StateMachine, _animName)
    {
        npc_client = npc_Client;
    }
    /// <summary>
    /// ���� ���� �� ��������Ʈ�� �����ϸ� �ش� �������� �̵� ����
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        if(npc_client.moveType == NPCMoveType.Idle)
        {
            return;
        }
        if(npc.targetWayPoint == null)
        {
            npc.NPCFindNextWayPoint();
        }
        if (npc.targetWayPoint && npc.navMeshAgent != null && npc.navMeshAgent.enabled)
        {
            npc.navMeshAgent.SetDestination(npc.targetWayPoint.position);
        }
    }
    public override void Exit() 
    {
        base.Exit();
    }
    /// <summary>
    /// NPC�� �������� �����ϸ� ���� ��������Ʈ�� �̵��ϸ� Move ���·� ��ȯ
    /// </summary>
    public override void Update()
    {
        base.Update();
        if (npc_client.moveType == NPCMoveType.Idle)
        {
            return;
        }
        if (!npc.NPCCheckRemainDistance())
        {
            Transform nextDestinatuion = npc.NPCFindNextWayPoint();
            if(npc.navMeshAgent != null && npc.navMeshAgent.enabled && nextDestinatuion != null)
            {
                npc.navMeshAgent.SetDestination(nextDestinatuion.position);
                //NPC_StateMachine.NPCChangeState(npc_client.npc_MoveState);
                NPC_StateMachine.NPCChangeState(npc_client.npc_MoveState);
            }
            
        }
    }
}
