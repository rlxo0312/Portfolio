using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어가 단축키로 사용할 아이템 슬롯을 관리하는 클래스
///
/// <para>사용 변수</para>
/// <para>public static PlayerUseItemUiManager Instance</para>
/// <para>public PlayerManagerReference playerManagerRef</para>
/// <para>public List&lt;ItemUseSlot&gt; itemSlot, List&lt;Item&gt; itemList</para>
///
/// <para>사용 메서드</para>
/// <para>public void AssignToUseSlot(InventoryItem item)</para>
/// <para>public void AssignToUseSlotAtIndex(int index, InventoryItem item)</para>
/// <para>public void UseInstantItem(InventoryItem inventoryItem)</para>
/// <para>public void SwapItemList(int indexA, int indexB)</para>
/// <para>public void ForceActivateAllSlotIcons()</para>
/// <para>public void DebugAllSlotImages()</para>
/// </summary>
public class PlayerUseItemUiManager : MonoBehaviour
{
    public static PlayerUseItemUiManager Instance { get; private set; }
    [System.Serializable]
    public class ItemUseSlot
    {
        public InventoryItem inventoryItem;
        public KeyCode keyCode;
        public PlayerUseItemUi playerUseItemUi;
    }
    public PlayerManagerReference playerManagerRef;
    public List<ItemUseSlot> itemSlot; 
    public List<Item> itemList = new List<Item>(); //private   
    /// <summary>
    /// 초기화: Instance 할당 및 슬롯 인덱스와 키코드 설정
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        for (int i = 0; i < itemSlot.Count; i++)
        {
            itemSlot[i].playerUseItemUi.slotIndex = i;
            itemSlot[i].playerUseItemUi.InitKeyCodeOnly(itemSlot[i].keyCode); 
        }
    }
    /// <summary>
    /// Start 시 슬롯 UI 초기화 및 아이템 리스트 구축
    /// </summary>
    private void Start()
    {
        if(playerManagerRef == null)
        {
            playerManagerRef = GetComponent<PlayerManagerReference>();  
        }

        foreach (var slot in itemSlot)
        {
            slot.inventoryItem = null;
            slot.playerUseItemUi?.InitKeyCodeOnly(slot.keyCode);
        }        

        foreach (var slot in itemSlot)       
        {
                var invenItem = slot.inventoryItem;
            //Debug.Log($"[Init Test] 슬롯에 할당된 InventoryItem: {invenItem}");
            //Debug.Log($"[Init Test] 해당 item_Data: {invenItem?.item_Data}");
            //slot.playerUseItemUi?.InitKeyCodeOnly(slot.keyCode); 
            if (invenItem == null || slot.playerUseItemUi == null || slot.inventoryItem.item_Data == null) //|| slot.inventoryItem.item_Data == null
            {
                //Debug.LogWarning("[Init Test] 슬롯 조건에 안 맞아서 Init() 호출 생략됨");
                continue;
            }
            Item itemElement = new Item(invenItem, slot.keyCode, slot.playerUseItemUi, playerManagerRef.PlayerManager);
            itemList.Add(itemElement);

            //slot.playerUseItemUi.keyCodeNameText.text = slot.keyCode.ToString();
            slot.playerUseItemUi.Init(invenItem.item_Data.itemSprite, slot.keyCode, invenItem);
        }

        for (int i = 0; i < itemSlot.Count; i++)
        {
            if (itemSlot[i].playerUseItemUi != null)
            {
                itemSlot[i].playerUseItemUi.slotIndex = i;            
            }
        }
        
    }
    /// <summary>
    /// 입력된 키코드를 감지하여 해당 아이템 사용 시도
    /// </summary>
    private void Update()
    {
        for(int i = 0; i < itemList.Count; i++)        
        {            
            if (i >= itemList.Count)
            {
                break;
            }
            KeyCode key = itemList[i].GetKey();
            if(Input.GetKeyDown(key))
            {
                //Debug.Log($"[InputDetected] Key: {key} 눌림 -> 아이템 사용 시도: {itemList[i].item.item_Data.itemName}");
                UseItem(i);
            }
        }
    }
    /// <summary>
    /// 현재 슬롯의 아이콘을 강제로 모두 활성화함
    /// </summary>
    public void ForceActivateAllSlotIcons()
    {
        for (int i = 0; i < itemSlot.Count; i++)
        {
            var slot = itemSlot[i];
            var ui = slot.playerUseItemUi;

            if (ui != null) 
            {
                ui.displayIcon.gameObject.SetActive(true);
                //ui.displayIcon.enabled = (ui.inventoryItem.item_Data.itemSprite != null);
                //ui.displayIcon.sprite = ui.inventoryItem.item_Data.itemSprite;
                Debug.Log($"[ForceActivateAllSlotIcons] 슬롯 {i} ({ui.name}) → displayIcon 활성화됨");
            }
            else
            {
                Debug.LogWarning($"[ForceActivateAllSlotIcons] 슬롯 {i}의 displayIcon이 null입니다.");
            }
        }
    }
    /// <summary>
    /// 아이템 사용 처리 및 수량에 따른 제거/갱신 처리
    /// </summary>
    /// <param name="index">사용할 아이템이 등록된 itemList의 인덱스</param>
    private void UseItem(int index)
    {
        if(index < 0 || index >= itemList.Count || playerManagerRef.PlayerManager.isPerformingAction)
        {
            return;
        } 

        var item = itemList[index];
        var inventoryItem = item?.item;
        Debug.Log($"[PlayerUseItemUiManager]:{itemList[index]}");

        if (item == null || item.item == null || item.item.item_Data == null)
        {
            return;
        }

        if (item.item.quantity <= 0)
        {
            Debug.LogWarning($"[UseItem] 이미 수량이 0인 아이템입니다. index: {index}");
            return;
        }


        if (item.isReady())
        {
            item.Active(this);
        }

        if(inventoryItem.quantity <= 0)
        {
            /*if (item.ItemUi != null)
            {
                item.ItemUi.ClearSlot();
                item.ItemUi.InitKeyCodeOnly(item.GetKey());
                Debug.Log($"[PlayerUseItemUiManager]UseItem - item.GetKey()({item.GetKey()})");
            }*/

            //inventoryItem.ClearAllLinkedSlots(); 
            UseReturnToPool(inventoryItem);

            if (index < itemList.Count && itemList[index] == item)
            {
                itemList.RemoveAt(index);   
            }

            if (item.ItemUi != null)
            {
                item.ItemUi.ClearSlot();
                item.ItemUi.InitKeyCodeOnly(item.GetKey());
                ForceActivateAllSlotIcons();
                Debug.Log($"[PlayerUseItemUiManager]UseItem - item.GetKey()({item.GetKey()})");
            }
        }
        else
        {
            inventoryItem.RefreshAllLinkedSlots();
        }
        
    }
    /// <summary>
    /// 인벤토리에서 즉시 아이템을 사용할 때 호출되는 함수
    /// </summary>
    /// <param name="inventoryItem">사용할 아이템</param>
    public void UseInstantItem(InventoryItem inventoryItem)
    {
        if (inventoryItem == null || !inventoryItem.IsAvailableItem || inventoryItem.quantity <= 0)
        {
            return;
        }

        Item temp = new Item(inventoryItem, KeyCode.None, null, playerManagerRef.PlayerManager);
        temp.Active(this);
             
        inventoryItem.quantity--;       

        if (inventoryItem.quantity <= 0)
        {
            var copy = new List<IItemLinkedSlot>(inventoryItem.linkedSlots);
            

            for(int i = 0; i < copy.Count; i++)
            {
                var slot = copy[i];
                slot?.ClearSlot();
            }

            inventoryItem.linkedSlots.Clear();

            if (!string.IsNullOrEmpty(inventoryItem.item_Data.itemKey) && inventoryItem.pooledObj != null )
            {
                ObjectPoolingManager.Instance.ReturnToPool(inventoryItem.item_Data.itemKey, inventoryItem.pooledObj);
                inventoryItem.pooledObj = null;
            } 
        }
        else
        {           
            inventoryItem.RefreshAllLinkedSlots();
        }      
    }
    /// <summary>
    /// 현재 슬롯들의 아이콘, 스프라이트 상태를 출력 (디버그용)
    /// </summary>
    public void DebugAllSlotImages()
    {
        for (int i = 0; i < itemSlot.Count; i++)
        {
            var slot = itemSlot[i];
            var ui = slot.playerUseItemUi;

            if (ui != null && ui.displayIcon != null)
            {
                Debug.Log($"[DebugAllSlotImages] 슬롯 {i} ({ui.name}) → displayIcon.name: {ui.displayIcon.name}, " +
                          $"sprite: {ui.displayIcon.sprite?.name ?? "없음"}, " +
                          $"enabled: {ui.displayIcon.enabled}, activeSelf: {ui.displayIcon.gameObject.activeSelf}");
            }
            else
            {
                Debug.LogWarning($"[DebugAllSlotImages] 슬롯 {i}의 displayIcon이 null입니다.");
            }
        }
    }
    /// <summary>
    /// 드래그된 아이템을 사용할 슬롯에 등록
    /// </summary>
    /// <param name="item">등록할 인벤토리 아이템</param>
    /*public void AssignToUseSlot(InventoryItem item)
    {
        if (item == null || item.item_Data == null) //|| item.item_Data == null || !item.item_Data.isStackable
        {
            Debug.Log("[AssignToUseSlot] item 또는 item_Data가 null임");
            return;
        }
        Debug.Log($"[AssignToUseSlot] 시도 중: {item.item_Data.itemName}, 수량: {item.quantity}");
        //Debug.Log($"[AssignToUseSlot] item: {item}, item_Data: {item.item_Data}");       
        for (int i = 0; i < itemSlot.Count; i++)
        {
            var slot = itemSlot[i];
            var existing = slot.inventoryItem;
            if (existing != null && item.IsStackable && slot.inventoryItem.item_Data == item.item_Data)
            {
                int max = existing.item_Data.maxStack;
                int canAdd = Mathf.Min(max - existing.quantity, item.quantity);

                if (canAdd > 0)
                {
                    existing.quantity += canAdd;
                    item.quantity -= canAdd;

                    if (!existing.linkedSlots.Contains(slot.playerUseItemUi))
                    {
                        existing.linkedSlots.Add(slot.playerUseItemUi);
                    }
                    existing.RefreshAllLinkedSlots();
                    item.RefreshAllLinkedSlots();                    
                }
                if (item.quantity <= 0)
                {
                    UseReturnToPool(item);
                }
                return;
            }            
        }
        
        for (int i = 0; i < itemSlot.Count && item.quantity > 0; i++)
        {
            var slot = itemSlot[i];
            Debug.Log($"[AssignToUseSlot] 슬롯 {i} - inventoryItem: {(slot.inventoryItem == null ? "null" : slot.inventoryItem.item_Data.itemName)}");
            if (slot.inventoryItem == null && slot.playerUseItemUi != null)
            {
                Debug.Log($"[AssignToUseSlot] 슬롯 {i}에 아이템 등록 시도");
                item.ClearAllLinkedSlots();

                slot.inventoryItem = item;
                var ui = slot.playerUseItemUi;
                ui.slotIndex = i; //추가한 부분 
                ui.Init(item.item_Data.itemSprite, slot.keyCode, item);


                item.linkedSlots.Add(ui);
                item.RefreshAllLinkedSlots();

                for (int j = 0; j < itemList.Count; j++)
                {
                    if (itemList[j].item == item || itemList[j].GetKey() == slot.keyCode)
                    {
                        itemList.RemoveAt(j);
                        break;
                    }
                }

                itemList.Add(new Item(item, slot.keyCode, ui, playerManagerRef.PlayerManager));              

                return;
            }
        }
        Debug.Log("[PlayerUseItemUiManager] 사용 가능한 슬롯이 없습니다.");
        UseReturnToPool(item);
    }*/
    /// <summary>
    /// 아이템을 풀에 반환하고 연결 슬롯 정보를 초기화
    /// </summary>
    /// <param name="item">반환할 인벤토리 아이템</param>
    private void UseReturnToPool(InventoryItem item)
    {
        if (item == null  || item.item_Data == null)//|| item.pooledObj == null
        {
            return;
        }
        item.ClearAllLinkedSlots();
        if (item.pooledObj != null && !string.IsNullOrEmpty(item.item_Data.itemKey))
        {
            ObjectPoolingManager.Instance.ReturnToPool(item.item_Data.itemKey, item.pooledObj);
            item.pooledObj = null;

            return;
        }
        //item.ClearAllLinkedSlots();
    }
    /// <summary>
    /// 특정 슬롯 인덱스에 아이템을 직접 등록하거나, 동일 아이템이면 수량을 합침
    /// </summary>
    /// <param name="index">대상 슬롯 인덱스</param>
    /// <param name="item">등록할 인벤토리 아이템</param>
    public void AssignToUseSlotAtIndex(int index, InventoryItem item)
    {
        //Debug.Log($"[AssignToUseSlotAtIndex] 시도 중: {item.item_Data.itemName}, 수량:{item.quantity}, 슬롯:{index}, Key:{itemSlot[index].keyCode}");
        /*
                if (index < 0 || index >= itemSlot.Count)
                {
                    Debug.LogWarning("[AssignToUseSlotAtIndex] 잘못된 슬롯 인덱스입니다.");
                    return;
                }

                var slot = itemSlot[index];

                if (slot.inventoryItem != null)
                {
                    Debug.Log($"[AssignToUseSlotAtIndex] 인덱스: {index}, 아이템: {item?.item_Data?.itemName}");
                    Debug.Log($"[AssignToUseSlotAtIndex] 슬롯 UI: {(slot.playerUseItemUi == null ? "NULL" : "존재함")}");

                    if (slot.inventoryItem.item_Data == item.item_Data && slot.inventoryItem.IsStackable)
                    {
                        int maxStack = slot.inventoryItem.item_Data.maxStack;
                        int canAdd = maxStack - slot.inventoryItem.quantity;
                        int moveAmount = Mathf.Min(canAdd, item.quantity);

                        slot.inventoryItem.quantity += moveAmount;
                        item.quantity -= moveAmount;

                        slot.inventoryItem.RefreshAllLinkedSlots();
                        item.RefreshAllLinkedSlots();

                        if (item.quantity <= 0)
                        {
                            Debug.Log("[AssignToUseSlotAtIndex] 모든 수량 이동됨");
                            return;
                        }
                    }
                    else
                    {
                        Debug.Log("[AssignToUseSlotAtIndex] 아이템 교체 중");               
                        slot.inventoryItem.ClearAllLinkedSlots();
                    }
                }

                if (slot.inventoryItem != null)
                {
                    slot.playerUseItemUi?.ClearSlot();
                }

                slot.inventoryItem = item;

                if (slot.playerUseItemUi != null)
                {
                    slot.playerUseItemUi.slotIndex = index;
                    slot.playerUseItemUi.Init(item.item_Data.itemSprite, slot.keyCode, item);            

                    if (!item.linkedSlots.Contains(slot.playerUseItemUi))
                    {
                        item.linkedSlots.Add(slot.playerUseItemUi);
                    }
                }

                item.RefreshAllLinkedSlots();
                itemList.RemoveAll(i => i.item == item);
                itemList.Add(new Item(item, slot.keyCode, slot.playerUseItemUi, playerManagerRef.PlayerManager));
                Debug.Log($"[AssignToUseSlotAtIndex] {item.item_Data.itemName} 등록됨 → 슬롯 {index}, Key: {slot.keyCode}");*/
        if (index < 0 || index >= itemSlot.Count)
        {
            Debug.LogWarning("[AssignToUseSlotAtIndex] 잘못된 슬롯 인덱스입니다.");
            return;
        }

        InventoryItem copiedItem = new InventoryItem(item);
        
        var slot = itemSlot[index];
        /*var slotUi = slot.playerUseItemUi.displayIcon;
        slotUi.gameObject.SetActive(true);*/        
        if (slot.inventoryItem != null &&
            slot.inventoryItem.item_Data == copiedItem.item_Data &&
            slot.inventoryItem.IsStackable)
        {
            int maxStack = slot.inventoryItem.item_Data.maxStack;
            int canAdd = maxStack - slot.inventoryItem.quantity;
            int moveAmount = Mathf.Min(canAdd, copiedItem.quantity);

            slot.inventoryItem.quantity += moveAmount;
            copiedItem.quantity -= moveAmount;

            slot.inventoryItem.RefreshAllLinkedSlots();
            ForceActivateAllSlotIcons();
            if (copiedItem.quantity <= 0)
            {
                Debug.Log("[AssignToUseSlotAtIndex] 모든 수량 이동됨");
                return;
            }
        }
        else if (slot.inventoryItem != null)
        {
            slot.inventoryItem.ClearAllLinkedSlots();
            slot.playerUseItemUi?.ClearSlot();
        }

        slot.inventoryItem = copiedItem;

        if (slot.playerUseItemUi != null)
        {
            slot.playerUseItemUi.slotIndex = index;
            slot.playerUseItemUi.Init(copiedItem.item_Data.itemSprite, slot.keyCode, copiedItem);

            if (slot.playerUseItemUi.displayIcon != null) 
    {
                slot.playerUseItemUi.displayIcon.sprite = copiedItem.item_Data.itemSprite;
                slot.playerUseItemUi.displayIcon.enabled = true;
            }

            if(slot.playerUseItemUi?.keyCodeNameText != null)
            {
                slot.playerUseItemUi.keyCodeNameText.text = slot.keyCode.ToString();
                //slot.playerUseItemUi.keyCodeNameText.SetText(slot.keyCode.ToString());
            }

            if (!copiedItem.linkedSlots.Contains(slot.playerUseItemUi))
            {
                copiedItem.linkedSlots.Add(slot.playerUseItemUi);
            }
        }

        copiedItem.RefreshAllLinkedSlots();
        itemList.RemoveAll(i => i.item == copiedItem);
        itemList.Add(new Item(copiedItem, slot.keyCode, slot.playerUseItemUi, playerManagerRef.PlayerManager));
        ForceActivateAllSlotIcons();
        Debug.Log($"[AssignToUseSlotAtIndex] {copiedItem.item_Data.itemName} 등록됨 → 슬롯 {index}, Key: {slot.keyCode}");

    }
    /// <summary>
    /// 두 슬롯 간의 아이템과 UI 상태를 교환
    /// </summary>
    /// <param name="indexA">첫 번째 슬롯 인덱스</param>
    /// <param name="indexB">두 번째 슬롯 인덱스</param>
    public void SwapItemList(int indexA, int indexB)
    {
        if(indexA < 0 || indexA > itemSlot.Count || indexB < 0 || indexB > itemSlot.Count)
        {
            return; 
        }

        var slotA = itemSlot[indexA];
        var slotB = itemSlot[indexB];

        var itemA = slotA.inventoryItem;
        var itemB = slotB.inventoryItem;
        
        slotA.playerUseItemUi?.ClearSlot();
        slotB.playerUseItemUi?.ClearSlot();
       
        slotA.inventoryItem = itemB;
        slotB.inventoryItem = itemA;

        if (slotA.playerUseItemUi != null && itemB != null)
        {
            slotA.playerUseItemUi.slotIndex = indexA;
            slotA.playerUseItemUi.Init(itemB.item_Data.itemSprite, slotA.keyCode, itemB);
        }

        if (slotB.playerUseItemUi != null && itemA != null)
        {
            slotB.playerUseItemUi.slotIndex = indexB;
            slotB.playerUseItemUi.Init(itemA.item_Data.itemSprite, slotB.keyCode, itemA);
        }
       
        itemList.RemoveAll(i => i.ItemUi.slotIndex == indexA || i.ItemUi.slotIndex == indexB);

        if (itemB != null)
        {
            itemList.Add(new Item(itemB, slotA.keyCode, slotA.playerUseItemUi, playerManagerRef.PlayerManager));
        }
        if (itemA != null)
        {
            itemList.Add(new Item(itemA, slotB.keyCode, slotB.playerUseItemUi, playerManagerRef.PlayerManager));
        }

        itemA?.RefreshAllLinkedSlots();
        itemB?.RefreshAllLinkedSlots();

        Debug.Log($"[SwapItemList] 슬롯 {indexA} ↔ {indexB} 교환 완료");

    }
}
