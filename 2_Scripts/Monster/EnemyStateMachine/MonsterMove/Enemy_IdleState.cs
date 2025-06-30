using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터가 대기(Idle) 상태일 때의 동작을 정의하는 클래스
/// 일정 시간 동안 대기하거나 플레이어를 발견하면 상태를 전환함
/// </summary>
public class Enemy_IdleState : Enemy_Monster_State
{
    /// <summary>
    /// Idle 상태 생성자: 애니메이션 이름과 상태 머신 연결
    /// </summary>
    /// <param name="_enemy">적 객체</param>
    /// <param name="_monster_StateMachine">상태 머신</param>
    /// <param name="_animationName">애니메이션 이름</param>
    public Enemy_IdleState(Enemy _enemy, Enemy_Monster_StateMachine _monster_StateMachine, string _animationName) : base(_enemy, _monster_StateMachine, _animationName)
    {
        
    }
    // 상태 진입 시 Idle 타이머를 초기화함
    public override void Enter()
    {
        base.Enter();
        
        stateTimer = enemy.idleTime;
    }
    // 상태 종료 시 호출되며, 현재는 특별한 처리를 하지 않음
    public override void Exit() 
    {
        base.Exit();
    }
    // 상태 지속 중 타겟 탐색 및 상태 전환 로직을 수행함
    public override void Update()
    {
        base.Update();

        
        Transform target = enemy.SearchTarget();
        if (target != null)
        {
           
            if (enemy.IsAvailableAttack)
            {
                enemy.ChangeState(EnemyStateType.Attack);
                return; //return을 사용하는 이유 : 하나의 조건을 만족하면 즉시 종료되기 때문 
            }
           
            enemy.ChangeState(EnemyStateType.Move);
            return;
            
        }
        else
        {            
            if (stateTimer <= 0)
            {
                enemy.ChangeState(EnemyStateType.Patrol);
            }
        }

    }
}
