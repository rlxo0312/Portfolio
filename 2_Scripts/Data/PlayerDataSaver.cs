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
/// 인벤토리 슬롯에 저장되는 아이템 정보(인덱스, adressKey, 수량)를 나타내는 클래스
/// </summary>
[System.Serializable]
public class ItemSaveData
{
    public int slotIndex;
    public string itemAddressKey;
    public int quantity;
}
/// <summary>
/// 플레이어의 위치(posX, posY, posZ), 능력치(MaxHP, MaxMP, hp, mp, attackPower, defense), 
/// 이동 정보(speed, runSpeed, jumpForce, backSpeed, fastBackSpeed) 
/// 및 인벤토리 상태(List&lt;ItemSaveData&gt; inventoryItems, List&lt;ItemSaveData&gt; slotItems)를 저장하는 데이터 클래스
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
/// 플레이어의 정보를 저장, 불러오기, CSV 내보내기 등을 처리 
/// <para>사용 변수</para>
/// <para>public Transform playerTransform, PlayerManager playerManager, PlayerMove playerMove,
/// InventorySlotManager inventorySlotManager,PlayerUseItemUiManager useItemUiManager </para>
/// <para>private string path</para>
/// 
/// <para>사용 메서드</para>
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
    //D:\새 폴더\UnityGame(URP)\RPG_ProjectB\Assets\Playerinfo\Player_Info.json
    private string path = @"D:\UnityGame(URP)\RPG_ProjectB\Assets\PlayerInfo\player_info.json";    

    private void Start()
    {
        if(!IsReady())
        {
            Debug.LogWarning($"[PlayerDataSaver] 참조 설정 누락");
            return;
        }

        StartCoroutine(LoadToGame());
        ExportToCsv();
    }
    /// <summary>
    /// 필수 참조 객체들이 참조되어 있는지 확인 
    /// </summary>
    /// <returns></returns>
    private bool IsReady()
    {
        return playerTransform != null && inventorySlotManager != null && useItemUiManager != null && playerManager != null && playerMove != null;
    }

    /// <summary>
    /// 플레이어의 상태 및 아이템 정보를 JSON에 저장
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
        //string path = @"D:\새 폴더\UnityGame(URP)\RPG_ProjectB\Assets\Playerinfo\Player_Info.json";
        File.WriteAllText(path, json);
        Debug.Log($"[SaveToJason] 저장완료: {path}");
    }
    /// <summary>
    /// 저장할 아이템 데이터 생성
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
    /// JSON으로 부터 저장된 플레이어의 정보를 불러와 게임에 반영
    /// </summary>
    /// <returns></returns>
    [ContextMenu("Load Player InfoData")]
    public IEnumerator LoadToGame()
    {
        if(!File.Exists(path))
        {
            Debug.Log($"[PlayerDataSaver] 파일을 찾을 수 없음: {path}");
            yield break;
        }

        string json = File.ReadAllText(path); 
        PlayerInfoData loadData = JsonUtility.FromJson<PlayerInfoData>(json);

        playerTransform.position = new Vector3(loadData.posX, loadData.posY, loadData.posZ);
        Debug.Log($"[LoadToGame] 플레이어의 위치 로드 완료: {playerTransform.position}"); 

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

        Debug.Log($"[LoadToGame] 플레이어의 정보 로드 완료: HP:{playerManager.HP}/{playerManager.MaxHP}," +
            $"MP:{playerManager.MP}/{playerManager.MaxMP}, 공격력:{playerManager.AttackPower}, 방어력:{playerManager.Defense}" +
            $"이동속도:{playerMove.playerSpeed}, 달리기 속도:{playerMove.runSpeed}, 점프력:{playerMove.jumpForce}" +
            $"후방 이동속도:{playerMove.backSpeed}, 후방 달리기 속도:{playerMove.fastBackSpeed}");        
       
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

        Debug.Log($"[LoadToGame] 데이터 로드 완료");
    }
    /// <summary>
    /// Addressables를 통해 아이템을 비동기 로드하고 인벤토리 또는 슬롯에 배치
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
                    Debug.Log($"[LoadItemAsync] 인벤토리에 추가됨: {itemData.itemName} x{saveData.quantity}");

                }
            }
            else if (slotIndex >= 0)
            {
                InventoryItem newItem = new InventoryItem(itemData, saveData.quantity);
                useItemUiManager.AssignToUseSlotAtIndex(slotIndex, newItem);
                Debug.Log($"[LoadItemAsync] 단축키 슬롯:{slotIndex} - {itemData.itemName} x{saveData.quantity}");
            }
        }
        else
        {
            Debug.LogWarning($"[LoadItemAsync] Addressables 로딩 실패: {saveData.itemAddressKey}");
        }
    }   

    /// <summary>
    /// 저장된 JSON파일 삭제 
    /// </summary>
    [ContextMenu("Delete Player InfoData")]
    public void DeleteSaveFile()
    {
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[PlayerDataSaver] 저장 파일 삭제 완료: {path}");
        }
        else
        {
            Debug.LogWarning($"[PlayerDataSaver] 삭제할 파일이 없습니다: {path}");
        }
    }
    /// <summary>
    /// 저장된 JSON 파일을 읽어와 CSV로 변환하여 저장
    /// </summary>
    [ContextMenu("Export Player Info to CSV")]
    public void ExportToCsv()
    {
        if(!File.Exists(path))
        {
            Debug.LogWarning($"[PlayerDataSaver] JSON 파일이 존재하지 않음: {path }");
        } 

        string json = File.ReadAllText(path); 
        PlayerInfoData data = JsonUtility.FromJson<PlayerInfoData>(json);

        string csvPath = @"D:\UnityGame(URP)\RPG_ProjectB\Assets\PlayerInfo\player_info.csv"; 

        using(StreamWriter writer = new StreamWriter(csvPath, false, new System.Text.UTF8Encoding(true))) 
        {
            writer.WriteLine($"PlayerPosition ,posX: {data.posX}, posY: {data.posY}, posZ: {data.posZ}");
            writer.WriteLine($"PlayerStat, (MaxHP:{data.maxHP}/HP:{data.hp}), (MaxMP:{data.maxMP}/MP:{data.mp})," +
                                             $"AttackPower:{data.attackPower}, Defense:{data.defense}");
            writer.WriteLine($"PlayerMove, 기본 이동속도:{data.speed}, 달리기 속도:{data.runSpeed}, 후방이동 속도:{data.backSpeed}," +
                                            $"후방 달리기 속도:{data.fastBackSpeed}, 점프력:{data.jumpForce}");
                        
            writer.WriteLine("\n-Inventory Items-");

            foreach (var item in data.inventoryItems)
            {
                writer.WriteLine($"Inventory Item, SlotIndex:{item.slotIndex}, ItemAdressKey:{item.itemAddressKey}, 수량:{item.quantity}");
              
            }

            writer.WriteLine("\n-slotItems-");

            foreach(var item in data.slotItems)
            {
                writer.WriteLine($"Slot Item, SlotIndex:{item.slotIndex}, ItemAdressKey:{item.itemAddressKey}, 수량:{item.quantity}");
            }
        }
        Debug.Log($"[PlayerDataSaver] CSV저장 완료:{csvPath}");
    }
}
