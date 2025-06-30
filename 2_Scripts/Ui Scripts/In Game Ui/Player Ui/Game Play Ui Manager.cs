using CameraSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 게임 중 키를 입력하여 설정창 UI를 제어하는 클래스
/// 
/// <para>사용 변수</para>
/// <para>private GameObject uiGameObject</para>
/// <para>public PlayerCameraMove playerCameraMove</para>
/// <para>public PlayerManager playerManager</para>
/// <para>public InventoryUi inventoryUi</para>
/// <para>public Inventory inventory</para>
/// 
/// <para>사용 메서드</para>
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
    /// 설정 UI 창을 켜고 끌 때 플레이어와 카메라 상태를 제어합니다.
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
    /// 외부에서 UI 활성화 여부를 받아 UI 상태를 조절합니다.
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
    /// ESC를 눌러 설정창을 닫고 UI 관련 상태를 초기화합니다.
    /// </summary>
    public void CloseUiMenu()
    {
        /*uiGameObject.SetActive(false);
        playerCameraMove.canCameraMove = true;*/
        isOpen = false;
        CallSettingUi(false);        
    }
    /// <summary>
    /// UI Scene으로 전환 (예: 메인메뉴로 돌아가기)
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
    /// 게임 종료 (에디터/빌드 환경 모두 대응)
    /// </summary>
    public void GameQuit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif 
        Application.Quit();
    }
}
