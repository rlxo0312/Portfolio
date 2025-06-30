using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��� �������� �����ϴ� Ŭ����
/// </summary>
public class DropItemSpawner : MonoBehaviour
{
    /// <summary>
    /// ������ ��ġ�� ��� ���̺��� ������� �������� ������
    /// </summary>
    /// <param name="position">�������� ��ӵ� ���� ��ġ</param>
    /// <param name="dropItemTable">��� ������ ���̺� (Ȯ�� �� ������ ���� ����)</param>
    public void SpwanDropItem(Vector3 position, DropItemTable dropItemTable)
    {
        if(dropItemTable == null)
        {
            Debug.Log("[DropItemSpawner] dropItemTable�� �������� �ʾҽ��ϴ�.");
            return;
        }
        foreach(var item in dropItemTable.dropItems)
        {
            float roll = Random.value;
            if (roll <= item.dropProbability)
            {
                int amount = Random.Range(item.minAmount, item.maxAmount + 1);

                for(int i =0; i<amount; i++)
                {
                   /* if (item.itemData == null)
                    {
                        Debug.Log("[DropItemSpawner] itemData�� �������� �ʾҽ��ϴ�.");
                    }*/
                    if(item.itemData == null || string.IsNullOrEmpty(item.itemData.itemKey))
                    {
                        Debug.Log("[DropItemSpawner] itemData�Ǵ� itemkey�� �������� �ʾҽ��ϴ�.");
                        continue;
                    }
                    if(item.itemData.itemPrefab == null)
                    {
                        Debug.Log("[DropItemSpawner] itemPrefab�� �������� �ʾҽ��ϴ�.");
                    }
                    Vector3 dropPos = position + new Vector3(Random.Range(-2f, 2f), 0.5f, Random.Range(-2f, 2f));
                    //GameObject dropItem = Instantiate(item.itemData.itemPrefab, dropPos, Quaternion.identity);
                    GameObject dropItem = ObjectPoolingManager.Instance.SpawnFromPool(item.itemData.itemKey, dropPos, Quaternion.identity);
                    var pickup = dropItem.GetComponent<ItemPickup>();
                    if(pickup != null)
                    {
                        pickup.SetUp(item.itemData);
                        pickup.itemKey = item.itemData.itemKey; 
                        dropItem.SetActive(true);
                        Debug.Log($"[DropItemSpawner] ��ӵ� ������: {item.itemData.itemName}");                        
                    }
                }
            }
        }
    }
}
