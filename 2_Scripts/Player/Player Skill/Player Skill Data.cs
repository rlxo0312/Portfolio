using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �÷��̾� ��ų�� ������ ������ ��� ScriptableObject Ŭ����.
/// ���ݷ�, ����, �ִϸ��̼�, ��Ÿ��, �����, ����Ʈ �� ��ų�� �پ��� �������� ����.
///
/// <para>��� ����</para>
/// <para>��ų  - public string skillName, public Sprite skillSprite, public string skillComent</para>
/// <para>�ִϸ��̼�  - ublic AnimationClip skillAnimationClip, public string animationTriggerName</para>
/// <para>���� �ɷ�ġ - public float playerBonusAttackPower, playerBonusDefense, playerAttackSkillDuration, playerDefenseSkillDuration
///public float playerAttackSkillMagnification, playerAttackSkillMagnificationDuration</para>
/// <para>���� �����  - public float DebuffMonsterAttack, DebuffMonsterDefense, DebuffMonsterDuration, DebuffMonsterStartDuration</para>
/// <para>��ų ��Ÿ��  - public float skillColldown, returnTime </para>
/// <para>��ų ������ ����  - public Vector3 changePlayerBoxSize, changePlayerBoxCenter,  public float playerBoxSizeStartDelay</para>
/// <para>�������� - public bool isInvincibility, public float invincibilityDuration</para>
/// <para>MP���, HPȸ��  - public float healAmount, healStartDuration, useMPAmount,  public bool isHeal, isUseMP</para>
/// <para>��ų Ÿ�� ��  - public int targetCount,  public float targetCountDuration</para>
/// <para>��ų ����Ʈ ����  - public List&lt;PlayerSkillEffectData&gt; skillEffectDatas</para>
/// <para>��Ÿ - public bool isUsedOneTime</para>
/// <para>��� �޼���</para>
/// <para>public void CheckPlayerBonusDefenseArea(), public void CheckPlayerTargetCount()</para>
/// </summary>
[CreateAssetMenu(fileName = "PlayerSkillData", menuName = "Data/PlayerSkillData", order = int.MaxValue)]
public class PlayerSkillData : ScriptableObject
{
    [Header("��ų ����")]
    public string skillName;
    public Sprite skillSprite;
    [TextArea(3, 5)]public string skillComent;

    [Header("�ִϸ��̼� ����")]
    public AnimationClip skillAnimationClip;
    public string animationTriggerName;

    [Header("�߰� ���ݷ� ����")]
    public float playerBonusAttackPower;
    [Header("�߰� ���� ����(�ִ� 50%����)"),Range(0,50)]
    public float playerBonusDefense;
    public float playerAttackSkillDuration;
    public float playerDefenseSkillDuration;

    [Header("���ݽ�ų ���ݷ� ����")]
    public float playerAttackSkillMagnification;
    public float playerAttackSkillMagnificationDuration;

    [Header("���� ��� �����")]
    public float DebuffMonsterAttack;
    public float DebuffMonsterDefense;
    public float DebuffMonsterDuration;
    public float DebuffMonsterStartDuration;

    [Header("��ų ��Ÿ��")]
    public float skillColldown;
    public bool isUsedOneTime;

    [Header("��ų ��Ÿ� ����")]
    public Vector3 changePlayerBoxSize;
    public float returnTime;
    public float playerBoxSizeStartDelay;

    [Header("manualColider �߽� ��ġ ��ȭ")]
    public Vector3 changePlayerBoxCenter;    

    [Header("��ų ����Ʈ")]
    public List<PlayerSkillEffectData> skillEffectDatas;   

    [Header("��ƿ��Ƽ")]
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
            Debug.LogWarning("[PlayerSkillData] playerBonusDefense ������ �ʰ� �Ǵ� ������ �����ϼ̽��ϴ�. �缳�� ��Ź�帳�ϴ�.");
        }
    }

    public void CheckPlayerTargetCount()
    {
        if(targetCount < 0 || targetCount > 20)
        {
            Debug.LogWarning("[PlayerSkillData] targetCount�� ������ �ʰ� �Ǵ� ������ �����ϼ̽��ϴ�. �缳�� ��Ź�帳�ϴ�.");
        }
    }
#endif
} 


