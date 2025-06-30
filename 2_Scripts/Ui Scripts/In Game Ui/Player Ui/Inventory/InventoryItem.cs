using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

/// <summary>
/// �κ��丮�� ����Ǵ� ������ ���� Ŭ����.
/// ������ �����Ϳ� ����, ���� ���� ���� �� Ǯ���� ������Ʈ�� �Բ� ������.
///
/// <para>��� ����</para>
/// <para>public Item_Data item_Data, public int quantity</para>
/// <para>public float cooldownTimer, cooldownTime, isCooldown</para>
/// <para>public List&lt;IItemLinkedSlot&gt; linkedSlots, public GameObject pooledObj</para>
/// <para>public bool IsStackable, public bool IsAvailableItem</para>
///
/// <para>��� �޼���</para>
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
    /// �κ��丮 ������ ������ (�⺻)
    /// </summary>
    /// <param name="data">������ �Ǵ� ������ ������</param>
    /// <param name="amount">�ʱ� ���� (�⺻�� 1)</param>
    public InventoryItem(Item_Data data, int amount = 1)
    {
        item_Data = data;
        quantity = Mathf.Clamp(amount, 1, data.maxStack);
    }
    /// <summary>
    /// �κ��丮 ������ ���� ������
    /// </summary>
    /// <param name="other">������ �ν��Ͻ�</param>
    public InventoryItem(InventoryItem other)
    {
        if (other == null)
        {
            Debug.LogError("[InventoryItem] ���� �����ڿ��� other�� null");
            return;
        }

        if (other.item_Data == null)
        {
            Debug.LogError("[InventoryItem] ���� �����ڿ��� item_Data�� null");
            return;
        }

        item_Data = other.item_Data;
        quantity = other.quantity;
        pooledObj = other.pooledObj;        
        linkedSlots = new List<IItemLinkedSlot>();
    }   
    /// <summary>
    /// ����� ��� ������ ���ΰ�ħ (UI ������Ʈ)
    /// null ������ �ڵ� ���ŵ�
    /// </summary>
    public void RefreshAllLinkedSlots()
    {

        for (int i = linkedSlots.Count - 1; i >= 0; i--)
        //for (int i = 0; i< linkedSlots.Count; i++)
        {
            if (linkedSlots[i] == null)
            {
                linkedSlots.RemoveAt(i); // null ���� ����
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
    /// ����� ��� ������ �����ϰ�, UI�� �ʱ�ȭ��
    /// Ư�� ������ ������ �� ����
    /// </summary>
    /// <param name="exceptSlot">������ ���� (����)</param>
    public void ClearAllLinkedSlots(PlayerUseItemUi exceptSlot = null)
    {
        
        if(linkedSlots == null || linkedSlots.Count == 0)
        {
            return;
        }

        List<IItemLinkedSlot> copy = new List<IItemLinkedSlot>(linkedSlots);
        linkedSlots.Clear();
        Debug.Log($"[ClearAllLinkedSlots] ���� - ���� ���: {exceptSlot?.name}");
        for (int i = copy.Count - 1; i >= 0; i--)
        //for(int i = 0; i < copy.Count; i++)
        {            
            var slot = copy[i];
            if (slot == null || object.ReferenceEquals(slot, exceptSlot))
            {
                continue;
            }
            Debug.Log($"[ClearAllLinkedSlots] -> ����� ����: {slot}");
            if (slot is PlayerUseItemUi uiSlot) // && uiSlot.item != null
            {                
                Debug.Log($"[ClearAllLinkedSlots] -> ���� ClearSlot ȣ��: {uiSlot.name}");
                uiSlot.ClearSlot();
            }
            //linkedSlots.Clear();
        }        
    }     
    /// <summary>
    /// �ش� �������� ���� �������� ����
    /// </summary>
    public bool IsStackable => item_Data.isStackable;
    /// <summary>
    /// �ش� �������� ��� ������ ���������� ����
    /// </summary>
    public bool IsAvailableItem => item_Data.isAvailableItem;
}
