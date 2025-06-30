using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC 상태의 기본 베이스 클래스
/// 
/// <para>사용 변수</para>
/// <para>protected: npc, NPC_StateMachine, stateTimer</para>
/// <para>private: animName</para>
/// 
/// <para>사용 메서드</para>
/// <para>Enter(), Exit(), Update()</para>
/// </summary>
public class NPC_State 
{
    protected NPC npc;
    protected NPC_StateMachine NPC_StateMachine;

    private string animName;

    protected float stateTimer;

    /// <summary>
    /// 생성자 - NPC 상태 설정을 초기화
    /// </summary>
    /// <param name="_npc">NPC 인스턴스</param>
    /// <param name="_NPC_StateMachine">상태 머신</param>
    /// <param name="_animName">애니메이션 이름</param>
    public NPC_State(NPC _npc, NPC_StateMachine _NPC_StateMachine, string _animName)
    {
        npc = _npc;
        NPC_StateMachine = _NPC_StateMachine;
        animName = _animName;
    }
    // 상태 진입 시 실행되는 로직 - 애니메이션 재생 포함
    public virtual void Enter()
    {
        npc.animator.CrossFade(animName, 0.2f);
    }
    public virtual void Exit() 
    {

    }
    // 상태 업데이트 - 타이머 감소 처리
    public virtual void Update() 
    {
        stateTimer -=Time.deltaTime;
    }
}
