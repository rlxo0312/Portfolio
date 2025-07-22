using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 아이템 데이터를 생성하는 scriptableobject
/// 
/// <para>사용 변수</para>
/// <para> public string itemName, itemKey, itemAdress, itemInfo</para> 
/// <para> public int maxStacki</para>
/// <para> public GameObject itemPrefab</para>
/// <para> public Sprite itemSprite</para>
/// <para> public List&lt;StatModifier&gt;  statModifiers</para>
/// </summary>
[CreateAssetMenu(fileName = "ItemData", menuName = "Data/ItemData")]
public class Item_Data : ScriptableObject
{
    public string itemName;
    public string itemKey;
    [Header("Adressable key 이름은 scriptableObject명과 동일하게")]
    public string itemAdress;
    public GameObject itemPrefab;
    public Sprite itemSprite;
    public bool isStackable = true;
    [Header("사용 가능 아이템 여부")]
    public bool isAvailableItem;
    public float itemCooldown;
    public int maxStack = 99; 
    
    [TextArea(3, 5)]
    public string itemInfo;

    [Header("사용 아이템 효과")]
    public float recoveryHP; 
    public float recoveryMP;
    public GameObject recoveryItemEffect;
    public Vector3 itemEffectPos;
    public float startItemEffectDuration;
    public float itemEffectDuration;
    public string itemPoolKey;

    [Header("스탯보정")]
    public List<StatModifier> statModifiers;
}
