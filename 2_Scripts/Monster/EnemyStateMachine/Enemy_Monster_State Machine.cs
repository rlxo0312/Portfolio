using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터 상태를 관리하는 상태 머신 클래스
/// 상태 전환 시 현재 상태의 Exit -> 새로운 상태의 Enter 순서로 호출
///
/// <para>사용 변수</para>
/// <para>public Enemy_Monster_State currentState</para>
///
/// <para>사용 메서드</para>
/// <para>public void Initilize(Enemy_Monster_State _monster_State)</para>
/// <para>public void ChangeState(Enemy_Monster_State _newState)</para>
/// </summary>
public class Enemy_Monster_StateMachine
{
    public Enemy_Monster_State currentState { get; private set; }   


    public void Initilize(Enemy_Monster_State _monster_State)
    {
        if(currentState != null)
        {
            currentState?.Exit();
        }
        currentState = _monster_State;
        currentState?.Enter();

    }
    public void ChangeState(Enemy_Monster_State _newState)
    {
        if (currentState == _newState)
        {
            return;
        }
        currentState?.Exit();
        currentState = _newState;
        currentState?.Enter();
    }
}
