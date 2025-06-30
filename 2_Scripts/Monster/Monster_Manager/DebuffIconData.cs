using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 디버프 타입에 따른 아이콘 정보를 담는 데이터 클래스.
/// 디버프 UI 아이콘에 필요한 풀 키, 프리팹, 스프라이트를 함께 보유함.
///
/// <para>사용 변수</para>
/// <para>public DebuffType debuffType</para>
/// <para>public string poolKey</para>
/// <para>public GameObject prefab</para>
/// <para>public Sprite iconSprite</para>
/// </summary>
[System.Serializable]
public class DebuffIconData 
{
    public DebuffType debuffType;
    public string poolKey;
    public GameObject prefab;
    public Sprite iconSprite;
}
