using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터의 시야를 담당하는 클래스입니다.
/// 
/// <para>사용 변수</para>
/// <para>public EnemyReference enemyref</para>
/// <para>public float fieldViewRange, viewAngle, findDelay, rayPosition</para>
/// <para>public LayerMask targetMask, obstacleMask</para>
/// <para>private List&lt;Transform&gt; visibleTargets</para>
/// <para>private Transform nearestTarget</para>
/// <para>private float distanceToTarget</para>
/// <para>public Transform NearestTarget { get; }</para>
/// <para>public List&lt;Transform&gt; VisibleTarget { get; }</para>
/// 
/// <para>타겟 탐지 처리</para>
/// <para>private IEnumerator FindTargetsWithDelay(float delay)</para>
/// <para>private void FIndVisibleTargets()</para>
/// <para>public void ForceSearch()</para>
/// </summary>
public class FieldOfView : MonoBehaviour
{
    //public Enemy enemy;
    public EnemyReference enemyref;

    [HideInInspector]public float fieldViewRange;
    [Range(0, 360)]
    public float viewAngle = 90f;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [Header("시야 각 내에 있는 타겟 중 가까이에 있는 대상을 찾는 변수")]

    private List<Transform> visibleTargets = new List<Transform>();
    private Transform nearestTarget;
    private float distanceToTarget = 0.0f;

    public float findDelay = 0.2f;   
    public float rayPosition = 0.0f;
    public Transform NearestTarget => nearestTarget; 
    public List<Transform> VisibleTarget => visibleTargets;

    private void Awake()
    {
        /*enemy = GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("Enemy component not found on this GameObject.");
        }
        fieldViewRange = enemy.viewRange;*/
        //viewRange = zombie.ViewRange;
        /*enemyref = GetComponent<EnemyReference>();
        if (enemyref == null && enemyref.Enemy == null)
        {
            Debug.LogError("Enemy component not found on this GameObject.");
        }
        fieldViewRange = enemyref.Enemy.viewRange;*/
    }
    // Start is called before the first frame update
    private void Start()
    {
        //StartCoroutine(FindTargetsWithDelay(findDelay));
        enemyref = GetComponent<EnemyReference>();
        if (enemyref == null && enemyref.Enemy == null)
        {
            Debug.LogError("Enemy component not found on this GameObject.");
        }
        fieldViewRange = enemyref.Enemy.viewRange;

    }
    /// <summary>
    /// 일정 시간 간격으로 시야 내 타겟을 탐색하는 코루틴
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            //yield return new WaitForSeconds(delay);
            yield return WaitForSecondsCache.Get(delay);
            FIndVisibleTargets();
        }
    }
    /// <summary>
    /// 시야 내에 보이는 타겟들을 찾아 리스트에 추가하고, 가장 가까운 타겟을 선정
    /// </summary>
    void FIndVisibleTargets()
    {
        distanceToTarget = 0.0f;
        nearestTarget = null;
        visibleTargets.Clear();

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position,fieldViewRange, targetMask);//fieldViewRange
        //Debug.Log($"[FOV] targetsInViewRadius.Length = {targetsInViewRadius.Length}");
        /*if (targetsInViewRadius.Length == 0)
        {
            Debug.LogWarning("[FOV] No targets in view radius.");
        }*/

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            //Vector3 dirToTarget = (target.position - transform.position).normalized;
            Vector3 lay = transform.position + Vector3.up * rayPosition;
            Vector3 dirToTarget = (target.position - lay).normalized;
            float dstToTarget = Vector3.Distance(lay, target.position);

            Debug.DrawRay(lay, dirToTarget * dstToTarget, Color.red, 1.0f);

            //float angle = Vector3.Angle(transform.forward, dirToTarget);
            //Debug.Log($"[FOV] Target: {target.name}, Angle: {angle}");

            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle /2)
            {
                //float dstToTarget = Vector3.Distance(transform.position, target.position);
                //Debug.Log($"[FOV] Distance to Target: {dstToTarget}");
                //Vector3 lay = transform.position + Vector3.up * rayPosition;
                if (!Physics.Raycast(lay, dirToTarget, dstToTarget, obstacleMask))//!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask)
                {
                    //Debug.Log($"[FOV] Target {target.name} is visible!");
                    visibleTargets.Add(target);

                    if(nearestTarget == null || distanceToTarget > dstToTarget)
                    {
                        nearestTarget = target;
                        distanceToTarget = dstToTarget;
                        //Debug.Log($"[FOV] VisibleTargets.Count: {visibleTargets.Count}, NearestTarget: {NearestTarget}");
                        //Debug.Log($"[FOV] Found nearest target: {nearestTarget.name}");
                    }
                }
            }
            /* else
             {
                 Debug.Log($"[FOV] Raycast hit obstacle before reaching {target.name}");
             }*/
            /*string nearestName = nearestTarget != null ? nearestTarget.name : "null";
            Debug.Log($"[FOV] VisibleTargets.Count: {visibleTargets.Count}, NearestTarget: {nearestName}");*/
        } 
    }
    /// <summary>
    /// 강제로 타겟 탐색을 실행
    /// </summary>
    public void ForceSearch()
    {
        FIndVisibleTargets();

    }
    /// <summary>
    /// 컴포넌트 활성화 시 자동으로 타겟 탐색 코루틴 시작
    /// </summary>
    private void OnEnable()
    {
        StartCoroutine(FindTargetsWithDelay(findDelay));
    }
    /*public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemy.viewRange);
    }*/
}
