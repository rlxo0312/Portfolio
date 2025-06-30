using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy ������Ʈ�� 1ȸ�� GetComponent �ϰ� �����ϱ� ���� ĳ�̿� Ŭ����
/// </summary>
[RequireComponent(typeof(Enemy))]
public class EnemyReference : MonoBehaviour
{
    private Enemy enemy;

    public Enemy Enemy => enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();  
        if(enemy == null)
        {
            Debug.LogWarning($"[EnemyReference]: {gameObject.name}�� Enemy������Ʈ�� �����ϴ�.");
        }
    }
}
