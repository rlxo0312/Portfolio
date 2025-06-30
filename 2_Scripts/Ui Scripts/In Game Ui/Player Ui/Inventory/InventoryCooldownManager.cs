using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �κ��丮 ������ ��ٿ��� ���� ���� �����۵��� �����ϴ� �Ŵ��� Ŭ����.
/// UI�� ���� �־ ��Ÿ���� ���ӵǵ��� ���������� ������.
///
/// <para>��� ����</para>
/// <para>private List&lt;InventoryItem&gt; cooldownItems</para>
///
/// <para>��� �޼���</para>
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
    /// ��ٿ� ó���� �������� ���
    /// </summary>
    /// <param name="item">��ٿ� ��� ������</param>
    public void RegisterCooldown(InventoryItem item)
    {
        if (item == null || cooldownItems.Contains(item))
            return;

        cooldownItems.Add(item);
    }
}
