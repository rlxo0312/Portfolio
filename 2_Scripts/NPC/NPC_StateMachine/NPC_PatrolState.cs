using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC가 지정된 경로를 따라 순찰하는 상태를 정의하는 클래스
/// Idle 타입이 아닌 경우 다음 웨이포인트로 이동하며 이동 상태로 전환
/// </summary>
public class NPC_PatrolState : NPC_State
{
    NPC_Client npc_client;
    /// <summary>
    /// Patrol 상태 생성자
    /// </summary>
    /// <param name="_npc">NPC 객체</param>
    /// <param name="_NPC_StateMachine">NPC 상태 머신</param>
    /// <param name="_animName">애니메이션 이름</param>
    /// <param name="npc_Client">NPC 클라이언트 정보</param>
    public NPC_PatrolState(NPC _npc, NPC_StateMachine _NPC_StateMachine, string _animName, NPC_Client npc_Client) : base(_npc, _NPC_StateMachine, _animName)
    {
        npc_client = npc_Client;
    }
    /// <summary>
    /// 상태 진입 시 웨이포인트가 존재하면 해당 지점으로 이동 시작
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
    /// NPC가 목적지에 도달하면 다음 웨이포인트로 이동하며 Move 상태로 전환
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
