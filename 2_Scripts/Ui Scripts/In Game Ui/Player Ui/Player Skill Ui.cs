using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// �÷��̾� ��ų UI�� �����ϴ� Ŭ����
///
/// <para>��� ����</para>
/// <para>public PlayerSkillData skillData</para>
///
/// <para>��� �޼���</para>
/// <para>public void Init(Sprite icon, KeyCode key, PlayerSkillData data)</para>
/// <para>public void OnPointerEnter(PointerEventData eventData)</para>
/// <para>public void OnPointerExit(PointerEventData eventData)</para>
/// </summary>
public class PlayerSkillUi : PlayerInGameSlotUi, IPointerEnterHandler, IPointerExitHandler
{
    public PlayerSkillData skillData;

    /// <summary>
    /// ��ų ������, ����Ű, ��ų �����͸� UI�� �ʱ�ȭ�ϴ� �޼���
    /// </summary>
    /// <param name="icon">��ų ������ �̹���</param>
    /// <param name="key">����Ű</param>
    /// <param name="data">��ų ������</param>
    public void Init(Sprite icon, KeyCode key, PlayerSkillData data)
    {
        displayIcon.sprite = icon;
        keyCodeNameText.text = key.ToString();
        cooldownText.text = "";
        //keyCodeNameText.SetText(key.ToString());
        //cooldownText.SetText("");
        skillData = data;
    }
    /// <summary>
    /// ���콺�� �ش� ��ų UI�� �������� �� ������ ǥ��
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("���콺 ����");
        if(skillData != null && tooltip != null)
        {
            tooltip.ShowSkillToolTip(skillData, eventData.position);
        }
    }
    /// <summary>
    /// ���콺�� ��ų UI���� ����� �� ������ ����
    /// </summary>
    /// <param name="eventData">param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if(tooltip != null)
        {
            tooltip.HideTooltip();
        }
    }

  
}
