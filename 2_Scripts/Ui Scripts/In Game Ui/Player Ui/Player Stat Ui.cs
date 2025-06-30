using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// �÷��̾��� ���� ����(Hp, Mp, ���ݷ�, ����)�� UI�� �ǽð����� ǥ���ϴ� Ŭ����
/// 
/// <para>��� ����</para>
/// <para>public GameObject statUiPanel</para>
/// <para>public TextMeshProUGUI hpText, mpText, attackText, defenseText</para>
/// <para>public GamePlayUiManager gamePlayUiManager</para>
/// 
/// <para>��� �޼���</para>
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
    /// UI�� Ȱ��ȭ�� �� ���� ������ ǥ���ϰ�, �̺�Ʈ�� ������
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
    /// UI�� ��Ȱ��ȭ�� �� �̺�Ʈ ������ ������
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
    /// �÷��̾��� ���� HP, MP, ���ݷ�, ������ UI�� �ݿ���
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
    /// ����â UI�� ���ų� ���� 
    /// ���� ��� ������ �����ϰ�, UI ���� ���¸� GamePlayUiManager�� ������
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
