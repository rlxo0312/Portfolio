using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾ �߻��� ����ü�� �����ϴ� Ŭ����.
/// �ӵ�, ������, ���� �ð� ���� �����ϸ� �浹 �� ������ ���ظ� ������ �ڵ� ��ȯ ó����.
///
/// <para>��� ����</para>
/// <para>public float speed, public float lifeTime, public float damage</para>
/// <para>public string PoolKey</para>
/// <para>private Coroutine autoReturnCoroutine, private Rigidbody rb</para>
///
/// <para>��� �޼���</para>  
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
    /// ����ü�� �ʱ�ȭ�ϸ� �ӵ�, �����ð�, �������� �����ϰ� �ڵ� ��ȯ �ڷ�ƾ�� ������
    /// </summary>
    /// <param name="speed">����ü �ӵ�</param>
    /// <param name="lifeTime">����ü ���� �ð�</param>
    /// <param name="damage">����ü ������</param>
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
    /// ���� �ð��� ������ ������Ʈ�� Ǯ�� ��ȯ
    /// </summary>
    /// <returns>�ڷ�ƾ</returns>
    private IEnumerator AutoReturnPool()
    {
        yield return WaitForSecondsCache.Get(lifeTime);
        ObjectPoolingManager.Instance.ReturnToPool(PoolKey, this.gameObject);
    }
    /// <summary>
    /// ���� �浹 �� �������� ����Ͽ� �����ϰ�, ������ �ؽ�Ʈ�� ����
    /// </summary>
    /// <param name="other">�浹�� �ݶ��̴�</param>
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
                Debug.Log($"[PlayerProjectile]  EnemyReference�� ���ų� Enemy�� null�Դϴ�. �浹 ������Ʈ: {other.name}");
            }
        }

        //ObjectPoolingManager.Instance.ReturnToPool(PoolKey, this.gameObject);
         
    }
}
