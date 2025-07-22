using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 플레이어의 인벤토리를 관리하는 클래스
/// <para>사용변수 : maxSlot, instance(싱글톤 인스턴스, Inventory), inventorySlotManager(클래스, InventorySlotManager)</para>
/// <para>사용 매서드 : public bool AddItem(Item_Data item, GameObject poolObj)</para>
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
    /// 아이템을 인벤토리에 추가하는 매서드
    /// </summary>
    /// <param name="item">추가할 아이템 데이터(Item_Data)</param>
    /// <param name="poolObj">추가 될 아이템 프리팹</param>
    /// <returns></returns>
    public bool AddItem(Item_Data item, GameObject poolObj, int amount = 1)
    {
        //Debug.Log($"[AddItem] 아이템: {item.itemName}, 수량: {amount}");
        if (inventorySlotManager == null)
        {
            Debug.Log("[Inventory] slotManager가 연결되어 있지 않습니다.");
        }        
        return inventorySlotManager.AddItemSlots(item, poolObj, amount);       
    }    
}
