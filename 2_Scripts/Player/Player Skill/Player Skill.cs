using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.UIElements.UxmlAttributeDescription;

/// <summary>
/// �÷��̾ ����� �� �ִ� ��ų ������ �������� ���� ���� �� ȿ���� �����ϴ� Ŭ����.
/// ��ų �ߵ�, �ִϸ��̼� Ʈ����, ���� ����, ����Ʈ ����, ����� ó�� �� ��ų ������ ó����.
///
/// <para>��� ����</para>
/// <para>public PlayerSkillData playerSkillData, public float lastUseTime</para>
/// <para>private KeyCode key, private PlayerSkillUi skillUi, private Animator animator, private PlayerManager playerManager</para>
///
/// <para>��� �޼���</para>
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
    /// PlayerSkill Ŭ���� ������. ��ų ������, Ű, UI, �ִϸ�����, �÷��̾ �������� �ʱ�ȭ
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
    /// ��ų�� ���� ��� ������ �������� Ȯ�� (��Ÿ�� ���)
    /// </summary>
    public bool isReady()
    {
        //Debug.Log($"[PlayerSkill]: lastUseTime - {lastUseTime}");
        return Time.time >= lastUseTime + playerSkillData.skillColldown;        
    }
    /// <summary>
    /// ��ų ����Ʈ�� �����ϰ� ����ü ���ο� ���� ó��
    /// </summary>
    /// <param name="data">����Ʈ ������</param>
    /// <param name="pos">����Ʈ ���� ��ġ</param>
    /// <returns>�ڷ�ƾ</returns>
    public virtual void Active(MonoBehaviour caller)
    {
        if (!isReady())
        {
            return;
        }
        playerManager.playerMove.GroundCheck();
        if (!playerManager.playerMove.IsGrounded)
        {
            Debug.Log("[PlayerSkill] �÷��̾ ���߿� �־� ��ų ��� ��ҵ�.");
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

            Debug.Log($"Animator Trigger Ȯ��: {playerSkillData.animationTriggerName}, ���� ����: {hasParam}");
        }
        for (int i = 0; i < animator.layerCount; i++)
        {
            Debug.Log($"Animator Layer {i} - {animator.GetLayerName(i)} - Weight: {animator.GetLayerWeight(i)}");
        }
*/
        if (!string.IsNullOrEmpty(playerSkillData.animationTriggerName) && animator != null)
        {
            Debug.Log($"[Skill] Trigger ����: {playerSkillData.animationTriggerName}");
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
    /// ���ݷ� ������ �����ϰ� ���� �ð� �� ���󺹱�
    /// </summary>
    /// <param name="data"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    private IEnumerator StartPlayerSkillEffect(PlayerSkillEffectData data, Transform pos)
    {
        if(data.skillEffect == null || string.IsNullOrEmpty(data.effectPoolKey))
        {
            Debug.Log($"[PlayerSkill] ���� PlayerSkillData�� skillEffect({data.skillEffect}) �Ǵ� " +
                $"effectPoolKey({data.effectPoolKey})�� null�̴� Ȯ�����ֽñ� �ٶ��ϴ�.");
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
            //Debug.Log($"[SkillEffect] ����Ʈ �θ� Transform: {pos.name}, ��ġ: {pos.position}");
            //Debug.Log($"[PlayerSkill] ����Ʈ Ǯ�� �õ�: {data.effectPoolKey}, ��ġ: {spawnPos}");
            GameObject effect = ObjectPoolingManager.Instance.SpawnFromPool(data.effectPoolKey
                    , spawnPos, spawnRotate); //���� rotation: pos.transform.rotation
            if (effect == null)
            {
                Debug.LogError($"[PlayerSkill] ����Ʈ ���� ����: {data.effectPoolKey}�� ������Ʈ Ǯ�� ����!");
                yield break;
            }
            else
            {
                //Debug.Log($"[Spawn ����] {effect.name} ������, ��ġ: {effect.transform.position}");
            }
            //Debug.Log($"[SkillEffect] ���� Transform: {pos.name}, ��ġ: {pos.position}");
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
    /// ���� ������ �����ϰ� ���� �ð� �� ���󺹱�
    /// </summary>
    /// <param name="data"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private IEnumerator ApplyAttackSkillBuff(PlayerSkillData data ,PlayerManager player)
    {
        float originalAttackPower = player.AttackPower;
        player.AttackPower += data.playerBonusAttackPower;
        Debug.Log($"[PlayerSkill]: �������� ���ݷ��� {data.playerBonusAttackPower}��ŭ ���� " +
            $"���� ���ݷ�: {player.AttackPower + data.playerBonusAttackPower}");

        //yield return new WaitForSeconds(data.playerAttackSkillDuration);
        yield return WaitForSecondsCache.Get(data.playerAttackSkillDuration);

        //player.AttackPower -= data.playerBonusAttackPower;
        player.AttackPower = originalAttackPower;
        Debug.Log("[PlayerSkill]: ���ݷ� ���� ����");
    }
    /// <summary>
    /// �÷��̾� ���� ����(Box Collider)�� �Ͻ������� Ȯ�� �� ����
    /// </summary>
    /// <param name="data"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private IEnumerator ApplyDefenseSkillBuff(PlayerSkillData data, PlayerManager player)
    {
        float originalDefense = player.Defense;
        player.Defense += data.playerBonusDefense;
        Debug.Log($"[PlayerSkill]: �������� ������ {data.playerBonusDefense}��ŭ ���� " +
            $"���� ������: {player.Defense + data.playerBonusDefense}");

        //yield return new WaitForSeconds(data.playerDefenseSkillDuration);
        yield return WaitForSecondsCache.Get(data.playerDefenseSkillDuration);

        //player.Defense -= data.playerBonusDefense;
        player.Defense = originalDefense;
        Debug.Log("[PlayerSkill]: ���� ���� ����");
    }
    /// <summary>
    /// ���� ���¸� ���� �ð� ������ �� ����
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
        Debug.Log("[Player Skill]ĳ������ �ڽ� ������ Ȯ�� �� �߽��̵�");
        
        //yield return new WaitForSeconds(data.returnTime);
        yield return WaitForSecondsCache.Get(data.returnTime);

        player.manualCollider.boxSize = originalBoxSize;
        player.manualCollider.boxCentor = originalBoxCenter;

    }
    /// <summary>
    /// ���ݷ� ������ �Ͻ������� ������Ų �� ���󺹱�
    /// </summary>
    /// <param name="data"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private IEnumerator ResetPlayerInvincibility(PlayerSkillData data, PlayerManager player)
    {
        player.isInvincibility = true;
        Debug.Log($"[PlayerSkill] �÷��̾��� �������� �ð� :{data.invincibilityDuration}");

        //yield return new WaitForSeconds(data.invincibilityDuration);
        yield return WaitForSecondsCache.Get(data.invincibilityDuration);

        player.isInvincibility = false;
        Debug.Log($"[PlayerSkill] �÷��̾��� �������� �ð� ����");
    }
    /// <summary>
    /// AttackTrigger �ߵ� �������� ��� �� ���� ó�� ����
    /// </summary>
    /// <param name="data"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private IEnumerator ResetPlayerAttackSkillMagnification(PlayerSkillData data, PlayerManager player)
    {
        float originalAttackPower = player.AttackPower;

        player.AttackPower *= data.playerAttackSkillMagnification;
        Debug.Log($"[PlayerSkill]�÷��̾��� ���ݽ�ų ��� :{data.playerAttackSkillMagnification}," +
            $" ���� �� ���ݷ�:{player.AttackPower}");

        //yield return new WaitForSeconds(data.playerAttackSkillMagnificationDuration);
        yield return WaitForSecondsCache.Get(data.playerAttackSkillMagnificationDuration);

        player.AttackPower = originalAttackPower;
        Debug.Log($"[PlayerSkill]�÷��̾� ���ݽ�ų ���� ����:{originalAttackPower}");
    }
    /// <summary>
    /// ��ų �ִϸ��̼� ���� �÷��̾� �̵�, ȸ��, ��Ʈ����� ������
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
    /// Ÿ�� ���� �Ͻ������� ������Ų �� ���󺹱�
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

            Debug.Log("[PlayerSkill] �÷��̾� ���� ����");
            yield return WaitForSecondsCache.Get(clip.length);

            player.isPerformingAction = false;
            player.applyRootMotion = true;
            player.canRotate = true;
            player.canMove = true;

            Debug.Log("[PlayerSkill] �÷��̾� ���� ����");
        }        
    }
    /// <summary>
    /// �ֺ� ���Ϳ��� ����� ȿ�� ����
    /// </summary>
    /// <param name="data"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private IEnumerator ResetPlayerIncreasingTargetCount(PlayerSkillData data, PlayerManager player)
    {
        int originalTargetCount = player.monsterTargetCount;
        player.monsterTargetCount = data.targetCount;
        Debug.Log($"[PlayerSkill]�������� ��ų ��� �� ���� Ÿ�� �� : {player.monsterTargetCount}");

        yield return WaitForSecondsCache.Get(data.targetCountDuration);

        player.monsterTargetCount = originalTargetCount;
        Debug.Log($"[PlayerSkill]�������� ���� Ÿ�� �� ���� :{player.monsterTargetCount}");
    }
    /// <summary>
    /// ���� �ð� �� �÷��̾� HP ȸ��
    /// </summary>
    /// <param name="data"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private IEnumerator PlayerMonsterDebuff(PlayerSkillData data, PlayerManager player)
    {
        yield return WaitForSecondsCache.Get(data.DebuffMonsterStartDuration);

        Collider[] colliders = player.manualCollider.GetColliderObject();
        //Debug.Log($"[PlayerMonsterDebuff] ������ �ݶ��̴� ��: {colliders.Length}");
        for (int i = 0; i < colliders.Length; i++)
        {
            var designate = colliders[i];
            EnemyReference enemyRef = designate.GetComponent<EnemyReference>(); 

            if(enemyRef == null || enemyRef.Enemy == null)
            {
                Debug.Log("[PlayerSkill] ���� ��Ÿ� �� ���Ͱ� ����");
                continue;
            }

            if(!enemyRef.Enemy.IsAlive)
            {
                Debug.Log("[PlayerSkill] ����� ������� �̹Ƿ� ���� �Ұ�");
                continue;
            }

            enemyRef.Enemy.ApplyDebuff(data.DebuffMonsterAttack, data.DebuffMonsterDefense, 
                data.DebuffMonsterDuration);
            
            Debug.Log($"[PlayerSkill] {enemyRef.Enemy.name}���� ����� ���� ��ġ attackDebuff:{data.DebuffMonsterAttack}" +
                $"DefenseDebuff:{data.DebuffMonsterDefense}, ���ӽð�:{data.DebuffMonsterDuration}");
        }        
    }
    /// <summary>
    /// ���� �ð� �� �÷��̾� HP ȸ��
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

        Debug.Log($"[PlayerSkill]ü�� ȸ�� : {beforeHP} -> {player.HP}");
    }
    /// <summary>
    /// ��ų ��� �� MP �Ҹ� ó��
    /// </summary>
    /// <param name="data"></param>
    /// <param name="player"></param>
    private void UseMP(PlayerSkillData data, PlayerManager player)
    {
        float beforeMP = player.MP; 
        if(player.MP < data.useMPAmount)
        {
            Debug.Log($"[PlayerSkill]MP�� {data.useMPAmount - beforeMP} ���� ");
            return; 
        }
        else
        {
            player.MP -= data.useMPAmount;
            Debug.Log($"[PlayerSkill]MP���:{data.useMPAmount}, ���� MP:{player.MP - data.useMPAmount}");
            player.OnChangerStats?.Invoke();
        }
    }
    
    public KeyCode GetKey() => key;
}
