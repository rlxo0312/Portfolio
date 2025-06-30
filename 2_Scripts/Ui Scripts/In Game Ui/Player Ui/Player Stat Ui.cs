using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// 플레이어의 현재 스탯(Hp, Mp, 공격력, 방어력)을 UI에 실시간으로 표시하는 클래스
/// 
/// <para>사용 변수</para>
/// <para>public GameObject statUiPanel</para>
/// <para>public TextMeshProUGUI hpText, mpText, attackText, defenseText</para>
/// <para>public GamePlayUiManager gamePlayUiManager</para>
/// 
/// <para>사용 메서드</para>
/// <para>private void OnEnable()</para>
/// <para>private void OnDisable()</para>
/// <para>private void UpdateStatText()</para>
/// <para>public void OpenStatUi()</para>
/// </summary>
public class PlayerStatUi : MonoBehaviour
{
    public GameObject statUiPanel;   
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;
    public GamePlayUiManager gamePlayUiManager;
    private StringBuilder sb = new StringBuilder();
    private bool isSubscribed = false;

    private void Awake()
    {
       
    }
    /// <summary>
    /// UI가 활성화될 때 현재 스탯을 표시하고, 이벤트를 구독함
    /// </summary>
    private void OnEnable()
    {
        UpdateStatText();
        var playerManager = gamePlayUiManager?.playerManager;
        if (playerManager != null && !isSubscribed)
        {
            playerManager.OnChangerStats += UpdateStatText;
            isSubscribed = true;
        }
    }
    /// <summary>
    /// UI가 비활성화될 때 이벤트 구독을 해제함
    /// </summary>
    private void OnDisable()
    {
        var playerManager = gamePlayUiManager?.playerManager;
        if (playerManager != null && isSubscribed)
        {
            playerManager.OnChangerStats -= UpdateStatText;
            isSubscribed = false;
        }
    }
    /// <summary>
    /// 플레이어의 현재 HP, MP, 공격력, 방어력을 UI에 반영함
    /// </summary>
    private void UpdateStatText()
    {
        var playerManger = gamePlayUiManager.playerManager;

        if(playerManger == null || playerManger.playerData == null)
        {
            return;
        }

        sb.Clear();
        sb.Append((int)playerManger.HP).Append("/").Append((int)playerManger.MaxHP);
        hpText.text = sb.ToString();

        sb.Clear();
        sb.Append((int)playerManger.MP).Append("/").Append((int)playerManger.MaxMP); 
        mpText.text = sb.ToString();

        sb.Clear();
        sb.Append(playerManger.AttackPower);
        attackText.text = sb.ToString();

        sb.Clear();
        sb.Append(playerManger.Defense);
        defenseText.text = sb.ToString();   
    }
    /// <summary>
    /// 스탯창 UI를 열거나 닫음 
    /// 열릴 경우 스탯을 갱신하고, UI 제어 상태를 GamePlayUiManager에 전달함
    /// </summary>
    public void OpenStatUi()
    {
        if(statUiPanel == null && gamePlayUiManager == null)
        {
            return;
        }
        bool isActive = !statUiPanel.activeSelf;
        statUiPanel.SetActive(isActive); 

        if(gamePlayUiManager.playerManager != null)
        {
            gamePlayUiManager.CallUi(isActive);
        } 

        if(isActive)
        {
            UpdateStatText();
        }
    }

}
