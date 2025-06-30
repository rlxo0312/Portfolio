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
/// ������ �̵�, ����, ����, ��Ȱ, ��Ʈ�� ���� �������� �����ϰ� �����ϴ� Ŭ����
/// <para>���� ������� �޼���</para>
/// <para>���� �� ���� ó��</para>
/// <para>public override void BeDamaged(float damage, Vector3 contactPos, GameObject hitEffectPrefabs = null), public bool IsAvailableAttack { get; }</para>
/// 
/// <para>�̵� �� Ÿ�� ����</para>
/// <para>public Transform SearchTarget(), public bool CheckRemainDistance(), public Transform FindnextWayPoint()</para>
/// 
/// <para>���� ��ȯ �� ��Ȱ</para>
/// <para>public virtual void ChangeState(EnemyStateType stateType), public virtual void ResetEnemyState(), public void Revive()
/// protected virtual void DecideNextState(), protected virtual void ReviveAnimation(), public virtual void ReviveDefaultState()
/// public abstract void InitializeRevivedState(), public enum EnemyStateType(Idle, Move, Run, Attack, FatalSkill, Dead, Patrol)</para>
/// 
/// <para>��ų ����</para>
/// <para>public virtual void InitMonsterSkillFlag(), public virtual void CheckMonsterSkillUse_HP(), 
/// private IEnumerator ExpandAndResetCollider(float timeToReturn)</para>
/// 
/// <para>��Ÿ ���</para>
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
    [Header("������ ���� ���� ����")]
    public float AttackRange;
    public Vector3 attackOffset = Vector3.zero;

    /*[Header("������ ���� ���� ����")]
    [SerializeField] private bool isInvincibility = false;
    public bool IsInvincibility => isInvincibility;*/

    protected Vector3 baseBoxSize;       
    [SerializeField, Header("�޸��� ���ӽð�")] private float runDuration;
    public float RunDuration => runDuration;
    [Header("���Ͱ� �޸��⸦ �ϱ� ���� �Ÿ� AttackRange + increasedRunRange�� ����̹Ƿ� ����")]
    public float increasedRunRange;
    //[Header("���� ��ų ��� ���ӽð�")]
    //[SerializeField] private float monsterSkillDuration;
    //public float MonsterSkillDuration => monsterSkillDuration;    
    //[SerializeField] private List<float> monsterSkillUseAccordingToHP = new List<float>();
    [SerializeField, Header("���� ��ų ��� ����(HP)")] private List<MonsterSkillCondition> monsterSkillUseAccordingToHP = new List<MonsterSkillCondition>();
    private List<bool> isMonsterSkillUsed;

    //[Header("������ ��ų ����� ���� ����")]
    //[SerializeField] private float skillBounusAttackPower;    
    //[SerializeField] private float skillBounusAttackPowerTime;
    [Header("������ ��ų ����� ����")]
    private float baseAttackPower;
    private float baseDefense;
    /*[Header("Enemy Data")]
    public Enemy_ZombieData zombieData;*/
    public Enemy_Monster_StateMachine enemy_monster_StateMachine { get; private set; }
    protected Dictionary<EnemyStateType, Enemy_Monster_State> stateMap;

    private Renderer[] renderers;
    private Material[] changeMaterials;
    private Color[] originalColor;    
    [SerializeField, Header("������ ��¦�Ÿ��� ����Ʈ �ֱ�"), Range(0.01f, 1f)] private float flashCycle;
    [SerializeField, Header("������ ��¦�Ÿ��� ��")] private Color invinvibleFlashColor;

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
    /// ������ �⺻ ���� 
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
        //Debug.LogWarning($"[Enemy] ���� {type}�� ��ϵ��� �ʾҽ��ϴ�.");
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
            Debug.LogWarning("[Enemy] ManualCollider �Ǵ� AttackManager�� ������� �ʾҽ��ϴ�.");
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
    /// ������ ��忡�� Enemy ������Ʈ�� ������ �� �ڵ����� ȣ��Ǵ� �޼���
    /// - ���� ��ų ���� ����Ʈ�� �� �׸��� ��ȿ���� �˻���
    /// - ��: bonusDefense ���� ������ �ʰ��ϴ� ��� ��� ���
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
    /// Enemy ���� ������Ʈ�� ã�� ����
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
                Debug.LogWarning("[Enemy] DropItemSpawner�� ã�� �� �����ϴ�.");
            }
        }
    }
    /// <summary>
    /// WayPoint ��θ� �ڵ� �Ҵ�
    /// </summary>
    protected void AssignWayPoints()
    {    
        if (wayPoints == null || wayPoints.Length == 0)
        {
            if (string.IsNullOrEmpty(WayPointPathName))
            {
                Debug.LogWarning("[Enemy] WayPointPathName�� �������� �ʾҽ��ϴ�.");
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
                //Debug.Log($"[Enemy] '{WayPointPathName}'���� WayPoints �ڵ� ���� �Ϸ� ({wayPoints.Length}��)");
            }
            else
            {
               // Debug.Log($"[Enemy] '{WayPointPathName}' ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }
    }    
    /// <summary>
    /// ���� ���� ���θ� ����
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
    /// �þ� �� ���� ����� Ÿ���� Ž��
    /// </summary>
    public Transform SearchTarget()
    {
        this.target = fieldOfView.NearestTarget;

        return target;
    }
    /// <summary>
    /// ���� ��ġ�� WayPoint �� �Ÿ��� ����Ͽ� ���� WayPoint�� �̵������� �Ǵ��ϴ� �޼���
    /// </summary>
    /// <returns>��ǥ ������ ���� ���������� false, ���� �̵� ���̸� true ��ȯ</returns>
    public bool CheckRemainDistance()
    {
        if (wayPoints == null || wayPoints.Length == 0 || wayPoints[wayPointIndex] == null)
        {
            //Debug.LogWarning("[Enemy] wayPoints �迭�� ����ֽ��ϴ�.");
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
    /// ���Ͱ� ������ ������ ã�� �ż���
    /// </summary>
    /// <returns>targetWayPoint</returns>
    public Transform FindnextWayPoint()
    {
        targetWayPoint = null; 
        /*if(wayPoints.Length > 0)
        {
            targetWayPoint = wayPoints[wayPointIndex];
            Debug.Log($"[Enemy] ���� WayPoint�� {targetWayPoint.name}");
        }
        else
        {
            Debug.LogWarning("[Enemy] WayPoints�� ����ְų� null�Դϴ�.");
        }*/ 
        if(wayPoints == null || wayPoints.Length == 0)
        {
            Debug.LogWarning("[Enemy] WayPoints�� null�̰ų� ����ֽ��ϴ�.");
            //AssignWayPoints("WarrokPaths");
            if (wayPoints == null || wayPoints.Length == 0)
            {
                Debug.LogError("[Enemy] WayPoints ���� ���� - ��� ��Ȯ�� �ʿ�");
                return null;
            }
        }
        targetWayPoint = wayPoints[wayPointIndex];
        //Debug.Log($"[Enemy] ���� WayPoint�� {targetWayPoint.name}");
        return targetWayPoint;
    }
    /// <summary>
    /// �ǰ� �� ������ ���� ó��
    /// </summary>
    public override void BeDamaged(float damage, Vector3 contactPos, GameObject hitEffectPrefabs = null)
    {
        base.BeDamaged(damage, contactPos, hitEffectPrefabs);        
    }
    /// <summary>
    /// ���Ϳ��� ������� �����ϴ� �޼���
    /// </summary>
    /// <param name="attackDebuff">���ݷ� ���� ��ġ</param>
    /// <param name="defenseDebuff">���� ���� ��ġ</param>
    /// <param name="debuffDuration">����� ���� �ð�</param>
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
    /// ����� ���� �ð��� ���� �� �ɷ�ġ �� ���¸� �����ϴ� �ڷ�ƾ
    /// </summary>
    /// <param name="debuffDuration">����� ���� �ð�</param>    
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
        Debug.Log($"[Enemy]{name}�� ����� ����");
    }
    /// <summary>
    /// ��ų ���� �ʱ�ȭ
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
    /// ü�� ���ǿ� ���� ��ų ��� �Ǵ�
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
                Debug.Log($"[Enemy]: {name} FatalSkill �ߵ�, HP :{hpPercent:F1}% ����");

                AttackPower += skill.bonusAttackPower;
                Debug.Log($"[Enemy]: {name}�� FatalSkill �ߵ�, ���� AttackPower: {AttackPower}");
                                
                Defense += skill.bonusDefense;
                
                Debug.Log($"[Enemy]: {name}�� FatalSkill �ߵ�, ���� Defense: {Defense}");

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
    /// attack collider ũ�� Ȯ�� �� ���� �ð� �� ����
    /// </summary>
    private IEnumerator ExpandAndResetCollider(Vector3 newSize ,float timeToReturn)
    {
       
        if (monster_AttackManager.menualCollider == null)
        {
            yield break;
        }
        monster_AttackManager.menualCollider.boxSize = newSize;
        Debug.Log("[Enemy] �ڽ� ũ�� Ȯ���");

        //yield return new WaitForSeconds(timeToReturn);
        yield return WaitForSecondsCache.Get(timeToReturn);

        monster_AttackManager.menualCollider.boxSize = baseBoxSize;
        Debug.Log("[Enemy] �ڽ� ũ�� ���󺹱���");
    }
    /// <summary>
    /// ���� �ð� �� ���ݷ� �ʱⰪ���� ����
    /// </summary>
    ///  /// <param name="timeToReturn">attackSkillDuration</param>
    /// <returns></returns>
    private IEnumerator ResetAttackPower(float timeToReturn)
    {
        //yield return new WaitForSeconds(timeToReturn);
        yield return WaitForSecondsCache.Get(timeToReturn);

        AttackPower = baseAttackPower;
        Debug.Log($"[Enemy] {name}�� AttackPower ����:{AttackPower}");
    }
    /// <summary>
    /// ���� �ð� �� ���� �ʱⰪ���� ����
    /// </summary>
    /// <param name="timeToReturn">defenseSkillDuration</param>
    /// <returns></returns>
    private IEnumerator ResetDefensePower(float timeToReturn)
    {
        //yield return new WaitForSeconds(timeToReturn);
        yield return WaitForSecondsCache.Get(timeToReturn);

        Defense = baseDefense;
        Debug.Log($"[Enemy] {name}�� Defense��ġ ����:{Defense}");
    }
    /// <summary>
    /// ���� �ð� ���� ����ó�� 
    /// </summary>
    /// <param name="timToReturn">invincibilityDuration</param>
    /// <returns></returns>
    private IEnumerator ResetInvincibility(float timToReturn)
    {
        isInvincibility = true;
        Debug.Log($"[Enemy] {name}�� �������� �ð�:{timToReturn}��");

        StartCoroutine(FlashWhileInvincible(timToReturn, flashCycle));

        //yield return new WaitForSeconds(timToReturn);
        yield return WaitForSecondsCache.Get(timToReturn);

        isInvincibility = false;
        Debug.Log($"[Enemy] {name}�� �����ð� ����");
    }
    /// <summary>
    /// ������ ��¦�Ÿ��� ȿ���� �ִ� �ڷ�ƾ
    /// </summary>
    /// <param name="invincibleDuration">���ӽð�</param>
    /// <param name="flashInverter">��¦�Ÿ��� �ð�</param>
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
                    /*Debug.Log($"[Enemy_InvincibleFlash] {name}�� ������ ����Ȯ��: {(isFlash ? "Invincible Color" : "Original Color")}");
                    Debug.Log(invinvibleFlashColor);
                    changeMaterials[i].SetColor("_BaseColor", isFlash ? invinvibleFlashColor : originalColor[i]);*/
                    Color flashColor = isFlash ? invinvibleFlashColor : originalColor[i];

                    // Base Color ����
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
            Debug.Log($"[Enemy_InvincibleFlash] {name}�� ������ ����Ȯ��: {(isFlash ? "Invincible Color" : "Original Color")}");
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
    /// ��ų ����Ʈ�� ���� �� �����ϰ� ���� �ð��� pool�� ��ȯ
    /// </summary>
    /// <param name="skilleffect"></param>
    /// <returns></returns>
    private IEnumerator StartSkillEffect(MonsterSkillCondition skilleffect)
    {
        if(skilleffect.skillEffect == null || string.IsNullOrEmpty(skilleffect.effectPoolKey))
        {
            Debug.Log($"[Enemy] ���� ��ų ����Ʈ�� �����ϴ�. prefab({skilleffect.skillEffect})�� " +
                $"poolKey({skilleffect.effectPoolKey})�� Ȯ�����ּ���");
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
            Debug.Log($"[Enemy] pool���� ��ȯ�� �� �����ϴ�. Ű({skilleffect.effectPoolKey})Ȯ��");
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
    /// ���� ���� �ʱ�ȭ (�������̵��)
    /// </summary>
    public virtual void ResetEnemyState()
    {
        
    }    
    /// <summary>
    /// ���� ��� ó�� �� Ǯ ��ȯ ����
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
    /// ���� ��Ȱ ó�� �� ���� �ʱ�ȭ
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
    /// ���Ͱ� ��Ȱ �� Ÿ���� �����ϰ� �����ϴ� �ż���
    /// </summary>
    protected virtual void DecideNextState()
    {
        
    }
    /// <summary>
    /// ������ ��Ȱ �� �ִϸ��̼��� �����ϴ� �ż��� 
    /// </summary>
    protected virtual void ReviveAnimation()
    {

    }
    /// <summary>
    /// ������ ��Ȱ �� �⺻ ���¸� �����ϴ� �ż��� 
    /// </summary>
    public virtual void ReviveDefaultState()
    {

    }
    /// <summary>
    /// Ǯ�� ��ȯ �� ���� �ð� �� ��Ȱ ����
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
    /// ������ ��� ó��
    /// </summary>
    public virtual void DropItem()
    {
        if(itemdropSpawner == null)
        {
            Debug.Log("[Enemy] DropItemSpawner�� �����ϴ�.");
            return;
        }
        var table = GetitemDropTable();
        if(table == null)
        {
            Debug.Log("[Enemy] DropItemTable�� �����ϴ�.");
            return;
        }
        itemdropSpawner.SpwanDropItem(transform.position, table);
    }   
    /// <summary>
    /// �ڽ� Ŭ�������� ��Ȱ �� �⺻ ���� ���������� �ݵ�� �������̵��ؾ� ��
    /// </summary>
    public abstract void InitializeRevivedState();    

}
