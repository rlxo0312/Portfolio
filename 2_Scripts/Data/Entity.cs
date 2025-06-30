using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ������ ���õ� ��� ������Ʈ(Player, Monster ��)�� ���� ��� Ŭ����
///
/// <para>��� ����</para>
/// <para>public float HP, MaxHP, MP, MaxMP, AttackPower, Defense</para>
/// <para>public bool IsAlive { get; }</para>
///
/// <para>�ʱ�ȭ �� �����ֱ�</para>
/// <para>private void OnInitalize()</para> 
/// <para>������Ʈ �ε�</para>
/// <para>public virtual void OnLoadComponents()</para>
///
/// <para>���� ó��</para>
/// <para>public virtual void BeDamaged(float damage, Vector3 contactPos, GameObject hitEffectPrefabs = null)</para>
/// </summary>
public class Entity : MonoBehaviour, IDamageable
{
    [Header("Status")]
    public float HP;
    public float MaxHP;
    public float MP;
    public float MaxMP;
    public float AttackPower;
    public float Defense;

    [Header("������ ���� ���� ����")]
    [SerializeField] public bool isInvincibility = false;
    public bool IsInvincibility => isInvincibility;

    public Animator animator;

    public bool IsAlive => HP > 0; 
    //public bool IsDead => HP <= 0;

    protected virtual void Awake()
    {
        OnLoadComponents();

    }
    protected virtual void Start()
    {
        OnInitalize();
    }

    protected virtual void Update()
    {

    }
    /// <summary>
    /// ���۽� HP, MP �ʱ�ȭ
    /// </summary>
    private void OnInitalize() //���۽� �ʱ�ȭ 
    {
        HP = MaxHP; 
        MP = MaxMP;
    }
    public virtual void OnLoadComponents()
    {
        animator = GetComponent<Animator>();
    }
    /// <summary>
    /// entity�� ����ϴ� ��󿡰� �������� ���ϴ� virtual �ż���
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="contactPos"></param>
    /// <param name="hitEffectPrefabs"></param>
    public virtual void BeDamaged(float damage, Vector3 contactPos, GameObject hitEffectPrefabs = null)
    {       

        float finalDamage = Mathf.Max(0,damage);
        HP -= finalDamage;   

        if(HP < 0)
        {
            HP = 0;
        }
    }
}
