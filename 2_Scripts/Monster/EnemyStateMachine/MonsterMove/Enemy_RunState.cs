using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터가 달리기(Run) 상태일 때의 동작을 정의하는 클래스
/// 특정 거리 이상일 때 Move 상태 대신 Run 상태로 전환되어 빠르게 추적함
/// </summary>
public class Enemy_RunState : Enemy_Monster_State
{
    /// <summary>
    /// Run 상태 생성자: 애니메이션 이름과 상태 머신 연결
    /// </summary>
    /// <param name="_enemy">적 객체</param>
    /// <param name="_monster_StateMachine">상태 머신</param>
    /// <param name="_animationName">애니메이션 이름</param>
    public Enemy_RunState(Enemy _enemy, Enemy_Monster_StateMachine _monster_StateMachine, string _animationName) : base(_enemy, _monster_StateMachine, _animationName)
    {

    }
    // 상태 진입 시 이동 속도 증가 및 애니메이션 재생
    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.RunDuration;       

        if (enemy.navMeshAgent != null && enemy.navMeshAgent.isOnNavMesh)
        {
            enemy.navMeshAgent.speed = enemy.chaseSpeed;
            enemy.navMeshAgent.isStopped = false;
        }
        enemy.animator?.CrossFade(enemy.RunAniName, 0.2f);
    }
    // 상태 종료 시 정지 및 경로 초기화
    public override void Exit() 
    {
        base.Exit();

        if(enemy.navMeshAgent != null &&enemy.navMeshAgent.isActiveAndEnabled &&
                 enemy.navMeshAgent.isOnNavMesh)
        {
            enemy.navMeshAgent.isStopped = true;
            enemy.navMeshAgent.ResetPath();
        }
    }
    // 타겟 추적 중 일정 거리 이내 도달 시 Move 또는 Attack 상태로 전환
    public override void Update() 
    {
        base.Update();
        if (enemy.target == null)
        {
            enemy.ChangeState(EnemyStateType.Patrol);
            return;
        }
       /* Transform target = enemy.SearchTarget();
        if (target != null && enemy.navMeshAgent.isActiveAndEnabled && enemy.navMeshAgent.isOnNavMesh)        
        {
            enemy.navMeshAgent.SetDestination(target.position);
        }

        if (enemy.IsAvailableAttack)
        {
            enemy.ChangeState(EnemyStateType.Attack);
            return;
        }

        if (stateTimer <= 0f)
        {
            enemy.ChangeState(EnemyStateType.Move);
        }*/ 

        if(enemy.navMeshAgent != null && enemy.navMeshAgent.isActiveAndEnabled && enemy.navMeshAgent.isOnNavMesh)
        {
            enemy.navMeshAgent.SetDestination(enemy.target.position);
        }

        float dist = (enemy.target.position - enemy.transform.position).sqrMagnitude;
        float hold = enemy.AttackRange + enemy.increasedRunRange; 

        if(dist < hold * hold)
        {
            enemy.ChangeState(EnemyStateType.Move);
        }

        if (enemy.IsAvailableAttack)
        {
            enemy.ChangeState(EnemyStateType.Attack);
        }

        if(stateTimer <= 0f)
        {
            enemy.ChangeState(EnemyStateType.Idle);
        }
    }
}
