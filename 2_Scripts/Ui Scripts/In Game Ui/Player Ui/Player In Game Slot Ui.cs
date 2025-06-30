using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �÷��̾� ��ų �� ������ ���Կ��� ���������� ����ϴ� UI ��Ҹ� ������ ���̽� Ŭ����
///
/// <para>��� ����</para>
/// <para>public Image displayIcon </para>
/// <para>public TextMeshProUGUI cooldownText, keyCodeNameText</para>
/// <para>public ToolTipUi tooltip</para>
///
/// <para>private float cooldownTimer, cooldownTime </para>
/// <para>private bool isCooldown </para>
///
/// <para>��� �޼���</para>
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
    /// �־��� �ð���ŭ ��ٿ��� �����ϰ�, ������ ���� ���� �� ��ٿ� �ؽ�Ʈ Ȱ��ȭ
    /// </summary>
    /// <param name="time">��ٿ� ���� �ð� (��)</param>
    public void StartCooldown(float time)
    {
        cooldownTime = time;
        cooldownTimer = time;
        isCooldown = true;
        displayIcon.color = new Color(1, 1, 1, 0.5f);
    }
    /// <summary>
    /// �� �����Ӹ��� ��ٿ� Ÿ�̸Ӹ� ���ҽ�Ű��, �ؽ�Ʈ �� ������ ���¸� ������
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
