using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 전투와 관련된 모든 오브젝트(Player, Monster 등)의 공통 기반 클래스
///
/// <para>사용 변수</para>
/// <para>public float HP, MaxHP, MP, MaxMP, AttackPower, Defense</para>
/// <para>public bool IsAlive { get; }</para>
///
/// <para>초기화 및 생명주기</para>
/// <para>private void OnInitalize()</para> 
/// <para>컴포넌트 로드</para>
/// <para>public virtual void OnLoadComponents()</para>
///
/// <para>전투 처리</para>
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

    [Header("몬스터의 무적 가능 여부")]
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
    /// 시작시 HP, MP 초기화
    /// </summary>
    private void OnInitalize() //시작시 초기화 
    {
        HP = MaxHP; 
        MP = MaxMP;
    }
    public virtual void OnLoadComponents()
    {
        animator = GetComponent<Animator>();
    }
    /// <summary>
    /// entity를 상속하는 대상에게 데미지를 가하는 virtual 매서드
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
