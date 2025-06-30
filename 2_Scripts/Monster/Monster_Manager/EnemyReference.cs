using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy 컴포넌트를 1회만 GetComponent 하고 재사용하기 위한 캐싱용 클래스
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
            Debug.LogWarning($"[EnemyReference]: {gameObject.name}의 Enemy컴포넌트가 없습니다.");
        }
    }
}
