using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType 
{
    HP, MP, Attack, Defense
}
/// <summary>
/// �÷��̾��� �ɷ�ġ(HP, MP, Attack, Defense)�� �����ϴ� ������ Ŭ����
/// �� ������ Modifier�� ���� ��� �� �ܺ� ȿ���� ���� �����ϸ�, IModifier �������̽� ������� ����
///
/// <para>��� ����</para>
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
/// ������ �⺻���� �������� ����ϸ�, IModifier �������̽��� ���� �ܺ� ȿ���� ������ �� �ִ� Ŭ����
/// BaseValue�� ����Ǹ� ��ϵ� �̺�Ʈ�� ���� �ǽð����� ModifiedValue�� ����
///
/// <para> ��� ����</para>
/// <para>private float baseValue, modifiedValue</para>
/// <para>private List&lt;IModifier&gt; modifiers</para>
/// <para>private event Action&lt;Modifier&gt; OnModifyValue</para>
///
/// <para>������Ƽ</para>
/// <para>public float BaseValue { get; set; }</para>
/// <para>public float ModifiedValue { get; set; }</para>
///
/// <para>�޼���</para>
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
    [NonSerialized] private float baseValue;                     // �⺻ ������
    [SerializeField] private float modifiedValue;                // �߰��� ������

    private event Action<Modifier> OnModifyValue;
    private List<IModifier> modifiers = new List<IModifier>(); // modifier�� ������ �������̽� ������ ���� �����ߴٰ� baseValue�� ��������ִ� ����Ʈ

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