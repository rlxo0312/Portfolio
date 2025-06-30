using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어 스킬의 데이터 정보를 담는 ScriptableObject 클래스.
/// 공격력, 방어력, 애니메이션, 쿨타임, 디버프, 이펙트 등 스킬의 다양한 설정값을 포함.
///
/// <para>사용 변수</para>
/// <para>스킬  - public string skillName, public Sprite skillSprite, public string skillComent</para>
/// <para>애니메이션  - ublic AnimationClip skillAnimationClip, public string animationTriggerName</para>
/// <para>증가 능력치 - public float playerBonusAttackPower, playerBonusDefense, playerAttackSkillDuration, playerDefenseSkillDuration
///public float playerAttackSkillMagnification, playerAttackSkillMagnificationDuration</para>
/// <para>몬스터 디버프  - public float DebuffMonsterAttack, DebuffMonsterDefense, DebuffMonsterDuration, DebuffMonsterStartDuration</para>
/// <para>스킬 쿨타임  - public float skillColldown, returnTime </para>
/// <para>스킬 사저리 증가  - public Vector3 changePlayerBoxSize, changePlayerBoxCenter,  public float playerBoxSizeStartDelay</para>
/// <para>무적관련 - public bool isInvincibility, public float invincibilityDuration</para>
/// <para>MP사용, HP회복  - public float healAmount, healStartDuration, useMPAmount,  public bool isHeal, isUseMP</para>
/// <para>스킬 타겟 수  - public int targetCount,  public float targetCountDuration</para>
/// <para>스킬 이펙트 관련  - public List&lt;PlayerSkillEffectData&gt; skillEffectDatas</para>
/// <para>기타 - public bool isUsedOneTime</para>
/// <para>사용 메서드</para>
/// <para>public void CheckPlayerBonusDefenseArea(), public void CheckPlayerTargetCount()</para>
/// </summary>
[CreateAssetMenu(fileName = "PlayerSkillData", menuName = "Data/PlayerSkillData", order = int.MaxValue)]
public class PlayerSkillData : ScriptableObject
{
    [Header("스킬 정보")]
    public string skillName;
    public Sprite skillSprite;
    [TextArea(3, 5)]public string skillComent;

    [Header("애니메이션 설정")]
    public AnimationClip skillAnimationClip;
    public string animationTriggerName;

    [Header("추가 공격력 스텟")]
    public float playerBonusAttackPower;
    [Header("추가 방어력 스텟(최대 50%까지)"),Range(0,50)]
    public float playerBonusDefense;
    public float playerAttackSkillDuration;
    public float playerDefenseSkillDuration;

    [Header("공격스킬 공격력 배율")]
    public float playerAttackSkillMagnification;
    public float playerAttackSkillMagnificationDuration;

    [Header("몬스터 대상 디버프")]
    public float DebuffMonsterAttack;
    public float DebuffMonsterDefense;
    public float DebuffMonsterDuration;
    public float DebuffMonsterStartDuration;

    [Header("스킬 쿨타임")]
    public float skillColldown;
    public bool isUsedOneTime;

    [Header("스킬 사거리 증가")]
    public Vector3 changePlayerBoxSize;
    public float returnTime;
    public float playerBoxSizeStartDelay;

    [Header("manualColider 중심 위치 변화")]
    public Vector3 changePlayerBoxCenter;    

    [Header("스킬 이펙트")]
    public List<PlayerSkillEffectData> skillEffectDatas;   

    [Header("유틸리티")]
    public bool isInvincibility;
    public float invincibilityDuration;
    public bool isHeal;
    public float healAmount;
    public float healStartDuration;
    [Range(0, 20)]public int targetCount;
    public float targetCountDuration;
    public bool isUseMP;
    public float useMPAmount;

#if UNITY_EDITOR
    public void CheckPlayerBonusDefenseArea()
    {
        if(playerBonusDefense < 0 || playerBonusDefense > 50)
        {
            Debug.LogWarning("[PlayerSkillData] playerBonusDefense 범위를 초과 또는 음수로 설정하셨습니다. 재설정 부탁드립니다.");
        }
    }

    public void CheckPlayerTargetCount()
    {
        if(targetCount < 0 || targetCount > 20)
        {
            Debug.LogWarning("[PlayerSkillData] targetCount가 범위를 초과 또는 음수로 설정하셨습니다. 재설정 부탁드립니다.");
        }
    }
#endif
} 


