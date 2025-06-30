using CameraSetting;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// NPC���� ��ȣ�ۿ��� Ʈ�����ϴ� Ŭ����
/// �÷��̾ ���� �ȿ� ������ ��ȣ�ۿ� Ű�� ǥ���ϰ�, F Ű �Է� �� ��ȭ �ý����� ������
/// ��ȭ ���� �� �÷��̾� �̵� �� ī�޶� ���� ��Ȱ��ȭ, ��ȭ ���� �� ����
/// 
/// <para>��� ����</para>
/// <para>public: questIcon(GameObject), npc_data(NPC_Data)</para>
/// <para>private GameObject : interactionKey, characterImage, characterInfoImage, player</para>/// 
/// <para>private Transform : playerSpot, CameraSpot,</para>
/// <para>private : PlayerManagerReference playerManagerRef, PlayerMove playerMove, PlayerCameraMove playerCameraMove</para>
/// <para>��� �޼���</para>
/// <para>OnTriggerEnter(Collider other), OnTriggerExit(Collider other),</para>
/// <para> StartInteraction(), EndInteraction()</para>
/// </summary>
public class NPC_InteractionTrigger : MonoBehaviour
{
    [Header("��ȣ�ۿ� ����")]
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
    /// �÷��̾ ��ȣ�ۿ� ������ ���� �� ȣ���
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
    /// �÷��̾ ��ȣ�ۿ� ������ ����� �� ȣ���
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
    /// ��ȭ ���� �� �÷��̾� �� ī�޶� ���� ����
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
        Debug.Log("[NPC_InteractionTrigger] ��ȭ ���� �� ���� ���� �Ϸ�");
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
            Debug.LogWarning("[NPC_InteractionTrigger] playerDialogue�� FindObjectOfType�� �ڵ� ����");
        }*/

    }
    
    // �Է� ���� �� F Ű�� ��ȣ�ۿ� ����   
    private void Update()
    {
        if(isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            StartInteraction();
        }
    }
    /// <summary>
    /// NPC���� ��ȭ ����: �÷��̾� �� ī�޶� �̵� ����, ��ȭ UI ����
    /// </summary>
    private void StartInteraction()
    {
        if (player == null || npc_data == null)
        {
            Debug.LogWarning("[NPC_InteractionTrigger] player �Ǵ� npc_data�� null�Դϴ�.");
            return;
        }

        if (PlayerDialogue.instance == null)
        {
            Debug.LogError("[NPC_InteractionTrigger] playerDialogue�� null�Դϴ�! �ν����� ���� �Ǵ� ������Ʈ Ȱ�� ���� Ȯ�� �ʿ�.");
            return;
        }

        if (npc_data.dialogueLines == null)
        {
            Debug.LogError("[NPC_InteractionTrigger] dialogueLines�� null�Դϴ�!");
        }
        else if (npc_data.dialogueLines.Count == 0)
        {
            Debug.LogError("[NPC_InteractionTrigger] dialogueLines.Count�� 0�Դϴ�!");
        }
        else if (npc_data.dialogueLines[0] == null)
        {
            Debug.LogError("[NPC_InteractionTrigger] dialogueLines[0]�� null�Դϴ�!");
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
            Debug.LogError("[NPC_InteractionTrigger] playerCameraMove�� null�Դϴ�! MainCamera�� PlayerCameraMove�� �پ� �ִ��� Ȯ���ϼ���.");
            return;
        }
        interactionKey.SetActive(false);
        //playerManager.playerAnimationManager.PlayerTargetAnimation("Idle", false, false); // �߰���
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


        Debug.Log("[NPC_InteractionTrigger] ��ȭ�غ� �Ϸ�");
        //playerDialogue.StartDialogue(npc_data.npcName, npc_data.dialogueLines, this);
        PlayerDialogue.instance.StartDialogue(npc_data.npcName, npc_data.dialogueLines, this);
    }
}
