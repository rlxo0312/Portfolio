using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 플레이어 스킬 UI를 제어하는 클래스
///
/// <para>사용 변수</para>
/// <para>public PlayerSkillData skillData</para>
///
/// <para>사용 메서드</para>
/// <para>public void Init(Sprite icon, KeyCode key, PlayerSkillData data)</para>
/// <para>public void OnPointerEnter(PointerEventData eventData)</para>
/// <para>public void OnPointerExit(PointerEventData eventData)</para>
/// </summary>
public class PlayerSkillUi : PlayerInGameSlotUi, IPointerEnterHandler, IPointerExitHandler
{
    public PlayerSkillData skillData;

    /// <summary>
    /// 스킬 아이콘, 단축키, 스킬 데이터를 UI에 초기화하는 메서드
    /// </summary>
    /// <param name="icon">스킬 아이콘 이미지</param>
    /// <param name="key">단축키</param>
    /// <param name="data">스킬 데이터</param>
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
    /// 마우스가 해당 스킬 UI에 진입했을 때 툴팁을 표시
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("마우스 진입");
        if(skillData != null && tooltip != null)
        {
            tooltip.ShowSkillToolTip(skillData, eventData.position);
        }
    }
    /// <summary>
    /// 마우스가 스킬 UI에서 벗어났을 때 툴팁을 숨김
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
