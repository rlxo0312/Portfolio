using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// �κ��丮�� ������ ����ϴ� Ŭ����.
/// ������ ���, �巡�� �� ���, ����Ŭ��, UI ����, ���� ǥ�� �� �پ��� ���ͷ��� ó��.
/// IDropHandler, IDragHandler �� �پ��� �������̽��� ����.
///
/// <para>��� ����</para>
/// <para>public InventoryItem item, public Image icon, public TextMeshProUGUI quantityText</para>
/// <para>private CanvasGroup canvasGroup, private float lastClickTime, private float doubleClickThreshold</para>
/// <para>public bool IsEmpty</para>
/// <para>private int lastCooltimeTextCooldown</para>
///
/// <para>��� �޼���</para>
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
    /// ������ ����ִ��� ����
    /// </summary>
    public bool IsEmpty => item == null || item.quantity <= 0;
    
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup == null)
        {
            Debug.Log("[Inventoryslot] canvusgroup�� �������� �ʾҽ��ϴ�.");
        }
    }
    private void Update()
    {
         if(item == null || !item.isCooldown)
         {
             return;
         }
        //Debug.Log($"[��Ÿ�� Ȯ��] ������: {item.item_Data.itemName}, isCooldown: {item.isCooldown}, timer: {item.cooldownTimer}");
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
    /// ���Կ� �������� ����
    /// </summary>
    /// <param name="poolObject">Ǯ���� ������Ʈ</param>
    /// <param name="newItem">���� ����� ������</param>
    public void SetItem(GameObject poolObject, InventoryItem newItem)
    {
        ClearSlot();
        if (newItem == null || newItem.item_Data == null)
        {
            Debug.Log("[InventorySlot] item�� null�Դϴ�.");
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
    /// ������ ������ �߰�
    /// </summary>
    /// <param name="amount">�߰��� ����</param>
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
    /// ������ �ʱ�ȭ�ϰ� ���
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
    /// �������� ���� �����ϰ� UI�� ������
    /// </summary>
    /// <param name="newItem">�Ҵ��� ������</param>
    /// <param name="poolObj">Ǯ���� ������Ʈ (����)</param>
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
    /// ���� UI�� ������ ������ �������� ������
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
    /// ����� UI ���Կ��� ���� �� �������� ������
    /// </summary>
    public void RefreshPlayerUseItemUI()
    {
        if (item == null || item.item_Data == null)//IsEmpty
        {
            Debug.LogWarning("[PlayerUseItemUi] inventoryItem �Ǵ� item_Data�� null��");
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
    /// �ٸ� ���Կ��� ��ӵ� �� ó��
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            return;
        }
        //Debug.Log($"[OnDrop] Drop �õ��� on {name}, �巡�׵� ������Ʈ: {eventData.pointerDrag?.name}");
        InventorySlot draggedSlot = eventData.pointerDrag?.GetComponent<InventorySlot>();
        /*if(draggedSlot == null || draggedSlot == this || draggedSlot.IsEmpty)
        {
            Debug.Log("[OnDrop] �巡�׵� ������ null�̰ų�, �ڱ� �ڽ��̰ų�, �������");
            return;
        }*/
        if (draggedSlot == null || draggedSlot == this || draggedSlot.item == null)
        {
            Debug.Log("[OnDrop] �巡�׵� ������ null�̰ų�, �ڱ� �ڽ��̰ų�, �������");
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
        /// �巡�� ���� ó��
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
    /// �巡�� ���� ó�� �� ���� ������ ��� �� ���� ó��
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
            Debug.Log("[InventorySlot] �κ��丮 �ܺη� �巡�׵Ǿ� ������");
            if (item != null && item.item_Data != null && item.pooledObj != null)
            {
                ObjectPoolingManager.Instance.ReturnToPool(item.item_Data.itemKey, item.pooledObj);
            }
            ClearSlot();
        }        

    }
    /// <summary>
    /// �巡�� �� ������ �̵� ó��
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
    /// ���콺 �����Ͱ� ���� ���� �ö��� �� ���� ǥ��
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
    /// ���콺 �����Ͱ� ������ ����� �� ���� ����
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUi.instance.tooltipUI.HideTooltip();
    }
    /// <summary>
    /// ���� ���� Ŭ�� �� ��� ������ ��� ó��
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null)
        {
            Debug.LogWarning("[InventorySlot] OnPointerClick �� inventoryItem�� null�Դϴ�.");
            return;
        }

        if (item.item_Data == null)
        {
            Debug.LogWarning("[InventorySlot] OnPointerClick �� item_Data�� null�Դϴ�.");
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
            Debug.Log($"[InventorySlot] {item.item_Data.itemName}�� ���� ��ٿ� ���Դϴ�.");
        }

        lastClickTime = Time.time;
    }
    /// <summary>
    /// �����ۿ� ��Ÿ���� ����
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
