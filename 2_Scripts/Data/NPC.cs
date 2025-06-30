using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum NPCMoveType
{
    Idle,
    Patrol,
    Wander
}
/// <summary>
/// NPC�� �̵� ���(Patrol, Static, Wander)�� �����ϸ�, NavMesh ������� WayPoint ��θ� ���� �̵��ϴ� Ŭ����.
///
/// <para>��� ����</para>
/// <para>public float idleTime, moveTime</para>
/// <para>private Transform[] wayPoints</para>
/// <para>public Transform targetWayPoint</para>
/// <para>private int wayPointIndex</para>
/// <para>public NPCMoveType moveType</para>
/// <para>public NavMeshAgent navMeshAgent</para>
/// <para>public Rigidbody rigidbody</para>
/// <para>public NPCUI npcUI</para>
/// <para>public NPC_StateMachine npc_StateMachine</para>
/// <para>public enum NPCMoveType(Idle, Patrol, Wander)</para>
///
/// <para>�ʱ�ȭ �� �����ֱ�</para>
/// <para>public override void OnLoadComponents()</para>
///
/// <para>WayPoint �̵� ����</para>
/// <para>public bool NPCCheckRemainDistance()</para>
/// <para>public Transform NPCFindNextWayPoint()</para>
///
/// </summary>
public class NPC : Client
{
    [Header("NPC Move Stat")]
    public float idleTime;
    public float moveTime;

    [Header("NPC Patrol")]
    [SerializeField] private Transform[] wayPoints;
    [HideInInspector] public Transform targetWayPoint = null;
    private int wayPointIndex = 0;
    public NPC_StateMachine npc_StateMachine { get; private set; }
    public NavMeshAgent navMeshAgent;
    public new Rigidbody rigidbody;
    
    public NPCUI npcUI;
    public NPCMoveType moveType = NPCMoveType.Patrol;
    protected override void Awake()
    {
        base.Awake();
        npc_StateMachine = new NPC_StateMachine();
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if(moveType == NPCMoveType.Idle && navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        npc_StateMachine.NPC_State.Update();
    }

    public override void OnLoadComponents()
    {
        base.OnLoadComponents();
        navMeshAgent = GetComponent<NavMeshAgent>();
        rigidbody = GetComponent<Rigidbody>();
    }
    /// <summary>
    /// WayPoint�� �����ߴ��� Ȯ�� �� �ε��� ������Ʈ
    /// </summary>
    /// <returns>���� ����</returns>
    public bool NPCCheckRemainDistance()
    {
        if(wayPoints.Length == 0)
        {
            return false;
        }
        if ((wayPoints[wayPointIndex].transform.position - transform.position).sqrMagnitude < 0.1)
        {
            if (wayPointIndex < wayPoints.Length - 1)
            {
                wayPointIndex++;
            }
            else
            {
                wayPointIndex = 0;
            }
            return false;
        }
        return true;
    }
    /// <summary>
    /// ���� WayPoint ��ȯ
    /// </summary>
    /// <returns>Ÿ�� WayPoint</returns>
    public Transform NPCFindNextWayPoint()
    {
        targetWayPoint = null;
        if (wayPoints.Length > 0)
        {
            targetWayPoint = wayPoints[wayPointIndex];
        }
        return targetWayPoint;
    }
}
