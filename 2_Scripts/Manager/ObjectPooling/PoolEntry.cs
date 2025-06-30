using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������Ʈ Ǯ�� �ý��ۿ��� ���� Ǯ�� ���� ������ ��� ������ Ŭ����
/// Ǯ Ÿ��, ���� Ű, ������, �ʱ� �ε� ���� ���� ������ �� ����
///
/// <para>��뺯��</para>
/// <para>public PoolTpye poolTpye</para>
/// <para>public string key</para>
/// <para>public GameObject prefab</para>
/// <para>public int preloadCount</para>
/// <para>public bool preload</para>
/// <para>public enum PoolTpye (Monster, ConsumptionItem, EquipmentItem, UI)</para>
/// </summary>
[System.Serializable]
public class PoolEntry 
{
    [Space(3)]
    public PoolTpye poolTpye;
    public string key;
    public GameObject prefab;
    public int preloadCount;
    public bool preload = true;
}

public enum PoolTpye 
{
    Monster, 
    ConsumptionItem,//�Һ������
    EquipmentItem,//��������
    UI,//UI����
    Skill // Skill����
}

