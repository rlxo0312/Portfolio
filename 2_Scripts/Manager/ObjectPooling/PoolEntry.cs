using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트 풀링 시스템에서 개별 풀의 설정 정보를 담는 데이터 클래스
/// 풀 타입, 고유 키, 프리팹, 초기 로드 수량 등을 지정할 수 있음
///
/// <para>사용변수</para>
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
    ConsumptionItem,//소비아이템
    EquipmentItem,//장비아이템
    UI,//UI관련
    Skill // Skill관련
}

