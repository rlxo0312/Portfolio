using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 사용 가능한 인벤토리 아이템의 정보를 관리하고, 플레이어의 HP/MP 회복 및 아이템 이펙트 실행 등 실제 동작을 처리하는 클래스
///
/// <para>사용 변수</para>
/// <para>public InventoryItem item, public float lastUseTime</para>
/// <para>private KeyCode itemKey, private PlayerUseItemUi itemUi, private PlayerManager playerManager</para>
/// <para>public PlayerUseItemUi ItemUi, public KeyCode GetKey()</para>
///
/// <para>사용 메서드</para>
/// <para>public Item(InventoryItem inventoryItem, KeyCode key, PlayerUseItemUi ui, PlayerManager manager), 
/// public bool isReady(), public virtual void Active(MonoBehaviour caller)</para>
/// <para>private void RecoveryHP(Item_Data data , PlayerManager player), private void RecoveryMP(Item_Data data, PlayerManager player),
/// private IEnumerator StartItemEffect(Item_Data data, Transform pos)</para>
/// </summary>
public class Item 
{
    public InventoryItem item;
    public float lastUseTime = -999f;

    private KeyCode itemKey;
    private PlayerUseItemUi itemUi; 
    //public PlayerUseItemUi itemUi;
    public PlayerUseItemUi ItemUi => itemUi;
    private PlayerManager playerManager;
    public KeyCode GetKey() => itemKey;
    /// <summary>
    /// Item 클래스의 생성자. 아이템, 키, UI, 플레이어 매니저를 받아 초기화
    /// </summary>
    public Item(InventoryItem inventoryItem, KeyCode key, PlayerUseItemUi ui, PlayerManager manager) 
    {
        this.item = inventoryItem;
        this.itemKey = key; 
        this.itemUi = ui;   
        this.playerManager = manager;

        if(itemUi != null)
        {
            itemUi.Init(inventoryItem.item_Data.itemSprite, key, inventoryItem);
        }
    }
    /// <summary>
    /// 아이템을 사용할 수 있는 상태인지 확인 (쿨타임 고려)
    /// </summary>
    /// <returns>사용 가능하면 true</returns>
    public bool isReady()
    {
        return Time.time >= lastUseTime + item.item_Data.itemCooldown;
    }
    /// <summary>
    /// 아이템을 실제로 사용하며, 회복 및 이펙트를 실행함
    /// </summary>
    /// <param name="caller">코루틴 실행을 위한 MonoBehaviour 참조</param>
    public virtual void Active(MonoBehaviour caller)
    {
        if (item == null || item.item_Data == null)
        {
            Debug.LogWarning("[Item.Active] item 또는 item_Data가 null입니다.");
            return;
        }

        if (!isReady())
        {
            return; 
        }

        InventoryItem backupItem = item;
        Item_Data backupData = item.item_Data;
        lastUseTime = Time.time;

        if (itemUi != null)
        {
            //var data = item.item_Data;
            /*InventoryItem backupItem = item;
            Item_Data backupData = item.item_Data;
            itemUi.ConsumeItem();*/


            itemUi.ConsumeItem();
            itemUi.StartCooldown(backupData.itemCooldown);
        }
        if (playerManager != null && backupData.isAvailableItem)
        {
             if (backupData.recoveryHP > 0f)
             {
                RecoveryHP(backupData, playerManager);
             }

             if (backupData.recoveryMP > 0f)
             {
                RecoveryMP(backupData, playerManager);
             }

             Transform itemEffectPos = playerManager.transform;
             caller.StartCoroutine(StartItemEffect(backupData, itemEffectPos));
        }
        
        
        
    }
    /// <summary>
    /// 플레이어의 HP를 회복시킴
    /// </summary>
    private void RecoveryHP(Item_Data data , PlayerManager player)
    {
        if(player.isDead)
        {
            return;
        }       

        if (player == null || player.playerData == null)
        {
            Debug.LogWarning("[Item] player 또는 playerStat이 null임");
            return;
        }
        float beforeHP = player.HP; 
        player.HP = Mathf.Min((player.HP + data.recoveryHP), player.MaxHP);
        player.OnChangerStats?.Invoke();
        Debug.Log($"[Item]등록된 아이템을 사용 :{beforeHP} -> {player.HP}");
    }
    /// <summary>
    /// 플레이어의 MP를 회복시킴
    /// </summary>
    private void RecoveryMP(Item_Data data, PlayerManager player)
    {
        if (player.isDead)
        {
            return;
        }

        if (player == null || player.playerData == null)
        {
            Debug.LogWarning("[Item] player 또는 playerStat이 null임");
            return;
        }
        float beforeMP = player.MP;
        player.MP = Mathf.Min((player.MP + data.recoveryMP), player.MaxMP);
        player.OnChangerStats?.Invoke();
        Debug.Log($"[Item]등록된 아이템을 사용 :{beforeMP} -> {player.MP}");
    }
    /// <summary>
    /// 아이템 사용 시 이펙트를 ObjectPoolingManager로부터 스폰하여 재생
    /// </summary>
    private IEnumerator StartItemEffect(Item_Data data, Transform pos)
    {        
        if (data == null || (data.recoveryItemEffect == null && string.IsNullOrEmpty(data.itemPoolKey)))
        {
            Debug.Log($"[Item] 현재 item의 recoveryItemEffect({data.recoveryItemEffect})가 없거나" +
                $" itemPoolKey({data.itemPoolKey})가 없음 확인 바람");
            yield break;
        }
        if(data.startItemEffectDuration > 0f)
        {
            yield return WaitForSecondsCache.Get(data.startItemEffectDuration); 
        }
        Vector3 spawnPos = pos.TransformPoint(data.itemEffectPos);
        Quaternion rotate = Quaternion.LookRotation(playerManager.transform.forward);

        GameObject effect = ObjectPoolingManager.Instance.SpawnFromPool(data.itemPoolKey, spawnPos, rotate); 

        if(effect == null)
        {
            Debug.Log($"[Item]이펙트 생성 실패: {data.itemPoolKey}가 없음");
            yield break;
        }
        else
        {
            Debug.Log($"[Item]이펙트 생성 성공");
            yield return WaitForSecondsCache.Get(data.itemEffectDuration);
            ObjectPoolingManager.Instance.ReturnToPool(data.itemPoolKey, effect);
        }        
    }
}
