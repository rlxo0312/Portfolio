using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����� Ÿ�Կ� ���� ������ ������ ��� ������ Ŭ����.
/// ����� UI �����ܿ� �ʿ��� Ǯ Ű, ������, ��������Ʈ�� �Բ� ������.
///
/// <para>��� ����</para>
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
