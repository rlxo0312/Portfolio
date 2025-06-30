using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인벤토리 내에서 쿨다운이 진행 중인 아이템들을 관리하는 매니저 클래스.
/// UI가 꺼져 있어도 쿨타임이 지속되도록 독립적으로 관리함.
///
/// <para>사용 변수</para>
/// <para>private List&lt;InventoryItem&gt; cooldownItems</para>
///
/// <para>사용 메서드</para>
/// <para>public void RegisterCooldown(InventoryItem item)</para>
/// <para>private void Update()</para>
/// </summary>
public class InventoryCooldownManager : MonoBehaviour
{
    public static InventoryCooldownManager Instance { get; private set; }

    private List<InventoryItem> cooldownItems = new List<InventoryItem>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        for (int i = cooldownItems.Count - 1; i >= 0; i--)        
        {
            var item = cooldownItems[i];
            if (item == null || !item.isCooldown)
            {
                cooldownItems.RemoveAt(i);
                continue;
            }

            item.cooldownTimer -= Time.deltaTime;

            if (item.cooldownTimer <= 0)
            {
                item.cooldownTimer = 0;
                item.isCooldown = false;   
                
                item.RefreshAllLinkedSlots();

                cooldownItems.RemoveAt(i);
                continue;
            }

            item.RefreshAllLinkedSlots();
        }
    }

    /// <summary>
    /// 쿨다운 처리할 아이템을 등록
    /// </summary>
    /// <param name="item">쿨다운 대상 아이템</param>
    public void RegisterCooldown(InventoryItem item)
    {
        if (item == null || cooldownItems.Contains(item))
            return;

        cooldownItems.Add(item);
    }
}
