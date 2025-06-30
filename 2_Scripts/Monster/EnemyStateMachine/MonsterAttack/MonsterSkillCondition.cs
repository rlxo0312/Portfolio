using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType 
{
    Attack,
    Utility
}
/// <summary>
/// 몬스터의 스킬 발동 조건과 효과를 정의하는 클래스
/// HP 비율, 추가 공격력, 애니메이션, 상태 전환 등을 설정할 수 있음
///
/// <para>사용 변수</para>
/// <para>public float hpPercent, bonusAttackPower, skillDuration, returnTime, skillEffectDuration,
/// skillEffectStartDelay</para>
/// <para>public string animationName, effectPoolKey </para>
/// <para>public EnemyStateType enemyState </para>
/// <para>public Vector3 changeBoxSize, skillEffectPos</para>
/// <para>public GameObject skillEffect</para>
/// <para>public enum SkillType(Attack, Utility)</para>
/// </summary>
[System.Serializable]
public class MonsterSkillCondition 
{
    [Header("스킬 타입 표시용")]
    public SkillType skillType;
    [Range(0, 100)]
    public float hpPercent;
    public float bonusAttackPower;
    
   [Header("몬스터의 최대 추가 방어율 40%까지"), Range(0, 40)]
    public float bonusDefense;
    public float attackSkillDuration;
    public float defenseSkillDuration;
    public string animationName;
    public EnemyStateType enemyState;

    [Header("스킬 사용시 범위 설정")]
    public Vector3 changeBoxSize;
    public float returnTime;

    [Header("스킬 사용시 이펙트 설정")]
    public GameObject skillEffect;
    public Vector3 skillEffectPos;

    [Header("스킬 이펙트 지속시간 설정")]
    public float skillEffectDuration;

    [Header("스킬 사용 시작시 이펙트 딜레이를 늦추는 설정")]
    public float skillEffectStartDelay;
    public string effectPoolKey;

    [Header("유틸리티 스킬")]
    public bool isInvincibility; //무적 여부
    public float invincibilityDuration;
    public bool isHeal;
    public float healAmount;

#if UNITY_EDITOR
    /// <summary>
    /// bonusDefense의 유효성을 검사하고 잘못된 값일 경우 경고 출력 
    /// </summary>
    public void CheckEnemyBonusDefenseArea()
    {
        if(bonusDefense < 0f || bonusDefense > 40f)
        {
            Debug.LogWarning("[MonsterSkillCodition] bonusDefense의 범위를 초과 또는 음수로 설정하셨습니다. 재설정 부탁드립니다.");
        }
    }
#endif
}
