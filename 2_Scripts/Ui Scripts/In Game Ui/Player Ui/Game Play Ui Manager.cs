using CameraSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ���� �� Ű�� �Է��Ͽ� ����â UI�� �����ϴ� Ŭ����
/// 
/// <para>��� ����</para>
/// <para>private GameObject uiGameObject</para>
/// <para>public PlayerCameraMove playerCameraMove</para>
/// <para>public PlayerManager playerManager</para>
/// <para>public MainUIButton mainUIButton</para>
/// <para>public InventoryUi inventoryUi</para>
/// <para>public Inventory inventory</para>
/// <para>public enum ConfirmType { None, ReturnToRoddy, QuitGame} </para>
/// <para> private ConfirmType currentConfirmType</para>
/// 
/// <para>��� �޼���</para>
/// <para>private void CallSettingUi(bool isOpen)</para>
/// <para>public void CallUi(bool isOpen)</para>
/// <para>public void CloseUiMenu()</para>
/// <para>public void ReturnToUIScene()</para>
/// <para>public void GameQuit()</para>
/// <para>private void PlayerMovementRestrictions(bool isOpen)</para>
/// </summary>
public class GamePlayUiManager : MonoBehaviour
{
    public enum ConfirmType { None, ReturnToRoddy, QuitGame} 

    [SerializeField] private GameObject uiGameObject;
    public PlayerCameraMove playerCameraMove;
    public PlayerManager playerManager;
    public MainUIButton mainUIButton;
    public InventoryUi inventoryUi;
    public Inventory inventory;
    public PlayerStatUi playerStatUi;    
    public PlayerDataSaver playerDataSaver;
    private ConfirmType currentConfirmType = ConfirmType.None;
    //private UiManager uiManager;
    private bool isOpen = false;
    private void Start()
    {
        mainUIButton.AgreeButton.onClick.AddListener(OnAgree); 
        mainUIButton.RefuseButton.onClick.AddListener(OnRefuse);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isOpen = !isOpen;
            CallSettingUi(isOpen);
            
        }
        if (Input.GetKeyDown(KeyCode.I) && inventoryUi != null)
        {            
            inventoryUi.OpenInventory();
        } 
        if(Input.GetKeyDown(KeyCode.T) && playerStatUi != null)
        {
            playerStatUi.OpenStatUi();
        }
    }
    /// <summary>
    /// ���� UI â�� �Ѱ� �� �� �÷��̾�� ī�޶� ���¸� ����
    /// </summar
    private void CallSettingUi(bool isOpen)
    {
        uiGameObject.SetActive(isOpen);

        PlayerMovementRestrictions(isOpen);
    }
    /// <summary>
    /// �ܺο��� UI Ȱ��ȭ ���θ� �޾� UI ���¸� ����
    /// </summary>
    public void CallUi(bool isOpen)
    {        
        PlayerMovementRestrictions(isOpen);
    } 
    /// <summary>
    /// UIȰ��ȭ �� �÷��̾��� �������� ������
    /// </summary>
    private void PlayerMovementRestrictions(bool isOpen)
    {
        if (playerCameraMove != null)
        {
            playerCameraMove.canCameraMove = !isOpen;
        }
        if (playerManager != null)
        {
            playerManager.isPerformingAction = isOpen;
            playerManager.applyRootMotion = !isOpen;
            playerManager.canRotate = !isOpen;
            playerManager.canMove = !isOpen;
            playerManager.canCombo = !isOpen;
            playerManager.playerMove.playerAnimator.SetFloat("MoveAmount", 0);
            playerManager.playerMove.playerAnimator.SetBool("IsRun", false);
            playerManager.playerMove.playerAnimator.SetBool("IsBack", false);
            playerManager.playerMove.playerAnimator.SetBool("IsBackFast", false);
        }
    }
    /// <summary>
    /// ESC�� ���� ����â�� �ݰ� UI ���� ���¸� �ʱ�ȭ
    /// </summary>
    public void CloseUiMenu()
    {
        /*uiGameObject.SetActive(false);
        playerCameraMove.canCameraMove = true;*/
        isOpen = false;
        CallSettingUi(false);        
    }
    /// <summary>
    /// ���� ������ ���忩�θ� ���� UIâ�� ��� 
    /// </summary>
    /// <param name="type"></param>
    public void CallAgreeButtonGroup(ConfirmType type) 
    {
        currentConfirmType = type;
        mainUIButton.AgreeButtonGroup.SetActive(true);
    }
    /// <summary>
    /// ���� ������ ������ ������ �� ConfirmType�� ���� �ش��ϴ� ConfirmType���� 
    /// </summary>
    public void OnAgree()
    {
        if(playerDataSaver != null)
        {
            playerDataSaver.SaveToJson();
        }

        switch (currentConfirmType)
        {
            case ConfirmType.ReturnToRoddy:
                LoadingUI.LoadScene("UI Scene");
                break;

            case ConfirmType.QuitGame:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
        }
        mainUIButton.AgreeButtonGroup.SetActive(false);
        currentConfirmType = ConfirmType.None;
    }
    /// <summary>
    /// ���� ������ ������ ������ �� ���� �����͸� ���� ������ ����
    /// </summary>
    public void OnRefuse()
    {
        mainUIButton.AgreeButtonGroup.SetActive(false);
        currentConfirmType = ConfirmType.None; 
        if(playerDataSaver != null)
        {
            playerDataSaver.DeleteSaveFile();
        }
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
