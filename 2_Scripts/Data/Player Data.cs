using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �÷��̾��� ���� ������ �����ϴ� ScriptableObject.
/// Stat Ŭ������ Modifier �ý����� Ȱ���Ͽ� �⺻ �ɷ�ġ�� ��� ȿ���� �Բ� ó��.
///
/// <para>��� ����</para>
/// <para>public Stat[] stats</para>
/// <para>public float MaxHP, MaxMP, AttackPower, Defense</para>
/// <para>public Action&lt;PlayerData&gt; ChangedStats</para>
///
/// <para>���� ��ȸ �� ����</para>
/// <para>public float GetBaseValue(StatType type)</para>
/// <para>public float GetModifiedValue(StatType type)</para>
/// <para>public void SetBaseValue(StatType type, float value)</para>
///
/// <para>�ʱ�ȭ �� ���� ����</para>
/// <para>private void OnEnable()</para>
/// <para>private void InitalizeStats()</para>
/// <para>private void OnModifiedValue(Modifier value)</para>
///
/// <para>������ ����/����</para>
/// <para>public void EquipItem(Item_Data item)</para>
/// <para>public void UnequipItem(Item_Data item)</para>
///
/// <para>���� ���� Ž��</para>
/// <para>private Stat GetStat(StatType type)</para>
/// </summary>
[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/PlayerData" ,order =int.MaxValue)]
public class PlayerData : ScriptableObject
{
    public Stat[] stats;

    public float MaxHP; 
    public float MaxMP;

    public float AttackPower;
    [Header("�����(&) �ִ� 90%����")]
    [Range(0,90)]
    public float Defense;

    public Action<PlayerData> ChangedStats;
#if UNITY_EDITOR
    /// <summary>
    /// �����Ϳ��� Defense ��ġ�� 0~90 ������ ����� ��� �޽����� ���
    /// </summary>
    public void CheckPlayerDefense()
    {
        if (Defense < 0 || Defense > 90)
        {
            Debug.LogWarning("[Player_Data] Defense ������ �ʰ� �Ǵ� ������ �����ϼ̽��ϴ�. �缳�� ��Ź�帳�ϴ�.");
        }
    }
    /// <summary>
    /// ScriptableObject�� �����Ϳ��� ���� ����� �� �ڵ� ȣ���
    /// </summary>
    private void OnValidate()
    {
        CheckPlayerDefense();
    }
#endif
    /// <summary>
    /// �ش� ������ �⺻���� ��ȯ
    /// </summary>
    public float GetBaseValue(StatType type)
    {
        foreach(Stat stat in stats)
        {
            if(stat.type == type)
            {
                return stat.value.BaseValue;
            }
        }
        Debug.LogError($"������ statTpye'{type}'�� �������� �ʽ��ϴ�.");
        return -1;
    }
    /// <summary>
    /// ��� �� ��� �������� ������ ���� ���� ��ȯ
    /// </summary>
    public float GetModifiedValue(StatType type)
    {
        foreach (Stat stat in stats)
        {
            if(stat.type == type)
            {
                return stat.value.ModifiedValue;
            }
        }
        Debug.LogError($"������ statTpye'{type}'�� �������� �ʽ��ϴ�.");
        return -1;
    }
    /// <summary>
    /// ������ �⺻���� ����
    /// </summary>
    public void SetBaseValue(StatType type, float value)
    {
        foreach(Stat stat in stats)
        {
            if(stat.type == type)
            {
                stat.value.BaseValue = value;
            }
        }
    }

    private void OnEnable()
    {
        InitalizeStats();
    } 
    /// <summary>
    /// ���� �迭�� ��ȸ�ϸ� �ʱ�ȭ ����
    /// </summary>
    private void InitalizeStats()
    {
        foreach(Stat stat in stats)
        {
            stat.value = new Modifier(OnModifiedValue);
        }

        SetBaseValue(StatType.HP, MaxHP);
        SetBaseValue(StatType.MP, MaxMP);
        SetBaseValue(StatType.Attack, AttackPower);
        SetBaseValue(StatType.Defense, Defense);

        MaxHP = GetModifiedValue(StatType.HP);
        MaxMP = GetModifiedValue(StatType.MP);
        AttackPower = GetModifiedValue(StatType.Attack);
        Defense = GetModifiedValue(StatType.Defense);
    }
    /// <summary>
    /// ������ ����Ǿ��� �� ȣ��Ǵ� �ݹ� ó��
    /// </summary>
    private void OnModifiedValue(Modifier value)
    {
        ChangedStats?.Invoke(this);
    }
    /// <summary>
    /// ��� �����ϸ� ���ȿ� Modifier ����
    /// </summary>
    public void EquipItem(Item_Data item)
    {
        foreach(var statModifier in item.statModifiers)
        {
            var stat = GetStat(statModifier.statType);
            if(stat != null)
            {
                stat.value.Addmodifier(statModifier);
            }
        }
    }
    /// <summary>
    /// ��� �����ϸ� ���ȿ��� Modifier ����
    /// </summary>
    public void UnequipItem(Item_Data item)
    {
        foreach(var statModifier in item.statModifiers)
        {
            var stat = GetStat(statModifier.statType);
            if(stat != null)
            {
                stat.value.RemoveModifier(statModifier);
            }
        }
    }
    /// <summary>
    /// Ư�� Ÿ���� ������ ã��
    /// </summary>
    private Stat GetStat(StatType type)
    {
        foreach(Stat stat in stats)
        {
            if(stat.type == type)
            {
                return stat;
            }
        }
        return null;
    } 
}
