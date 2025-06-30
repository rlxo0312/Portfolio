using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

/// <summary>
/// 인벤토리에 저장되는 아이템 정보 클래스.
/// 아이템 데이터와 수량, 슬롯 연결 정보 및 풀링된 오브젝트를 함께 관리함.
///
/// <para>사용 변수</para>
/// <para>public Item_Data item_Data, public int quantity</para>
/// <para>public float cooldownTimer, cooldownTime, isCooldown</para>
/// <para>public List&lt;IItemLinkedSlot&gt; linkedSlots, public GameObject pooledObj</para>
/// <para>public bool IsStackable, public bool IsAvailableItem</para>
///
/// <para>사용 메서드</para>
/// <para>public InventoryItem(Item_Data data, int amount = 1)</para>
/// <para>public InventoryItem(InventoryItem other)</para>
/// <para>public void RefreshAllLinkedSlots()</para>
/// <para>public void ClearAllLinkedSlots(PlayerUseItemUi exceptSlot = null)</para>
/// </summary>
[System.Serializable]
public class InventoryItem 
{
    public Item_Data item_Data;
    public int quantity;
    public List<IItemLinkedSlot> linkedSlots = new List<IItemLinkedSlot>();
    public GameObject pooledObj;

    public float cooldownTimer;
    public float cooldownTime;
    public bool isCooldown;
    /// <summary>
    /// 인벤토리 아이템 생성자 (기본)
    /// </summary>
    /// <param name="data">기준이 되는 아이템 데이터</param>
    /// <param name="amount">초기 수량 (기본값 1)</param>
    public InventoryItem(Item_Data data, int amount = 1)
    {
        item_Data = data;
        quantity = Mathf.Clamp(amount, 1, data.maxStack);
    }
    /// <summary>
    /// 인벤토리 아이템 복사 생성자
    /// </summary>
    /// <param name="other">복사할 인스턴스</param>
    public InventoryItem(InventoryItem other)
    {
        if (other == null)
        {
            Debug.LogError("[InventoryItem] 복사 생성자에서 other가 null");
            return;
        }

        if (other.item_Data == null)
        {
            Debug.LogError("[InventoryItem] 복사 생성자에서 item_Data가 null");
            return;
        }

        item_Data = other.item_Data;
        quantity = other.quantity;
        pooledObj = other.pooledObj;        
        linkedSlots = new List<IItemLinkedSlot>();
    }   
    /// <summary>
    /// 연결된 모든 슬롯을 새로고침 (UI 업데이트)
    /// null 참조는 자동 제거됨
    /// </summary>
    public void RefreshAllLinkedSlots()
    {

        for (int i = linkedSlots.Count - 1; i >= 0; i--)
        //for (int i = 0; i< linkedSlots.Count; i++)
        {
            if (linkedSlots[i] == null)
            {
                linkedSlots.RemoveAt(i); // null 참조 제거
            }
            else
            {
                if(quantity > 0)
                {
                    linkedSlots[i].RefreshPlayerUseItemUI();
                }
            }
        }
    }
    /// <summary>
    /// 연결된 모든 슬롯을 제거하고, UI를 초기화함
    /// 특정 슬롯은 제외할 수 있음
    /// </summary>
    /// <param name="exceptSlot">제외할 슬롯 (선택)</param>
    public void ClearAllLinkedSlots(PlayerUseItemUi exceptSlot = null)
    {
        
        if(linkedSlots == null || linkedSlots.Count == 0)
        {
            return;
        }

        List<IItemLinkedSlot> copy = new List<IItemLinkedSlot>(linkedSlots);
        linkedSlots.Clear();
        Debug.Log($"[ClearAllLinkedSlots] 실행 - 제외 대상: {exceptSlot?.name}");
        for (int i = copy.Count - 1; i >= 0; i--)
        //for(int i = 0; i < copy.Count; i++)
        {            
            var slot = copy[i];
            if (slot == null || object.ReferenceEquals(slot, exceptSlot))
            {
                continue;
            }
            Debug.Log($"[ClearAllLinkedSlots] -> 지우는 슬롯: {slot}");
            if (slot is PlayerUseItemUi uiSlot) // && uiSlot.item != null
            {                
                Debug.Log($"[ClearAllLinkedSlots] -> 실제 ClearSlot 호출: {uiSlot.name}");
                uiSlot.ClearSlot();
            }
            //linkedSlots.Clear();
        }        
    }     
    /// <summary>
    /// 해당 아이템이 스택 가능한지 여부
    /// </summary>
    public bool IsStackable => item_Data.isStackable;
    /// <summary>
    /// 해당 아이템이 사용 가능한 아이템인지 여부
    /// </summary>
    public bool IsAvailableItem => item_Data.isAvailableItem;
}
