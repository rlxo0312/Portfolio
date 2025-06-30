using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 보스 몬스터의 치명적인 패턴(페이탈 어택) 상태를 정의하는 클래스
/// 일정 시간 동안 애니메이션을 재생하며, 시간이 끝나면 이동 상태로 전환됨
/// NavMeshAgent를 일시적으로 멈추고 다시 활성화하는 방식으로 구현됨
///
/// <para>사용 변수</para>
/// <para>private float FatalAttackDuration</para>
/// </summary>
public class Enemy_FatalSkill : Enemy_Monster_State
{
    private float FatalSkillDuration;

    /// <summary>
    /// 생성자: 치명적 공격 상태를 생성하며, 지속 시간과 애니메이션 이름 등을 초기화함
    /// </summary>
    /// <param name="_enemy">적 개체</param>
    /// <param name="_monster_StateMachine">상태머신</param>
    /// <param name="_animationName">재생할 애니메이션 이름</param>
    /// <param name="_fatalSkillDuration">페이탈 스킬 지속 시간</param>
    public Enemy_FatalSkill(Enemy _enemy, Enemy_Monster_StateMachine _monster_StateMachine, string _animationName, float _fatalSkillDuration) 
        : base(_enemy, _monster_StateMachine, _animationName)
    {
        FatalSkillDuration = _fatalSkillDuration;
    }
    // 상태에 진입했을 때 호출되며, NavMeshAgent를 멈추고 애니메이션을 재생함
    public override void Enter()
    {
        base.Enter();
        enemy.navMeshAgent.isStopped = true;
        enemy.animator?.CrossFade(enemy.FatalSkillAniName, 0.2f);
        //stateTimer = enemy.MonsterSkillDuration;
        stateTimer = FatalSkillDuration;
    }
    // 상태가 진행되는 동안 매 프레임 호출되며, 시간이 종료되면 이동 상태로 전환됨
    public override void Update()
    {
        base.Update();

        if(stateTimer <= 0)
        {
            enemy.ChangeState(EnemyStateType.Move);
        }
    }
    // 상태에서 빠져나올 때 NavMeshAgent를 다시 활성화하고 경로를 초기화함
    public override void Exit()
    {
        base.Exit();
        enemy.navMeshAgent.isStopped=false;
        enemy.navMeshAgent.ResetPath();
    }
}
