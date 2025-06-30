using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �̺�Ʈ ���Ե��� �����ϰ�, ������ �߰� ������ ó���ϴ� Ŭ���� 
/// <para>��� ����</para>
/// <para>public List&lt;InventorySlot&gt; slotList</para>
/// <para>��� �޼���</para>
/// <para>public void LoadSlots()</para>
/// <para>public bool AddItemSlots(Item_Data item_Data, GameObject poolObj), HasEmptySlot()</para>
/// <para>public InventorySlot FindStackableSlot(Item_Data data),  GetEmptySlot()</para>
/// <para>private void LogEmptySlotCount()</para>
/// </summary>
public class InventorySlotManager : MonoBehaviour
{
    public List<InventorySlot> slotList = new List<InventorySlot>();


    /// <summary>
    /// ������ ������ �������� ������ �� �ִ� ������ ã��
    /// </summary>
    /// <param name="data">ã�� ������ ������</param>
    /// <returns>���� ������ ���� �Ǵ� null</returns>
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
    /// ��� �ִ� ������ ��ȯ
    /// </summary>
    /// <returns>��� �ִ� ���� �Ǵ� null</returns>
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
    /// ������ Hierarchy���� �ڵ����� ã�� ����Ʈ�� ����
    /// </summary>
    public void LoadSlots()
    {        
        slotList.Clear();        
        /*foreach (Transform slotContainer in transform) // Slot GameObject��
        {
            InventorySlot slot = slotContainer.GetComponentInChildren<InventorySlot>(); // InternalSlot���� ã��
            if (slot != null)
            {
                slotList.Add(slot);
                //ebug.Log($"[InventorySlotManager] ���� �߰���: {slot.name}");
            }
            else
            {
                Debug.LogWarning($"[InventorySlotManager] {slotContainer.name} �� InventorySlot�� �����ϴ�.");
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
                Debug.LogWarning($"[InventorySlotManager] {slotContainer.name} �� InventorySlot�� �����ϴ�.");
            }
        }
        //Debug.Log($"[InventorySlotManager] ���� ��: {slotList.Count}");
    }
    /// <summary>
    /// ���Կ� �������� �߰�
    /// </summary>
    /// <param name="item_Data">������ ����</param>
    /// <param name="poolObj">Ǯ���� ������ ���� ������Ʈ</param>
    /// <returns>������ �߰� ���� ����</returns>
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
    /// ���� �κ��丮���� ��� �ִ� ������ ������ ����Ͽ� �α׷� �����
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
        Debug.Log($"[InventorySlotManager] ���� ��� �ִ� ���� ��: {emptyCount}");
    }

    /// <summary>
    /// ��� �ִ� ������ �����ϴ��� ���� Ȯ��
    /// </summary>
    /// <returns>�ϳ��� ��� �ִ� ������ ������ true</returns>
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
