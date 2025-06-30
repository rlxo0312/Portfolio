using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 스킬 이펙트의 세부 정보를 정의하는 ScriptableObject.
/// 이펙트 프리팹, 위치, 회전, 사용 부위, 투사체 여부 및 딜레이 등 다양한 이펙트 설정을 포함.
///
/// <para>사용 변수</para>
/// <para>스킬 이펙트 - public GameObject skillEffect </para>
/// <para>이펙트 생성 위치, 회전 - public Transform skillEffectTransform, public Vector3 skillEffectPos, skillRotation</para>
/// <para>이펙트 생성 위치(player의 특정위치) - public bool useWeaponTransform, useShieldTransform, useBodyTransform, useRightHandTransform, useLeftHandTransform</para>
/// <para>attackTrigger 타이밍 - public bool isAttackTrigger, public float AttackTriggerStartDuration</para>
/// <para>투사체 이펙트 관련 - public bool isProjectile, public float projectileSpeed, projectileLifeTime</para>
/// <para>이펙트 poolKey - public string effectPoolKey</para>
/// <para>이펙트 타이밍 조절 - public float playerSkillEffectStartDelay, skillEffectDuration</para>
/// <para>이펙트 개수 - public int skillEffectCount</para>
/// </summary>
[CreateAssetMenu(fileName = "PlayerSkillEffectData", menuName = "Data/PlayerSkillEffectData", order = int.MaxValue)]
public class PlayerSkillEffectData : ScriptableObject
{
    public GameObject skillEffect;
    public Vector3 skillEffectPos;
    [Tooltip("skillRotation 미지정시 회전 방향은 전방으로 고정함")]
    public Vector3 skillRotation;
    [Header("skillEffectTransform을 설정하지 않으면 플레이어 발에서 생성")]
    public Transform skillEffectTransform;

    [Header("skillEffectTransform의 위치를 검, 방패, 손, 몸전체으로 설정 여부")]
    public bool useWeaponTransform;
    public bool useShieldTransform; 
    public bool useBodyTransform;
    public bool useRightHandTransform;
    public bool useLeftHandTransform;

    [Header("해당 스킬 이펙트에 AttackTrigger발동 유무")]
    public bool isAttackTrigger;
    public float AttackTriggerStartDuration;

    [Header("투사체 스킬 사용 여부")]
    public bool isProjectile;
    public float projectileSpeed;
    public float projectileLifeTime;

    [Header("poolKey이름은 해당 prefab이름으로")] 
    public string effectPoolKey;
    [Header("스킬 시작시 딜레이 설정")]
    public float playerSkillEffectStartDelay;
    [Header("스킬 이펙트 시간")]
    public float skillEffectDuration;
    public int skillEffectCount = 1; 
}
