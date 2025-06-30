using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ��� ������ �κ��丮 �������� ������ �����ϰ�, �÷��̾��� HP/MP ȸ�� �� ������ ����Ʈ ���� �� ���� ������ ó���ϴ� Ŭ����
///
/// <para>��� ����</para>
/// <para>public InventoryItem item, public float lastUseTime</para>
/// <para>private KeyCode itemKey, private PlayerUseItemUi itemUi, private PlayerManager playerManager</para>
/// <para>public PlayerUseItemUi ItemUi, public KeyCode GetKey()</para>
///
/// <para>��� �޼���</para>
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
    /// Item Ŭ������ ������. ������, Ű, UI, �÷��̾� �Ŵ����� �޾� �ʱ�ȭ
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
    /// �������� ����� �� �ִ� �������� Ȯ�� (��Ÿ�� ���)
    /// </summary>
    /// <returns>��� �����ϸ� true</returns>
    public bool isReady()
    {
        return Time.time >= lastUseTime + item.item_Data.itemCooldown;
    }
    /// <summary>
    /// �������� ������ ����ϸ�, ȸ�� �� ����Ʈ�� ������
    /// </summary>
    /// <param name="caller">�ڷ�ƾ ������ ���� MonoBehaviour ����</param>
    public virtual void Active(MonoBehaviour caller)
    {
        if (item == null || item.item_Data == null)
        {
            Debug.LogWarning("[Item.Active] item �Ǵ� item_Data�� null�Դϴ�.");
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
    /// �÷��̾��� HP�� ȸ����Ŵ
    /// </summary>
    private void RecoveryHP(Item_Data data , PlayerManager player)
    {
        if(player.isDead)
        {
            return;
        }       

        if (player == null || player.playerData == null)
        {
            Debug.LogWarning("[Item] player �Ǵ� playerStat�� null��");
            return;
        }
        float beforeHP = player.HP; 
        player.HP = Mathf.Min((player.HP + data.recoveryHP), player.MaxHP);
        player.OnChangerStats?.Invoke();
        Debug.Log($"[Item]��ϵ� �������� ��� :{beforeHP} -> {player.HP}");
    }
    /// <summary>
    /// �÷��̾��� MP�� ȸ����Ŵ
    /// </summary>
    private void RecoveryMP(Item_Data data, PlayerManager player)
    {
        if (player.isDead)
        {
            return;
        }

        if (player == null || player.playerData == null)
        {
            Debug.LogWarning("[Item] player �Ǵ� playerStat�� null��");
            return;
        }
        float beforeMP = player.MP;
        player.MP = Mathf.Min((player.MP + data.recoveryMP), player.MaxMP);
        player.OnChangerStats?.Invoke();
        Debug.Log($"[Item]��ϵ� �������� ��� :{beforeMP} -> {player.MP}");
    }
    /// <summary>
    /// ������ ��� �� ����Ʈ�� ObjectPoolingManager�κ��� �����Ͽ� ���
    /// </summary>
    private IEnumerator StartItemEffect(Item_Data data, Transform pos)
    {        
        if (data == null || (data.recoveryItemEffect == null && string.IsNullOrEmpty(data.itemPoolKey)))
        {
            Debug.Log($"[Item] ���� item�� recoveryItemEffect({data.recoveryItemEffect})�� ���ų�" +
                $" itemPoolKey({data.itemPoolKey})�� ���� Ȯ�� �ٶ�");
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
            Debug.Log($"[Item]����Ʈ ���� ����: {data.itemPoolKey}�� ����");
            yield break;
        }
        else
        {
            Debug.Log($"[Item]����Ʈ ���� ����");
            yield return WaitForSecondsCache.Get(data.itemEffectDuration);
            ObjectPoolingManager.Instance.ReturnToPool(data.itemPoolKey, effect);
        }        
    }
}
