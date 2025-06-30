using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// npc에서 animator를 가져오는 class 
/// <para>컴포넌트 로드</para>
/// <para>public virtual void OnLoadComponents()</para>
/// </summary>
public class Client : MonoBehaviour
{
    public Animator animator;
   protected virtual void Awake()
    {
        OnLoadComponents();
    }

    protected virtual void Start()
    {

    }
    protected virtual void Update()
    {

    }
    public virtual void OnLoadComponents()
    {
        animator = GetComponent<Animator>();
    }
}
