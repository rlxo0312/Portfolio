using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 몬스터 상태 클래스의 기본이 되는 추상 상태 클래스
/// 상태 간 공통 동작(애니메이션 재생, 타이머 관리 등)을 정의
///
/// <para>사용 변수</para>
/// <para>protected Enemy_Monster_StateMachine monster_StateMachine, Enemy enemy</para>
/// <para>private string animationName</para>
/// <para>protected float stateTimer</para>
/// </summary>
public class Enemy_Monster_State 
{
    protected Enemy_Monster_StateMachine monster_StateMachine;
    protected Enemy enemy;

    private string animationName;

    protected float stateTimer;
    /// <summary>
    /// 몬스터 상태 생성자 - 적, 상태머신, 애니메이션 이름 초기화
    /// </summary>
    public Enemy_Monster_State(Enemy _enemy, Enemy_Monster_StateMachine _monster_StateMachine, string _animationName)
    {
        enemy = _enemy;
        monster_StateMachine = _monster_StateMachine;
        animationName = _animationName;
    }
    // 상태 진입 시 애니메이션 재생 등 초기 설정 수행
    public virtual void Enter()
    {
        if (enemy.animator != null && enemy.animator.isActiveAndEnabled && enemy.animator.gameObject.activeInHierarchy)
        {
            enemy.animator.CrossFade(enemy.DefaultAniName, 0.2f);
        }
        else
        {
            Debug.LogWarning("[Enemy_Monster_State]Animator is not ready for CrossFade.");
        }
        enemy.animator.CrossFade(animationName, 0.2f);
    }
    public virtual void Exit() 
    {

    }
    // 매 프레임 호출되며, 상태 타이머 감소 등의 로직 수행
    public virtual void Update() 
    {
        stateTimer -=Time.deltaTime;
    }
}
