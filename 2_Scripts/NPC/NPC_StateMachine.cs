using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC 상태를 전환하는 상태 머신 클래스
/// 
/// <para>사용 변수</para>
/// <para>public: NPC_State</para>
/// 
/// <para>사용 메서드</para>
/// <para>NPCInitilize(NPC_State), NPCChangeState(NPC_State)</para>
/// </summary>
public class NPC_StateMachine 
{
    public NPC_State NPC_State { get; private set; }

    /// <summary>
    /// 초기 상태 설정 및 상태 진입 처리
    /// </summary>
    /// <param name="_startsTate">초기 NPC 상태</param>
    public void NPCInitilize(NPC_State _startsTate)
    {
        NPC_State = _startsTate;
        NPC_State.Enter();
    }
    /// <summary>
    /// 상태 변경 처리 - 동일한 상태일 경우 변경하지 않음
    /// </summary>
    /// <param name="_newNPCState">변경할 상태</param>
    public void NPCChangeState(NPC_State _newNPCState)
    {
        if (NPC_State == _newNPCState)
        {
            return;
        }
        NPC_State.Exit();
        NPC_State = _newNPCState;
        NPC_State.Enter();
    }
}
