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
/// <para>public MainUIButton mainUIButton</para>
/// <para>public InventoryUi inventoryUi</para>
/// <para>public Inventory inventory</para>
/// <para>public enum ConfirmType { None, ReturnToRoddy, QuitGame} </para>
/// <para> private ConfirmType currentConfirmType</para>
/// 
/// <para>사용 메서드</para>
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
    /// 설정 UI 창을 켜고 끌 때 플레이어와 카메라 상태를 제어
    /// </summar
    private void CallSettingUi(bool isOpen)
    {
        uiGameObject.SetActive(isOpen);

        PlayerMovementRestrictions(isOpen);
    }
    /// <summary>
    /// 외부에서 UI 활성화 여부를 받아 UI 상태를 조절
    /// </summary>
    public void CallUi(bool isOpen)
    {        
        PlayerMovementRestrictions(isOpen);
    } 
    /// <summary>
    /// UI활성화 시 플레이어의 움직임을 제한함
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
    /// ESC를 눌러 설정창을 닫고 UI 관련 상태를 초기화
    /// </summary>
    public void CloseUiMenu()
    {
        /*uiGameObject.SetActive(false);
        playerCameraMove.canCameraMove = true;*/
        isOpen = false;
        CallSettingUi(false);        
    }
    /// <summary>
    /// 게임 데이터 저장여부를 묻는 UI창을 띄움 
    /// </summary>
    /// <param name="type"></param>
    public void CallAgreeButtonGroup(ConfirmType type) 
    {
        currentConfirmType = type;
        mainUIButton.AgreeButtonGroup.SetActive(true);
    }
    /// <summary>
    /// 게임 데이터 저장을 수락할 시 ConfirmType에 따라 해당하는 ConfirmType실행 
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
    /// 게임 데이터 저장을 거절할 시 게임 데이터를 비우고 게임을 종료
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
