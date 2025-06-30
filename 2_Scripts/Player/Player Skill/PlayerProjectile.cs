using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 발사한 투사체를 제어하는 클래스.
/// 속도, 데미지, 지속 시간 등을 설정하며 충돌 시 적에게 피해를 입히고 자동 반환 처리함.
///
/// <para>사용 변수</para>
/// <para>public float speed, public float lifeTime, public float damage</para>
/// <para>public string PoolKey</para>
/// <para>private Coroutine autoReturnCoroutine, private Rigidbody rb</para>
///
/// <para>사용 메서드</para>  
/// <para>public void Initilize(float speed, float lifeTime, float damage)</para>
/// <para>private IEnumerator AutoReturnPool()</para>
/// <para>private void OnTriggerEnter(Collider other)</para>
/// </summary>
public class PlayerProjectile : MonoBehaviour
{
    public float speed;
    public float lifeTime;
    public float damage;
    public string PoolKey;
    private Coroutine autoReturnCoroutine;
    private Rigidbody rb;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();        
    }
    private void OnEnable()
    {
        /*if(rb != null)
        {
            rb.velocity = transform.forward * speed;  
        }
        autoReturnCoroutine = StartCoroutine(AutoReturnPool());*/
    }

    private void OnDisable()
    {
        if(autoReturnCoroutine != null)
        {
            StopCoroutine(autoReturnCoroutine);
            autoReturnCoroutine = null;
        }

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
    }
    /// <summary>
    /// 투사체를 초기화하며 속도, 생존시간, 데미지를 설정하고 자동 반환 코루틴을 실행함
    /// </summary>
    /// <param name="speed">투사체 속도</param>
    /// <param name="lifeTime">투사체 유지 시간</param>
    /// <param name="damage">투사체 데미지</param>
    public void Initilize(float speed, float lifeTime, float damage)
    {
        this.speed = speed;
        this.lifeTime = lifeTime;   
        this.damage = damage;

        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }

        if (autoReturnCoroutine != null)
        {
            StopCoroutine(autoReturnCoroutine);
        }

        autoReturnCoroutine = StartCoroutine(AutoReturnPool());
    }
    /// <summary>
    /// 일정 시간이 지나면 오브젝트를 풀로 반환
    /// </summary>
    /// <returns>코루틴</returns>
    private IEnumerator AutoReturnPool()
    {
        yield return WaitForSecondsCache.Get(lifeTime);
        ObjectPoolingManager.Instance.ReturnToPool(PoolKey, this.gameObject);
    }
    /// <summary>
    /// 적과 충돌 시 데미지를 계산하여 적용하고, 데미지 텍스트를 생성
    /// </summary>
    /// <param name="other">충돌한 콜라이더</param>
    private void OnTriggerEnter(Collider other)
    {
        if(!gameObject.activeInHierarchy)
        {
            return;
        }

        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            var enemyRef = other.GetComponent<EnemyReference>();

            if (enemyRef != null && enemyRef.Enemy != null)
            {
                float finalDamage = damage * (100 - enemyRef.Enemy.Defense)/100;
                enemyRef.Enemy.BeDamaged(finalDamage, transform.position);

                if (DamageTextSpawner.Instance != null)
                {
                    DamageTextSpawner.Instance.ShowDamage(enemyRef.transform.position, finalDamage);
                }
            }
            else 
            {
                Debug.Log($"[PlayerProjectile]  EnemyReference가 없거나 Enemy가 null입니다. 충돌 오브젝트: {other.name}");
            }
        }

        //ObjectPoolingManager.Instance.ReturnToPool(PoolKey, this.gameObject);
         
    }
}
