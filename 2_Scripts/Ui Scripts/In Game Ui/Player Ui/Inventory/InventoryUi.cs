using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// �κ��丮 UI�� �����ϰ�, �巡�� �̵� �� ������ ǥ�� ����� ó���ϴ� Ŭ����
///
/// <para>��� ����</para>
/// <para>public GameObject inventoryPanel</para>
/// <para>public RectTransform panelTransform</para>
/// <para>public Canvas canvus</para>
/// <para>public GamePlayUiManager gamePlayUiManager</para>
/// <para>public Image dragGhostImage</para>
/// <para>public ToolTipUi tooltipUI</para>
/// <para>public RectTransform inventoryBackground</para>
///
/// <para>��� �޼���</para>
/// <para>public void OpenInventory()</para>
/// <para>public void ShowDragIcon(Sprite sprite, Vector2 position)</para>
/// <para>public void MoveDragIcon(Vector2 position)</para>
/// <para>public void HideDragIcon()</para>
/// </summary>
public class InventoryUi : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public static InventoryUi instance { get; private set; }
    [Header("�г�")]
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
    /// �г��� �巡���� �� UI ��ġ �̵� ó��
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        panelTransform.anchoredPosition += eventData.delta / canvus.scaleFactor;
    }
    /// <summary>
    /// ���콺�� Ŭ���� �� UI ���� ��ġ ����
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
    /// �κ��丮 â�� ����/������ ���� �ʱ�ȭ �� UI ���� �ݿ�
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
    /// �巡�� ��Ʈ �������� ������ 
    /// </summary>
    /// <param name="sprite">ǥ���� ��������Ʈ</param>
    /// <param name="position">������ ��ġ</param>
    public void ShowDragIcon(Sprite sprite, Vector2 position)//, Vector2 size
    {
        if (dragGhostImage == null || sprite == null) return;

        dragGhostImage.sprite = sprite;
        //dragGhostImage.rectTransform.sizeDelta = size;
        dragGhostImage.transform.position = position;
        dragGhostImage.color = new Color(1, 1, 1, 0.9f); // ��¦ ����
        dragGhostImage.gameObject.SetActive(true);
    }
    /// <summary>
    /// �巡�� ������ ��ġ�� ���콺 ��ġ�� ����
    /// </summary>
    /// <param name="position">���콺 ��ġ</param>
    public void MoveDragIcon(Vector2 position)
    {
        if (dragGhostImage != null && dragGhostImage.gameObject.activeSelf)
        {
            dragGhostImage.transform.position = position;
        }
    }
    /// <summary>
    /// �巡�� ��Ʈ �������� ����
    /// </summary>
    public void HideDragIcon()
    {
        if (dragGhostImage != null)
        {
            dragGhostImage.gameObject.SetActive(false);
        }
    }

}
