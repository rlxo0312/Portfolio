using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType 
{
    Attack,
    Utility
}
/// <summary>
/// ������ ��ų �ߵ� ���ǰ� ȿ���� �����ϴ� Ŭ����
/// HP ����, �߰� ���ݷ�, �ִϸ��̼�, ���� ��ȯ ���� ������ �� ����
///
/// <para>��� ����</para>
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
    [Header("��ų Ÿ�� ǥ�ÿ�")]
    public SkillType skillType;
    [Range(0, 100)]
    public float hpPercent;
    public float bonusAttackPower;
    
   [Header("������ �ִ� �߰� ����� 40%����"), Range(0, 40)]
    public float bonusDefense;
    public float attackSkillDuration;
    public float defenseSkillDuration;
    public string animationName;
    public EnemyStateType enemyState;

    [Header("��ų ���� ���� ����")]
    public Vector3 changeBoxSize;
    public float returnTime;

    [Header("��ų ���� ����Ʈ ����")]
    public GameObject skillEffect;
    public Vector3 skillEffectPos;

    [Header("��ų ����Ʈ ���ӽð� ����")]
    public float skillEffectDuration;

    [Header("��ų ��� ���۽� ����Ʈ �����̸� ���ߴ� ����")]
    public float skillEffectStartDelay;
    public string effectPoolKey;

    [Header("��ƿ��Ƽ ��ų")]
    public bool isInvincibility; //���� ����
    public float invincibilityDuration;
    public bool isHeal;
    public float healAmount;

#if UNITY_EDITOR
    /// <summary>
    /// bonusDefense�� ��ȿ���� �˻��ϰ� �߸��� ���� ��� ��� ��� 
    /// </summary>
    public void CheckEnemyBonusDefenseArea()
    {
        if(bonusDefense < 0f || bonusDefense > 40f)
        {
            Debug.LogWarning("[MonsterSkillCodition] bonusDefense�� ������ �ʰ� �Ǵ� ������ �����ϼ̽��ϴ�. �缳�� ��Ź�帳�ϴ�.");
        }
    }
#endif
}
