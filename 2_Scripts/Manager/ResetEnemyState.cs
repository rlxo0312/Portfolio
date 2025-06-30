using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 몬스터의 Animator 상태 머신에서 특정 상태에 진입 시 자동으로 호출되는 StateMachineBehaviour
/// 상태 진입 시 Enemy의 ResetEnemyState()를 강제로 실행하여 AI 상태를 정상화함
///
/// <para>메서드</para>
/// <para>public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)</para>
/// </summary>
public class ResetEnemyState : StateMachineBehaviour
{     
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<EnemyReference>())
        {
            var monster = animator.GetComponent<EnemyReference>();
            monster.Enemy.ResetEnemyState();
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
