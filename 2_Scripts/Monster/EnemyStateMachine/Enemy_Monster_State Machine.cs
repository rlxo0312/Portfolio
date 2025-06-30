using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ���¸� �����ϴ� ���� �ӽ� Ŭ����
/// ���� ��ȯ �� ���� ������ Exit -> ���ο� ������ Enter ������ ȣ��
///
/// <para>��� ����</para>
/// <para>public Enemy_Monster_State currentState</para>
///
/// <para>��� �޼���</para>
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
