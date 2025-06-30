using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ �þ߸� ����ϴ� Ŭ�����Դϴ�.
/// 
/// <para>��� ����</para>
/// <para>public EnemyReference enemyref</para>
/// <para>public float fieldViewRange, viewAngle, findDelay, rayPosition</para>
/// <para>public LayerMask targetMask, obstacleMask</para>
/// <para>private List&lt;Transform&gt; visibleTargets</para>
/// <para>private Transform nearestTarget</para>
/// <para>private float distanceToTarget</para>
/// <para>public Transform NearestTarget { get; }</para>
/// <para>public List&lt;Transform&gt; VisibleTarget { get; }</para>
/// 
/// <para>Ÿ�� Ž�� ó��</para>
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

    [Header("�þ� �� ���� �ִ� Ÿ�� �� �����̿� �ִ� ����� ã�� ����")]

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
    /// ���� �ð� �������� �þ� �� Ÿ���� Ž���ϴ� �ڷ�ƾ
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
    /// �þ� ���� ���̴� Ÿ�ٵ��� ã�� ����Ʈ�� �߰��ϰ�, ���� ����� Ÿ���� ����
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
    /// ������ Ÿ�� Ž���� ����
    /// </summary>
    public void ForceSearch()
    {
        FIndVisibleTargets();

    }
    /// <summary>
    /// ������Ʈ Ȱ��ȭ �� �ڵ����� Ÿ�� Ž�� �ڷ�ƾ ����
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
