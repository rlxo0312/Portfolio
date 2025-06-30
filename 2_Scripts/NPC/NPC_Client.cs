using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// NPC_Client 클래스는 NPC의 상태를 관리하는 상태 머신 기반 NPC 구현체이고,
/// 순찰(Patrol), 이동(Move), 대기(Idle) 상태를 가질 수 있으며, 
/// NPC의 이름 데이터를 NPC UI에 반영하는 기능을 포함함
/// 
/// <para>사용 변수</para>
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
            npc_StateMachine.NPCInitilize(npc_IdleState); // Static이면 Idle 상태로 시작
        }
        else
        {
            npc_StateMachine.NPCInitilize(npc_PatrolState); // 그 외는 Patrol 상태로 시작
        }

        if (npc_Data != null && npcUI != null)
        {
            npcUI.SetName(npc_Data.npcName);
        }
        else
        {
            Debug.LogError($"[NPC_Client] {npc_Data.npcName}npcUI 또는 npcData가 Null입니다.");
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
