using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

/// <summary>
/// �÷��̾ ����Ű ���Կ��� ����ϴ� ������ UI ���� Ŭ����
///
/// <para>��� ����</para>
/// <para>public TextMeshProUGUI itemCountText</para>
/// <para>public InventoryItem inventoryItem</para>
/// <para>public int slotIndex</para>
/// <para>private static GameObject dragIcon, PlayerUseItemUi draggedSlot</para>
///
/// <para>��� �޼���</para>
/// <para>public void Init(Sprite sprite, KeyCode key, InventoryItem newItem)</para>
/// <para>public void InitKeyCodeOnly(KeyCode key)</para>
/// <para>public void ClearSlot()</para>
/// <para>public void ConsumeItem()</para>
/// <para>public void RefreshPlayerUseItemUI()</para>
/// <para>public void OnPointerClick(PointerEventData eventData), public void OnBeginDrag(PointerEventData eventData), public void OnEndDrag(PointerEventData eventData)
///  ,public void OnDrop(PointerEventData eventData), public void OnDrag(PointerEventData eventData)</para>
/// </summary>
public class PlayerUseItemUi : PlayerInGameSlotUi, IPointerEnterHandler, IPointerExitHandler, IItemLinkedSlot, IDropHandler
                                                 , IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    
    public TextMeshProUGUI itemCountText;
    public InventoryItem inventoryItem;
    public int slotIndex = -1;
    [SerializeField] private GameObject dragIconInstance; 
    private static GameObject dragIcon;
    private static PlayerUseItemUi draggedSlot;

    /// <summary>
    /// ������Ʈ�� �ε�� �� ȣ��
    /// dragIconInstance�� �����Ǿ� �ִٸ� static dragIcon���� �ʱ�ȭ
    /// </summary>
    private void Awake()
    {
        
        if (dragIcon == null && dragIconInstance != null)
        {
            dragIcon = dragIconInstance;
        }        
    }
    /// <summary>
    /// ���� ������Ʈ�� Ȱ��ȭ�� �� ���ʷ� ȣ��
    /// ���� �ε����� �ڵ����� ����Ͽ� ����
    /// </summary>
    private void Start()
    {
        for (int i = 0; i < PlayerUseItemUiManager.Instance.itemSlot.Count; i++)
        {
            var slot = PlayerUseItemUiManager.Instance.itemSlot[i].playerUseItemUi;
            if (slot == this)
            {
                slotIndex = i;                
                //Debug.Log($"[AutoSlotIndex] {gameObject.name} �� ���� �ε���: {slotIndex}");
                break;
            } 
            
        }
        //Debug.Log($"[Awake] {gameObject.name}�� ���� �ε���: {slotIndex}");
    }    
    /// <summary>
    /// ������ �ʱ�ȭ�ϰ� �����ܰ� Ű ǥ�ø� ����
    /// </summary>
    /// <param name="sprite">������ ������</param>
    /// <param name="key">������ Ű</param>
    /// <param name="newItem">����� InventoryItem</param>
    public void Init(Sprite sprite, KeyCode key, InventoryItem newItem)
    {        
        //Debug.Log($"[Init] ȣ��� - Sprite: {(sprite == null ? "null" : sprite.name)}");
        this.inventoryItem = newItem;

        if (inventoryItem == null || inventoryItem.item_Data == null)
        {
            Debug.LogWarning("[Init] inventoryItem �Ǵ� item_Data�� null��");
            return;
        }

        if (!inventoryItem.linkedSlots.Contains(this))
        {
            inventoryItem.linkedSlots.Add(this);
            Debug.Log($"[Init] {this.name} -> linkedSlots�� �߰��� (item: {newItem.item_Data.itemName})");
        }
        if (displayIcon != null)
        {            
            displayIcon.sprite = sprite;
            displayIcon.enabled = (sprite != null);            
            displayIcon.rectTransform.anchoredPosition = Vector2.zero;  
        }
       // Debug.Log($"[{name}] displayIcon name: {displayIcon?.name}, sprite: {displayIcon?.sprite?.name}, activeSelf: {displayIcon?.gameObject.activeSelf}");
        if (keyCodeNameText != null)
        {
            keyCodeNameText.text = key.ToString(); // key ����
            //keyCodeNameText.SetText(key.ToString());
        }       
        

        //Debug.Log($"[Init_RefreshPlayerUseItemUI��] {gameObject.name} �� displayIcon: {displayIcon.name}, sprite: {displayIcon.sprite?.name}, enabled: {displayIcon.enabled}");
        RefreshPlayerUseItemUI();
        //Debug.Log($"[Init_RefreshPlayerUseItemUI��] {gameObject.name} �� displayIcon: {displayIcon.name}, sprite: {displayIcon.sprite?.name}, enabled: {displayIcon.enabled}");
        //Debug.Log($"inventoryItem : {inventoryItem}, item_Data {inventoryItem.item_Data} , displayIcon.itemSprite {displayIcon.sprite}" +
        //    $"item_Data.itemSprite {inventoryItem.item_Data.itemSprite}");
        //Debug.Log($"[DEBUG] displayIcon.enabled: {displayIcon.enabled}, color.a: {displayIcon.color.a}, sprite: {displayIcon.sprite}");
        //PlayerUseItemUiManager.Instance.DebugAllSlotImages();
    }
    /// <summary>
    /// Ű�� ǥ���ϰ� ������ ���� �ʱ�ȭ
    /// </summary>
    /// <param name="key">������ Ű</param>
    public void InitKeyCodeOnly(KeyCode key)
    {
       if (keyCodeNameText != null)
        {
            keyCodeNameText.text = key.ToString();
            //keyCodeNameText.SetText(key.ToString());
        }
        /*if (displayIcon != null)
        {
            displayIcon.sprite = null;
            displayIcon.enabled = false;           
        }*/
        cooldownText.text = "";
        //cooldownText.SetText("");
        inventoryItem = null;
    }
    /// <summary>
    /// ���콺 Ŀ���� ���� ���� �ö��� �� ȣ��
    /// ������ ���� ������ ǥ��
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(inventoryItem == null || inventoryItem.item_Data == null)
        {
            return;
        }
       if(tooltip != null && inventoryItem.item_Data != null)
        {
            tooltip.ShowItemTooltip(inventoryItem.item_Data, eventData.position);
        }
    }
    /// <summary>
    /// ���콺 Ŀ���� ������ ��� �� ȣ��
    /// ǥ�� ���� ������ ������ ����
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if(tooltip != null)
        {
            tooltip.HideTooltip();
        }
    }
    /// <summary>
    /// ���� ������ �������� 1�� ���(�Һ�)�ϰ�, ������ 0�̸� ������ �ʱ�ȭ
    /// ���� ���Կ� ����Ǿ� ���� ��� ��ü UI�� ����
    /// </summary>
    public void ConsumeItem()
    {
        if (inventoryItem == null)
        {
            return;
        }

        inventoryItem.quantity--;        

        if(inventoryItem.quantity <= 0)
        {   
            inventoryItem.ClearAllLinkedSlots();
            
            ClearSlot();
        }
        else
        {           
            inventoryItem.RefreshAllLinkedSlots();           
        }
    }

    /// <summary>
    /// ������ ������ �ʱ�ȭ�Ͽ� �����ܰ� �ؽ�Ʈ�� �����ϰ� ���ᵵ ����
    /// </summary>
    public void ClearSlot()
    {
        var slotList = PlayerUseItemUiManager.Instance.itemSlot;
        Debug.Log($"[ClearSlot] ȣ���: {gameObject.name}");
        // ��ũ ����
        if (inventoryItem != null)
        {
            //inventoryItem.ClearAllLinkedSlots(this);
            //inventoryItem.linkedSlots.Remove(this);
            if(inventoryItem.linkedSlots.Contains(this))
            {
                inventoryItem.linkedSlots.Remove(this);
            }
        }

        //inventoryItem = null;
        if (keyCodeNameText != null && PlayerUseItemUiManager.Instance != null &&
            slotIndex >= 0 && slotIndex < slotList.Count)
        {
            keyCodeNameText.text = slotList[slotIndex].keyCode.ToString();
            //keyCodeNameText.SetText(slotList[slotIndex].keyCode.ToString());
        }

        if (displayIcon != null)
        {            
            displayIcon.sprite = null;
            displayIcon.enabled = false;            
            //displayIcon.color = new Color(1, 1, 1, 1);
        }        

        if (itemCountText != null)
        {
            itemCountText.text = string.Empty;
            //itemCountText.SetText("");
        }
        
        if (cooldownText != null)
        {
            cooldownText.text = string.Empty;
            //cooldownText.SetText("");
        }

        if (PlayerUseItemUiManager.Instance?.itemList != null) 
        {
            PlayerUseItemUiManager.Instance.itemList.RemoveAll(i => i.item == this.inventoryItem);
        }

        //var slotList = PlayerUseItemUiManager.Instance.itemSlot;           
        if (PlayerUseItemUiManager.Instance != null && slotIndex >= 0)
        {
            //var slotList = PlayerUseItemUiManager.Instance.itemSlot;
            if (slotIndex < slotList.Count && slotList[slotIndex].playerUseItemUi == this)
            {
                slotList[slotIndex].inventoryItem = null; 
            }
            
        }


        inventoryItem = null;
        //Debug.Log($"[ClearSlot] {gameObject.name} �� displayIcon: {displayIcon.name}, enabled: {displayIcon.enabled} (Before Reset)");
        if (dragIcon != null)
        {
            dragIcon.SetActive(false);
        }
        
    }
    /// <summary>
    /// ������ ���� UI�� �ǽð����� ���� ������ �������� ����ȭ
    /// �������� null�̰ų� �߸��� ��� ������ �ʱ�ȭ
    /// </summary>
    public void RefreshPlayerUseItemUI()
    {       
        if (inventoryItem == null || inventoryItem.item_Data == null)
        {
            Debug.LogWarning("[PlayerUseItemUi] inventoryItem �Ǵ� item_Data�� null�Դϴ�.");
            ClearSlot();
            if (displayIcon != null)
            {
                displayIcon.sprite = null;
                //displayIcon.enabled = false;
            }

            if (itemCountText != null)
            {
                itemCountText.text = string.Empty;
                //itemCountText.SetText("");
            }
            return;
        }
       
        if (displayIcon == null || itemCountText == null)
        {
            Debug.LogWarning("[PlayerUseItemUi] displayIcon �Ǵ� itemCountText�� null�Դϴ�.");
            return;
        }
        if (displayIcon != null)
        {
            //PlayerUseItemUiManager.Instance.ForceActivateAllSlotIcons();
            displayIcon.sprite = inventoryItem.item_Data.itemSprite;
            //displayIcon.enabled = true;
            displayIcon.enabled = (inventoryItem.item_Data.itemSprite != null);            
            displayIcon.rectTransform.anchoredPosition = Vector2.zero;            
        }

        /*if(keyCodeNameText != null)
        {
            *//*var itemList = PlayerUseItemUiManager.Instance.itemList;
            for (int i = 0; i< itemList.Count; i++)
            {
                var item = itemList[i];
                keyCodeNameText.text = item.GetKey().ToString();
            }  *//*
            var slot = PlayerUseItemUiManager.Instance.itemSlot[slotIndex];
            keyCodeNameText.text = slot.keyCode.ToString();
        }*/

        if (itemCountText != null)
        {
            itemCountText.text = inventoryItem.IsStackable && inventoryItem.quantity > 1
                ? inventoryItem.quantity.ToString()
                : string.Empty;
            /*if(inventoryItem.IsStackable && inventoryItem.quantity > 1)
            {
                itemCountText.SetText("{0}", inventoryItem.quantity);
            }
            else
            {
                itemCountText.SetText("");
            }*/
        }        
    }
    /// <summary>
    /// ������ ���콺�� Ŭ������ �� ȣ��Ǵ� �̺�Ʈ �޼���
    /// ��Ŭ�� �� �������� �κ��丮�� ��ȯ
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            ReturnItemToInventory();
        }
    }
    /// <summary>
    /// ��Ŭ�� �� �κ��丮�� �������� ��ȯ�ϴ� �޼���
    /// �κ��丮�� �� ������ ã�� �ش� �������� �߰��ϰ�, ���� ������ ���
    /// </summary>
    private void ReturnItemToInventory()
    {
        if (inventoryItem == null || inventoryItem.item_Data == null)
        {            
            return;
        }       
       
        var slots = Inventory.instance.inventorySlotManager.slotList;
        for (int i =0; i < slots.Count; i++)
        {
            var slot = slots[i];
            if(slot.IsEmpty)
            {
                
                Inventory.instance.AddItem(inventoryItem.item_Data, inventoryItem.pooledObj, inventoryItem.quantity);
                
                inventoryItem.ClearAllLinkedSlots();
                ClearSlot();
                PlayerUseItemUiManager.Instance.ForceActivateAllSlotIcons();

                return;
            }
        }

        Debug.Log("[ReturnItemToInventory] �κ��丮�� �� ������ ����");
    }
    /// <summary>
    /// �巡�� ���� �� ȣ��Ǵ� �̺�Ʈ �޼���
    /// �巡�� �������� Ȱ��ȭ�ϰ�, draggedSlot�� ����
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {

        if(inventoryItem == null || inventoryItem.item_Data == null)
        {
            Debug.LogWarning("[OnBeginDrag] item or item.item_Data is null");
            return; 
        }

        draggedSlot = this;
        Debug.Log($"[OnBeginDrag] draggedSlot set. item: {inventoryItem.item_Data.itemName}, quantity: {inventoryItem.quantity}");

        dragIcon.SetActive(true);
        Image dragIconImage = dragIcon.GetComponent<Image>();
        if(displayIcon.sprite != null)
        {
            dragIconImage.sprite = displayIcon.sprite;            
            dragIconImage.raycastTarget = false;            
        }      
        dragIconImage.transform.position = eventData.position;
    }
    /// <summary>
    /// �巡�� ���� �� ȣ��Ǵ� �̺�Ʈ �޼���
    /// dragIcon�� ��Ȱ��ȭ�ϰ� draggedSlot�� �ʱ�ȭ
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {       
        StartCoroutine(ResetDragSlotNextFrame());        
    }
    /// <summary>
    /// �巡�� �� ���� �����ӿ� dragIcon�� ��Ȱ��ȭ�ϰ� draggedSlot�� �ʱ�ȭ
    /// </summary>
    private IEnumerator ResetDragSlotNextFrame()
    {
        yield return null;
        if(dragIcon != null)
        {
            dragIcon.SetActive(false);
        }
        draggedSlot = null;
    }
    /// <summary>
    /// �巡�� �߿� �������� ���콺�� ����ٴϵ��� ��
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            dragIcon.transform.position = eventData.position;
        }       
    }
    /// <summary>
    /// �巡�� �������� ������� �� ȣ��Ǵ� �̺�Ʈ �޼���
    /// ���� �������̸� ������ ��ġ��, �ƴϸ� �ش� ���Կ� �������� ���
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        dragIcon.SetActive(false); 

        
        if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent<InventorySlot>(out var inventorySlot))
        {
            if (inventorySlot.item == null)
            {
                Debug.LogWarning("[OnDrop] InventorySlot���� �Ѿ�� �������� null�Դϴ�.");
                return;
            }

            PlayerUseItemUiManager.Instance.AssignToUseSlotAtIndex(this.slotIndex, inventorySlot.item);             
            inventorySlot.ClearSlot(); 
            return;
        }
        
        if (draggedSlot == null || draggedSlot == this || draggedSlot.inventoryItem == null)
        {
            Debug.LogWarning("[OnDrop] draggedSlot�� null�̰ų� �ڱ� �ڽ��Դϴ�.");
            return;
        }
        if (draggedSlot.inventoryItem.item_Data == null)
        {
            Debug.LogWarning("[OnDrop] draggedSlot�� item_Data�� null�Դϴ�.");
            return;
        }
        
        if (this.inventoryItem != null &&
            draggedSlot.inventoryItem.item_Data == this.inventoryItem.item_Data &&
            this.inventoryItem.IsStackable)
        {
            int maxStack = inventoryItem.item_Data.maxStack;
            int canAdd = maxStack - inventoryItem.quantity;
            int moveAmount = Mathf.Min(canAdd, draggedSlot.inventoryItem.quantity);

            if (moveAmount > 0)
            {
                inventoryItem.quantity += moveAmount;
                draggedSlot.inventoryItem.quantity -= moveAmount;

                inventoryItem.RefreshAllLinkedSlots();
                draggedSlot.inventoryItem.RefreshAllLinkedSlots();

                if (draggedSlot.inventoryItem.quantity <= 0)
                {
                    StartCoroutine(ClearAfterDelay(draggedSlot));
                }
            }
            
            draggedSlot = null;           
            return;
        }
        if (draggedSlot != null && draggedSlot.inventoryItem != null)
        {
            Debug.Log($"[OnDrop] ���� ���� �ε���: {this.slotIndex}, �巡�׵� ������: {draggedSlot.inventoryItem.item_Data.itemName}");
        }
       
        PlayerUseItemUiManager.Instance.AssignToUseSlotAtIndex(this.slotIndex, draggedSlot.inventoryItem);
        StartCoroutine(ClearAfterDelay(draggedSlot));

        draggedSlot = null;
        dragIcon.SetActive(false);
        
    }
    /// <summary>
    /// �巡�׷� ���յ� �������� ������ ���� �����ӿ��� �ʱ�ȭ
    /// </summary>
    /// <param name="slot">����� �� ����</param>
    private IEnumerator ClearAfterDelay(PlayerUseItemUi slot)
    {
        yield return null;
      
        if (slot != null)
        {
            slot.ClearSlot();
        }
    }
}
