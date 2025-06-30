using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraSetting;
/// <summary>
/// �÷��̾��� ���¿� ����� �����ϴ� Ŭ����
/// 
/// <para>��� ����</para>
/// <para>public bool isPerformingAction, applyRootMotion, canRotate, canMove, canCombo, isDead</para>
/// <para>public bool isSeatBlocking, IsBlocking</para>
/// <para>public PlayerMove playerMove, PlayerAnimationManager playerAnimationManager, PlayerAudioManager playerAudioManager, 
/// PlayerInGameUi playerInGameUi,PlayerAttackManager playerAttackManager </para>
/// <para>public Vector3 baseBoxSize</para>
/// <para>��� �޼���</para>
/// <para>public override void BeDamaged(float damage, Vector3 contactPos, GameObject hitEffectPrefabs = null)</para>
/// </summary>
public class PlayerManager : Player
{
    [Header("Common Player Data")]
    [HideInInspector] public CharacterController characterController;
    //[HideInInspector] public  Animator playerAnimator;
    [Header("�÷��̾� ���� ����")]
    public bool isPerformingAction = false;
    public bool applyRootMotion = false;
    public bool canRotate = true;
    public bool canMove = true;
    public bool canCombo = false;
    public bool isDead = false;

    /* [Header("�÷��̾� ������ ����")]
     public bool isPlayerMovePaused = false;*/
    public bool isSeatBlocking = false;
    public bool IsBlocking { get; private set; }
    [Header("Player Manager Scripts")]
    //[HideInInspector] public PlayerAnimationManager playerAnimationManager;
    [HideInInspector] public PlayerMove playerMove;
    [HideInInspector] public PlayerAnimationManager playerAnimationManager;
    [HideInInspector] public PlayerAudioManager playerAudioManager;
    [HideInInspector] public PlayerAttackManager playerAttackManager;
    public PlayerInGameUi playerInGameUi;

    [Header("�÷��̾��� ����Ʈ ��� ��ġ")]
    public Transform weaponTransform;
    public Transform shieldTransform;
    public Transform bodyTransform;
    public Transform rightHandTransform;
    public Transform leftHandTransform;

    [Header("���� Ÿ�� ��")]
    public int monsterTargetCount = 6;
    
    protected override void Start()
    {
        base.Start();
        playerInGameUi.InitializeSlider();
        playerAttackManager = GetComponent<PlayerAttackManager>();
    }

    protected override void Awake()
    {
        base.Awake();
        characterController = GetComponent<CharacterController>();
        playerMove = GetComponent<PlayerMove>();
        playerAnimationManager = GetComponent<PlayerAnimationManager>();    
        playerAudioManager = GetComponent<PlayerAudioManager>();           
        
    }
    protected override void Update()
    {
        base.Update();        
        IsBlocking = Input.GetMouseButton(1);
    }
    public override void BeDamaged(float damage, Vector3 contactPos, GameObject hitEffectPrefabs = null)
    {
        float finalDamage = damage;
        if (!IsAlive)
        {
            isDead = true;
            playerInGameUi.hpSlider.value = 0;
            animator.CrossFade("Dead", 0.2f);
            return;
        }

        // ���ŷ ����
        bool isBlocking = IsBlocking;
        bool isSeat = playerMove.IsSeat;
        bool isSeatBlock = playerMove.playerAnimator.GetBool("isSeatBlock");
               
        if (isBlocking && isSeat && isSeatBlock)
        {
            finalDamage *= 0.8f;
            playerMove.playerAnimator.SetBool("SetBool", true);
            playerAnimationManager.PlayerTargetAnimation("Seat Block Hit", false);
            StartCoroutine(ResetSetBlockHit(playerMove.seatBlockHItExitBoolTime));            
            //Debug.Log("[PlayerManager] ���� ���¿��� ��� ����");            
        }       
        else if (isBlocking)
        {
            finalDamage *= 0.8f;
            playerMove.playerAnimator.SetBool("isBlockHit", true);
            playerAnimationManager.PlayerTargetAnimation("Hit Shield", false);
            StartCoroutine(ResetDefenseHit(playerMove.defenseHitExitTime));
            //Debug.Log("[PlayerManager] �� �ִ� ���¿��� ��� ����");            
        }       
        else if (playerMove.isInputCtrl)
        {
            playerMove.playerAnimator.SetBool("isSeatHit", true);
            playerAnimationManager.PlayerTargetAnimation("Seat HIt", false);
            StartCoroutine(ResetSeatHIt(playerMove.seatHitExitSetBoolTIme));           
            //Debug.Log("[PlayerManager] �ɱ� �� �ǰ�");
        }      
        else
        {
            playerAnimationManager.PlayerTargetAnimation("Hit", false);            
            //Debug.Log("[PlayerManager] �Ϲ� �ǰ�");
        }
        base.BeDamaged(finalDamage, contactPos, hitEffectPrefabs);
        playerInGameUi.hpSlider.value = HP;
        if (DamageTextSpawner.Instance != null && isBlocking)
        {
            DamageTextSpawner.Instance.ShowDamage(transform.position, finalDamage, true);
        }
        else if (DamageTextSpawner.Instance != null && isInvincibility)
        {            
            DamageTextSpawner.Instance.ShowDamage(transform.position, finalDamage, false, true);
        }
        else
        {
            DamageTextSpawner.Instance.ShowDamage(transform.position, finalDamage);
        }
        

        //base.BeDamaged(finalDamage, contactPos, hitEffectPrefabs);           
        OnChangerStats?.Invoke();

        /// <summary>
        /// ���� �ð� �� isSeatHit ���� false�� �ǵ����ϴ�.
        /// </summary>
        IEnumerator ResetSeatHIt(float time)
        {
            //yield return new WaitForSeconds(time);
            yield return WaitForSecondsCache.Get(time);
            playerMove.playerAnimator.SetBool("isSeatHit", false);
        }
        /// <summary>
        /// ���� �ð� �� SeatBlockHitTrigger�� �����մϴ�.
        /// </summary>
        IEnumerator ResetSetBlockHit(float time)
        {
            //yield return new WaitForSeconds(time);
            yield return WaitForSecondsCache.Get(time);
            //playerMove.playerAnimator.SetBool("isSeatBlockHit", false);
            playerMove.playerAnimator.SetBool("SetBool", false);
        }
        IEnumerator ResetDefenseHit(float time)
        {
            //yield return new WaitForSeconds(time);
            yield return WaitForSecondsCache.Get(time);
            playerMove.playerAnimator.SetBool("isBlockHit", false);
        }
    }
}
