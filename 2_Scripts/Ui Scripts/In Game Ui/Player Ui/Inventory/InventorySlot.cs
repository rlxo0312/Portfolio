using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 인벤토리의 슬롯을 담당하는 클래스.
/// 아이템 등록, 드래그 앤 드롭, 더블클릭, UI 갱신, 툴팁 표시 등 다양한 인터랙션 처리.
/// IDropHandler, IDragHandler 등 다양한 인터페이스를 구현.
///
/// <para>사용 변수</para>
/// <para>public InventoryItem item, public Image icon, public TextMeshProUGUI quantityText</para>
/// <para>private CanvasGroup canvasGroup, private float lastClickTime, private float doubleClickThreshold</para>
/// <para>public bool IsEmpty</para>
/// <para>private int lastCooltimeTextCooldown</para>
///
/// <para>사용 메서드</para>
/// <para>public void SetItem(GameObject poolObject, InventoryItem newItem)</para>
/// <para>public void StartItemCooldown(float time, InventoryItem item)</para>
/// <para>public void AddQuantity(int amount)</para>
/// <para>public void ClearSlot()</para>
/// <para>public void AssignItem(InventoryItem newItem, GameObject poolObj = null)</para>
/// <para>public void RefreshSlotUI()</para>
/// <para>public void RefreshPlayerUseItemUI()</para>
/// <para>public void OnDrop(PointerEventData eventData), public void OnBeginDrag(PointerEventData eventData), public void OnEndDrag(PointerEventData eventData)
/// public void OnDrag(PointerEventData eventData), public void OnPointerEnter(PointerEventData eventData), public void OnPointerExit(PointerEventData eventData)
/// public void OnPointerClick(PointerEventData eventData)</para>
/// </summary>
[System.Serializable]
public class InventorySlot : MonoBehaviour, IDropHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, 
                                            IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IItemLinkedSlot
{
    public Image icon;
    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI coolTimeText; 
    public InventoryItem item { get; private set; }

    //private GameObject pooledItem;
    //private Canvas canvas;
    //private Transform originalTransform;
    //private Vector2 originalPosition;
    private CanvasGroup canvasGroup;

    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.3f;
    private int lastCooltimeTextCooldown = -1; 
    /// 슬롯이 비어있는지 여부
    /// </summary>
    public bool IsEmpty => item == null || item.quantity <= 0;
    
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup == null)
        {
            Debug.Log("[Inventoryslot] canvusgroup이 설정되지 않았습니다.");
        }
    }
    private void Update()
    {
         if(item == null || !item.isCooldown)
         {
             return;
         }
        //Debug.Log($"[쿨타임 확인] 아이템: {item.item_Data.itemName}, isCooldown: {item.isCooldown}, timer: {item.cooldownTimer}");
        //item.cooldownTimer -= Time.deltaTime;
        //coolTimeText.text = Mathf.Ceil(item.cooldownTimer).ToString();
        /*int currentCooldown = Mathf.CeilToInt(item.cooldownTimer);

        if (currentCooldown != lastCooltimeTextCooldown)
        {
            if (coolTimeText != null)
            {
                //coolTimeText.SetText("{0}", currentCooldown);
                coolTimeText.text = currentCooldown.ToString();
            }

            lastCooltimeTextCooldown = currentCooldown;
        }*/

        if (item.cooldownTimer <= 0.01f)
         {
            item.cooldownTimer = 0;
            item.isCooldown = false;
            //coolTimeText.SetText("");
            coolTimeText.text = "";
            icon.color = new Color(1, 1, 1, 1);
            lastCooltimeTextCooldown = -1;
            return;
         }

        int currentCooldown = Mathf.CeilToInt(item.cooldownTimer);

        if (currentCooldown != lastCooltimeTextCooldown)
        {
            if (coolTimeText != null)
            {
                //coolTimeText.SetText("{0}", currentCooldown);
                coolTimeText.text = currentCooldown.ToString();
            }

            lastCooltimeTextCooldown = currentCooldown;
        }
    } 
    /// <summary>
    /// 슬롯에 아이템을 설정
    /// </summary>
    /// <param name="poolObject">풀링된 오브젝트</param>
    /// <param name="newItem">새로 등록할 아이템</param>
    public void SetItem(GameObject poolObject, InventoryItem newItem)
    {
        ClearSlot();
        if (newItem == null || newItem.item_Data == null)
        {
            Debug.Log("[InventorySlot] item이 null입니다.");
            return;
        }        
        this.item = newItem;

        if (poolObject != null)
        {
            item.pooledObj = poolObject;
        }

        if (!item.linkedSlots.Contains(this))
        {
            item.linkedSlots.Add(this);
        }

        RefreshSlotUI();       
    }
    /// <summary>
    /// 아이템 수량을 추가
    /// </summary>
    /// <param name="amount">추가할 수량</param>
    public void AddQuantity(int amount)
    {
        if(IsEmpty || !item.IsStackable)
        {
            return;
        }
        item.quantity = Mathf.Min(item.quantity + amount, item.item_Data.maxStack);
        quantityText.text = item.quantity.ToString();    
        //quantityText.SetText("{0}", item.quantity);
    }
    /// <summary>
    /// 슬롯을 초기화하고 비움
    /// </summary>
    public void ClearSlot()
    {
        
        if (item != null )
        {
            if(item.linkedSlots.Contains(this))
            {
                item.linkedSlots.Remove(this);
                if (item.linkedSlots.Count == 0)
                {
                    item.pooledObj = null;
                }
            }            
            item = null;
        }
       
        if(icon != null)
        {
            icon.sprite = null;
            icon.enabled = false;
        }
        
        if(quantityText != null)
        {
            quantityText.text = string.Empty;
            //quantityText.SetText("");
        }
    }

    /// <summary>
    /// 아이템을 직접 지정하고 UI를 갱신함
    /// </summary>
    /// <param name="newItem">할당할 아이템</param>
    /// <param name="poolObj">풀링된 오브젝트 (선택)</param>
    public void AssignItem(InventoryItem newItem, GameObject poolObj = null)
    {
        item = newItem;

        if(poolObj != null)
        {
            item.pooledObj = poolObj;
        }
        if (item != null && !item.linkedSlots.Contains(this))
        {
            item.linkedSlots.Add(this);
        }
        RefreshSlotUI();
    }

    /// <summary>
    /// 슬롯 UI를 수량과 아이콘 기준으로 갱신함
    /// </summary>
    public void RefreshSlotUI()
    {
        if (item != null && item.item_Data != null)
        {
            if(icon != null)
            {
               icon.sprite = item.item_Data.itemSprite;
               icon.enabled = icon.sprite != null;
            }

            

            if (quantityText != null)
            {
                quantityText.text = item.IsStackable && item.quantity > 1
                    ? item.quantity.ToString()
                    : string.Empty;
                /*if(item.IsStackable && item.quantity > 1)
                {
                    quantityText.SetText("{0}", item.quantity);
                }
                else
                {
                    quantityText.SetText("");
                }*/
            }
        }
        else
        {
            if(icon != null)
            {
               icon.sprite = null;
               icon.enabled = false;
            }

            if (quantityText != null)
            {
                quantityText.text = "";
                //quantityText.SetText("");
            }
        }
    }

    /// <summary>
    /// 연결된 UI 슬롯에서 수량 및 아이콘을 갱신함
    /// </summary>
    public void RefreshPlayerUseItemUI()
    {
        if (item == null || item.item_Data == null)//IsEmpty
        {
            Debug.LogWarning("[PlayerUseItemUi] inventoryItem 또는 item_Data가 null임");
            ClearSlot(); 
            return;
        }
        if(icon != null)
        {
            icon.sprite = item.item_Data.itemSprite;
            icon.enabled = true;
        }
        if(quantityText != null)
        {
            quantityText.text = item.IsStackable && item.quantity > 1 ? item.quantity.ToString() : string.Empty;    
            /*if(item.IsStackable && item.quantity > 1)
            {
                quantityText.SetText("{0}", item.quantity);
            }
            else
            {
                quantityText.SetText("");
            }*/
        }
        if (coolTimeText != null)
        {
            if (item.isCooldown)
            {
                int remaining = Mathf.CeilToInt(item.cooldownTimer);
                coolTimeText.text = remaining > 0 ? remaining.ToString() : "";
            }
            else
            {
                coolTimeText.text = "";
            }
        }
    }
    /// <summary>
    /// 다른 슬롯에서 드롭될 때 처리
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            return;
        }
        //Debug.Log($"[OnDrop] Drop 시도됨 on {name}, 드래그된 오브젝트: {eventData.pointerDrag?.name}");
        InventorySlot draggedSlot = eventData.pointerDrag?.GetComponent<InventorySlot>();
        /*if(draggedSlot == null || draggedSlot == this || draggedSlot.IsEmpty)
        {
            Debug.Log("[OnDrop] 드래그된 슬롯이 null이거나, 자기 자신이거나, 비어있음");
            return;
        }*/
        if (draggedSlot == null || draggedSlot == this || draggedSlot.item == null)
        {
            Debug.Log("[OnDrop] 드래그된 슬롯이 null이거나, 자기 자신이거나, 비어있음");
            return;
        }

        if (this.IsEmpty)
        {
            var copyItem = new InventoryItem(draggedSlot.item);
            SetItem(draggedSlot.item.pooledObj, copyItem);
            //SetItem(draggedSlot.item.pooledObj, draggedSlot.item);            
            draggedSlot.ClearSlot();
            return;
        }

        if (this.item.item_Data == draggedSlot.item.item_Data && this.item.IsStackable)
        {
            int total = item.quantity + draggedSlot.item.quantity;
            int max = item.item_Data.maxStack;

            int canAdd = Mathf.Min(max - item.quantity, draggedSlot.item.quantity);
            if (canAdd > 0)
            {
                item.quantity += canAdd;
                draggedSlot.item.quantity -= canAdd;

                var linkSlot = draggedSlot.item.linkedSlots;
                for (int i = 0; i < linkSlot.Count; i++)
                {
                    var ui = linkSlot[i];

                    if (!item.linkedSlots.Contains(ui))
                    {
                        item.linkedSlots.Add(ui);
                    }
                }

                item.RefreshAllLinkedSlots();
                draggedSlot.item.RefreshAllLinkedSlots();

                if (draggedSlot.item.quantity <= 0)
                {
                    draggedSlot.item.ClearAllLinkedSlots();
                    draggedSlot.ClearSlot();
                }
            }
            return;
        }

        InventoryItem tempItem = new InventoryItem(this.item);
        GameObject tempPooledItem = this.item.pooledObj;
        this.SetItem(draggedSlot.item.pooledObj, draggedSlot.item);       
        draggedSlot.SetItem(tempPooledItem, tempItem);        
        //draggedSlot.SetItem(tempPooledItem, draggedSlot.item);

    }
        /// <summary>
        /// 드래그 시작 처리
        /// </summary>
        /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsEmpty)
        {
            return;
        }       

        InventoryUi.instance.ShowDragIcon(icon.sprite, eventData.position);
       
        canvasGroup.blocksRaycasts = false;
        eventData.pointerDrag = gameObject;
    }
    /// <summary>
    /// 드래그 종료 처리 및 슬롯 밖으로 드롭 시 삭제 처리
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        InventoryUi.instance.HideDragIcon();
        
        canvasGroup.blocksRaycasts = true;

        bool isInsideInventory = RectTransformUtility.RectangleContainsScreenPoint(
        InventoryUi.instance.inventoryBackground,
        eventData.position, eventData.enterEventCamera);

        bool isOnUseSlot = false;       

        var itemSlot = PlayerUseItemUiManager.Instance.itemSlot;
        for (int i = 0; i < itemSlot.Count; i++)
        {
            var slot = itemSlot[i];

            if(slot.playerUseItemUi == null)
            {
                return;
            }

            RectTransform useSlotRect = slot.playerUseItemUi.GetComponent<RectTransform>(); 

            if(RectTransformUtility.RectangleContainsScreenPoint(useSlotRect, eventData.position, eventData.enterEventCamera))
            {
                //PlayerUseItemUiManager.Instance.AssignToUseSlot(item);
                ClearSlot();
                isOnUseSlot = true;
                break;
            }
        }

        if (!isInsideInventory && !isOnUseSlot)
        {
            Debug.Log("[InventorySlot] 인벤토리 외부로 드래그되어 삭제됨");
            if (item != null && item.item_Data != null && item.pooledObj != null)
            {
                ObjectPoolingManager.Instance.ReturnToPool(item.item_Data.itemKey, item.pooledObj);
            }
            ClearSlot();
        }        

    }
    /// <summary>
    /// 드래그 중 아이콘 이동 처리
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
       
        if (IsEmpty)
        {
            return;
        }
        InventoryUi.instance.MoveDragIcon(eventData.position);        
    }
    /// <summary>
    /// 마우스 포인터가 슬롯 위에 올라갔을 때 툴팁 표시
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {

        if (item == null || item.item_Data == null)
        {
            return;
        }
        InventoryUi.instance.tooltipUI.ShowItemTooltip(item.item_Data, eventData.position);
    }

    /// <summary>
    /// 마우스 포인터가 슬롯을 벗어났을 때 툴팁 숨김
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUi.instance.tooltipUI.HideTooltip();
    }
    /// <summary>
    /// 슬롯 더블 클릭 시 즉시 아이템 사용 처리
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null)
        {
            Debug.LogWarning("[InventorySlot] OnPointerClick 시 inventoryItem이 null입니다.");
            return;
        }

        if (item.item_Data == null)
        {
            Debug.LogWarning("[InventorySlot] OnPointerClick 시 item_Data가 null입니다.");
            return;
        }

       /* if (item == null || item.item_Data == null)
        {
            return;
        }       
*/
        if (item.IsAvailableItem && !item.isCooldown)
        {
            float timeSinceLastClick = Time.time - lastClickTime;
            if(timeSinceLastClick <= doubleClickThreshold)
            {
                float cooldown = item.item_Data.itemCooldown;
                var backupItem = item;
                //PlayerUseItemUiManager.Instance.UseInstantItem(item); 
                PlayerUseItemUiManager.Instance.UseInstantItem(backupItem);
                if (backupItem != null && backupItem.quantity > 0)
                {
                    StartItemCooldown(cooldown, backupItem);
                    //InventoryCooldownManager.Instance.RegisterCooldown(backupItem);
                }
            }
        }
        else if (item.isCooldown)
        {
            Debug.Log($"[InventorySlot] {item.item_Data.itemName}은 현재 쿨다운 중입니다.");
        }

        lastClickTime = Time.time;
    }
    /// <summary>
    /// 아이템에 쿨타임을 적용
    /// </summary>
    /// <param name="time"></param>
    /// <param name="item"></param>
    public void StartItemCooldown(float time, InventoryItem item)
    {
        if (item == null || item.item_Data == null)
        {
            return;
        }       

        item.cooldownTime = time;
        item.cooldownTimer = time; 
        item.isCooldown = true;
        icon.color = new Color(1, 1, 1, 0.5f);
        InventoryCooldownManager.Instance.RegisterCooldown(item);
    }
}
