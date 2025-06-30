using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾� ��ų ����Ʈ�� ���� ������ �����ϴ� ScriptableObject.
/// ����Ʈ ������, ��ġ, ȸ��, ��� ����, ����ü ���� �� ������ �� �پ��� ����Ʈ ������ ����.
///
/// <para>��� ����</para>
/// <para>��ų ����Ʈ - public GameObject skillEffect </para>
/// <para>����Ʈ ���� ��ġ, ȸ�� - public Transform skillEffectTransform, public Vector3 skillEffectPos, skillRotation</para>
/// <para>����Ʈ ���� ��ġ(player�� Ư����ġ) - public bool useWeaponTransform, useShieldTransform, useBodyTransform, useRightHandTransform, useLeftHandTransform</para>
/// <para>attackTrigger Ÿ�̹� - public bool isAttackTrigger, public float AttackTriggerStartDuration</para>
/// <para>����ü ����Ʈ ���� - public bool isProjectile, public float projectileSpeed, projectileLifeTime</para>
/// <para>����Ʈ poolKey - public string effectPoolKey</para>
/// <para>����Ʈ Ÿ�̹� ���� - public float playerSkillEffectStartDelay, skillEffectDuration</para>
/// <para>����Ʈ ���� - public int skillEffectCount</para>
/// </summary>
[CreateAssetMenu(fileName = "PlayerSkillEffectData", menuName = "Data/PlayerSkillEffectData", order = int.MaxValue)]
public class PlayerSkillEffectData : ScriptableObject
{
    public GameObject skillEffect;
    public Vector3 skillEffectPos;
    [Tooltip("skillRotation �������� ȸ�� ������ �������� ������")]
    public Vector3 skillRotation;
    [Header("skillEffectTransform�� �������� ������ �÷��̾� �߿��� ����")]
    public Transform skillEffectTransform;

    [Header("skillEffectTransform�� ��ġ�� ��, ����, ��, ����ü���� ���� ����")]
    public bool useWeaponTransform;
    public bool useShieldTransform; 
    public bool useBodyTransform;
    public bool useRightHandTransform;
    public bool useLeftHandTransform;

    [Header("�ش� ��ų ����Ʈ�� AttackTrigger�ߵ� ����")]
    public bool isAttackTrigger;
    public float AttackTriggerStartDuration;

    [Header("����ü ��ų ��� ����")]
    public bool isProjectile;
    public float projectileSpeed;
    public float projectileLifeTime;

    [Header("poolKey�̸��� �ش� prefab�̸�����")] 
    public string effectPoolKey;
    [Header("��ų ���۽� ������ ����")]
    public float playerSkillEffectStartDelay;
    [Header("��ų ����Ʈ �ð�")]
    public float skillEffectDuration;
    public int skillEffectCount = 1; 
}
