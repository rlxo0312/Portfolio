using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 특정 스탯(StatType)에 대한 수정 값을 정의하는 클래스
/// IModifier 인터페이스를 구현하여 Modifier 시스템에서 적용 가능한 스탯 보정값을 제공
///
/// <para>상태 변수</para>
/// <para>public StatType statType</para>
/// <para>public float value</para>
/// <para>public enum StatType (HP, MP, Attack, Defense)</para>
///
/// <para>사용 메서드</para>
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
