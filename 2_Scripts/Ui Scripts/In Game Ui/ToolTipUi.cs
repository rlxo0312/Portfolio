using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/// <summary>
/// 게임오브젝트 설명 툴팁 UI 표시 클래스
/// <para>사용 변수</para>
/// <para>public: TextMeshProUGUI itemNameText, TextMeshProUGUI itemInfoText, GameObject tooltipPanel</para>
/// 
/// <para>사용 매서드</para>
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
    /// 특정 조건시 아이템의 툴팁을 보여줌 
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
    /// 특정 조건시 스킬 툴팁을 보여줌
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
    /// 아이템 툴팁을 숨김
    /// </summary>
    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}
