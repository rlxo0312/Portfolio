using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 플레이어의 Animator 상태 머신에서 특정 상태에 진입할 때 자동으로 호출되는 StateMachineBehaviour
/// 상태 진입 시 PlayerManager의 상태 제어 변수들을 초기화하여, 다음 행동을 수행할 수 있도록 함
///
/// <para>사용 변수</para>
/// <para>PlayerManagerReference playerManagerRef</para>
///
/// <para>메서드</para>
/// <para>public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)</para>
/// </summary>
public class ResetActionFlag : StateMachineBehaviour
{
    //PlayerManager playerManager;
    PlayerManagerReference playerManagerRef;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        /*if(playerManager == null)
        {
            playerManager = animator.GetComponent<PlayerManager>();
        }*/
        if(playerManagerRef == null)
        {
            playerManagerRef = animator.GetComponent<PlayerManagerReference>();
        }
        playerManagerRef.PlayerManager.isPerformingAction = false;
        playerManagerRef.PlayerManager.applyRootMotion = false;
        playerManagerRef.PlayerManager.canRotate = true;
        playerManagerRef.PlayerManager.canMove = true;
        playerManagerRef.PlayerManager.canCombo = false;
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
