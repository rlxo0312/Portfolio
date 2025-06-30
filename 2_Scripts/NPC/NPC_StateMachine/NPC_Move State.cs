using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC의 이동 상태를 담당하는 클래스
/// 일정 시간 동안 이동 후 Idle 상태로 전환됨
/// </summary>
public class NPC_MoveState : NPC_State
{
    NPC_Client npc_client;
    /// <summary>
    /// 이동 상태 생성자
    /// </summary>
    /// <param name="_npc">NPC 본체</param>
    /// <param name="_NPC_StateMachine">NPC 상태 머신</param>
    /// <param name="_animName">애니메이션 이름</param>
    /// <param name="npc_Client">NPC 클라이언트 참조 (Idle 상태 전환용)</param>
    public NPC_MoveState(NPC _npc, NPC_StateMachine _NPC_StateMachine, string _animName, NPC_Client npc_Client ) : base(_npc, _NPC_StateMachine, _animName)
    {
       npc_client = npc_Client;
    }
    // 상태 진입 시 이동 시간 초기화
    public override void Enter()
    {
        base.Enter();
        stateTimer = npc.moveTime;
    }
    // 상태 종료 시 이동 경로 초기화
    public override void Exit() 
    {
        base.Exit();
        npc.navMeshAgent.ResetPath();
    }
    // 상태 업데이트: 바로 Idle 상태로 전환 (임시 로직일 가능성 있음)
    public override void Update()
    {
        base.Update();
        NPC_StateMachine.NPCChangeState(npc_client.npc_IdleState);
    }
}
