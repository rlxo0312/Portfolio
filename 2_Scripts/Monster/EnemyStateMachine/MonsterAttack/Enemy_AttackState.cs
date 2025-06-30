using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터의 공격 상태를 정의하는 클래스
/// 상태 진입 시 공격 가능 여부를 확인하고, 조건에 따라 Idle 상태로 전환함
///</summary>
public class Enemy_AttackState : Enemy_Monster_State
{
    /// <summary>
    /// 생성자: Enemy, 상태머신, 애니메이션 이름을 초기화
    /// </summary>
    /// <param name="_enemy">적 개체</param>
    /// <param name="_monster_StateMachine">상태머신</param>
    /// <param name="_animationName">애니메이션 이름</param>
    public Enemy_AttackState(Enemy _enemy, Enemy_Monster_StateMachine _monster_StateMachine, string _animationName) : base(_enemy, _monster_StateMachine, _animationName)
    {
        
    }
    // 공격 상태에 진입할 때 호출되며, 공격 가능 여부를 판단하고 공격 로그를 출력
    public override void Enter()
    {
        base.Enter();
        //stateTimer = enemy_Monster_Zombie.moveTime;  
        //Debug.Log("좀비 공격");
        Debug.Log($"[Enemy_AttackState] {enemy.name} 공격시작 , {enemy.name}의 공격력: {enemy.AttackPower}");
        if (!enemy.IsAvailableAttack && enemy.target != null)
        {
            enemy.ChangeState(EnemyStateType.Idle);
            return;
        }
    }
    // 공격 상태에서 나갈 때 호출되는 메서드
    public override void Exit()
    {
        base.Exit();

    }
    // 매 프레임 호출되며, 상태 갱신을 담당
    public override void Update()
    {
        base.Update();
        
    }
}
