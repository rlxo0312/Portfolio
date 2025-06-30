using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerManager ������Ʈ�� 1ȸ�� GetComponent �ϰ� �����ϱ� ���� ĳ�̿� Ŭ����
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
            Debug.LogWarning($"[PlayerManagerReference] {name}�� PlayerManager ������Ʈ�� �����ϴ�.");
        }
    }
}
