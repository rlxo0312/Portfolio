using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 몬스터가 정찰(Patrol) 상태일 때의 동작을 정의하는 클래스
/// 타겟 탐색, 웨이포인트 이동 등을 처리함
/// </summary>
public class Enemy_Patrol : Enemy_Monster_State
{
    /// <summary>
    /// Patrol 상태 생성자: 애니메이션 이름과 상태 머신 연결
    /// </summary>
    /// <param name="_enemy">적 객체</param>
    /// <param name="_monster_StateMachine">상태 머신</param>
    /// <param name="_animationName">애니메이션 이름</param>
    public Enemy_Patrol(Enemy _enemy, Enemy_Monster_StateMachine _monster_StateMachine, string _animationName) : base(_enemy, _monster_StateMachine, _animationName)
    {
       
    }
    // 상태 진입 시 웨이포인트를 설정하고 목적지로 이동함
    public override void Enter()
    {
        base.Enter();      
       if(enemy.targetWayPoint == null)
        {
            enemy.FindnextWayPoint();
        }
       if(enemy.navMeshAgent != null && enemy.navMeshAgent.isActiveAndEnabled && enemy.navMeshAgent.isOnNavMesh)
        {
            enemy.navMeshAgent.SetDestination(enemy.targetWayPoint.position);
        }
        else
        {
            Debug.Log("[Enemy_Patrol] NavMeshAgent not ready for SetDestination.");
        }
        if (enemy.targetWayPoint)
        {
            enemy.navMeshAgent.SetDestination(enemy.targetWayPoint.position);
        }
    }
    public override void Exit() 
    {
        base.Exit();
    }
    // 상태 지속 중 타겟 탐색, 거리 체크 및 다음 웨이포인트 설정 처리
    public override void Update()
    {
        base.Update();
        Transform target = enemy.SearchTarget();
        if (target)
        {
            if (enemy.IsAvailableAttack)
            {
                enemy.animator.CrossFade(enemy.AttackAniName, 0.2f);
                enemy.ChangeState(EnemyStateType.Attack);
            }
            else
            {
                enemy.ChangeState(EnemyStateType.Move);
            }
        }
        else
        {
            if (!enemy.CheckRemainDistance())
            {
                Transform nextDistance = enemy.FindnextWayPoint();
                if (nextDistance != null && enemy.navMeshAgent != null &&
                    enemy.navMeshAgent.isActiveAndEnabled && enemy.navMeshAgent.isOnNavMesh)
                {
                    enemy.navMeshAgent.SetDestination(nextDistance.position);
                }
                else
                {
                    Debug.LogWarning("[Enemy_Patrol] Can't set destination during patrol update.");
                }
            }
        }

    }
}
