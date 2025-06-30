using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 플레이어의 manualCollider와 playerData를 초기화하고,
/// 피해 처리와 능력치 초기화를 담당하는 클래스
/// <para>사용 변수 : ManualCollider manualCollider,  Action OnChangerStats, PlayerData playerData</para>
/// </summary>
public class Player : Entity
{
    public ManualCollider manualCollider;

    public Action OnChangerStats;
    public PlayerData playerData;

    protected override void Awake()
    {
        base.Awake(); 
        manualCollider = GetComponentInChildren<ManualCollider>();
    }
    protected override void Start()
    {
        InitPlayerData();
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
    }
    /// <summary>
    /// PlayerData로부터 MaxHP, MaxMP, AttackPower, Defense를 초기화
    /// </summary>
    private void InitPlayerData()
    {
        //Debug.Log("InitPlayerData");
        MaxHP = playerData.MaxHP;
        MaxMP = playerData.MaxMP;   
        AttackPower = playerData.AttackPower;
        Defense = playerData.Defense;   
    }

    public override void BeDamaged(float damage, Vector3 contactPos, GameObject hitEffectPrefabs = null)
    {
        base.BeDamaged(damage, contactPos, hitEffectPrefabs);
    }
}
