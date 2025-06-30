using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerManager 컴포넌트를 1회만 GetComponent 하고 재사용하기 위한 캐싱용 클래스
/// </summary>
public class PlayerManagerReference : MonoBehaviour
{
    private PlayerManager playerManager;
    public PlayerManager PlayerManager => playerManager;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        if (playerManager == null)
        {
            Debug.LogWarning($"[PlayerManagerReference] {name}에 PlayerManager 컴포넌트가 없습니다.");
        }
    }
}
