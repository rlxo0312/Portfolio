using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public enum EnemyStateType 
{
    Idle,
    Move,
    Run,
    Attack,
    Dead,
    Patrol,
    FatalSkill1,
}
/// <summary>
/// 몬스터의 이동, 추적, 공격, 부활, 패트롤 등을 공통으로 선언하고 관리하는 클래스
/// <para>현재 사용중인 메서드</para>
/// <para>공격 및 피해 처리</para>
/// <para>public override void BeDamaged(float damage, Vector3 contactPos, GameObject hitEffectPrefabs = null), public bool IsAvailableAttack { get; }</para>
/// 
/// <para>이동 및 타겟 추적</para>
/// <para>public Transform SearchTarget(), public bool CheckRemainDistance(), public Transform FindnextWayPoint()</para>
/// 
/// <para>상태 전환 및 부활</para>
/// <para>public virtual void ChangeState(EnemyStateType stateType), public virtual void ResetEnemyState(), public void Revive()
/// protected virtual void DecideNextState(), protected virtual void ReviveAnimation(), public virtual void ReviveDefaultState()
/// public abstract void InitializeRevivedState(), public enum EnemyStateType(Idle, Move, Run, Attack, FatalSkill, Dead, Patrol)</para>
/// 
/// <para>스킬 관련</para>
/// <para>public virtual void InitMonsterSkillFlag(), public virtual void CheckMonsterSkillUse_HP(), 
/// private IEnumerator ExpandAndResetCollider(float timeToReturn)</para>
/// 
/// <para>기타 기능</para>
/// <para>public virtual void Die(), public virtual void DropItem(), private IEnumerator DelayedReturnToPool()
/// public virtual DropItemTable GetitemDropTable(), public virtual void LoadStatsFromData(Enemy_Data data) 
/// protected void AssignWayPoints(), public void OnDrawGizmos()</para> 
/// </summary>
public abstract class Enemy : Entity
{
    [Header("Enemy Stat")]

    public float idleTime;
    public float moveTime;
    public float chaseSpeed;
    public float viewRange;
    public float reviveDelay;
    [Header("몬스터의 공격 가능 범위")]
    public float AttackRange;
    public Vector3 attackOffset = Vector3.zero;

    /*[Header("몬스터의 무적 가능 여부")]
    [SerializeField] private bool isInvincibility = false;
    public bool IsInvincibility => isInvincibility;*/

    protected Vector3 baseBoxSize;       
    [SerializeField, Header("달리기 지속시간")] private float runDuration;
    public float RunDuration => runDuration;
    [Header("몬스터가 달리기를 하기 위한 거리 AttackRange + increasedRunRange의 계산이므로 주의")]
    public float increasedRunRange;
    //[Header("몬스터 스킬 사용 지속시간")]
    //[SerializeField] private float monsterSkillDuration;
    //public float MonsterSkillDuration => monsterSkillDuration;    
    //[SerializeField] private List<float> monsterSkillUseAccordingToHP = new List<float>();
    [SerializeField, Header("몬스터 스킬 사용 조건(HP)")] private List<MonsterSkillCondition> monsterSkillUseAccordingToHP = new List<MonsterSkillCondition>();
    private List<bool> isMonsterSkillUsed;

    //[Header("몬스터의 스킬 사용후 스텟 증가")]
    //[SerializeField] private float skillBounusAttackPower;    
    //[SerializeField] private float skillBounusAttackPowerTime;
    [Header("몬스터의 스킬 사용전 스텟")]
    private float baseAttackPower;
    private float baseDefense;
    /*[Header("Enemy Data")]
    public Enemy_ZombieData zombieData;*/
    public Enemy_Monster_StateMachine enemy_monster_StateMachine { get; private set; }
    protected Dictionary<EnemyStateType, Enemy_Monster_State> stateMap;

    private Renderer[] renderers;
    private Material[] changeMaterials;
    private Color[] originalColor;    
    [SerializeField, Header("무적시 반짝거리는 이펙트 주기"), Range(0.01f, 1f)] private float flashCycle;
    [SerializeField, Header("무적시 반짝거리는 색")] private Color invinvibleFlashColor;

    [Header("----------------------------------------------------------------------------------")]
    public NavMeshAgent navMeshAgent;
    public new Rigidbody rigidbody; 
    public Monster_AttackManager monster_AttackManager;
    
    [Header("Search Target")]
    private FieldOfView fieldOfView;
    public LayerMask targetMask;
    public Transform target;     

    [Header("Patrol")]
    protected Transform[] wayPoints; //[SerializeField] 
    //public Transform[] WayPoints => wayPoints;
    [HideInInspector] public Transform targetWayPoint = null;
    private int wayPointIndex = 0;
    //private bool wayPointsAssigned = false;
    public virtual string WayPointPathName => "";

    [Header("Revie Position")] 
    public Transform revivePoint = null;
    [SerializeField]private string pathName; 
    public string PathName => pathName;
    public MonsterBattleUi monsterBattleUi;
    protected bool isDead = false;
    public bool isDebuffed;
    
    /// <summary>
    /// 몬스터의 기본 동작 
    /// </summary>
    public abstract string DefaultAniName { get; }
    public abstract string DeadAniName { get; } 
    public abstract string AttackAniName { get; }
    public virtual string RunAniName { get; }
    public virtual string FatalSkillAniName { get; }
    //public virtual string WayPointPathName { get; }

    
    public DropItemSpawner itemdropSpawner;
    public virtual DropItemTable GetitemDropTable() => null;
    public virtual string PoolKey { get; }
    public virtual Enemy_Monster_State GetState(EnemyStateType type)
    {
        if(stateMap != null && stateMap.TryGetValue(type, out var state))
        {
            return state;
        }
        //Debug.LogWarning($"[Enemy] 상태 {type}이 등록되지 않았습니다.");
        return null;
    }
    public virtual void ChangeState(EnemyStateType stateType)
    {
        var state = GetState(stateType);
        if(state != null)
        {
            enemy_monster_StateMachine.ChangeState(state);
        }
    }
    public virtual void LoadStatsFromData(Enemy_Data data)
    {
        MaxHP = data.HP;
        AttackPower = data.attackPower;
        Defense = data.defense;
        AttackRange = data.attackRange;
    }
    protected override void Awake()
    {
        base.Awake();

        baseAttackPower = AttackPower;
        baseDefense = Defense;        

        if(monster_AttackManager.menualCollider != null && monster_AttackManager != null)
        {
            baseBoxSize = monster_AttackManager.menualCollider.boxSize;
        }
        else
        {
            Debug.LogWarning("[Enemy] ManualCollider 또는 AttackManager가 연결되지 않았습니다.");
        }

        enemy_monster_StateMachine = new Enemy_Monster_StateMachine();
        stateMap = new Dictionary<EnemyStateType, Enemy_Monster_State>();       
        
        if (monsterBattleUi != null)
        {
            monsterBattleUi.MinValue = 0;
            monsterBattleUi.MaxValue = MaxHP;
            monsterBattleUi.Value = MaxHP;
        }

        if (enemy_monster_StateMachine == null)
        {
            Debug.LogError("Enemy_Monster_StateMachine is not initialized.");
        }

        renderers = GetComponentsInChildren<Renderer>();
        changeMaterials = new Material[renderers.Length];
        originalColor = new Color[renderers.Length];

        for(int i = 0; i < renderers.Length; i++)
        {
            var renderer = renderers[i];
            if (renderer != null && renderer.sharedMaterial.HasProperty("_BaseColor"))
            {
                Material mat = new Material(renderer.sharedMaterial);
                renderer.material = mat;

                changeMaterials[i] = mat;
                originalColor[i] = mat.GetColor("_BaseColor");
            }
        }

    }
#if UNITY_EDITOR
    /// <summary>
    /// 에디터 모드에서 Enemy 오브젝트가 수정될 때 자동으로 호출되는 메서드
    /// - 몬스터 스킬 조건 리스트의 각 항목의 유효성을 검사함
    /// - 예: bonusDefense 값이 범위를 초과하는 경우 경고 출력
    /// </summary>
    private void OnValidate()
    {
        if (monsterSkillUseAccordingToHP == null)
        {
            return;
        }
            foreach (var skill in monsterSkillUseAccordingToHP)
        {
            if (skill != null)
            {
                skill.CheckEnemyBonusDefenseArea();
            }
        }        
    }
#endif   
    protected override void Start()
    {
        base.Start();
        AssignWayPoints();
    }
    protected override void Update()
    {
        base.Update();
        enemy_monster_StateMachine?.currentState?.Update();        
    }
    /// <summary>
    /// Enemy 관련 컴포넌트를 찾아 연결
    /// </summary>
    public override void OnLoadComponents()
    {
        base.OnLoadComponents();
        rigidbody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        fieldOfView = GetComponent<FieldOfView>();
        monster_AttackManager = GetComponent<Monster_AttackManager>();

        if (itemdropSpawner == null)
        {
            itemdropSpawner = FindObjectOfType<DropItemSpawner>();
            if (itemdropSpawner == null)
            {
                Debug.LogWarning("[Enemy] DropItemSpawner를 찾을 수 없습니다.");
            }
        }
    }
    /// <summary>
    /// WayPoint 경로를 자동 할당
    /// </summary>
    protected void AssignWayPoints()
    {    
        if (wayPoints == null || wayPoints.Length == 0)
        {
            if (string.IsNullOrEmpty(WayPointPathName))
            {
                Debug.LogWarning("[Enemy] WayPointPathName이 설정되지 않았습니다.");
                return;
            }
            if (wayPoints != null && wayPoints.Length > 0)
            {
                return;
            }
            Transform pathRoot = GameObject.Find(WayPointPathName)?.transform;
            if (pathRoot != null)
            {
                var wayPointList = new List<Transform>();
                foreach (Transform child in pathRoot)
                {
                    wayPointList.Add(child);
                }
                wayPoints = wayPointList.ToArray();
                //wayPointsAssigned = true;
                //Debug.Log($"[Enemy] '{WayPointPathName}'에서 WayPoints 자동 설정 완료 ({wayPoints.Length}개)");
            }
            else
            {
               // Debug.Log($"[Enemy] '{WayPointPathName}' 오브젝트를 찾을 수 없습니다.");
            }
        }
    }    
    /// <summary>
    /// 공격 가능 여부를 판정
    /// </summary>
    public bool IsAvailableAttack
    {
        get
        {
            /*if (!target)
            {
                return false;
            }

            if((target.position - transform.position).sqrMagnitude < AttackRange)
            {
                return true;
            }
            else
            {
                return false;
            }*/
         
            {
                if (target == null)
                {
                    return false;
                }

                Vector3 attackCenter = transform.position + transform.TransformDirection(attackOffset);
                return (target.position - attackCenter).sqrMagnitude < AttackRange * AttackRange;
            }
        }
       
    }
    /// <summary>
    /// 시야 내 가장 가까운 타겟을 탐색
    /// </summary>
    public Transform SearchTarget()
    {
        this.target = fieldOfView.NearestTarget;

        return target;
    }
    /// <summary>
    /// 현재 위치와 WayPoint 간 거리를 계산하여 다음 WayPoint로 이동할지를 판단하는 메서드
    /// </summary>
    /// <returns>목표 지점에 거의 도달했으면 false, 아직 이동 중이면 true 반환</returns>
    public bool CheckRemainDistance()
    {
        if (wayPoints == null || wayPoints.Length == 0 || wayPoints[wayPointIndex] == null)
        {
            //Debug.LogWarning("[Enemy] wayPoints 배열이 비어있습니다.");
            return true;
        }

        if ((wayPoints[wayPointIndex].transform.position - transform.position).sqrMagnitude < 0.1f)
        {
            /*if(wayPointIndex < wayPoints.Length - 1)
            {
                wayPointIndex++;
            }
            else
            {
                wayPointIndex = 0;
               
            }

            return false;*/
            wayPointIndex = (wayPointIndex + 1) % wayPoints.Length;
            return false;
        }
        return true;
    }
    /// <summary>
    /// 몬스터가 지정된 지점을 찾는 매서드
    /// </summary>
    /// <returns>targetWayPoint</returns>
    public Transform FindnextWayPoint()
    {
        targetWayPoint = null; 
        /*if(wayPoints.Length > 0)
        {
            targetWayPoint = wayPoints[wayPointIndex];
            Debug.Log($"[Enemy] 다음 WayPoint는 {targetWayPoint.name}");
        }
        else
        {
            Debug.LogWarning("[Enemy] WayPoints가 비어있거나 null입니다.");
        }*/ 
        if(wayPoints == null || wayPoints.Length == 0)
        {
            Debug.LogWarning("[Enemy] WayPoints가 null이거나 비어있습니다.");
            //AssignWayPoints("WarrokPaths");
            if (wayPoints == null || wayPoints.Length == 0)
            {
                Debug.LogError("[Enemy] WayPoints 설정 실패 - 경로 재확인 필요");
                return null;
            }
        }
        targetWayPoint = wayPoints[wayPointIndex];
        //Debug.Log($"[Enemy] 다음 WayPoint는 {targetWayPoint.name}");
        return targetWayPoint;
    }
    /// <summary>
    /// 피격 시 데미지 적용 처리
    /// </summary>
    public override void BeDamaged(float damage, Vector3 contactPos, GameObject hitEffectPrefabs = null)
    {
        base.BeDamaged(damage, contactPos, hitEffectPrefabs);        
    }
    /// <summary>
    /// 몬스터에게 디버프를 적용하는 메서드
    /// </summary>
    /// <param name="attackDebuff">공격력 감소 수치</param>
    /// <param name="defenseDebuff">방어력 감소 수치</param>
    /// <param name="debuffDuration">디버프 지속 시간</param>
    public void ApplyDebuff(float attackDebuff, float defenseDebuff, float debuffDuration)
    {
        //Debug.Log($"[ApplyDebuff] Called on {name} / isDebuffed: {isDebuffed}");
        if (isDebuffed)
        {
            return;
        }

        isDebuffed = true;

       // bool isDebuffApply = false;
        AttackPower = Mathf.Max(1, AttackPower - attackDebuff);
        Defense = Mathf.Max(0, Defense - defenseDebuff);

        if(monsterBattleUi != null)
        {
            /*if(attackDebuff > 0f && monsterBattleUi.attackDebuffIconPrefab != null)
            {
                GameObject icon = ObjectPoolingManager.Instance.SpawnFromPool(monsterBattleUi.attackDebuffIconKey, 
                    monsterBattleUi.debuffIconPos.position, Quaternion.identity);
                //monsterBattleUi.debuffIcon.Add(icon);
                //monsterBattleUi.debuffKey.Add(monsterBattleUi.attackDebuffIconKey);
                monsterBattleUi.AddDebuffIcon(icon, monsterBattleUi.attackDebuffIconKey);
                monsterBattleUi.ShowNextDebuffIcons(monsterBattleUi.AttackDebuffIconSprite);
                isDebuffApply = true;
            }

            if (attackDebuff > 0f && monsterBattleUi.defenseDebuffIconPrefab != null)
            {
                GameObject icon = ObjectPoolingManager.Instance.SpawnFromPool(monsterBattleUi.defenseDebuffIconKey,
                    monsterBattleUi.debuffIconPos.position, Quaternion.identity);
                //monsterBattleUi.debuffIcon.Add(icon);
                //monsterBattleUi.debuffKey.Add(monsterBattleUi.attackDebuffIconKey);
                monsterBattleUi.AddDebuffIcon(icon, monsterBattleUi.defenseDebuffIconKey);
                monsterBattleUi.ShowNextDebuffIcons(monsterBattleUi.DefenseDebuffIconSprite);
                isDebuffApply = true;
            }  
            */
            if(attackDebuff > 0f)
            {
                monsterBattleUi.ShowDebuff(DebuffType.attack, debuffDuration);
                //isDebuffApply = true;
            }
            if(defenseDebuff > 0f)
            {
                monsterBattleUi.ShowDebuff(DebuffType.defense, debuffDuration);
                //isDebuffApply = true;
            }
        }

        if(attackDebuff > 0f || defenseDebuff > 0f) //isDebuffApply
        {
            StartCoroutine(ResetDebuff(debuffDuration));
        }        
    }
    /// <summary>
    /// 디버프 지속 시간이 끝난 후 능력치 및 상태를 복구하는 코루틴
    /// </summary>
    /// <param name="debuffDuration">디버프 지속 시간</param>    
    private IEnumerator ResetDebuff(float debuffDuration)
    {
        yield return WaitForSecondsCache.Get(debuffDuration);

        AttackPower = baseAttackPower;
        Defense = baseDefense;
        isDebuffed = false;

        /* for(int i =0; i < monsterBattleUi.debuffIcon.Count; i++)
         {
             var icon = monsterBattleUi.debuffIcon[i];
             var key = monsterBattleUi.debuffKey[i];
             if (icon != null && key != null)
             {
                 ObjectPoolingManager.Instance.ReturnToPool(key, icon);
             }

         }
         monsterBattleUi.debuffIcon.Clear();
         monsterBattleUi.debuffKey.Clear();*/
        if(monsterBattleUi != null)
        {
            //monsterBattleUi.ClearAndReturnDebuffs();
            //monsterBattleUi.ClearDebuffIcons();
            monsterBattleUi.ClearAllDebuffs();
        }
        Debug.Log($"[Enemy]{name}의 디버프 종료");
    }
    /// <summary>
    /// 스킬 조건 초기화
    /// </summary>
    public virtual void InitMonsterSkillFlag()
    {
        isMonsterSkillUsed = new List<bool>();
        /*for(int i = 0; i < monsterSkillUseAccordingToHP.Count; i++)
        {
            isMonsterSkillUsed.Add(false);
        }*/
        foreach(var _ in monsterSkillUseAccordingToHP)
        {
            isMonsterSkillUsed.Add(false);
        }
    }
    /// <summary>
    /// 체력 조건에 따라 스킬 사용 판단
    /// </summary>
    public virtual void CheckMonsterSkillUse_HP()
    {
        if(isDead || monsterSkillUseAccordingToHP == null || monsterSkillUseAccordingToHP.Count == 0)
        {
            return;
        }

        float hpPercent = (HP/MaxHP) * 100f; 

        for(int i = 0; i < monsterSkillUseAccordingToHP.Count; i++)
        {
            var skill = monsterSkillUseAccordingToHP[i];
            if (!isMonsterSkillUsed[i] && hpPercent <= skill.hpPercent)
            {
                isMonsterSkillUsed[i] = true;
                Debug.Log($"[Enemy]: {name} FatalSkill 발동, HP :{hpPercent:F1}% 이하");

                AttackPower += skill.bonusAttackPower;
                Debug.Log($"[Enemy]: {name}의 FatalSkill 발동, 현재 AttackPower: {AttackPower}");
                                
                Defense += skill.bonusDefense;
                
                Debug.Log($"[Enemy]: {name}의 FatalSkill 발동, 현재 Defense: {Defense}");

                StartCoroutine(ResetAttackPower(skill.attackSkillDuration));
                StartCoroutine(ResetDefensePower(skill.defenseSkillDuration));

                if(skill.changeBoxSize != Vector3.zero)
                {
                    StartCoroutine(ExpandAndResetCollider(skill.changeBoxSize, skill.returnTime));
                }

                if(skill.skillEffect != null)
                {
                    StartCoroutine(StartSkillEffect(skill));
                }

                if(skill.isInvincibility && skill.invincibilityDuration > 0f)
                {
                    StartCoroutine(ResetInvincibility(skill.invincibilityDuration));                   
                }
                
                var skillState = new Enemy_FatalSkill(this, enemy_monster_StateMachine, skill.animationName, skill.attackSkillDuration);
                
                enemy_monster_StateMachine.ChangeState(skillState);
                break;
            }
        }
    }
    /// <summary>
    /// attack collider 크기 확대 후 일정 시간 후 복구
    /// </summary>
    private IEnumerator ExpandAndResetCollider(Vector3 newSize ,float timeToReturn)
    {
       
        if (monster_AttackManager.menualCollider == null)
        {
            yield break;
        }
        monster_AttackManager.menualCollider.boxSize = newSize;
        Debug.Log("[Enemy] 박스 크기 확대됨");

        //yield return new WaitForSeconds(timeToReturn);
        yield return WaitForSecondsCache.Get(timeToReturn);

        monster_AttackManager.menualCollider.boxSize = baseBoxSize;
        Debug.Log("[Enemy] 박스 크기 원상복구됨");
    }
    /// <summary>
    /// 일정 시간 후 공격력 초기값으로 복구
    /// </summary>
    ///  /// <param name="timeToReturn">attackSkillDuration</param>
    /// <returns></returns>
    private IEnumerator ResetAttackPower(float timeToReturn)
    {
        //yield return new WaitForSeconds(timeToReturn);
        yield return WaitForSecondsCache.Get(timeToReturn);

        AttackPower = baseAttackPower;
        Debug.Log($"[Enemy] {name}의 AttackPower 복구:{AttackPower}");
    }
    /// <summary>
    /// 일정 시간 후 방어력 초기값으로 복구
    /// </summary>
    /// <param name="timeToReturn">defenseSkillDuration</param>
    /// <returns></returns>
    private IEnumerator ResetDefensePower(float timeToReturn)
    {
        //yield return new WaitForSeconds(timeToReturn);
        yield return WaitForSecondsCache.Get(timeToReturn);

        Defense = baseDefense;
        Debug.Log($"[Enemy] {name}의 Defense수치 복수:{Defense}");
    }
    /// <summary>
    /// 일정 시간 동안 무적처리 
    /// </summary>
    /// <param name="timToReturn">invincibilityDuration</param>
    /// <returns></returns>
    private IEnumerator ResetInvincibility(float timToReturn)
    {
        isInvincibility = true;
        Debug.Log($"[Enemy] {name}의 무적지속 시간:{timToReturn}초");

        StartCoroutine(FlashWhileInvincible(timToReturn, flashCycle));

        //yield return new WaitForSeconds(timToReturn);
        yield return WaitForSecondsCache.Get(timToReturn);

        isInvincibility = false;
        Debug.Log($"[Enemy] {name}의 무적시간 종료");
    }
    /// <summary>
    /// 무적시 반짝거리는 효과를 주는 코루틴
    /// </summary>
    /// <param name="invincibleDuration">지속시간</param>
    /// <param name="flashInverter">반짝거리는 시간</param>
    /// <returns></returns>
    private IEnumerator FlashWhileInvincible(float invincibleDuration, float flashInverter)
    {
        float timer = 0f;
        bool isFlash = true;

        while(timer < invincibleDuration)
        {
            for(int i = 0; i < changeMaterials.Length; i++)
            {                
                if (changeMaterials[i] != null) 
                {
                    /*Debug.Log($"[Enemy_InvincibleFlash] {name}의 무적시 색깔확인: {(isFlash ? "Invincible Color" : "Original Color")}");
                    Debug.Log(invinvibleFlashColor);
                    changeMaterials[i].SetColor("_BaseColor", isFlash ? invinvibleFlashColor : originalColor[i]);*/
                    Color flashColor = isFlash ? invinvibleFlashColor : originalColor[i];

                    // Base Color 설정
                    if (changeMaterials[i].HasProperty("_BaseColor"))
                    {
                        changeMaterials[i].SetColor("_BaseColor", flashColor);
                    }

                    if (changeMaterials[i].HasProperty("_Color"))
                    {
                        changeMaterials[i].SetColor("_Color", flashColor);
                    }


                    if (changeMaterials[i].HasProperty("_EmissionColor"))
                    {
                        Color emissionColor = isFlash ? invinvibleFlashColor * 2f : Color.black;
                        changeMaterials[i].SetColor("_EmissionColor", emissionColor);
                        changeMaterials[i].EnableKeyword("_EMISSION");
                    }
                }
            }
            Debug.Log($"[Enemy_InvincibleFlash] {name}의 무적시 색깔확인: {(isFlash ? "Invincible Color" : "Original Color")}");
            isFlash = !isFlash;
            timer += flashInverter;
            //yield return new WaitForSeconds(flashInverter);
            yield return WaitForSecondsCache.Get(flashInverter);
        }
        for(int i = 0; i < changeMaterials.Length; i++)
        {            
            if (changeMaterials[i] != null)
            {
                //SetMaterialColor(mat, originalColor[i]);
                //changeMaterials[i].SetColor("_BaseColor", originalColor[i]);
                if (changeMaterials[i].HasProperty("_BaseColor"))
                {
                    changeMaterials[i].SetColor("_BaseColor", originalColor[i]);
                }

                if (changeMaterials[i].HasProperty("_Color"))
                {
                    changeMaterials[i].SetColor("_Color", originalColor[i]);
                }

                if (changeMaterials[i].HasProperty("_EmissionColor"))
                {
                    changeMaterials[i].SetColor("_EmissionColor", Color.black);
                    changeMaterials[i].DisableKeyword("_EMISSION");
                }
            }
        }
    }
    
    /// <summary>
    /// 스킬 이펙트를 지연 후 생성하고 일정 시간후 pool로 반환
    /// </summary>
    /// <param name="skilleffect"></param>
    /// <returns></returns>
    private IEnumerator StartSkillEffect(MonsterSkillCondition skilleffect)
    {
        if(skilleffect.skillEffect == null || string.IsNullOrEmpty(skilleffect.effectPoolKey))
        {
            Debug.Log($"[Enemy] 현재 스킬 이펙트가 없습니다. prefab({skilleffect.skillEffect})과 " +
                $"poolKey({skilleffect.effectPoolKey})를 확인해주세요");
            yield break;
        }

        if(skilleffect.skillEffectStartDelay > 0f)
        {
            //yield return new WaitForSeconds(skilleffect.skillEffectStartDelay);
            yield return WaitForSecondsCache.Get(skilleffect.skillEffectStartDelay);
        }

        Vector3 spawnPos = transform.position + skilleffect.skillEffectPos;

        GameObject effect = ObjectPoolingManager.Instance.SpawnFromPool(skilleffect.effectPoolKey, spawnPos, Quaternion.identity);//Quaternion.identity, LookRotation(transform.forward)

        if (effect == null)
        {
            Debug.Log($"[Enemy] pool에서 반환할 수 없습니다. 키({skilleffect.effectPoolKey})확인");
        }
        //effect.SetActive(true);
        //yield return new WaitForSeconds(skilleffect.skillEffectDuration);
        yield return WaitForSecondsCache.Get(skilleffect.skillEffectDuration);
        ObjectPoolingManager.Instance.ReturnToPool(skilleffect.effectPoolKey, effect);
    }

    public void OnDrawGizmos()
    {
        if (!enabled || transform == null)
        {
            return;
        }

        
        //if (enemy == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position ,viewRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(attackOffset), AttackRange); 

        if(increasedRunRange <= 0)
        {
            return;
        }
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, AttackRange + increasedRunRange);       
        
        /*Gizmos.color = Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(monster_AttackManager.menualCollider.transform.position, baseBoxSize); */      
    }
    /// <summary>
    /// 상태 강제 초기화 (오버라이드용)
    /// </summary>
    public virtual void ResetEnemyState()
    {
        
    }    
    /// <summary>
    /// 몬스터 사망 처리 및 풀 반환 예약
    /// </summary>
    public virtual void Die()
    {
        DropItem();
        if (monsterBattleUi != null)
        {
            monsterBattleUi.ClearAllDebuffs();
        }
        isDebuffed = false;
        AttackPower = baseAttackPower;
        Defense = baseDefense;
        StartCoroutine(DelayedReturnToPool());
    }
    /// <summary>
    /// 몬스터 부활 처리 및 스텟 초기화
    /// </summar
    public void Revive()
    {
        //AssignWayPoints("WarrokPaths");
        //ResetEnemyState();
        //StartCoroutine(ReviveCoroutine());
        monsterBattleUi.gameObject.SetActive(true);
        HP = MaxHP;
        monsterBattleUi.Value = MaxHP;

        AttackPower = baseAttackPower;
        Defense = baseDefense;
        isDead = false;
        isDebuffed = false;
        InitMonsterSkillFlag();

        if(monsterBattleUi != null) 
        {
            monsterBattleUi.ClearAllDebuffs();
        }
        //AssignWayPoints();        
    }
    /// <summary>
    /// 몬스터가 부활 후 타겟을 설정하고 공격하는 매서드
    /// </summary>
    protected virtual void DecideNextState()
    {
        
    }
    /// <summary>
    /// 몬스터의 부활 후 애니메이션을 설정하는 매서드 
    /// </summary>
    protected virtual void ReviveAnimation()
    {

    }
    /// <summary>
    /// 몬스터의 부활 후 기본 상태를 설정하는 매서드 
    /// </summary>
    public virtual void ReviveDefaultState()
    {

    }
    /// <summary>
    /// 풀링 반환 후 일정 시간 후 부활 예약
    /// </summary>
    private IEnumerator DelayedReturnToPool()
    {
        //yield return new WaitForSeconds(reviveDelay);
        yield return WaitForSecondsCache.Get(reviveDelay);

        if (!gameObject.activeSelf)
        {
            yield break;
        }

        ObjectPoolingManager.Instance.ReturnToPool(PoolKey, gameObject);
        if (revivePoint != null)
        {
            MonsterReviveManager.instance.ScheduleRevie(PoolKey, revivePoint.position, reviveDelay);
        }
        if (monsterBattleUi != null)
        {
            monsterBattleUi.gameObject.SetActive(false);
        }

    }
    /// <summary>
    /// 아이템 드롭 처리
    /// </summary>
    public virtual void DropItem()
    {
        if(itemdropSpawner == null)
        {
            Debug.Log("[Enemy] DropItemSpawner가 없습니다.");
            return;
        }
        var table = GetitemDropTable();
        if(table == null)
        {
            Debug.Log("[Enemy] DropItemTable이 없습니다.");
            return;
        }
        itemdropSpawner.SpwanDropItem(transform.position, table);
    }   
    /// <summary>
    /// 자식 클래스에서 부활 시 기본 상태 설정용으로 반드시 오버라이드해야 함
    /// </summary>
    public abstract void InitializeRevivedState();    

}
