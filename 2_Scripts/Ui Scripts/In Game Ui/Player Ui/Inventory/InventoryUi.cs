using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 인벤토리 UI를 제어하고, 드래그 이동 및 아이콘 표시 기능을 처리하는 클래스
///
/// <para>사용 변수</para>
/// <para>public GameObject inventoryPanel</para>
/// <para>public RectTransform panelTransform</para>
/// <para>public Canvas canvus</para>
/// <para>public GamePlayUiManager gamePlayUiManager</para>
/// <para>public Image dragGhostImage</para>
/// <para>public ToolTipUi tooltipUI</para>
/// <para>public RectTransform inventoryBackground</para>
///
/// <para>사용 메서드</para>
/// <para>public void OpenInventory()</para>
/// <para>public void ShowDragIcon(Sprite sprite, Vector2 position)</para>
/// <para>public void MoveDragIcon(Vector2 position)</para>
/// <para>public void HideDragIcon()</para>
/// </summary>
public class InventoryUi : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public static InventoryUi instance { get; private set; }
    [Header("패널")]
    public GameObject inventoryPanel;
    public RectTransform panelTransform;
    public Canvas canvus;
    public GamePlayUiManager gamePlayUiManager;
    public Image dragGhostImage;
    private Vector2 pointeroffset;
    public ToolTipUi tooltipUI;
    public RectTransform inventoryBackground;

    private void Awake()
    {
        instance = this;
        if (dragGhostImage != null)
        {
            dragGhostImage.raycastTarget = false; 
            dragGhostImage.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 패널을 드래그할 때 UI 위치 이동 처리
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        panelTransform.anchoredPosition += eventData.delta / canvus.scaleFactor;
    }
    /// <summary>
    /// 마우스를 클릭할 때 UI 기준 위치 갱신
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 localPointerPosition; 
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(canvus.transform as RectTransform, 
            eventData.position, eventData.pressEventCamera, out localPointerPosition))
        {
            panelTransform.localPosition = localPointerPosition - pointeroffset;
        }
    }
    /// <summary>
    /// 인벤토리 창을 열고/닫으며 슬롯 초기화 및 UI 상태 반영
    /// </summary>
    public void OpenInventory()
    {
        bool isActive = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isActive);

        if(gamePlayUiManager != null)
        {
            gamePlayUiManager.CallUi(isActive);
        }
        if (isActive)
        {
            Inventory.instance.inventorySlotManager.LoadSlots();
        }
    }
    /// <summary>
    /// 드래그 고스트 아이콘을 보여줌 
    /// </summary>
    /// <param name="sprite">표시할 스프라이트</param>
    /// <param name="position">아이콘 위치</param>
    public void ShowDragIcon(Sprite sprite, Vector2 position)//, Vector2 size
    {
        if (dragGhostImage == null || sprite == null) return;

        dragGhostImage.sprite = sprite;
        //dragGhostImage.rectTransform.sizeDelta = size;
        dragGhostImage.transform.position = position;
        dragGhostImage.color = new Color(1, 1, 1, 0.9f); // 살짝 투명
        dragGhostImage.gameObject.SetActive(true);
    }
    /// <summary>
    /// 드래그 아이콘 위치를 마우스 위치로 갱신
    /// </summary>
    /// <param name="position">마우스 위치</param>
    public void MoveDragIcon(Vector2 position)
    {
        if (dragGhostImage != null && dragGhostImage.gameObject.activeSelf)
        {
            dragGhostImage.transform.position = position;
        }
    }
    /// <summary>
    /// 드래그 고스트 아이콘을 숨김
    /// </summary>
    public void HideDragIcon()
    {
        if (dragGhostImage != null)
        {
            dragGhostImage.gameObject.SetActive(false);
        }
    }

}
