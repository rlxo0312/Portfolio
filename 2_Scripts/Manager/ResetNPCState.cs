using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// NPC의 Animator 상태 머신에서 특정 상태에 진입할 때 자동으로 호출되는 StateMachineBehaviour
/// 상태 진입 시 NPC_Client의 애니메이션을 강제로 "NPC_Idle"로 전환시켜 AI 상태를 초기화
///
/// <para>메서드</para>
/// <para>public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)</para>
/// </summary>
public class ResetNPCState : StateMachineBehaviour
{
     //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<NPC_Client>())
        {
            var npc = animator.GetComponent<NPC_Client>();
            npc.animator.CrossFade("NPC_Idle", 0.2f);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
