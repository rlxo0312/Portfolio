using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
/// <summary>
/// 플레이어의 애니메이션을 제어하는 클래스 
/// <para>사용중인 함수</para>
/// <para>PlayerTargetAnimation</para>
/// <para>PlayerTargerAnimation("애니메이션 이름", bool isPerfomingActiom, bool appllyRootMotion, bool canRotate, bool canMove)</para>
/// </summary>
public class PlayerAnimationManager : MonoBehaviour//, IAttackable
{
    //PlayerManager playerManager;
    PlayerManagerReference playerManagerRef;
    //public ManualCollider manualCollider;
    [Header("애니메이션 제어 변수")]
    [SerializeField]public Animator animator;
    // 컴포넌트 초기화
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
    /// 지정된 애니메이션을 실행하고, 플레이어의 상태 값을 함께 설정함
    /// </summary>
    /// <param name="targetAnimation">실행할 애니메이션 이름</param>
    /// <param name="isPerformingAction">행동 수행 중 여부</param>
    /// <param name="applyRootMotion">RootMotion 적용 여부</param>
    /// <param name="canRotate">회전 가능 여부</param>
    /// <param name="canMove">이동 가능 여부</param>
    public void PlayerTargetAnimation(string targetAnimation, bool isPerformingAction, bool applyRootMotion = true, bool canRotate = false, bool canMove = false)
    {
        animator.CrossFade(targetAnimation, 0.2f);
        playerManagerRef.PlayerManager.isPerformingAction = isPerformingAction;
        playerManagerRef.PlayerManager.applyRootMotion = applyRootMotion;
        playerManagerRef.PlayerManager.canRotate = canRotate;
        playerManagerRef.PlayerManager.canMove = canMove;
    }
   
}
