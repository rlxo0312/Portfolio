using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/// <summary>
/// ���ӿ�����Ʈ ���� ���� UI ǥ�� Ŭ����
/// <para>��� ����</para>
/// <para>public: TextMeshProUGUI itemNameText, TextMeshProUGUI itemInfoText, GameObject tooltipPanel</para>
/// 
/// <para>��� �ż���</para>
/// <para>public howTooltip(Item_Data data, Vector2 position), public void ShowSkillToolTip(PlayerSkillData data, Vector2 position),
/// public HideTooltip()</para>
/// </summary>
public class ToolTipUi : MonoBehaviour
{
    public TextMeshProUGUI gameObjectNameText;
    public TextMeshProUGUI gameObjectInfoText;
    public GameObject tooltipPanel;
    public Vector2 toolTipPosition;

    private void Awake()
    {
        tooltipPanel.SetActive(false);
    }
    /// <summary>
    /// Ư�� ���ǽ� �������� ������ ������ 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="position"></param>
    public void ShowItemTooltip(Item_Data data, Vector2 position)
    {
        if (data == null)
        {
            return;
        }

        gameObjectNameText.text = data.itemName;
        gameObjectInfoText.text = data.itemInfo;
        //gameObjectNameText.SetText(data.itemName);
        //gameObjectInfoText.SetText(data.itemInfo);
        tooltipPanel.SetActive(true);
        tooltipPanel.transform.position = position + toolTipPosition;
    }
    /// <summary>
    /// Ư�� ���ǽ� ��ų ������ ������
    /// </summary>
    /// <param name="data"></param>
    /// <param name="position"></param>
    public void ShowSkillToolTip(PlayerSkillData data, Vector2 position)
    {
        if (data == null)
        {
            return;
        }

        gameObjectNameText.text = data.skillName;
        gameObjectInfoText.text = data.skillComent;
        //gameObjectNameText.SetText(data.skillName);
        //gameObjectInfoText.SetText(data.skillComent);
        tooltipPanel.SetActive(true);
        tooltipPanel.transform.position = position + toolTipPosition;
    }
    /// <summary>
    /// ������ ������ ����
    /// </summary>
    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}
