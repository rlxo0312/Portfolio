using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FieldOfView))]
public class Enemy_Boss_Warrok : Enemy
{
    public Enemy_MoveState moveState_Warrok { get; private set; }
    public Enemy_AttackState attackState_Warrok { get; private set; }
    public Enemy_Dead deadState_Warrok { get; private set; }
    public Enemy_IdleState idleState_Warrok { get; private set; }
    public Enemy_Patrol patrolState_Warrok { get; private set; }
    public Enemy_RunState runState_Warrok { get; private set; }
    public Enemy_FatalSkill fatalAttack_Warrok { get; private set; }
    public Enemy_Data warrokData;
    public GameObject warrokPrefab;
    private static bool isPrefabRegistered = false;
    private bool isInitialized = false;
    [Header("Monster Item Drop")]
    public DropItemTable warrokDropItemTable;
    public override string PoolKey => warrokData != null? warrokData.poolKey : "Warrok";
    public override string WayPointPathName => PathName;
    //public string pathName; 
    //[Header("inspector 내부에 해당 몬스터의 navmeshPath들의 부모이름")]
    //[SerializeField] private string pathNameWarrok = "WarrokPaths";
    //public override string WayPointPathName => pathNameWarrok;
    [Header("Monster Default Animation(몬스터의 각walk권장)")]
    [SerializeField] private string defaultAniName_Warrok = "WarrokWalk";
    [Header("Monster Animation Name (몬스터 애니메이션 이름 적기(대소문자 띄워쓰기 구분)")]
    [SerializeField] private string moveAnimation_Warrok = "WarrokWalk";
    [SerializeField] private string idleAnimation_Warrok = "WarrokIdle";
    [SerializeField] private string attackAnimation_Warrok = "WarrokAttack2";
    [SerializeField] private string deadAnimation_Warrok = "WarrokDead";
    [SerializeField] private string patrolAnimation_Warrok = "WarrokWalk";
    [SerializeField] private string runAnimation_Warrok = "WarrokRun";
    [SerializeField] private string fatalSkillAnimation_Warrok = "WarrokPowerAttack";
    public override string DefaultAniName => defaultAniName_Warrok;
    public override string AttackAniName => attackAnimation_Warrok;
    public override string DeadAniName => deadAnimation_Warrok;
    public override string RunAniName => runAnimation_Warrok;
    public override string FatalSkillAniName => fatalSkillAnimation_Warrok;
    public override DropItemTable GetitemDropTable()
    {
        return warrokDropItemTable;
    }
   /* protected void OnEnable()
    {
        if (enemy_monster_StateMachine != null)
        {
            enemy_monster_StateMachine.Initilize(patrolState_Warrok);
        }
        else
        {
            Debug.LogError("[Enemy_Boss_Warrok] StateMachine is null.");
        }
    }*/

    protected override void Awake()
    {
        base.Awake();

        InitMonsterSkillFlag();
        OnLoadComponents();

        moveState_Warrok = new Enemy_MoveState(this, enemy_monster_StateMachine, moveAnimation_Warrok);
        idleState_Warrok = new Enemy_IdleState(this, enemy_monster_StateMachine, idleAnimation_Warrok);
        deadState_Warrok = new Enemy_Dead(this, enemy_monster_StateMachine, deadAnimation_Warrok);
        attackState_Warrok = new Enemy_AttackState(this, enemy_monster_StateMachine, attackAnimation_Warrok);
        patrolState_Warrok = new Enemy_Patrol(this, enemy_monster_StateMachine, patrolAnimation_Warrok);
        runState_Warrok = new Enemy_RunState(this, enemy_monster_StateMachine, runAnimation_Warrok);
        //fatalAttack_Warrok = new Enemy_FatalAttack(this, enemy_monster_StateMachine, fatalAttackAnimation_Warrok);

        /*stateMap = new Dictionary<EnemyStateType, Enemy_Monster_State>()
        {
            { EnemyStateType.Move, moveState_Warrok },
            { EnemyStateType.Idle, idleState_Warrok },
            { EnemyStateType.Dead, deadState_Warrok },
            { EnemyStateType.Attack, attackState_Warrok },
            { EnemyStateType.Patrol, patrolState_Warrok },
            { EnemyStateType.Run, RunState_Warrok},
            { EnemyStateType.FatalAttack, FatalAttack_Warrok}
        };*/
        stateMap.Add(EnemyStateType.Move, moveState_Warrok);
        stateMap.Add(EnemyStateType.Idle, idleState_Warrok);
        stateMap.Add(EnemyStateType.Dead, deadState_Warrok);
        stateMap.Add(EnemyStateType.Attack, attackState_Warrok);
        stateMap.Add(EnemyStateType.Patrol, patrolState_Warrok);
        stateMap.Add(EnemyStateType.Run, runState_Warrok);
        //stateMap.Add(EnemyStateType.FatalAttack, fatalAttack_Warrok);
    }
    protected override void Start()
    {
        base.Start();
        Debug.Log($"[WarrokDebug] Assign 시작 전 wayPoints null? {wayPoints == null}");
        //wayPoints = null;
        //AssignWayPoints("WarrokPaths");
        Debug.Log($"[WarrokDebug] Assign 이후 wayPoints null? {wayPoints == null}, 개수: {wayPoints?.Length}");
        Transform pathRoot = GameObject.Find("WarrokPaths")?.transform;
        if (pathRoot == null)
        {
            Debug.LogError("[AssignWayPoints] WarrokPaths 오브젝트를 찾을 수 없습니다!");
        }
        if (wayPoints == null || wayPoints.Length == 0)
        {
            Debug.LogWarning("[Enemy_Boss_Warrok] wayPoints가 비어있어서 상태 초기화를 생략합니다.");
            return; 
        }
        if (enemy_monster_StateMachine != null)
        {
            enemy_monster_StateMachine.Initilize(patrolState_Warrok);
        }
        else
        {
            Debug.LogError("[Enemy_Boss_Warrok] StateMachine is null.");
        }

        if (!isPrefabRegistered)
        {
            if(warrokPrefab != null)
            {
                ObjectPoolingManager.Instance.RegisterPrefab(PoolKey, warrokPrefab);
                isPrefabRegistered=true;
                
            }
            else
            {
                Debug.LogError("[Enemy_Boss_Warrok] warrokPrefab is not assigned in Inspector.");
            }
        }
        if(warrokPrefab != null && monsterBattleUi != null)
        {
            monsterBattleUi.SetName(warrokData.enemyName);
        }
        else
        {
            Debug.LogError("monsterBattleUi 또는 warrokData가 Null입니다.");
        }
    }
    protected override void Update()
    {
        base.Update();
        if (!isInitialized && wayPoints != null && wayPoints.Length > 0)
        {
            enemy_monster_StateMachine.Initilize(patrolState_Warrok);
            isInitialized = true;
        }
        CheckMonsterSkillUse_HP();
    }
    public override void OnLoadComponents()
    {
        base.OnLoadComponents();
        if(warrokData != null) 
        {
            LoadStatsFromData(warrokData);
        }  
        /*if(WayPoints == null || WayPoints.Length == 0)
        {
            Transform pathRoot = GameObject.Find("WarrokPaths")?.transform;
            if(pathRoot != null)
            {
                wayPoints = new Transform[pathRoot.childCount];
            }
        }*/
    }
    public override void BeDamaged(float damage, Vector3 contactPos, GameObject hitEffectPrefabs = null)
    {
        base.BeDamaged(damage, contactPos, hitEffectPrefabs);
        if (IsAlive)
        {
            animator?.CrossFade("WarrokHit", 0.1f);

            monsterBattleUi.Value = HP;
            if (!target)
            {
                transform.forward = contactPos;
                navMeshAgent.SetDestination(contactPos);
            }
        }
        else
        {
            monsterBattleUi.Value = 0;
            Die();
        }
    }      
    public override void ResetEnemyState()
    {
        base.ResetEnemyState();
        enemy_monster_StateMachine.ChangeState(idleState_Warrok);
    }
    public override void Die()
    {
        if (isDead)
        {
            return;
        }
        isDead = true;
        HP = 0;
        QuestManager.instance?.OnMOnsterKilled(warrokData.enemyName);
        enemy_monster_StateMachine.ChangeState(deadState_Warrok);
        base.Die();
    }
    public override void InitializeRevivedState()
    {
        enemy_monster_StateMachine.ChangeState(idleState_Warrok);
        ReviveAnimation();
        ReviveDefaultState();
    }
    protected override void DecideNextState()
    {
        base.DecideNextState();
        /*if (target && IsAvailableAttack)
        {
            enemy_monster_StateMachine.ChangeState(attackState_Warrok);
        }
        else if (target)
        {
            float distanceBetween = (target.position - transform.position).sqrMagnitude;
            float chaesRange = AttackRange + increasedRunRange;
            Debug.Log($"[Warrok] target 거리 sqr: {distanceBetween}, 비교값: {chaesRange * chaesRange}");
            if (distanceBetween >= chaesRange * chaesRange)
            {
                enemy_monster_StateMachine.ChangeState(runState_Warrok);
                Debug.Log("[Enemy_Boss_Warrok - Warrok] 타겟이 멀리 있어 Run 상태 전환");
            }
            else
            {
                enemy_monster_StateMachine.ChangeState(moveState_Warrok);
                Debug.Log("[Enemy_Boss_Warrok - Warrok] 가까워서 Move 상태 전환");
            }
        }
        else
        {
            Debug.LogWarning("[Enemy_Boss_Warrok - DecideNextState] No valid target, switching to Idle");
            enemy_monster_StateMachine.ChangeState(patrolState_Warrok);
        }*/
        if (target)
        {
            float distanceBetween = (target.position - transform.position).sqrMagnitude;
            float chaesRange = AttackRange + increasedRunRange;
            Debug.Log($"[Warrok] Target 거리: {Mathf.Sqrt(distanceBetween):F2}, Chase 조건: {chaesRange}"); 

            if(distanceBetween >= chaesRange * chaesRange)
            {
                enemy_monster_StateMachine.ChangeState(runState_Warrok);
                Debug.Log("[Warrok] Run 상태 전환");
            }
            else if (IsAvailableAttack)
            {
                enemy_monster_StateMachine.ChangeState(attackState_Warrok);
            }
            else
            {
                enemy_monster_StateMachine.ChangeState(moveState_Warrok);
            }
        }
    }
    protected override void ReviveAnimation()
    {
        base.ReviveAnimation();
        if(animator != null && animator.isActiveAndEnabled)
        {
            animator.Play(moveAnimation_Warrok, 0, 0);
        }
    }
    public override void ReviveDefaultState()
    {
        if(enemy_monster_StateMachine != null)
        {
            Debug.LogWarning("[Enemy_Boss_Warrok] - [ReviveDefaultState] StateMachine is null, initializing...");
        }
        base.ReviveDefaultState();

        if(enemy_monster_StateMachine != null && patrolState_Warrok != null)
        {
            enemy_monster_StateMachine.ChangeState(patrolState_Warrok);
        }
        else
        {
            Debug.LogWarning("[Enemy_Boss_Warrok] StateMachine or Patrol state is null.");
        }
    }
}
