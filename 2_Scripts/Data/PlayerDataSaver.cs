using CameraSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEditor.Progress;

/// <summary>
/// �κ��丮 ���Կ� ����Ǵ� ������ ����(�ε���, adressKey, ����)�� ��Ÿ���� Ŭ����
/// </summary>
[System.Serializable]
public class ItemSaveData
{
    public int slotIndex;
    public string itemAddressKey;
    public int quantity;
}
/// <summary>
/// �÷��̾��� ��ġ(posX, posY, posZ), �ɷ�ġ(MaxHP, MaxMP, hp, mp, attackPower, defense), 
/// �̵� ����(speed, runSpeed, jumpForce, backSpeed, fastBackSpeed) 
/// �� �κ��丮 ����(List&lt;ItemSaveData&gt; inventoryItems, List&lt;ItemSaveData&gt; slotItems)�� �����ϴ� ������ Ŭ����
/// </summary>
[System.Serializable]
public class PlayerInfoData
{
    public float posX, posY, posZ;
    public float maxHP, maxMP;
    public float hp, mp, attackPower, defense;
    public float speed, runSpeed, jumpForce, backSpeed, fastBackSpeed;
    public List<ItemSaveData> inventoryItems = new List<ItemSaveData>();
    public List<ItemSaveData> slotItems = new List<ItemSaveData>(); 
}

/// <summary>
/// �÷��̾��� ������ ����, �ҷ�����, CSV �������� ���� ó�� 
/// <para>��� ����</para>
/// <para>public Transform playerTransform, PlayerManager playerManager, PlayerMove playerMove,
/// InventorySlotManager inventorySlotManager,PlayerUseItemUiManager useItemUiManager </para>
/// <para>private string path</para>
/// 
/// <para>��� �޼���</para>
/// <para>private bool IsReady(), ItemSaveData CreateItemSaveData(InventoryItem item, int slotIndex),
/// IEnumerator LoadItemAsync(ItemSaveData saveData, bool toInventory, int slotIndex = -1)</para>
/// <para>public void SaveToJson(), IEnumerator LoadToGame(), void DeleteSaveFile(), void DeleteSaveFile(),
/// void ExportToCsv()</para>
/// </summary>
public class PlayerDataSaver : MonoBehaviour
{
    public Transform playerTransform;
    public PlayerManager playerManager;
    public PlayerMove playerMove;
    public InventorySlotManager inventorySlotManager;
    public PlayerUseItemUiManager useItemUiManager;
    //D:\�� ����\UnityGame(URP)\RPG_ProjectB\Assets\Playerinfo\Player_Info.json
    private string path = @"D:\UnityGame(URP)\RPG_ProjectB\Assets\PlayerInfo\player_info.json";    

    private void Start()
    {
        if(!IsReady())
        {
            Debug.LogWarning($"[PlayerDataSaver] ���� ���� ����");
            return;
        }

        StartCoroutine(LoadToGame());
        ExportToCsv();
    }
    /// <summary>
    /// �ʼ� ���� ��ü���� �����Ǿ� �ִ��� Ȯ�� 
    /// </summary>
    /// <returns></returns>
    private bool IsReady()
    {
        return playerTransform != null && inventorySlotManager != null && useItemUiManager != null && playerManager != null && playerMove != null;
    }

    /// <summary>
    /// �÷��̾��� ���� �� ������ ������ JSON�� ����
    /// </summary>
    [ContextMenu("Save Player InfoData")]
    public void SaveToJson()
    {
        PlayerInfoData data = new PlayerInfoData();
        
        data.posX = playerTransform.position.x;
        data.posY = playerTransform.position.y; 
        data.posZ = playerTransform.position.z;

        data.maxHP = playerManager.MaxHP;
        data.maxMP = playerManager.MaxMP;
        data.hp = playerManager.HP; 
        data.mp = playerManager.MP;
        data.attackPower = playerManager.AttackPower;   
        data.defense = playerManager.Defense;

        data.speed = playerMove.playerSpeed;
        data.runSpeed = playerMove.runSpeed; 
        data.jumpForce = playerMove.jumpForce;
        data.backSpeed = playerMove.backSpeed;
        data.fastBackSpeed = playerMove.fastBackSpeed;  

        for(int i = 0; i < inventorySlotManager.slotList.Count; i++)
        {
            var slot = inventorySlotManager.slotList[i];
            var item = slot.item;

            if (item != null && item.item_Data != null)
            {
                /*ItemSaveData inventoryData = new ItemSaveData
                {
                    slotIndex = i,
                    itemAddressKey = item.item_Data.itemAdress,
                    quantity = item.quantity
                };

                data.inventoryItems.Add(inventoryData); */
                data.inventoryItems.Add(CreateItemSaveData(item, i));
            }           
        } 

        for(int i = 0; i < useItemUiManager.itemSlot.Count; i++)
        {
            var slot = useItemUiManager.itemSlot[i];
            var slotItem = slot.inventoryItem;
            if (slotItem != null && slotItem.item_Data != null)
            {
                /*ItemSaveData slotItemData = new ItemSaveData
                { 
                    itemAddressKey = slotItem.item_Data.itemAdress,
                    quantity = slotItem.quantity
                };  

                data.slotItems.Add(slotItemData);*/
                data.slotItems.Add(CreateItemSaveData(slotItem, i));
            }
           
        }
        string json = JsonUtility.ToJson(data, true);
        //string path = @"D:\�� ����\UnityGame(URP)\RPG_ProjectB\Assets\Playerinfo\Player_Info.json";
        File.WriteAllText(path, json);
        Debug.Log($"[SaveToJason] ����Ϸ�: {path}");
    }
    /// <summary>
    /// ������ ������ ������ ����
    /// </summary>
    /// <param name="item"></param>
    /// <param name="slotIndex"></param>
    /// <returns></returns>
    private ItemSaveData CreateItemSaveData(InventoryItem item, int slotIndex)
    {
        return new ItemSaveData
        { 
            slotIndex = slotIndex,
            itemAddressKey = item.item_Data.itemAdress,
            quantity = item.quantity
        };
    } 

    /// <summary>
    /// JSON���� ���� ����� �÷��̾��� ������ �ҷ��� ���ӿ� �ݿ�
    /// </summary>
    /// <returns></returns>
    [ContextMenu("Load Player InfoData")]
    public IEnumerator LoadToGame()
    {
        if(!File.Exists(path))
        {
            Debug.Log($"[PlayerDataSaver] ������ ã�� �� ����: {path}");
            yield break;
        }

        string json = File.ReadAllText(path); 
        PlayerInfoData loadData = JsonUtility.FromJson<PlayerInfoData>(json);

        playerTransform.position = new Vector3(loadData.posX, loadData.posY, loadData.posZ);
        Debug.Log($"[LoadToGame] �÷��̾��� ��ġ �ε� �Ϸ�: {playerTransform.position}"); 

        playerManager.isLoadData = true;
        playerManager.MaxHP = loadData.maxHP;
        playerManager.HP = loadData.hp;
        playerManager.MaxMP = loadData.maxMP; 
        playerManager.MP = loadData.mp; 
        playerManager.AttackPower = loadData.attackPower;
        playerManager.Defense = loadData.defense;

        playerMove.playerSpeed = loadData.speed;
        playerMove.runSpeed = loadData.runSpeed;    
        playerMove.jumpForce= loadData.jumpForce;
        playerMove.backSpeed = loadData.backSpeed;  
        playerMove.fastBackSpeed = loadData.fastBackSpeed;

        Debug.Log($"[LoadToGame] �÷��̾��� ���� �ε� �Ϸ�: HP:{playerManager.HP}/{playerManager.MaxHP}," +
            $"MP:{playerManager.MP}/{playerManager.MaxMP}, ���ݷ�:{playerManager.AttackPower}, ����:{playerManager.Defense}" +
            $"�̵��ӵ�:{playerMove.playerSpeed}, �޸��� �ӵ�:{playerMove.runSpeed}, ������:{playerMove.jumpForce}" +
            $"�Ĺ� �̵��ӵ�:{playerMove.backSpeed}, �Ĺ� �޸��� �ӵ�:{playerMove.fastBackSpeed}");        
       
        for (int i = 0; i < loadData.inventoryItems.Count; i++)
        {
            var itemSaveData = loadData.inventoryItems[i]; 
            if(string.IsNullOrEmpty(itemSaveData.itemAddressKey) || itemSaveData.quantity <= 0)
            {
                continue;
            }
            yield return LoadItemAsync(itemSaveData, true, itemSaveData.slotIndex);
        }

        for(int i = 0; i < loadData.slotItems.Count; i++)
        {
            var itemSaveData = loadData.slotItems[i];    

            if(string.IsNullOrEmpty(itemSaveData.itemAddressKey) || itemSaveData.quantity <= 0)
            {
                continue;
            }
            yield return LoadItemAsync(itemSaveData, false, itemSaveData.slotIndex);
        }

        Debug.Log($"[LoadToGame] ������ �ε� �Ϸ�");
    }
    /// <summary>
    /// Addressables�� ���� �������� �񵿱� �ε��ϰ� �κ��丮 �Ǵ� ���Կ� ��ġ
    /// </summary>
    /// <param name="saveData"></param>
    /// <param name="toInventory"></param>
    /// <param name="slotIndex"></param>
    /// <returns></returns>
    private IEnumerator LoadItemAsync(ItemSaveData saveData, bool toInventory, int slotIndex = -1)
    {
        var handle = Addressables.LoadAssetAsync<Item_Data>(saveData.itemAddressKey);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var itemData = handle.Result;

            if (toInventory)
            {
                if(slotIndex >= 0)
                {
                    var pool = ObjectPoolingManager.Instance.GetPool(itemData.itemKey);
                    GameObject poolObj = pool?.GetFromPool();

                    //Inventory.instance.AddItem(itemData, poolObj, saveData.quantity);                
                    inventorySlotManager.AssignItemToSlotAtIndex(slotIndex, itemData, poolObj, saveData.quantity);                    
                    Debug.Log($"[LoadItemAsync] �κ��丮�� �߰���: {itemData.itemName} x{saveData.quantity}");

                }
            }
            else if (slotIndex >= 0)
            {
                InventoryItem newItem = new InventoryItem(itemData, saveData.quantity);
                useItemUiManager.AssignToUseSlotAtIndex(slotIndex, newItem);
                Debug.Log($"[LoadItemAsync] ����Ű ����:{slotIndex} - {itemData.itemName} x{saveData.quantity}");
            }
        }
        else
        {
            Debug.LogWarning($"[LoadItemAsync] Addressables �ε� ����: {saveData.itemAddressKey}");
        }
    }   

    /// <summary>
    /// ����� JSON���� ���� 
    /// </summary>
    [ContextMenu("Delete Player InfoData")]
    public void DeleteSaveFile()
    {
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[PlayerDataSaver] ���� ���� ���� �Ϸ�: {path}");
        }
        else
        {
            Debug.LogWarning($"[PlayerDataSaver] ������ ������ �����ϴ�: {path}");
        }
    }
    /// <summary>
    /// ����� JSON ������ �о�� CSV�� ��ȯ�Ͽ� ����
    /// </summary>
    [ContextMenu("Export Player Info to CSV")]
    public void ExportToCsv()
    {
        if(!File.Exists(path))
        {
            Debug.LogWarning($"[PlayerDataSaver] JSON ������ �������� ����: {path }");
        } 

        string json = File.ReadAllText(path); 
        PlayerInfoData data = JsonUtility.FromJson<PlayerInfoData>(json);

        string csvPath = @"D:\UnityGame(URP)\RPG_ProjectB\Assets\PlayerInfo\player_info.csv"; 

        using(StreamWriter writer = new StreamWriter(csvPath, false, new System.Text.UTF8Encoding(true))) 
        {
            writer.WriteLine($"PlayerPosition ,posX: {data.posX}, posY: {data.posY}, posZ: {data.posZ}");
            writer.WriteLine($"PlayerStat, (MaxHP:{data.maxHP}/HP:{data.hp}), (MaxMP:{data.maxMP}/MP:{data.mp})," +
                                             $"AttackPower:{data.attackPower}, Defense:{data.defense}");
            writer.WriteLine($"PlayerMove, �⺻ �̵��ӵ�:{data.speed}, �޸��� �ӵ�:{data.runSpeed}, �Ĺ��̵� �ӵ�:{data.backSpeed}," +
                                            $"�Ĺ� �޸��� �ӵ�:{data.fastBackSpeed}, ������:{data.jumpForce}");
                        
            writer.WriteLine("\n-Inventory Items-");

            foreach (var item in data.inventoryItems)
            {
                writer.WriteLine($"Inventory Item, SlotIndex:{item.slotIndex}, ItemAdressKey:{item.itemAddressKey}, ����:{item.quantity}");
              
            }

            writer.WriteLine("\n-slotItems-");

            foreach(var item in data.slotItems)
            {
                writer.WriteLine($"Slot Item, SlotIndex:{item.slotIndex}, ItemAdressKey:{item.itemAddressKey}, ����:{item.quantity}");
            }
        }
        Debug.Log($"[PlayerDataSaver] CSV���� �Ϸ�:{csvPath}");
    }
}
