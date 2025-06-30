using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// NPC_Client Ŭ������ NPC�� ���¸� �����ϴ� ���� �ӽ� ��� NPC ����ü�̰�,
/// ����(Patrol), �̵�(Move), ���(Idle) ���¸� ���� �� ������, 
/// NPC�� �̸� �����͸� NPC UI�� �ݿ��ϴ� ����� ������
/// 
/// <para>��� ����</para>
/// <para>public NPC_PatrolState npc_PatrolState, public NPC_MoveState npc_MoveState, public NPC_IdleState npc_IdleState.
/// public NPC_Data npc_Data </para>
/// </summary>
public class NPC_Client : NPC
{
   
    public NPC_PatrolState npc_PatrolState { get; private set; } 
    public NPC_MoveState npc_MoveState { get; private set; }
    public NPC_IdleState npc_IdleState { get; private set; }
    public NPC_Data npc_Data;
    //public NPCUI NPCUI;
    
    protected override void Awake()
    {
        base.Awake();
        OnLoadComponents();        
        
        npc_PatrolState = new NPC_PatrolState(this, npc_StateMachine, "NPC_Walk", this);
        npc_IdleState = new NPC_IdleState(this, npc_StateMachine, "NPC_Idle", this);
        npc_MoveState = new NPC_MoveState(this, npc_StateMachine, "NPC_Walk", this); 
    }

    protected override void Start()
    {
        base.Start();
        npc_StateMachine.NPCInitilize(npc_PatrolState);

        /*Debug.Log($"[DEBUG] NPC_Data:{npc_Data}");
        Debug.Log($"[DEBUG] npc_Data.npcName:{npc_Data.npcName}");
        Debug.Log($"[DEBUG] NPC_UI:{npcUI}");*/

        if (moveType == NPCMoveType.Idle)
        {
            npc_StateMachine.NPCInitilize(npc_IdleState); // Static�̸� Idle ���·� ����
        }
        else
        {
            npc_StateMachine.NPCInitilize(npc_PatrolState); // �� �ܴ� Patrol ���·� ����
        }

        if (npc_Data != null && npcUI != null)
        {
            npcUI.SetName(npc_Data.npcName);
        }
        else
        {
            Debug.LogError($"[NPC_Client] {npc_Data.npcName}npcUI �Ǵ� npcData�� Null�Դϴ�.");
        }
    }
    protected override void Update()
    {       
        base.Update();
        //npc_StateMachine.NPC_State?.Update();
    }
    public override void OnLoadComponents()
    {
        base.OnLoadComponents();
        
    } 

   
}
