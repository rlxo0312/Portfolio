using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 이벤트 슬롯들을 관리하고, 아이템 추가 로직을 처리하는 클래스 
/// <para>사용 변수</para>
/// <para>public List&lt;InventorySlot&gt; slotList</para>
/// <para>사용 메서드</para>
/// <para>public void LoadSlots()</para>
/// <para>public bool AddItemSlots(Item_Data item_Data, GameObject poolObj), HasEmptySlot()</para>
/// <para>public InventorySlot FindStackableSlot(Item_Data data),  GetEmptySlot()</para>
/// <para>private void LogEmptySlotCount()</para>
/// </summary>
public class InventorySlotManager : MonoBehaviour
{
    public List<InventorySlot> slotList = new List<InventorySlot>();


    /// <summary>
    /// 동일한 종류의 아이템을 스택할 수 있는 슬롯을 찾음
    /// </summary>
    /// <param name="data">찾을 아이템 데이터</param>
    /// <returns>스택 가능한 슬롯 또는 null</returns>
    public InventorySlot FindStackableSlot(Item_Data data)
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            var slot = slotList[i];
            var slots = slot.item;
            if (slots != null && slots.item_Data == data 
                && slots.quantity < slots.item_Data.maxStack)
            {
                return slot;
            }
        }
        return null;
    }

    /// <summary>
    /// 비어 있는 슬롯을 반환
    /// </summary>
    /// <returns>비어 있는 슬롯 또는 null</returns>
    public InventorySlot GetEmptySlot()
    {
        for(int i = 0; i < slotList.Count; i++)
        {
            var slot = slotList[i];
            if(slot.IsEmpty)
            {
                return slot;
            }
        }
        return null;
    }
    private void Start()
    {
        LoadSlots();
    }
    /// <summary>
    /// 슬롯을 Hierarchy에서 자동으로 찾아 리스트로 구성
    /// </summary>
    public void LoadSlots()
    {        
        slotList.Clear();        
        /*foreach (Transform slotContainer in transform) // Slot GameObject들
        {
            InventorySlot slot = slotContainer.GetComponentInChildren<InventorySlot>(); // InternalSlot에서 찾음
            if (slot != null)
            {
                slotList.Add(slot);
                //ebug.Log($"[InventorySlotManager] 슬롯 추가됨: {slot.name}");
            }
            else
            {
                Debug.LogWarning($"[InventorySlotManager] {slotContainer.name} 에 InventorySlot이 없습니다.");
            }
        }*/
        int childCount = transform.childCount; 
        for(int i = 0; i < childCount; i++) 
        {
            Transform slotContainer = transform.GetChild(i); 
            InventorySlot slot = slotContainer.GetComponentInChildren<InventorySlot>();
            if (slot != null)
            {
                slotList.Add(slot);
            }
            else
            {
                Debug.LogWarning($"[InventorySlotManager] {slotContainer.name} 에 InventorySlot이 없습니다.");
            }
        }
        //Debug.Log($"[InventorySlotManager] 슬롯 수: {slotList.Count}");
    }
    /// <summary>
    /// 슬롯에 아이템을 추가
    /// </summary>
    /// <param name="item_Data">아이템 정보</param>
    /// <param name="poolObj">풀에서 가져온 게임 오브젝트</param>
    /// <returns>아이템 추가 성공 여부</returns>
    public bool AddItemSlots(Item_Data item_Data, GameObject poolObj, int quantitiy) //,int amount = 1
    {
        /*if(slotList.Count == 0)
        {
            return false;
        }*/
       
        var stackableSlot = FindStackableSlot(item_Data);
        if (stackableSlot != null)
        {
            stackableSlot.item.quantity += quantitiy;
            //stackableSlot.item.quantity += amount;
            stackableSlot.item.RefreshAllLinkedSlots(); 
            return true;    
        }
        var emptySlot = GetEmptySlot();
        if (emptySlot != null)
        {
            InventoryItem newItem = new InventoryItem(item_Data, quantitiy);
            newItem.pooledObj = poolObj;
            emptySlot.AssignItem(newItem);
            return true;
        }

        return false;       
    }    
    /// <summary>
    /// 현재 인벤토리에서 비어 있는 슬롯의 개수를 계산하여 로그로 출력함
    /// </summary>
    private void LogEmptySlotCount()
    {
        if (slotList.Count == 0)
        {
            return;
        }
        int emptyCount = 0;
        
        for(int i = 0; i < slotList.Count; i++)
        {
            var slot = slotList[i];
            if(slot.IsEmpty)
            {
                emptyCount++;
            }
        }
        Debug.Log($"[InventorySlotManager] 현재 비어 있는 슬롯 수: {emptyCount}");
    }

    /// <summary>
    /// 비어 있는 슬롯이 존재하는지 여부 확인
    /// </summary>
    /// <returns>하나라도 비어 있는 슬롯이 있으면 true</returns>
    public bool HasEmptySlot()
    {
        for(int i = 0; i < slotList.Count; i++)
        {
            var slot = slotList[i];
            if(slot.IsEmpty)
            {
                return true;
            }
        }
        return false;
    }
}
