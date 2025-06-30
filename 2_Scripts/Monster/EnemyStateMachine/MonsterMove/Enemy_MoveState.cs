using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터가 이동(Move) 상태일 때의 동작을 정의하는 클래스
/// 타겟을 추적하거나, 거리에 따라 Run 또는 Attack 상태로 전환함
/// </summary>
public class Enemy_MoveState : Enemy_Monster_State
{
    /// <summary>
    /// Move 상태 생성자: 애니메이션 이름과 상태 머신 연결
    /// </summary>
    /// <param name="_enemy">적 객체</param>
    /// <param name="_monster_StateMachine">상태 머신</param>
    /// <param name="_animationName">애니메이션 이름</param>
    public Enemy_MoveState(Enemy _enemy, Enemy_Monster_StateMachine _monster_StateMachine, string _animationName) : base(_enemy, _monster_StateMachine, _animationName)
    {
        
    }
    // 상태 진입 시 이동 타이머를 초기화함
    public override void Enter()
    {
        base.Enter();        
        stateTimer = enemy.moveTime;
    }
    // 상태 종료 시 경로를 초기화함
    public override void Exit() 
    {
        base.Exit();          
        enemy.navMeshAgent.ResetPath();
        
    }
    // 상태 지속 중 타겟 추적 및 상태 전환 로직을 수행함
    public override void Update()
    {
        base.Update();        
        if(enemy.target == null)
        {
            enemy.ChangeState(EnemyStateType.Patrol);
            return;
        }

        if (enemy.navMeshAgent != null && enemy.navMeshAgent.isActiveAndEnabled && enemy.navMeshAgent.isOnNavMesh)
        {
            enemy.navMeshAgent.SetDestination(enemy.target.position);
        }

        if (enemy.IsAvailableAttack)
        {
            enemy.ChangeState(EnemyStateType.Attack);
            return;
        }

        float dist = (enemy.target.position - enemy.transform.position).sqrMagnitude;
        float hold = enemy.AttackRange + enemy.increasedRunRange;
        var runState = enemy.GetState(EnemyStateType.Run); 

        if(runState != null)
        {
            if(dist > hold * hold)
            {
                enemy.ChangeState(EnemyStateType.Run);
            }
        }
        else
        {
            enemy.ChangeState(EnemyStateType.Move);
        }
    }
}
