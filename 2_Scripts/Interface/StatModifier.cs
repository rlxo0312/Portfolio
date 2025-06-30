using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Ư�� ����(StatType)�� ���� ���� ���� �����ϴ� Ŭ����
/// IModifier �������̽��� �����Ͽ� Modifier �ý��ۿ��� ���� ������ ���� �������� ����
///
/// <para>���� ����</para>
/// <para>public StatType statType</para>
/// <para>public float value</para>
/// <para>public enum StatType (HP, MP, Attack, Defense)</para>
///
/// <para>��� �޼���</para>
/// <para>public void AddValue(ref float target)</para>
/// </summary>
public class StatModifier : IModifier
{
    public StatType statType;
    public float value;
    public void AddValue(ref float target)
    {
        target += value;
    }
}
