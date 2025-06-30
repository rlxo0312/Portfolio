using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 드롭 아이템을 생성하는 클래스
/// </summary>
public class DropItemSpawner : MonoBehaviour
{
    /// <summary>
    /// 지정된 위치에 드롭 테이블을 기반으로 아이템을 생성함
    /// </summary>
    /// <param name="position">아이템이 드롭될 기준 위치</param>
    /// <param name="dropItemTable">드롭 아이템 테이블 (확률 및 아이템 정보 포함)</param>
    public void SpwanDropItem(Vector3 position, DropItemTable dropItemTable)
    {
        if(dropItemTable == null)
        {
            Debug.Log("[DropItemSpawner] dropItemTable이 설정되지 않았습니다.");
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
                        Debug.Log("[DropItemSpawner] itemData가 설정되지 않았습니다.");
                    }*/
                    if(item.itemData == null || string.IsNullOrEmpty(item.itemData.itemKey))
                    {
                        Debug.Log("[DropItemSpawner] itemData또는 itemkey가 설정되지 않았습니다.");
                        continue;
                    }
                    if(item.itemData.itemPrefab == null)
                    {
                        Debug.Log("[DropItemSpawner] itemPrefab이 설정되지 않았습니다.");
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
                        Debug.Log($"[DropItemSpawner] 드롭된 아이템: {item.itemData.itemName}");                        
                    }
                }
            }
        }
    }
}
