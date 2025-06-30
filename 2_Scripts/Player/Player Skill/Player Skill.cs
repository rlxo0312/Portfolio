using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.UIElements.UxmlAttributeDescription;

/// <summary>
/// 플레이어가 사용할 수 있는 스킬 정보를 바탕으로 실제 실행 및 효과를 제어하는 클래스.
/// 스킬 발동, 애니메이션 트리거, 버프 적용, 이펙트 생성, 디버프 처리 등 스킬 전반을 처리함.
///
/// <para>사용 변수</para>
/// <para>public PlayerSkillData playerSkillData, public float lastUseTime</para>
/// <para>private KeyCode key, private PlayerSkillUi skillUi, private Animator animator, private PlayerManager playerManager</para>
///
/// <para>사용 메서드</para>
/// <para>public PlayerSkill(PlayerSkillData data, KeyCode key, PlayerSkillUi ui, Animator animator, PlayerManager manager)</para>
/// <para>public bool isReady()</para>
/// <para>public virtual void Active(MonoBehaviour caller)</para>
/// <para>private IEnumerator StartPlayerSkillEffect(PlayerSkillEffectData data, Transform pos), ApplyAttackSkillBuff(PlayerSkillData data, PlayerManager player)
///  ,ApplyDefenseSkillBuff(PlayerSkillData data, PlayerManager player)
///  ,ExpendAndResetPlayerAttackCollider(PlayerSkillData data, PlayerManager player), ResetPlayerInvincibility(PlayerSkillData data, PlayerManager player)
///  ,ResetPlayerAttackSkillMagnification(PlayerSkillData data, PlayerManager player), AttackTriggerTiming(PlayerSkillEffectData data, PlayerManager player)
///  ,PlayerMotionConstraint(PlayerManager player, AnimationClip clip), ResetPlayerIncreasingTargetCount(PlayerSkillData data, PlayerManager player)
///  ,PlayerMonsterDebuff(PlayerSkillData data, PlayerManager player), PlayerHealHP(PlayerSkillData data, PlayerManager player)</para>
/// <para>private void UseMP(PlayerSkillData data, PlayerManager player)</para>
/// <para>public KeyCode GetKey()</para>
/// </summary>
public class PlayerSkill
{
    public PlayerSkillData playerSkillData;
    public float lastUseTime = -999f;
    
    private KeyCode key;
    private PlayerSkillUi skillUi;
    private Animator animator;   
    private PlayerManager playerManager;

    /// <summary>
    /// PlayerSkill 클래스 생성자. 스킬 데이터, 키, UI, 애니메이터, 플레이어를 바탕으로 초기화
    /// </summary>
    public PlayerSkill(PlayerSkillData data, KeyCode key, PlayerSkillUi ui, Animator animator, PlayerManager manager)
    {
        this.playerSkillData = data;
        this.key = key;
        this.skillUi = ui;
        this.animator = animator;
        this.playerManager = manager;

        skillUi.Init(data.skillSprite, key, data);        

    }
    /// <summary>
    /// 스킬이 현재 사용 가능한 상태인지 확인 (쿨타임 고려)
    /// </summary>
    public bool isReady()
    {
        //Debug.Log($"[PlayerSkill]: lastUseTime - {lastUseTime}");
        return Time.time >= lastUseTime + playerSkillData.skillColldown;        
    }
    /// <summary>
    /// 스킬 이펙트를 생성하고 투사체 여부에 따라 처리
    /// </summary>
    /// <param name="data">이펙트 데이터</param>
    /// <param name="pos">이펙트 생성 위치</param>
    /// <returns>코루틴</returns>
    public virtual void Active(MonoBehaviour caller)
    {
        if (!isReady())
        {
            return;
        }
        playerManager.playerMove.GroundCheck();
        if (!playerManager.playerMove.IsGrounded)
        {
            Debug.Log("[PlayerSkill] 플레이어가 공중에 있어 스킬 사용 취소됨.");
            return;
        }

        lastUseTime = Time.time;
        skillUi.StartCooldown(playerSkillData.skillColldown);

        /*if (animator != null)
        {
            bool hasParam = false;
            foreach (var param in animator.parameters)
            {
                if (param.name == playerSkillData.animationTriggerName)
                {
                    hasParam = true;
                    break;
                }
            }

            Debug.Log($"Animator Trigger 확인: {playerSkillData.animationTriggerName}, 존재 여부: {hasParam}");
        }
        for (int i = 0; i < animator.layerCount; i++)
        {
            Debug.Log($"Animator Layer {i} - {animator.GetLayerName(i)} - Weight: {animator.GetLayerWeight(i)}");
        }
*/
        if (!string.IsNullOrEmpty(playerSkillData.animationTriggerName) && animator != null)
        {
            Debug.Log($"[Skill] Trigger 실행: {playerSkillData.animationTriggerName}");
            animator.SetTrigger(playerSkillData.animationTriggerName);
        }

        
        if(playerManager != null && caller != null) 
        {
            if(playerSkillData.isUseMP == true && playerSkillData.useMPAmount > 0f)
            {
                UseMP(playerSkillData, playerManager);
            }
            if(playerSkillData.playerAttackSkillDuration > 0f && playerSkillData.playerBonusAttackPower > 0f)
            {
                caller.StartCoroutine(ApplyAttackSkillBuff(playerSkillData, playerManager));
            }

            if(playerSkillData.playerDefenseSkillDuration > 0f && playerSkillData.playerBonusDefense > 0f)
            {
                caller.StartCoroutine(ApplyDefenseSkillBuff(playerSkillData, playerManager));
            } 

            if(playerSkillData.playerAttackSkillMagnification > 0f && 
                playerSkillData.playerAttackSkillMagnificationDuration > 0f)
            {
                caller.StartCoroutine(ResetPlayerAttackSkillMagnification(playerSkillData, playerManager));
                
            } 

            if(playerSkillData.healAmount > 0f && playerSkillData.isHeal == true)
            {
                caller.StartCoroutine(PlayerHealHP(playerSkillData, playerManager));
            }

            if(playerSkillData.targetCount > 0 && playerSkillData.targetCountDuration > 0f)
            {
                caller.StartCoroutine(ResetPlayerIncreasingTargetCount(playerSkillData, playerManager));
            }

            if(playerSkillData.invincibilityDuration > 0f && playerSkillData.isInvincibility == true)
            {
                caller.StartCoroutine(ResetPlayerInvincibility(playerSkillData, playerManager));
            }

            if(playerManager.manualCollider != null)
            {
                caller.StartCoroutine(ExpendAndResetPlayerAttackCollider(playerSkillData, playerManager));               
            }            
            
            if((playerSkillData.DebuffMonsterAttack > 0f || playerSkillData.DebuffMonsterDefense > 0f)
                && playerSkillData.DebuffMonsterDuration > 0f)
            {
                caller.StartCoroutine(PlayerMonsterDebuff(playerSkillData, playerManager));
            }

            if(playerSkillData.skillAnimationClip != null)
            {                
                caller.StartCoroutine(PlayerMotionConstraint(playerManager, playerSkillData.skillAnimationClip));
            }
        }               

        /*foreach(var effectData in playerSkillData.skillEffectDatas)
        {
            if(effectData.skillEffect != null)
            {
                Transform skillEffectTrs = effectData.skillEffectTransform != null ?
                    effectData.skillEffectTransform : playerManager.transform;
                caller.StartCoroutine(StartPlayerSkillEffect(effectData, skillEffectTrs));
            }
        }*/
        for(int i = 0; i < playerSkillData.skillEffectDatas.Count; i++)
        {
            var effectData = playerSkillData.skillEffectDatas[i];

            if (effectData.isAttackTrigger && effectData.AttackTriggerStartDuration > 0f)
            {
                caller.StartCoroutine(AttackTriggerTiming(effectData, playerManager));
            }

            if (effectData.skillEffect == null)
            {
                continue;
            }
          
            Transform skillEffectTrs =  playerManager.transform;

            if(effectData.useBodyTransform)
            {
                skillEffectTrs = playerManager.bodyTransform;
            }
            else if(effectData.useWeaponTransform)
            {
                skillEffectTrs = playerManager.weaponTransform;
            }
            else if(effectData.useShieldTransform)
            {
                skillEffectTrs = playerManager.shieldTransform;
            }
            else if(effectData.useRightHandTransform)
            {
                skillEffectTrs = playerManager.rightHandTransform;  
            }
            else if(effectData.useLeftHandTransform)
            {
                skillEffectTrs = playerManager.leftHandTransform;
            }
            else if(effectData.skillEffectTransform != null)
            {
                skillEffectTrs = effectData.skillEffectTransform;
            }           

            caller.StartCoroutine(StartPlayerSkillEffect(effectData, skillEffectTrs));            
            
        }      
    }
    /// <summary>
    /// 공격력 버프를 적용하고 일정 시간 후 원상복구
    /// </summary>
    /// <param name="data"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    private IEnumerator StartPlayerSkillEffect(PlayerSkillEffectData data, Transform pos)
    {
        if(data.skillEffect == null || string.IsNullOrEmpty(data.effectPoolKey))
        {
            Debug.Log($"[PlayerSkill] 현재 PlayerSkillData의 skillEffect({data.skillEffect}) 또는 " +
                $"effectPoolKey({data.effectPoolKey})가 null이니 확인해주시길 바랍니다.");
            yield break;
        }

        if(data.playerSkillEffectStartDelay > 0f)
        {
            //yield return new WaitForSeconds(data.playerSkillEffectStartDelay);
            yield return WaitForSecondsCache.Get(data.playerSkillEffectStartDelay);
        }
        for (int i = 0; i < data.skillEffectCount; i++)
        {
            //Vector3 spawnPos = pos.position + data.skillEffectPos;
            Vector3 spawnPos = pos.TransformPoint(data.skillEffectPos);

            Quaternion spawnRotate; //= pos.rotation * Quaternion.Euler(data.skillRotation);
            Quaternion prefabRotation = data.skillEffect.transform.localRotation;
            if (data.skillRotation == Vector3.zero)// Vector3.zero
            {
                //Quaternion prefabRotation = data.skillEffect.transform.rotation;
                //spawnRotate = pos.rotation * Quaternion.Euler(playerManager.transform.forward);
                Quaternion lookRotate = Quaternion.LookRotation(playerManager.transform.forward);
                spawnRotate = lookRotate * prefabRotation;
            }
            else
            {
                //Quaternion prefabRotation = data.skillEffect.transform.localRotation;
                spawnRotate = pos.rotation * Quaternion.Euler(data.skillRotation) * prefabRotation;
            }
            //Debug.Log($"[SkillEffect] 이펙트 부모 Transform: {pos.name}, 위치: {pos.position}");
            //Debug.Log($"[PlayerSkill] 이펙트 풀링 시도: {data.effectPoolKey}, 위치: {spawnPos}");
            GameObject effect = ObjectPoolingManager.Instance.SpawnFromPool(data.effectPoolKey
                    , spawnPos, spawnRotate); //기존 rotation: pos.transform.rotation
            if (effect == null)
            {
                Debug.LogError($"[PlayerSkill] 이펙트 생성 실패: {data.effectPoolKey}가 오브젝트 풀에 없음!");
                yield break;
            }
            else
            {
                //Debug.Log($"[Spawn 성공] {effect.name} 생성됨, 위치: {effect.transform.position}");
            }
            //Debug.Log($"[SkillEffect] 기준 Transform: {pos.name}, 위치: {pos.position}");
            //yield return new WaitForSeconds(data.skillEffectDuration); 
            if (data.isProjectile)
            {
                PlayerProjectile playerProjectile = effect.GetComponent<PlayerProjectile>();
                if(playerProjectile != null)
                {
                    playerProjectile.PoolKey = data.effectPoolKey;
                    //playerProjectile.speed = data.projectileSpeed;
                    //playerProjectile.lifeTime = data.projectileLifeTime;                    
                    playerProjectile.Initilize(data.projectileSpeed, data.projectileLifeTime, playerManager.AttackPower);
                    //float skillDamage = pla
                }
            }
            else
            {
                yield return WaitForSecondsCache.Get(data.skillEffectDuration);
                
                ObjectPoolingManager.Instance.ReturnToPool(data.effectPoolKey, effect);
            }
        }
    }
    /// <summary>
    /// 방어력 버프를 적용하고 일정 시간 후 원상복구
    /// </summary>
    /// <param name="data"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private IEnumerator ApplyAttackSkillBuff(PlayerSkillData data ,PlayerManager player)
    {
        float originalAttackPower = player.AttackPower;
        player.AttackPower += data.playerBonusAttackPower;
        Debug.Log($"[PlayerSkill]: 시전자의 공격력이 {data.playerBonusAttackPower}만큼 증가 " +
            $"현재 공격력: {player.AttackPower + data.playerBonusAttackPower}");

        //yield return new WaitForSeconds(data.playerAttackSkillDuration);
        yield return WaitForSecondsCache.Get(data.playerAttackSkillDuration);

        //player.AttackPower -= data.playerBonusAttackPower;
        player.AttackPower = originalAttackPower;
        Debug.Log("[PlayerSkill]: 공격력 버프 해제");
    }
    /// <summary>
    /// 플레이어 공격 범위(Box Collider)를 일시적으로 확장 후 복구
    /// </summary>
    /// <param name="data"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private IEnumerator ApplyDefenseSkillBuff(PlayerSkillData data, PlayerManager player)
    {
        float originalDefense = player.Defense;
        player.Defense += data.playerBonusDefense;
        Debug.Log($"[PlayerSkill]: 시전자의 방어력이 {data.playerBonusDefense}만큼 증가 " +
            $"현재 방어력이: {player.Defense + data.playerBonusDefense}");

        //yield return new WaitForSeconds(data.playerDefenseSkillDuration);
        yield return WaitForSecondsCache.Get(data.playerDefenseSkillDuration);

        //player.Defense -= data.playerBonusDefense;
        player.Defense = originalDefense;
        Debug.Log("[PlayerSkill]: 방어력 버프 해제");
    }
    /// <summary>
    /// 무적 상태를 일정 시간 유지한 후 해제
    /// </summary>
    /// <param name="data"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private IEnumerator ExpendAndResetPlayerAttackCollider(PlayerSkillData data, PlayerManager player)
    {
        Vector3 originalBoxSize = player.manualCollider.boxSize;
        Vector3 originalBoxCenter = player.manualCollider.boxCentor;
        if(player.manualCollider == null)
        {
            yield break;
        }

        if(data.playerBoxSizeStartDelay > 0f)
        {
            //yield return new WaitForSeconds(data.playerBoxSizeStartDelay);
            yield return WaitForSecondsCache.Get(data.playerBoxSizeStartDelay);
        }

        player.manualCollider.boxSize = data.changePlayerBoxSize;

        Vector3 newBoxCenter = player.transform.InverseTransformDirection(
            player.transform.TransformDirection(data.changePlayerBoxCenter));
        player.manualCollider.boxCentor = newBoxCenter;
        Debug.Log("[Player Skill]캐릭터의 박스 사이즈 확대 및 중심이동");
        
        //yield return new WaitForSeconds(data.returnTime);
        yield return WaitForSecondsCache.Get(data.returnTime);

        player.manualCollider.boxSize = originalBoxSize;
        player.manualCollider.boxCentor = originalBoxCenter;

    }
    /// <summary>
    /// 공격력 배율을 일시적으로 증가시킨 후 원상복구
    /// </summary>
    /// <param name="data"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private IEnumerator ResetPlayerInvincibility(PlayerSkillData data, PlayerManager player)
    {
        player.isInvincibility = true;
        Debug.Log($"[PlayerSkill] 플레이어의 무적지속 시간 :{data.invincibilityDuration}");

        //yield return new WaitForSeconds(data.invincibilityDuration);
        yield return WaitForSecondsCache.Get(data.invincibilityDuration);

        player.isInvincibility = false;
        Debug.Log($"[PlayerSkill] 플레이어의 무적지속 시간 종료");
    }
    /// <summary>
    /// AttackTrigger 발동 시점까지 대기 후 공격 처리 실행
    /// </summary>
    /// <param name="data"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private IEnumerator ResetPlayerAttackSkillMagnification(PlayerSkillData data, PlayerManager player)
    {
        float originalAttackPower = player.AttackPower;

        player.AttackPower *= data.playerAttackSkillMagnification;
        Debug.Log($"[PlayerSkill]플레이어의 공격스킬 배수 :{data.playerAttackSkillMagnification}," +
            $" 현재 총 공격력:{player.AttackPower}");

        //yield return new WaitForSeconds(data.playerAttackSkillMagnificationDuration);
        yield return WaitForSecondsCache.Get(data.playerAttackSkillMagnificationDuration);

        player.AttackPower = originalAttackPower;
        Debug.Log($"[PlayerSkill]플레이어 공격스킬 배율 복구:{originalAttackPower}");
    }
    /// <summary>
    /// 스킬 애니메이션 동안 플레이어 이동, 회전, 루트모션을 제한함
    /// </summary>
    /// <param name="data"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private IEnumerator AttackTriggerTiming(PlayerSkillEffectData data, PlayerManager player)
    {              
        yield return WaitForSecondsCache.Get(data.AttackTriggerStartDuration);
        player.playerAttackManager?.AttackTrigger();        
    }
    /// <summary>
    /// 타겟 수를 일시적으로 증가시킨 후 원상복구
    /// </summary>
    /// <param name="player"></param>
    /// <param name="clip"></param>
    /// <returns></returns>
    private IEnumerator PlayerMotionConstraint(PlayerManager player, AnimationClip clip)
    {
        if (player != null && clip != null )
        {            
            player.isPerformingAction = true;
            player.applyRootMotion = false;
            player.canRotate = false;
            player.canMove = false;

            Debug.Log("[PlayerSkill] 플레이어 제약 시작");
            yield return WaitForSecondsCache.Get(clip.length);

            player.isPerformingAction = false;
            player.applyRootMotion = true;
            player.canRotate = true;
            player.canMove = true;

            Debug.Log("[PlayerSkill] 플레이어 제약 종료");
        }        
    }
    /// <summary>
    /// 주변 몬스터에게 디버프 효과 적용
    /// </summary>
    /// <param name="data"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private IEnumerator ResetPlayerIncreasingTargetCount(PlayerSkillData data, PlayerManager player)
    {
        int originalTargetCount = player.monsterTargetCount;
        player.monsterTargetCount = data.targetCount;
        Debug.Log($"[PlayerSkill]시전자의 스킬 사용 후 몬스터 타겟 수 : {player.monsterTargetCount}");

        yield return WaitForSecondsCache.Get(data.targetCountDuration);

        player.monsterTargetCount = originalTargetCount;
        Debug.Log($"[PlayerSkill]시전자의 몬스터 타겟 수 복구 :{player.monsterTargetCount}");
    }
    /// <summary>
    /// 일정 시간 후 플레이어 HP 회복
    /// </summary>
    /// <param name="data"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private IEnumerator PlayerMonsterDebuff(PlayerSkillData data, PlayerManager player)
    {
        yield return WaitForSecondsCache.Get(data.DebuffMonsterStartDuration);

        Collider[] colliders = player.manualCollider.GetColliderObject();
        //Debug.Log($"[PlayerMonsterDebuff] 감지된 콜라이더 수: {colliders.Length}");
        for (int i = 0; i < colliders.Length; i++)
        {
            var designate = colliders[i];
            EnemyReference enemyRef = designate.GetComponent<EnemyReference>(); 

            if(enemyRef == null || enemyRef.Enemy == null)
            {
                Debug.Log("[PlayerSkill] 현재 사거리 내 몬스터가 없음");
                continue;
            }

            if(!enemyRef.Enemy.IsAlive)
            {
                Debug.Log("[PlayerSkill] 대상이 사망상태 이므로 적용 불가");
                continue;
            }

            enemyRef.Enemy.ApplyDebuff(data.DebuffMonsterAttack, data.DebuffMonsterDefense, 
                data.DebuffMonsterDuration);
            
            Debug.Log($"[PlayerSkill] {enemyRef.Enemy.name}에게 디버프 적용 수치 attackDebuff:{data.DebuffMonsterAttack}" +
                $"DefenseDebuff:{data.DebuffMonsterDefense}, 지속시간:{data.DebuffMonsterDuration}");
        }        
    }
    /// <summary>
    /// 일정 시간 후 플레이어 HP 회복
    /// </summary>
    /// <param name="data"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private IEnumerator PlayerHealHP(PlayerSkillData data, PlayerManager player)
    {
        yield return WaitForSecondsCache.Get(data.healStartDuration);

        if(player.HP <= 0)
        {
            yield break;
        }

        /*if((data.healAmount + player.HP) < player.MaxHP)
        {
            player.HP += data.healAmount;
        } 

        if((data.healAmount + player.HP) >= player.MaxHP)
        {
            player.HP = player.MaxHP; 
        }*/
        float beforeHP = player.HP;
        player.HP = Mathf.Min((player.HP + data.healAmount), player.MaxHP);
        player.OnChangerStats?.Invoke();

        Debug.Log($"[PlayerSkill]체력 회복 : {beforeHP} -> {player.HP}");
    }
    /// <summary>
    /// 스킬 사용 시 MP 소모 처리
    /// </summary>
    /// <param name="data"></param>
    /// <param name="player"></param>
    private void UseMP(PlayerSkillData data, PlayerManager player)
    {
        float beforeMP = player.MP; 
        if(player.MP < data.useMPAmount)
        {
            Debug.Log($"[PlayerSkill]MP가 {data.useMPAmount - beforeMP} 부족 ");
            return; 
        }
        else
        {
            player.MP -= data.useMPAmount;
            Debug.Log($"[PlayerSkill]MP사용:{data.useMPAmount}, 남은 MP:{player.MP - data.useMPAmount}");
            player.OnChangerStats?.Invoke();
        }
    }
    
    public KeyCode GetKey() => key;
}
