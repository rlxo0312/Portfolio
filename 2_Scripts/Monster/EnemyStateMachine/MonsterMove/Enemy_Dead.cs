using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// ���Ͱ� ������� ���� ���¸� �����ϴ� Ŭ����
/// ��� �ִϸ��̼��� ����ϰ� ���� �ð� �� ���� ��� ó���� ������
/// NavMeshAgent�� ��Ȱ��ȭ�ϰ� ���� ���� ������� ������
///
/// <para>��� ����</para>
/// <para>stateTimer = 5f �� ��� ��� �ð� ����</para>
/// </summary>
public class Enemy_Dead : Enemy_Monster_State
{    
    public Enemy_Dead(Enemy _enemy, Enemy_Monster_StateMachine _monster_StateMachine, string _animationName) : base(_enemy, _monster_StateMachine, _animationName)
    {
        
    }
    public override void Enter()
    {
        base.Enter();
        //Debug.Log("Entering Dead State");        
        enemy.navMeshAgent.isStopped = true;       
        enemy.animator.CrossFade(enemy.DeadAniName, 0.2f);        
        stateTimer = 5f;
    }
    public override void Exit() 
    {
        base.Exit();
        //Debug.Log("Exiting Dead State");

    }
    public override void Update()
    {
        base.Update();
        if(stateTimer < 0)
        {
            if (!enemy.navMeshAgent.isOnNavMesh)
            {
                Debug.LogWarning($"{enemy.name} is not on NavMesh!");
            }
            //Debug.Log($"Animator active: {enemy.animator?.isActiveAndEnabled}");
            //Debug.Log($"NavMeshAgent active: {enemy.navMeshAgent?.isActiveAndEnabled}, isOnNavMesh: {enemy.navMeshAgent?.isOnNavMesh}");
            //Debug.Log($"Animator GameObject Active: {enemy.animator.gameObject.activeInHierarchy}");
            //Debug.Log($"NavMeshAgent GameObject Active: {enemy.navMeshAgent.gameObject.activeInHierarchy}");
            //Debug.Log($"[ChangeState ��] Animator enabled: {zombie.animator.enabled}, Animator gameObject active: {zombie.animator.gameObject.activeInHierarchy}");            
            enemy.Die();
        }
    }  
   
}
