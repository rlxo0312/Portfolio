using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType 
{
    HP, MP, Attack, Defense
}
/// <summary>
/// 플레이어의 능력치(HP, MP, Attack, Defense)를 관리하는 데이터 클래스
/// 각 스탯은 Modifier를 통해 장비 및 외부 효과로 수정 가능하며, IModifier 인터페이스 기반으로 동작
///
/// <para>사용 변수</para>
/// <para>public StatType type</para>
/// <para>public Modifier value</para>
/// <para>public enum StatType (HP, MP, Attack, Defense)</para>
///
/// </summary>
[System.Serializable]
public class Stat 
{
    public StatType type;
    public Modifier value;

}
/// <summary>
/// 스탯의 기본값과 수정값을 계산하며, IModifier 인터페이스를 통해 외부 효과를 적용할 수 있는 클래스
/// BaseValue가 변경되면 등록된 이벤트를 통해 실시간으로 ModifiedValue가 갱신
///
/// <para> 사용 변수</para>
/// <para>private float baseValue, modifiedValue</para>
/// <para>private List&lt;IModifier&gt; modifiers</para>
/// <para>private event Action&lt;Modifier&gt; OnModifyValue</para>
///
/// <para>프로퍼티</para>
/// <para>public float BaseValue { get; set; }</para>
/// <para>public float ModifiedValue { get; set; }</para>
///
/// <para>메서드</para>
/// <para>public Modifier(Action&lt;Modifier&gt; method = null)</para>
/// <para>public void RegisterModifierEvent(Action&lt;Modifier&gt; method)</para>
/// <para>public void UnRegisterModifierEvent(Action&lt;Modifier&gt; method)</para>
/// <para>public void Addmodifier(IModifier modifier)</para>
/// <para>public void RemoveModifier(IModifier modifier)</para>
/// <para>private void UpdateModifiedValue()</para>
/// </summary>
[System.Serializable]
public class Modifier
{
    [NonSerialized] private float baseValue;                     // 기본 데이터
    [SerializeField] private float modifiedValue;                // 추가될 데이터

    private event Action<Modifier> OnModifyValue;
    private List<IModifier> modifiers = new List<IModifier>(); // modifier로 지정된 인터페이스 벨류를 전부 보관했다가 baseValue에 적용시켜주는 리스트

    public float BaseValue
    {
        get => baseValue;
        set
        {
            baseValue = value;
            UpdateModifiedValue();
        }
    }

    public float ModifiedValue
    {
        get => modifiedValue;
        set => modifiedValue = value;
    } 

    public Modifier(Action<Modifier> method = null)
    {
        modifiedValue = baseValue;
        RegisterModifierEvent(method);
    }
    public void RegisterModifierEvent(Action<Modifier> method)
    {
        if (method != null)
        {
            OnModifyValue += method;
        }
    }
    public void UnRegisterModifierEvent(Action<Modifier> method)
    {
        if (method != null)
        {
            OnModifyValue -= method;
        }
    }
    private void UpdateModifiedValue()
    {
        float valueToAdd = 0;

        foreach (IModifier modifier in modifiers)
        {
            modifier.AddValue(ref valueToAdd);
        }

        ModifiedValue = baseValue + valueToAdd;

        OnModifyValue?.Invoke(this);
    }
    public void Addmodifier(IModifier modifier)
    {
        modifiers.Add(modifier);
        UpdateModifiedValue();
    }
    public void RemoveModifier(IModifier modifier)
    {
        modifiers.Remove(modifier);
        UpdateModifiedValue();
    }
}