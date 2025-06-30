using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// npc���� animator�� �������� class 
/// <para>������Ʈ �ε�</para>
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
