using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
/// <summary>
/// �÷��̾��� �ִϸ��̼��� �����ϴ� Ŭ���� 
/// <para>������� �Լ�</para>
/// <para>PlayerTargetAnimation</para>
/// <para>PlayerTargerAnimation("�ִϸ��̼� �̸�", bool isPerfomingActiom, bool appllyRootMotion, bool canRotate, bool canMove)</para>
/// </summary>
public class PlayerAnimationManager : MonoBehaviour//, IAttackable
{
    //PlayerManager playerManager;
    PlayerManagerReference playerManagerRef;
    //public ManualCollider manualCollider;
    [Header("�ִϸ��̼� ���� ����")]
    [SerializeField]public Animator animator;
    // ������Ʈ �ʱ�ȭ
    private void Awake()
    {
        //playerManager = GetComponent<PlayerManager>();
        animator = GetComponentInChildren<Animator>();
        //manualCollider = GetComponent<ManualCollider>();    
    }
    private void Start()
    {
        playerManagerRef = GetComponent<PlayerManagerReference>();
    }
    /// <summary>
    /// ������ �ִϸ��̼��� �����ϰ�, �÷��̾��� ���� ���� �Բ� ������
    /// </summary>
    /// <param name="targetAnimation">������ �ִϸ��̼� �̸�</param>
    /// <param name="isPerformingAction">�ൿ ���� �� ����</param>
    /// <param name="applyRootMotion">RootMotion ���� ����</param>
    /// <param name="canRotate">ȸ�� ���� ����</param>
    /// <param name="canMove">�̵� ���� ����</param>
    public void PlayerTargetAnimation(string targetAnimation, bool isPerformingAction, bool applyRootMotion = true, bool canRotate = false, bool canMove = false)
    {
        animator.CrossFade(targetAnimation, 0.2f);
        playerManagerRef.PlayerManager.isPerformingAction = isPerformingAction;
        playerManagerRef.PlayerManager.applyRootMotion = applyRootMotion;
        playerManagerRef.PlayerManager.canRotate = canRotate;
        playerManagerRef.PlayerManager.canMove = canMove;
    }
   
}
