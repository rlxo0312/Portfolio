using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �÷��̾ ����Ű�� ����� ������ ������ �����ϴ� Ŭ����
///
/// <para>��� ����</para>
/// <para>public static PlayerUseItemUiManager Instance</para>
/// <para>public PlayerManagerReference playerManagerRef</para>
/// <para>public List&lt;ItemUseSlot&gt; itemSlot, List&lt;Item&gt; itemList</para>
///
/// <para>��� �޼���</para>
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
    /// �ʱ�ȭ: Instance �Ҵ� �� ���� �ε����� Ű�ڵ� ����
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
    /// Start �� ���� UI �ʱ�ȭ �� ������ ����Ʈ ����
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
            //Debug.Log($"[Init Test] ���Կ� �Ҵ�� InventoryItem: {invenItem}");
            //Debug.Log($"[Init Test] �ش� item_Data: {invenItem?.item_Data}");
            //slot.playerUseItemUi?.InitKeyCodeOnly(slot.keyCode); 
            if (invenItem == null || slot.playerUseItemUi == null || slot.inventoryItem.item_Data == null) //|| slot.inventoryItem.item_Data == null
            {
                //Debug.LogWarning("[Init Test] ���� ���ǿ� �� �¾Ƽ� Init() ȣ�� ������");
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
    /// �Էµ� Ű�ڵ带 �����Ͽ� �ش� ������ ��� �õ�
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
                //Debug.Log($"[InputDetected] Key: {key} ���� -> ������ ��� �õ�: {itemList[i].item.item_Data.itemName}");
                UseItem(i);
            }
        }
    }
    /// <summary>
    /// ���� ������ �������� ������ ��� Ȱ��ȭ��
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
                Debug.Log($"[ForceActivateAllSlotIcons] ���� {i} ({ui.name}) �� displayIcon Ȱ��ȭ��");
            }
            else
            {
                Debug.LogWarning($"[ForceActivateAllSlotIcons] ���� {i}�� displayIcon�� null�Դϴ�.");
            }
        }
    }
    /// <summary>
    /// ������ ��� ó�� �� ������ ���� ����/���� ó��
    /// </summary>
    /// <param name="index">����� �������� ��ϵ� itemList�� �ε���</param>
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
            Debug.LogWarning($"[UseItem] �̹� ������ 0�� �������Դϴ�. index: {index}");
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
    /// �κ��丮���� ��� �������� ����� �� ȣ��Ǵ� �Լ�
    /// </summary>
    /// <param name="inventoryItem">����� ������</param>
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
    /// ���� ���Ե��� ������, ��������Ʈ ���¸� ��� (����׿�)
    /// </summary>
    public void DebugAllSlotImages()
    {
        for (int i = 0; i < itemSlot.Count; i++)
        {
            var slot = itemSlot[i];
            var ui = slot.playerUseItemUi;

            if (ui != null && ui.displayIcon != null)
            {
                Debug.Log($"[DebugAllSlotImages] ���� {i} ({ui.name}) �� displayIcon.name: {ui.displayIcon.name}, " +
                          $"sprite: {ui.displayIcon.sprite?.name ?? "����"}, " +
                          $"enabled: {ui.displayIcon.enabled}, activeSelf: {ui.displayIcon.gameObject.activeSelf}");
            }
            else
            {
                Debug.LogWarning($"[DebugAllSlotImages] ���� {i}�� displayIcon�� null�Դϴ�.");
            }
        }
    }
    /// <summary>
    /// �巡�׵� �������� ����� ���Կ� ���
    /// </summary>
    /// <param name="item">����� �κ��丮 ������</param>
    /*public void AssignToUseSlot(InventoryItem item)
    {
        if (item == null || item.item_Data == null) //|| item.item_Data == null || !item.item_Data.isStackable
        {
            Debug.Log("[AssignToUseSlot] item �Ǵ� item_Data�� null��");
            return;
        }
        Debug.Log($"[AssignToUseSlot] �õ� ��: {item.item_Data.itemName}, ����: {item.quantity}");
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
            Debug.Log($"[AssignToUseSlot] ���� {i} - inventoryItem: {(slot.inventoryItem == null ? "null" : slot.inventoryItem.item_Data.itemName)}");
            if (slot.inventoryItem == null && slot.playerUseItemUi != null)
            {
                Debug.Log($"[AssignToUseSlot] ���� {i}�� ������ ��� �õ�");
                item.ClearAllLinkedSlots();

                slot.inventoryItem = item;
                var ui = slot.playerUseItemUi;
                ui.slotIndex = i; //�߰��� �κ� 
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
        Debug.Log("[PlayerUseItemUiManager] ��� ������ ������ �����ϴ�.");
        UseReturnToPool(item);
    }*/
    /// <summary>
    /// �������� Ǯ�� ��ȯ�ϰ� ���� ���� ������ �ʱ�ȭ
    /// </summary>
    /// <param name="item">��ȯ�� �κ��丮 ������</param>
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
    /// Ư�� ���� �ε����� �������� ���� ����ϰų�, ���� �������̸� ������ ��ħ
    /// </summary>
    /// <param name="index">��� ���� �ε���</param>
    /// <param name="item">����� �κ��丮 ������</param>
    public void AssignToUseSlotAtIndex(int index, InventoryItem item)
    {
        //Debug.Log($"[AssignToUseSlotAtIndex] �õ� ��: {item.item_Data.itemName}, ����:{item.quantity}, ����:{index}, Key:{itemSlot[index].keyCode}");
        /*
                if (index < 0 || index >= itemSlot.Count)
                {
                    Debug.LogWarning("[AssignToUseSlotAtIndex] �߸��� ���� �ε����Դϴ�.");
                    return;
                }

                var slot = itemSlot[index];

                if (slot.inventoryItem != null)
                {
                    Debug.Log($"[AssignToUseSlotAtIndex] �ε���: {index}, ������: {item?.item_Data?.itemName}");
                    Debug.Log($"[AssignToUseSlotAtIndex] ���� UI: {(slot.playerUseItemUi == null ? "NULL" : "������")}");

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
                            Debug.Log("[AssignToUseSlotAtIndex] ��� ���� �̵���");
                            return;
                        }
                    }
                    else
                    {
                        Debug.Log("[AssignToUseSlotAtIndex] ������ ��ü ��");               
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
                Debug.Log($"[AssignToUseSlotAtIndex] {item.item_Data.itemName} ��ϵ� �� ���� {index}, Key: {slot.keyCode}");*/
        if (index < 0 || index >= itemSlot.Count)
        {
            Debug.LogWarning("[AssignToUseSlotAtIndex] �߸��� ���� �ε����Դϴ�.");
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
                Debug.Log("[AssignToUseSlotAtIndex] ��� ���� �̵���");
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
        Debug.Log($"[AssignToUseSlotAtIndex] {copiedItem.item_Data.itemName} ��ϵ� �� ���� {index}, Key: {slot.keyCode}");

    }
    /// <summary>
    /// �� ���� ���� �����۰� UI ���¸� ��ȯ
    /// </summary>
    /// <param name="indexA">ù ��° ���� �ε���</param>
    /// <param name="indexB">�� ��° ���� �ε���</param>
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

        Debug.Log($"[SwapItemList] ���� {indexA} �� {indexB} ��ȯ �Ϸ�");

    }
}
