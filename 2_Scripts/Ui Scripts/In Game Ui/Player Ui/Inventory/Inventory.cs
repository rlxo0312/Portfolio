using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �÷��̾��� �κ��丮�� �����ϴ� Ŭ����
/// <para>��뺯�� : maxSlot, instance(�̱��� �ν��Ͻ�, Inventory), inventorySlotManager(Ŭ����, InventorySlotManager)</para>
/// <para>��� �ż��� : public bool AddItem(Item_Data item, GameObject poolObj)</para>
/// </summary>
public class Inventory : MonoBehaviour
{
    //public List<Item_Data> items = new List<Item_Data>();
    public int maxSlot = 16;
    public InventorySlotManager inventorySlotManager;
    public static Inventory instance { get; private set; }
    
    public void Awake()
    {
        instance = this; 
    }
    /// <summary>
    /// �������� �κ��丮�� �߰��ϴ� �ż���
    /// </summary>
    /// <param name="item">�߰��� ������ ������(Item_Data)</param>
    /// <param name="poolObj">�߰� �� ������ ������</param>
    /// <returns></returns>
    public bool AddItem(Item_Data item, GameObject poolObj, int amount = 1)
    {
        //Debug.Log($"[AddItem] ������: {item.itemName}, ����: {amount}");
        if (inventorySlotManager == null)
        {
            Debug.Log("[Inventory] slotManager�� ����Ǿ� ���� �ʽ��ϴ�.");
        }        
        return inventorySlotManager.AddItemSlots(item, poolObj, amount);       
    }    
}
