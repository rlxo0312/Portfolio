using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC의 Idle 상태를 나타내는 클래스
/// 일정 시간 대기한 후 Patrol 상태로 전환됨
/// </summary>
public class NPC_IdleState : NPC_State
{
    NPC_Client npc_client;
    public NPC_IdleState(NPC _npc, NPC_StateMachine _NPC_StateMachine, string _animName, NPC_Client npc_Client) : base(_npc, _NPC_StateMachine, _animName)
    {
        npc_client = npc_Client;
    }
    // 상태 진입 시 대기 타이머 초기화
    public override void Enter()
    {
        base.Enter();
        stateTimer = npc.idleTime;
    }

    public override void Exit()
    {
        base.Exit();
       
    }
    // 대기 시간이 종료되면 Patrol 상태로 전환
    public override void Update()
    {
        base.Update();

        if(stateTimer <= 0)
        {           
            NPC_StateMachine.NPCChangeState(npc_client.npc_PatrolState);
        }
    }
}
