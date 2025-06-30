using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC ���¸� ��ȯ�ϴ� ���� �ӽ� Ŭ����
/// 
/// <para>��� ����</para>
/// <para>public: NPC_State</para>
/// 
/// <para>��� �޼���</para>
/// <para>NPCInitilize(NPC_State), NPCChangeState(NPC_State)</para>
/// </summary>
public class NPC_StateMachine 
{
    public NPC_State NPC_State { get; private set; }

    /// <summary>
    /// �ʱ� ���� ���� �� ���� ���� ó��
    /// </summary>
    /// <param name="_startsTate">�ʱ� NPC ����</param>
    public void NPCInitilize(NPC_State _startsTate)
    {
        NPC_State = _startsTate;
        NPC_State.Enter();
    }
    /// <summary>
    /// ���� ���� ó�� - ������ ������ ��� �������� ����
    /// </summary>
    /// <param name="_newNPCState">������ ����</param>
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
