using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

namespace CameraSetting
{
    /// <summary>
    /// �÷��̾��� �������� �����ϴ� Ŭ����
    /// 
    /// <para>��� ����</para>
    /// <para>playerSpeed, runSpeed, backSpeed, jumpForce, groundCheck ���� ����</para>
    /// <para>public bool IsGrounded, IsSeat, isInputCtrl, PlayerAnimationManager ��</para>
    /// 
    /// <para>��� �޼���</para>
    /// <para>GroundCheck() - ���� ���� �� �ִϸ����� �ݿ�</para>
    /// <para>Movement() - �⺻/����/�ɱ�/���/�޺�/���� �� ��� �̵� ó��</para>
    /// <para>HandleAttackAction() - �⺻ ���� ó��</para>
    /// <para>HandleComboAttack() - �޺� ���� Ʈ����</para>
    /// <para>HandleDefenceInput() - ��� �ִϸ��̼� ó��</para>
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMove : MonoBehaviour
    {
        //PlayerManager playerManager;
        PlayerManagerReference playerManagerRef;
        [Header("�÷��̾� ������ ���� ����")]
        [SerializeField] private float playerSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] private float jumpForce;
        [SerializeField] private float backSpeed;
        [SerializeField] private float faseBackSpeed;

        private bool isSeat = true;
        public bool IsSeat => isSeat;
        private bool isComboAttack = false;
        private bool hasAttacked = false;
        /*private bool isSeatBlocking = false;
        public bool IsSeatBlocking => isSeatBlocking;*/
        //private bool isJumpAttack = false;
        [HideInInspector] public bool isInputCtrl = false;        

        private CharacterController characterController;
        [Header("ī�޶� ���� ����")]
        [SerializeField] PlayerCameraMove playerCameraMove;
        [SerializeField] float smoothRotation = 100;
        Quaternion targetRotation;
        
        [Header("�������� ����")]
        [SerializeField] private float gravityModifier = 3f;
        [SerializeField] private Vector3 groundCheckPoint;
        [SerializeField] private float groundCheckRadius;
        [SerializeField] private LayerMask groundLayer;
        private bool isGrounded;
        public bool IsGrounded => isGrounded;
        [Header("�̵��ӵ�")]
        [SerializeField]private float activeMoveSpeed;
        private Vector3 movement;

        [Header("�ִϸ�����")]
        public Animator playerAnimator;
         
        [Header("�÷��̾� �ִϸ��̼� �Ŵ���")]
        [HideInInspector] public PlayerAnimationManager playerAnimationManager;

        [Header("�÷��̾� �ߴϸ��̼� ���ӽð�")]
        public float seatHitExitSetBoolTIme = 0.0f;
        public float seatBlockHItExitBoolTime = 0.0f;
        public float defenseHitExitTime = 0.0f; 
        private void Awake()
        {
            //playerManager = GetComponent<PlayerManager>();
        }
        // Start is called before the first frame update
        void Start()
        {
            characterController = GetComponent<CharacterController>();
            playerAnimator = GetComponentInChildren<Animator>();
            characterController.enabled = true;
            playerManagerRef = GetComponent<PlayerManagerReference>();
        }

        // Update is called once per frame
        void Update()
        {
           
            hasAttacked = false;
            Movement();
            if (!hasAttacked)
            {
                HandleComboAttack();
            }
        }
        /// <summary>
        /// ���� ���� �� �ִϸ����Ϳ� Ground ���¸� �ݿ��մϴ�.
        /// </summary>
        public void GroundCheck()
        {
            isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckPoint), groundCheckRadius, groundLayer);
            playerAnimator.SetBool("IsGround", isGrounded);
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawWireSphere(transform.TransformPoint(groundCheckPoint), groundCheckRadius);
        }
        /// <summary>
        /// ĳ������ �̵�, ����, �ɱ�, ���, �޺� �� ��ü �Է��� ó���մϴ�.
        /// </summary>
        private void Movement()
        {            
            if (playerManagerRef.PlayerManager.isPerformingAction)
            {
                return;
            }
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 moveInput = new Vector3(horizontal, 0, vertical).normalized;
            float moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

            Vector3 moveDirection = playerCameraMove.transform.forward * moveInput.z +
                                    playerCameraMove.transform.right * moveInput.x;
            moveDirection.y = 0;
            //s�Է½� �ڷ� ���� �ִϸ��̼� �� �ڷ� ������ ����
            if (vertical < 0 || Input.GetKey(KeyCode.S))
            {
               if(moveAmount == 0)
               {
                    playerAnimator.SetBool("IsBack", false);
               }
               else
               {
                    activeMoveSpeed = backSpeed;
                    moveAmount = -0.2f;                    
                    //playerAnimator.SetFloat("MoveAmount", moveAmount, 0.2f, Time.deltaTime);
                    playerAnimator.SetBool("IsBack", true);                    
                    //Debug.Log($"playerMoveBackSpeed : {activeMoveSpeed}");
               }
            }
            else
            {
                activeMoveSpeed = playerSpeed;
            }
            //s + shift�Է½� ������ �ڷΰ��� �ִϸ��̼� �� �ڷ� ������ ����
            if(Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.LeftShift))
            {

                activeMoveSpeed = faseBackSpeed;
                playerAnimator.SetBool("IsBackFast", true);
                playerManagerRef.PlayerManager.playerAudioManager.RunStepSFX();
                moveAmount = -0.4f;
                //playerAnimator.SetFloat("MoveAmount", moveAmount, 0.2f, Time.deltaTime);
                //Debug.Log($"playerMoveBackfastSpeed : {activeMoveSpeed}");
            }
            else
            {
                playerAnimator.SetBool("IsBackFast", false);
                activeMoveSpeed = playerSpeed;
            }
            //w + shift�Է½� �޸��� ����  
            if (Input.GetKey(KeyCode.W) &&Input.GetKey(KeyCode.LeftShift))
            {
                if(moveAmount == 0)
                {
                    //playerManager.playerAudioManager.PlayAllStop();
                    playerAnimator.SetBool("IsRun", false);
                }
                else
                {
                    activeMoveSpeed = runSpeed;
                    moveAmount++;
                    playerAnimator.SetBool("IsRun", true);
                    //Debug.Log($"playerrunspeed: {activeMoveSpeed}");                    
                    //Debug.Log($"moveDirection ��:{moveAmount}");
                }
                
            }
            else
            {
                activeMoveSpeed = playerSpeed;
                playerAnimator.SetBool("IsRun", false);
                //playerManager.playerAudioManager.PlayAllStop();
            }
              
            float yValue = movement.y;
            movement = moveDirection * activeMoveSpeed;
            movement.y = yValue;
                       
            GroundCheck();
            //ĳ���Ͱ� ������ ���� �� y�� ���� 0���� ����
            if (characterController.isGrounded)
            {
                movement.y = 0;
            }
            //���� ���� 
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                //isComboAttack = false; 
                movement.y = jumpForce;
                playerManagerRef.PlayerManager.playerAnimationManager.PlayerTargetAnimation("Jump",false,false,true,true);                
                playerAnimator.CrossFade("Jump", 0.2f);               
            }
           /* else if(!isGrounded && Input.GetMouseButton(0))
            {
                playerAnimator.SetBool("isJumpAttack", true);
                playerManager.playerAnimationManager.PlayerTargetAnimation("Jump Attack", false, false,true, true); 
            } */
            else if (!isGrounded)
            {
                if (Input.GetMouseButton(0))
                {                    
                    playerAnimator.SetBool("isJumpAttack", true);
                    playerManagerRef.PlayerManager.playerAnimationManager.PlayerTargetAnimation("Jump Attack", false, false, true, true);
                }                 
            }
            else
            {                
                playerAnimator.SetBool("isJumpAttack", false);
            }
            movement.y += Physics.gravity.y * gravityModifier * Time.deltaTime;
            //ĳ������ ������ ���� moveDirection�� �μ��� ��� 
            if (moveAmount > 0 )
            {
                targetRotation = Quaternion.LookRotation(moveDirection);                
                Invoke("PlayFootSoundDelayed", 0.995f);
                //playerManager.playerAudioManager.PlayFootSoundSFX();
            }
            else
            {
                
                playerManagerRef.PlayerManager.playerAudioManager.PlayAllStop();
            }
            
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, smoothRotation);

            //ĳ���Ͱ� �������� ���ɽ� Blend Tree�� ������ ���� 
            if (playerManagerRef.PlayerManager.canMove)
            {
                characterController.Move(movement * Time.deltaTime);
                //float moveAmountRange = Mathf.Clamp(moveAmount, 0f, 2f);
                playerAnimator.SetFloat("MoveAmount", moveAmount, 0.2f, Time.deltaTime);
            }

            if (isGrounded && Input.GetKeyDown(KeyCode.LeftControl))
            {
                isInputCtrl = true;
                isComboAttack = false;
                if (isSeat)
                {
                    playerAnimator.SetBool("IsSeat", true);
                    playerManagerRef.PlayerManager.playerAnimationManager.PlayerTargetAnimation("Seat", false, false, true, false);
                }                
                else
                {
                    playerAnimator.SetBool("IsSeat", false);
                    playerManagerRef.PlayerManager.playerAnimationManager.PlayerTargetAnimation("Seat and Wake up", true, false, true, false);
                    isInputCtrl = false;
                    isComboAttack = true;
                }
                isSeat = !isSeat;
                /*Debug.Log($"Check isPerformingActionbool: {playerManager.isPerformingAction}");
                Debug.Log($"Check canMove bool: {playerManager.canMove}");
                Debug.Log($"Check applyRootMotion bool: {playerManager.applyRootMotion}");
                Debug.Log($"Check canRotate bool: {playerManager.canRotate}");*/
            }            

            /*if(isGrounded && !isInputCtrl && Input.GetMouseButton(1))
            {
                playerAnimator.SetBool("isDefense", true);
                HandleDefenceInput();
            }
            if(Input.GetMouseButtonUp(1))
            {
                playerAnimator.SetBool("isDefense", false);
                playerManager.playerAnimationManager.PlayerTargetAnimation("Defense and Idle", false);
            }*/

            if (isGrounded && isInputCtrl && Input.GetMouseButton(0))
            {
                playerAnimator.SetBool("isSeatAttack", true);                
                playerManagerRef.PlayerManager.playerAnimationManager.PlayerTargetAnimation("Seat and Attack", false, false, true, false);
                //isInputCtrl = false;
                isComboAttack = false;
            }            
            else if(isGrounded && !isInputCtrl &&Input.GetMouseButton(0)) 
            {                
                isComboAttack = true;
                HandleAttackAction();                
                
            }
            else if (isGrounded && !isInputCtrl && Input.GetMouseButton(1))
            {
                //playerAnimator.SetBool("isDefense", true);
                HandleDefenceInput();
            }            
            else if (isGrounded && isInputCtrl && Input.GetMouseButton(1))
            {
                playerManagerRef.PlayerManager.isSeatBlocking = true;                
                playerAnimator.SetBool("isSeatBlock", true);
                playerManagerRef.PlayerManager.playerAnimationManager.PlayerTargetAnimation("Seat Block", false, false, true, false);
            }
            else
            {
                playerManagerRef.PlayerManager.isSeatBlocking = false;
                playerAnimator.SetBool("isSeatAttack", false);                
                playerAnimator.SetBool("isSeatBlock", false);
                //playerAnimator.SetBool("isDefense", false);
                isComboAttack = true;                      
            }
            
        }
        private void PlayFootSoundDelayed()
        {
            playerManagerRef.PlayerManager.playerAudioManager.PlayFootSoundSFX();
        }
        /// <summary>
        /// ���� Ű �Է� �� �⺻ ������ �����մϴ�.
        /// </summary>
        private void HandleAttackAction()
        {
            if (hasAttacked)
            {
                return;
            }
            playerManagerRef.PlayerManager.playerAnimationManager.PlayerTargetAnimation("ATK_01", true, true, true, false);
            playerManagerRef.PlayerManager.canCombo = true;
            hasAttacked = true;
        }
        /// <summary>
        /// �޺� ���� �Է��� �����ϰ� �����մϴ�.
        /// </summary>
        private void HandleComboAttack()
        {
            if (!playerManagerRef.PlayerManager.canCombo || !isComboAttack || hasAttacked)
            {
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (isInputCtrl)
                {
                    return;
                }
                playerManagerRef.PlayerManager.animator.SetTrigger("DoAttack"); 
                
            }            
        }
        /// <summary>
        /// ��� Ű �Է� �� ��� �ִϸ��̼��� �����մϴ�.
        /// </summary>
        private void HandleDefenceInput()
        {
            playerManagerRef.PlayerManager.playerAnimationManager.PlayerTargetAnimation("Denfence",true, true, true, true);
           
        }
    }
}