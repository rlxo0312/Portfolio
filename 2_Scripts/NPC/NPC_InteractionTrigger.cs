using CameraSetting;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// NPC와의 상호작용을 트리거하는 클래스
/// 플레이어가 범위 안에 들어오면 상호작용 키를 표시하고, F 키 입력 시 대화 시스템을 시작함
/// 대화 시작 시 플레이어 이동 및 카메라 제어 비활성화, 대화 종료 시 복구
/// 
/// <para>사용 변수</para>
/// <para>public: questIcon(GameObject), npc_data(NPC_Data)</para>
/// <para>private GameObject : interactionKey, characterImage, characterInfoImage, player</para>/// 
/// <para>private Transform : playerSpot, CameraSpot,</para>
/// <para>private : PlayerManagerReference playerManagerRef, PlayerMove playerMove, PlayerCameraMove playerCameraMove</para>
/// <para>사용 메서드</para>
/// <para>OnTriggerEnter(Collider other), OnTriggerExit(Collider other),</para>
/// <para> StartInteraction(), EndInteraction()</para>
/// </summary>
public class NPC_InteractionTrigger : MonoBehaviour
{
    [Header("상호작용 설정")]
    [SerializeField] private GameObject interactionKey;
    [SerializeField] private Transform playerSpot;
    [SerializeField] private Transform CameraSpot;
    [SerializeField] private GameObject characterImage;
    [SerializeField] private GameObject characterInfoImage;
     public GameObject questIcon;
    //[SerializeField]private PlayerDialogue playerDialogue;
    //[SerializeField] private PlayerDialogue playerDialogue;
    //[SerializeField] private List<string> dialogueLines;

    [Header("NPC Data")]
    public NPC_Data npc_data;

    private bool isPlayerInRange;
    private GameObject player;
    //private PlayerManager playerManager;
    private PlayerManagerReference playerManagerRef;
    private PlayerMove playerMove;
    private PlayerCameraMove playerCameraMove;
    //private NPC_Client npc_Client;

    /// <summary>
    /// 플레이어가 상호작용 범위에 들어올 때 호출됨
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactionKey.SetActive(true);
            isPlayerInRange = true;
            player = other.gameObject; 

            //playerManager = player.GetComponent<PlayerManager>();
            playerMove = player.GetComponent<PlayerMove>();
            playerCameraMove = Camera.main.GetComponent<PlayerCameraMove>();
            playerManagerRef = player.GetComponent<PlayerManagerReference>();
        }
    }
    /// <summary>
    /// 플레이어가 상호작용 범위를 벗어났을 때 호출됨
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactionKey.SetActive(false);
            isPlayerInRange = false;
            player = null;
        }
    }
    /// <summary>
    /// 대화 종료 시 플레이어 및 카메라 상태 복구
    /// </summary>
    public void EndInteraction()
    {
        interactionKey.SetActive(true);
        playerManagerRef.PlayerManager.canMove = true;
        playerManagerRef.PlayerManager.canRotate = true;
        //playerManager.canCombo = true;
        playerMove.enabled = true;
        player.GetComponent<CharacterController>().enabled = true;
        playerCameraMove.canCameraMove = true;  

        if(characterImage != null)
        {
            characterImage.SetActive(true);
        }
        if(characterInfoImage != null)
        {
            characterInfoImage.SetActive(true);
        }

        interactionKey.SetActive(true);
        Debug.Log("[NPC_InteractionTrigger] 대화 종료 후 상태 복원 완료");
    }
    /*private void Start()
    {
        playerDialogue = FindObjectOfType<PlayerDialogue>();
    }*/
    private void Awake()
    {
       /* if (playerDialogue == null)
        {
            playerDialogue = FindObjectOfType<PlayerDialogue>();
            Debug.LogWarning("[NPC_InteractionTrigger] playerDialogue를 FindObjectOfType로 자동 연결");
        }*/

    }
    
    // 입력 감지 후 F 키로 상호작용 시작   
    private void Update()
    {
        if(isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            StartInteraction();
        }
    }
    /// <summary>
    /// NPC와의 대화 시작: 플레이어 및 카메라 이동 정지, 대화 UI 시작
    /// </summary>
    private void StartInteraction()
    {
        if (player == null || npc_data == null)
        {
            Debug.LogWarning("[NPC_InteractionTrigger] player 또는 npc_data가 null입니다.");
            return;
        }

        if (PlayerDialogue.instance == null)
        {
            Debug.LogError("[NPC_InteractionTrigger] playerDialogue가 null입니다! 인스펙터 연결 또는 오브젝트 활성 여부 확인 필요.");
            return;
        }

        if (npc_data.dialogueLines == null)
        {
            Debug.LogError("[NPC_InteractionTrigger] dialogueLines가 null입니다!");
        }
        else if (npc_data.dialogueLines.Count == 0)
        {
            Debug.LogError("[NPC_InteractionTrigger] dialogueLines.Count가 0입니다!");
        }
        else if (npc_data.dialogueLines[0] == null)
        {
            Debug.LogError("[NPC_InteractionTrigger] dialogueLines[0]이 null입니다!");
        }
        //Debug.Log($"[DEBUG] npc_data != null: {npc_data != null}");
        //Debug.Log($"[DEBUG] npc_data.npcName: {(npc_data.npcName != null ? npc_data.npcName : "NULL")}");
        //Debug.Log($"[DEBUG] dialogueLines count: {npc_data.dialogueLines?.Count}");
        //GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null || npc_data == null)
        {
            return;
        }
        if (playerCameraMove == null)
        {
            Debug.LogError("[NPC_InteractionTrigger] playerCameraMove가 null입니다! MainCamera에 PlayerCameraMove가 붙어 있는지 확인하세요.");
            return;
        }
        interactionKey.SetActive(false);
        //playerManager.playerAnimationManager.PlayerTargetAnimation("Idle", false, false); // 추가점
        playerMove.playerAnimator.SetFloat("MoveAmount", 0);
        playerMove.playerAnimator.SetBool("IsRun", false);
        playerMove.playerAnimator.SetBool("IsBack", false);
        playerMove.playerAnimator.SetBool("IsBackFast", false);
        playerManagerRef.PlayerManager.canMove = false;
        playerManagerRef.PlayerManager.canRotate = false;
        playerManagerRef.PlayerManager.canCombo = false;
        playerMove.enabled = false;
        player.GetComponent<CharacterController>().enabled = false;
        playerCameraMove.canCameraMove = false;
        
        Camera.main.transform.position = CameraSpot.position;
        Camera.main.transform.rotation = CameraSpot.rotation;
        player.transform.position = playerSpot.position;
        player.transform.rotation = playerSpot.rotation;
        

        if (characterImage != null)
        {
            characterImage.gameObject.SetActive(false);
        }
        if (characterInfoImage != null)
        {
            characterInfoImage.gameObject.SetActive(false);
        }


        Debug.Log("[NPC_InteractionTrigger] 대화준비 완료");
        //playerDialogue.StartDialogue(npc_data.npcName, npc_data.dialogueLines, this);
        PlayerDialogue.instance.StartDialogue(npc_data.npcName, npc_data.dialogueLines, this);
    }
}
