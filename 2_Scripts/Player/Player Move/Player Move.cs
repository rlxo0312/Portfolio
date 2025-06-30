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
    /// 플레이어의 움직임을 제어하는 클래스
    /// 
    /// <para>사용 변수</para>
    /// <para>playerSpeed, runSpeed, backSpeed, jumpForce, groundCheck 관련 값들</para>
    /// <para>public bool IsGrounded, IsSeat, isInputCtrl, PlayerAnimationManager 등</para>
    /// 
    /// <para>사용 메서드</para>
    /// <para>GroundCheck() - 지면 감지 및 애니메이터 반영</para>
    /// <para>Movement() - 기본/점프/앉기/방어/콤보/후진 등 모든 이동 처리</para>
    /// <para>HandleAttackAction() - 기본 공격 처리</para>
    /// <para>HandleComboAttack() - 콤보 공격 트리거</para>
    /// <para>HandleDefenceInput() - 방어 애니메이션 처리</para>
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMove : MonoBehaviour
    {
        //PlayerManager playerManager;
        PlayerManagerReference playerManagerRef;
        [Header("플레이어 움직임 제어 변수")]
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
        [Header("카메라 제어 변수")]
        [SerializeField] PlayerCameraMove playerCameraMove;
        [SerializeField] float smoothRotation = 100;
        Quaternion targetRotation;
        
        [Header("점프제어 변수")]
        [SerializeField] private float gravityModifier = 3f;
        [SerializeField] private Vector3 groundCheckPoint;
        [SerializeField] private float groundCheckRadius;
        [SerializeField] private LayerMask groundLayer;
        private bool isGrounded;
        public bool IsGrounded => isGrounded;
        [Header("이동속도")]
        [SerializeField]private float activeMoveSpeed;
        private Vector3 movement;

        [Header("애니메이터")]
        public Animator playerAnimator;
         
        [Header("플레이어 애니메이션 매니저")]
        [HideInInspector] public PlayerAnimationManager playerAnimationManager;

        [Header("플레이어 야니메이션 지속시간")]
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
        /// 지면 감지 및 애니메이터에 Ground 상태를 반영합니다.
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
        /// 캐릭터의 이동, 점프, 앉기, 방어, 콤보 등 전체 입력을 처리합니다.
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
            //s입력시 뒤로 가는 애니메이션 밑 뒤로 움직임 구현
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
            //s + shift입력시 빠르게 뒤로가는 애니메이션 밑 뒤로 움직임 구현
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
            //w + shift입력시 달리기 구현  
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
                    //Debug.Log($"moveDirection 값:{moveAmount}");
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
            //캐릭터가 땅위에 있을 시 y의 값을 0으로 지정
            if (characterController.isGrounded)
            {
                movement.y = 0;
            }
            //점프 구현 
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
            //캐릭터의 움직임 구현 moveDirection을 인수로 사용 
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

            //캐릭터가 움직임이 가능시 Blend Tree로 움직임 구현 
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
        /// 공격 키 입력 시 기본 공격을 실행합니다.
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
        /// 콤보 공격 입력을 감지하고 실행합니다.
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
        /// 방어 키 입력 시 방어 애니메이션을 실행합니다.
        /// </summary>
        private void HandleDefenceInput()
        {
            playerManagerRef.PlayerManager.playerAnimationManager.PlayerTargetAnimation("Denfence",true, true, true, true);
           
        }
    }
}