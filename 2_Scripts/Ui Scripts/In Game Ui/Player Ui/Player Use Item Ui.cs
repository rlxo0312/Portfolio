using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

/// <summary>
/// 플레이어가 단축키 슬롯에서 사용하는 아이템 UI 슬롯 클래스
///
/// <para>사용 변수</para>
/// <para>public TextMeshProUGUI itemCountText</para>
/// <para>public InventoryItem inventoryItem</para>
/// <para>public int slotIndex</para>
/// <para>private static GameObject dragIcon, PlayerUseItemUi draggedSlot</para>
///
/// <para>사용 메서드</para>
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
    /// 컴포넌트가 로드될 때 호출
    /// dragIconInstance가 설정되어 있다면 static dragIcon으로 초기화
    /// </summary>
    private void Awake()
    {
        
        if (dragIcon == null && dragIconInstance != null)
        {
            dragIcon = dragIconInstance;
        }        
    }
    /// <summary>
    /// 게임 오브젝트가 활성화될 때 최초로 호출
    /// 슬롯 인덱스를 자동으로 계산하여 설정
    /// </summary>
    private void Start()
    {
        for (int i = 0; i < PlayerUseItemUiManager.Instance.itemSlot.Count; i++)
        {
            var slot = PlayerUseItemUiManager.Instance.itemSlot[i].playerUseItemUi;
            if (slot == this)
            {
                slotIndex = i;                
                //Debug.Log($"[AutoSlotIndex] {gameObject.name} → 슬롯 인덱스: {slotIndex}");
                break;
            } 
            
        }
        //Debug.Log($"[Awake] {gameObject.name}의 슬롯 인덱스: {slotIndex}");
    }    
    /// <summary>
    /// 슬롯을 초기화하고 아이콘과 키 표시를 설정
    /// </summary>
    /// <param name="sprite">아이템 아이콘</param>
    /// <param name="key">지정된 키</param>
    /// <param name="newItem">등록할 InventoryItem</param>
    public void Init(Sprite sprite, KeyCode key, InventoryItem newItem)
    {        
        //Debug.Log($"[Init] 호출됨 - Sprite: {(sprite == null ? "null" : sprite.name)}");
        this.inventoryItem = newItem;

        if (inventoryItem == null || inventoryItem.item_Data == null)
        {
            Debug.LogWarning("[Init] inventoryItem 또는 item_Data가 null임");
            return;
        }

        if (!inventoryItem.linkedSlots.Contains(this))
        {
            inventoryItem.linkedSlots.Add(this);
            Debug.Log($"[Init] {this.name} -> linkedSlots에 추가됨 (item: {newItem.item_Data.itemName})");
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
            keyCodeNameText.text = key.ToString(); // key 유지
            //keyCodeNameText.SetText(key.ToString());
        }       
        

        //Debug.Log($"[Init_RefreshPlayerUseItemUI전] {gameObject.name} → displayIcon: {displayIcon.name}, sprite: {displayIcon.sprite?.name}, enabled: {displayIcon.enabled}");
        RefreshPlayerUseItemUI();
        //Debug.Log($"[Init_RefreshPlayerUseItemUI후] {gameObject.name} → displayIcon: {displayIcon.name}, sprite: {displayIcon.sprite?.name}, enabled: {displayIcon.enabled}");
        //Debug.Log($"inventoryItem : {inventoryItem}, item_Data {inventoryItem.item_Data} , displayIcon.itemSprite {displayIcon.sprite}" +
        //    $"item_Data.itemSprite {inventoryItem.item_Data.itemSprite}");
        //Debug.Log($"[DEBUG] displayIcon.enabled: {displayIcon.enabled}, color.a: {displayIcon.color.a}, sprite: {displayIcon.sprite}");
        //PlayerUseItemUiManager.Instance.DebugAllSlotImages();
    }
    /// <summary>
    /// 키만 표시하고 아이콘 등은 초기화
    /// </summary>
    /// <param name="key">지정할 키</param>
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
    /// 마우스 커서가 슬롯 위에 올라갔을 때 호출
    /// 아이템 정보 툴팁을 표시
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
    /// 마우스 커서가 슬롯을 벗어날 때 호출
    /// 표시 중인 아이템 툴팁을 숨김
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
    /// 현재 슬롯의 아이템을 1개 사용(소비)하고, 수량이 0이면 슬롯을 초기화
    /// 여러 슬롯에 연결되어 있을 경우 전체 UI를 갱신
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
    /// 슬롯을 완전히 초기화하여 아이콘과 텍스트를 제거하고 연결도 끊음
    /// </summary>
    public void ClearSlot()
    {
        var slotList = PlayerUseItemUiManager.Instance.itemSlot;
        Debug.Log($"[ClearSlot] 호출됨: {gameObject.name}");
        // 링크 제거
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
        //Debug.Log($"[ClearSlot] {gameObject.name} → displayIcon: {displayIcon.name}, enabled: {displayIcon.enabled} (Before Reset)");
        if (dragIcon != null)
        {
            dragIcon.SetActive(false);
        }
        
    }
    /// <summary>
    /// 아이템 슬롯 UI를 실시간으로 갱신 수량과 아이콘을 동기화
    /// 아이템이 null이거나 잘못된 경우 슬롯을 초기화
    /// </summary>
    public void RefreshPlayerUseItemUI()
    {       
        if (inventoryItem == null || inventoryItem.item_Data == null)
        {
            Debug.LogWarning("[PlayerUseItemUi] inventoryItem 또는 item_Data가 null입니다.");
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
            Debug.LogWarning("[PlayerUseItemUi] displayIcon 또는 itemCountText가 null입니다.");
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
    /// 슬롯을 마우스로 클릭했을 때 호출되는 이벤트 메서드
    /// 우클릭 시 아이템을 인벤토리로 반환
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
    /// 우클릭 시 인벤토리로 아이템을 반환하는 메서드
    /// 인벤토리의 빈 슬롯을 찾아 해당 아이템을 추가하고, 현재 슬롯을 비움
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

        Debug.Log("[ReturnItemToInventory] 인벤토리에 빈 슬롯이 없음");
    }
    /// <summary>
    /// 드래그 시작 시 호출되는 이벤트 메서드
    /// 드래그 아이콘을 활성화하고, draggedSlot을 설정
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
    /// 드래그 종료 시 호출되는 이벤트 메서드
    /// dragIcon을 비활성화하고 draggedSlot을 초기화
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {       
        StartCoroutine(ResetDragSlotNextFrame());        
    }
    /// <summary>
    /// 드래그 후 다음 프레임에 dragIcon을 비활성화하고 draggedSlot을 초기화
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
    /// 드래그 중에 아이콘이 마우스를 따라다니도록 함
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
    /// 드래그 아이템을 드롭했을 때 호출되는 이벤트 메서드
    /// 같은 아이템이면 수량을 합치고, 아니면 해당 슬롯에 아이템을 등록
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        dragIcon.SetActive(false); 

        
        if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent<InventorySlot>(out var inventorySlot))
        {
            if (inventorySlot.item == null)
            {
                Debug.LogWarning("[OnDrop] InventorySlot에서 넘어온 아이템이 null입니다.");
                return;
            }

            PlayerUseItemUiManager.Instance.AssignToUseSlotAtIndex(this.slotIndex, inventorySlot.item);             
            inventorySlot.ClearSlot(); 
            return;
        }
        
        if (draggedSlot == null || draggedSlot == this || draggedSlot.inventoryItem == null)
        {
            Debug.LogWarning("[OnDrop] draggedSlot이 null이거나 자기 자신입니다.");
            return;
        }
        if (draggedSlot.inventoryItem.item_Data == null)
        {
            Debug.LogWarning("[OnDrop] draggedSlot의 item_Data가 null입니다.");
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
            Debug.Log($"[OnDrop] 현재 슬롯 인덱스: {this.slotIndex}, 드래그된 아이템: {draggedSlot.inventoryItem.item_Data.itemName}");
        }
       
        PlayerUseItemUiManager.Instance.AssignToUseSlotAtIndex(this.slotIndex, draggedSlot.inventoryItem);
        StartCoroutine(ClearAfterDelay(draggedSlot));

        draggedSlot = null;
        dragIcon.SetActive(false);
        
    }
    /// <summary>
    /// 드래그로 병합된 아이템의 슬롯을 다음 프레임에서 초기화
    /// </summary>
    /// <param name="slot">비워야 할 슬롯</param>
    private IEnumerator ClearAfterDelay(PlayerUseItemUi slot)
    {
        yield return null;
      
        if (slot != null)
        {
            slot.ClearSlot();
        }
    }
}
