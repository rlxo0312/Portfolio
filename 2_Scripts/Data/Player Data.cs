using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 플레이어의 스탯 정보를 관리하는 ScriptableObject.
/// Stat 클래스와 Modifier 시스템을 활용하여 기본 능력치와 장비 효과를 함께 처리.
///
/// <para>사용 변수</para>
/// <para>public Stat[] stats</para>
/// <para>public float MaxHP, MaxMP, AttackPower, Defense</para>
/// <para>public Action&lt;PlayerData&gt; ChangedStats</para>
///
/// <para>스탯 조회 및 설정</para>
/// <para>public float GetBaseValue(StatType type)</para>
/// <para>public float GetModifiedValue(StatType type)</para>
/// <para>public void SetBaseValue(StatType type, float value)</para>
///
/// <para>초기화 및 스탯 적용</para>
/// <para>private void OnEnable()</para>
/// <para>private void InitalizeStats()</para>
/// <para>private void OnModifiedValue(Modifier value)</para>
///
/// <para>아이템 장착/해제</para>
/// <para>public void EquipItem(Item_Data item)</para>
/// <para>public void UnequipItem(Item_Data item)</para>
///
/// <para>내부 스탯 탐색</para>
/// <para>private Stat GetStat(StatType type)</para>
/// </summary>
[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/PlayerData" ,order =int.MaxValue)]
public class PlayerData : ScriptableObject
{
    public Stat[] stats;

    public float MaxHP; 
    public float MaxMP;

    public float AttackPower;
    [Header("방어율(&) 최대 90%까지")]
    [Range(0,90)]
    public float Defense;

    public Action<PlayerData> ChangedStats;
#if UNITY_EDITOR
    /// <summary>
    /// 에디터에서 Defense 수치가 0~90 범위를 벗어나면 경고 메시지를 출력
    /// </summary>
    public void CheckPlayerDefense()
    {
        if (Defense < 0 || Defense > 90)
        {
            Debug.LogWarning("[Player_Data] Defense 범위를 초과 또는 음수로 설정하셨습니다. 재설정 부탁드립니다.");
        }
    }
    /// <summary>
    /// ScriptableObject가 에디터에서 값이 변경될 때 자동 호출됨
    /// </summary>
    private void OnValidate()
    {
        CheckPlayerDefense();
    }
#endif
    /// <summary>
    /// 해당 스탯의 기본값을 반환
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
        Debug.LogError($"지정한 statTpye'{type}'은 존재하지 않습니다.");
        return -1;
    }
    /// <summary>
    /// 장비 등 모든 수정값을 적용한 최종 값을 반환
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
        Debug.LogError($"지정한 statTpye'{type}'은 존재하지 않습니다.");
        return -1;
    }
    /// <summary>
    /// 스탯의 기본값을 설정
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
    /// 스탯 배열을 순회하며 초기화 수행
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
    /// 스탯이 변경되었을 때 호출되는 콜백 처리
    /// </summary>
    private void OnModifiedValue(Modifier value)
    {
        ChangedStats?.Invoke(this);
    }
    /// <summary>
    /// 장비를 장착하며 스탯에 Modifier 적용
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
    /// 장비를 해제하며 스탯에서 Modifier 제거
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
    /// 특정 타입의 스탯을 찾음
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
