using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// 몬스터가 사망했을 때의 상태를 정의하는 클래스
/// 사망 애니메이션을 재생하고 일정 시간 후 실제 사망 처리를 진행함
/// NavMeshAgent를 비활성화하고 이후 상태 변경까지 관리함
///
/// <para>사용 변수</para>
/// <para>stateTimer = 5f 로 사망 대기 시간 설정</para>
/// </summary>
public class Enemy_Dead : Enemy_Monster_State
{    
    public Enemy_Dead(Enemy _enemy, Enemy_Monster_StateMachine _monster_StateMachine, string _animationName) : base(_enemy, _monster_StateMachine, _animationName)
    {
        
    }
    public override void Enter()
    {
        base.Enter();
        //Debug.Log("Entering Dead State");        
        enemy.navMeshAgent.isStopped = true;       
        enemy.animator.CrossFade(enemy.DeadAniName, 0.2f);        
        stateTimer = 5f;
    }
    public override void Exit() 
    {
        base.Exit();
        //Debug.Log("Exiting Dead State");

    }
    public override void Update()
    {
        base.Update();
        if(stateTimer < 0)
        {
            if (!enemy.navMeshAgent.isOnNavMesh)
            {
                Debug.LogWarning($"{enemy.name} is not on NavMesh!");
            }
            //Debug.Log($"Animator active: {enemy.animator?.isActiveAndEnabled}");
            //Debug.Log($"NavMeshAgent active: {enemy.navMeshAgent?.isActiveAndEnabled}, isOnNavMesh: {enemy.navMeshAgent?.isOnNavMesh}");
            //Debug.Log($"Animator GameObject Active: {enemy.animator.gameObject.activeInHierarchy}");
            //Debug.Log($"NavMeshAgent GameObject Active: {enemy.navMeshAgent.gameObject.activeInHierarchy}");
            //Debug.Log($"[ChangeState 전] Animator enabled: {zombie.animator.enabled}, Animator gameObject active: {zombie.animator.gameObject.activeInHierarchy}");            
            enemy.Die();
        }
    }  
   
}
