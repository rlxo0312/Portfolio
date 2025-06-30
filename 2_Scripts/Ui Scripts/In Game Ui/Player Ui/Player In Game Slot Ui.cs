using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어 스킬 및 아이템 슬롯에서 공통적으로 사용하는 UI 요소를 정의한 베이스 클래스
///
/// <para>사용 변수</para>
/// <para>public Image displayIcon </para>
/// <para>public TextMeshProUGUI cooldownText, keyCodeNameText</para>
/// <para>public ToolTipUi tooltip</para>
///
/// <para>private float cooldownTimer, cooldownTime </para>
/// <para>private bool isCooldown </para>
///
/// <para>사용 메서드</para>
/// <para>public void StartCooldown(float time) </para>
/// </summary>
public class PlayerInGameSlotUi : MonoBehaviour
{
    public Image displayIcon;
    public TextMeshProUGUI cooldownText;
    public TextMeshProUGUI keyCodeNameText;
    public ToolTipUi tooltip;

    private float cooldownTimer;
    private float cooldownTime;
    private int lastCooldownTextCooldown = -1;   
    private bool isCooldown;
    /// <summary>
    /// 주어진 시간만큼 쿨다운을 시작하고, 아이콘 투명도 변경 및 쿨다운 텍스트 활성화
    /// </summary>
    /// <param name="time">쿨다운 지속 시간 (초)</param>
    public void StartCooldown(float time)
    {
        cooldownTime = time;
        cooldownTimer = time;
        isCooldown = true;
        displayIcon.color = new Color(1, 1, 1, 0.5f);
    }
    /// <summary>
    /// 매 프레임마다 쿨다운 타이머를 감소시키고, 텍스트 및 아이콘 상태를 갱신함
    /// </summary>
    private void Update()
    {
        /*if (isCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownText.text = Mathf.Ceil(cooldownTimer).ToString();

            if (cooldownTimer <= 0)
            {
                isCooldown = false;
                cooldownText.text = "";
                displayIcon.color = new Color(1, 1, 1, 1f);
            }

        }*/ 
        if(!isCooldown)
        {
            return;
        }
        cooldownTimer -= Time.deltaTime;
        int currentCooldown = Mathf.CeilToInt(cooldownTimer);

        if(currentCooldown != lastCooldownTextCooldown)
        {
            if (cooldownText != null)
            {
                //cooldownText.SetText("{0}", currentCooldown);
                cooldownText.text = currentCooldown.ToString();
            }

            lastCooldownTextCooldown = currentCooldown;
        }
        if (cooldownTimer <= 0)
        {
            isCooldown = false;
            //cooldownText.SetText("");
            cooldownText.text = "";
            displayIcon.color = new Color(1, 1, 1, 1f);
            lastCooldownTextCooldown = -1;            
        }
    }

}
