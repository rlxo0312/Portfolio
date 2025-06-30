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
/// <para>public InventoryUi inventoryUi</para>
/// <para>public Inventory inventory</para>
/// 
/// <para>��� �޼���</para>
/// <para>private void Update()</para>
/// <para>private void CallSettingUi(bool isOpen)</para>
/// <para>public void CallUi(bool isOpen)</para>
/// <para>public void CloseUiMenu()</para>
/// <para>public void ReturnToUIScene()</para>
/// <para>public void GameQuit()</para>
/// </summary>
public class GamePlayUiManager : MonoBehaviour
{
    [SerializeField] private GameObject uiGameObject;
    public PlayerCameraMove playerCameraMove;
    public PlayerManager playerManager;
    public InventoryUi inventoryUi;
    public Inventory inventory;
    public PlayerStatUi playerStatUi;   
    //private UiManager uiManager;
    private bool isOpen = false;
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
    /// ���� UI â�� �Ѱ� �� �� �÷��̾�� ī�޶� ���¸� �����մϴ�.
    /// </summar
    private void CallSettingUi(bool isOpen)
    {
        uiGameObject.SetActive(isOpen);

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
    /// �ܺο��� UI Ȱ��ȭ ���θ� �޾� UI ���¸� �����մϴ�.
    /// </summary>
    public void CallUi(bool isOpen)
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
    /// ESC�� ���� ����â�� �ݰ� UI ���� ���¸� �ʱ�ȭ�մϴ�.
    /// </summary>
    public void CloseUiMenu()
    {
        /*uiGameObject.SetActive(false);
        playerCameraMove.canCameraMove = true;*/
        isOpen = false;
        CallSettingUi(false);        
    }
    /// <summary>
    /// UI Scene���� ��ȯ (��: ���θ޴��� ���ư���)
    /// </summary>
    public void ReturnToUIScene()
    {        
        LoadingUI.LoadScene("UI Scene");
    }
    /*public void CantMoveInUi()
    {
        playerCameraMove.GetComponentInParent<PlayerMove>(); 
        
    }*/
    // <summary>
    /// ���� ���� (������/���� ȯ�� ��� ����)
    /// </summary>
    public void GameQuit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif 
        Application.Quit();
    }
}
