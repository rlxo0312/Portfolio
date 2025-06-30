using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FieldOfView))]
public class Enemy_Monster_Zombie : Enemy
{
    public Enemy_MoveState moveState { get; private set; } 
    public Enemy_AttackState attackState { get; private set; }
    public Enemy_Dead deadState { get; private set; }
    public Enemy_IdleState idleState { get; private set; }
    public Enemy_Patrol patrolState { get; private set; }
    public Enemy_Data zombieData;
    public GameObject zombiePrefab;
    private static bool isPrefabRegistered = false;
    //private const string POOL_KEY = "ZombieA";
    public override string PoolKey => zombieData != null ? zombieData.poolKey : "default zombie";    
    public override string WayPointPathName => PathName;
    //public string pathName;
    [Header("Monster Item Drop")]
    public DropItemTable zombieDropItemTable;
    [Header("Monster Default Animation(몬스터의 각walk권장)")]
    [SerializeField] private string defaultAniName = "Walk";
    [Header("Monster Animation Name (몬스터 애니메이션 이름 적기(대소문자 띄워쓰기 구분)")]
    [SerializeField] private string moveAnimation = "Walk";
    [SerializeField] private string idleAnimation = "Idle";
    [SerializeField] private string attackAnimation = "Attack";
    [SerializeField] private string deadAnimation = "Dead";
    [SerializeField] private string patrolAnimation = "Walk";
    public override string DefaultAniName => defaultAniName;
    public override string DeadAniName => deadAnimation;
    public override string AttackAniName => attackAnimation;    
    //public override string 
    /*public override string PoolKey 
    {
        get { return POOL_KEY; }  
    }*/
    public override DropItemTable GetitemDropTable()
    {
        return zombieDropItemTable;
    }    
    protected override void Awake()
    {
        base.Awake();
        OnLoadComponents();

        moveState = new Enemy_MoveState(this, enemy_monster_StateMachine, moveAnimation);
        attackState = new Enemy_AttackState(this, enemy_monster_StateMachine, attackAnimation);
        deadState = new Enemy_Dead(this, enemy_monster_StateMachine, deadAnimation);
        idleState = new Enemy_IdleState(this, enemy_monster_StateMachine, idleAnimation);
        patrolState = new Enemy_Patrol(this, enemy_monster_StateMachine, patrolAnimation);

        /*stateMap = new Dictionary<EnemyStateType, Enemy_Monster_State>
        {
            { EnemyStateType.Move, moveState },
            { EnemyStateType.Idle, idleState },
            { EnemyStateType.Attack, attackState },
            { EnemyStateType.Dead, deadState},
            { EnemyStateType.Patrol, patrolState}
        };*/
        stateMap.Add(EnemyStateType.Move, moveState);
        stateMap.Add(EnemyStateType.Idle, idleState);
        stateMap.Add(EnemyStateType.Attack, attackState);
        stateMap.Add(EnemyStateType.Dead, deadState);
        stateMap.Add(EnemyStateType.Patrol, patrolState);
    } 
    protected override void Start()
    {
        base.Start();
        if(enemy_monster_StateMachine != null)
        {
            enemy_monster_StateMachine.Initilize(patrolState);
        }
        else
        {
            Debug.LogError("[Enemy_Monster_Zombie] StateMachine is null.");
        }

        /*Debug.Log($"[DEBUG] zombieData: {zombieData}");
        Debug.Log($"[DEBUG] zombieData.zombieName: {zombieData?.zombieName}");
        Debug.Log($"[DEBUG] monsterBattleUi: {monsterBattleUi}");*/
        /*if (monsterBattleUi != null)
        {
            Debug.Log($"[Zombie Init] monsterBattleUi is assigned.");
            if (zombieData != null)
            {
                Debug.Log($"[Zombie Init] Zombie Name: {zombieData.zombieName}");
            }
            else
            {
                Debug.LogWarning("[Zombie Init] zombieData is not assigned!");
            }
        }
        else
        {
            Debug.LogError("[Zombie Init] monsterBattleUi is not assigned in Inspector!");
        }*/
        if (!isPrefabRegistered)
        {
            if (zombiePrefab != null)
            {
                //ObjectPooling.instance.RegisterPrefab("ZombieA", zombiePrefab);
                ObjectPoolingManager.Instance.RegisterPrefab(PoolKey, zombiePrefab); //POOLKEY
                isPrefabRegistered = true;
            }
            else
            {
                Debug.LogError("[Enemy_Monster_Zombie] zombiePrefab is not assigned in Inspector.");
            }
        }
        if (zombieData != null && monsterBattleUi != null)
        {
            monsterBattleUi.SetName(zombieData.enemyName);
        }
        else
        {
            Debug.LogError("monsterBattleUi 또는 zombieData가 Null입니다.");
        }

    }
    protected override void Update()
    {
        base.Update();
    }
    public override void OnLoadComponents()
    {
        base.OnLoadComponents();
        if(zombieData != null)
        {
            LoadStatsFromData(zombieData);
        }        
    }
    public override void BeDamaged(float damage, Vector3 contactPos, GameObject hitEffectPrefabs = null)
    {
        base.BeDamaged(damage, contactPos, hitEffectPrefabs);
                
        if (IsAlive)
        {
            animator?.CrossFade("ZombieHit", 0.1f);           
           // enemy_monster_StateMachine.ChangeState(idleState);         

            monsterBattleUi.Value = HP;
            if (!target)
            {
                transform.forward = contactPos;
                navMeshAgent.SetDestination(contactPos);
            }            
        }
        else
        {
            //enemy_monster_StateMachine.ChangeState(deadState);
            monsterBattleUi.Value = 0;
            Die();
        }
    }
    public override void ResetEnemyState()
    {
        base.ResetEnemyState();
        enemy_monster_StateMachine.ChangeState(idleState);
    }
    public override void Die()
    {
        /*Debug.Log("[Die] Called on " + gameObject.name);
        if (enemy_monster_StateMachine == null)
        {
            Debug.LogError("[Die] enemy_monster_StateMachine is null");
            return;
        }
        if (deadState == null)
        {
            Debug.LogError("[Die] deadState is null");
            return;
        }*/
        
        if (isDead)
        {
            return;
        }
        isDead= true;
        HP = 0;
        QuestManager.instance?.OnMOnsterKilled(zombieData.enemyName);
        enemy_monster_StateMachine.ChangeState(deadState);
        //gameObject.SetActive(false);
        base.Die();        
        //StartCoroutine(DelayedReturnToPool());
    }
    
    public override void InitializeRevivedState()
    {
        /*monsterBattleUi.gameObject.SetActive(true);        
        HP = MaxHP;
        monsterBattleUi.Value = HP;*/
        enemy_monster_StateMachine.ChangeState(idleState);
        ReviveAnimation();
        ReviveDefaultState();
    }
    protected override void DecideNextState()
    {
        base.DecideNextState();
        if (target && IsAvailableAttack)
        {
            //enemy_monster_StateMachine.ChangeState(((Enemy_Monster_Zombie)this).attackState);
            enemy_monster_StateMachine.ChangeState(attackState);
        }
        else if(target)
        {
            enemy_monster_StateMachine.ChangeState(moveState);
        }
        else
        {
            //enemy_monster_StateMachine.ChangeState(((Enemy_Monster_Zombie)this).patrolState);
            Debug.LogWarning("[Enemy_Monster_Zombie - DecideNextState] No valid target, switching to Idle");
            enemy_monster_StateMachine.ChangeState(patrolState);
        }
    }
    protected override void ReviveAnimation()
    {
        base.ReviveAnimation();
        if (animator != null && animator.isActiveAndEnabled)
        {
            //animator.Rebind();
            //animator.Update(0f);
            //animator.CrossFade("Idle", 0.2f);
            //animator.Play("Walk", 0, 0);
            animator.Play(moveAnimation, 0, 0);
            /*if (IsAvailableAttack)
            {
                animator.CrossFade("Attack",0.2f);
            }*/
        }
    }
    public override void ReviveDefaultState()
    {
        if (enemy_monster_StateMachine == null)
        {
            // 만약 StateMachine이 null이라면 초기화해준다.
            Debug.LogWarning("[Enemy_Monster_Zombe] - [ReviveDefaultState] StateMachine is null, initializing...");
            
        }
        base.ReviveDefaultState();
        if(enemy_monster_StateMachine != null && patrolState != null)
        {
            enemy_monster_StateMachine.ChangeState(patrolState);
        }
        else
        {
            Debug.LogWarning("[Enemy_Monster_Zombie] StateMachine or Patrol state is null.");
        }
    }
    
}
