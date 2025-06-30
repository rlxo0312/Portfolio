using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �÷��̾��� manualCollider�� playerData�� �ʱ�ȭ�ϰ�,
/// ���� ó���� �ɷ�ġ �ʱ�ȭ�� ����ϴ� Ŭ����
/// <para>��� ���� : ManualCollider manualCollider,  Action OnChangerStats, PlayerData playerData</para>
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
    /// PlayerData�κ��� MaxHP, MaxMP, AttackPower, Defense�� �ʱ�ȭ
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
