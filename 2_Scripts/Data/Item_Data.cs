using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ������ �����͸� �����ϴ� scriptableobject
/// 
/// <para>��� ����</para>
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
    [Header("Adressable key �̸��� scriptableObject��� �����ϰ�")]
    public string itemAdress;
    public GameObject itemPrefab;
    public Sprite itemSprite;
    public bool isStackable = true;
    [Header("��� ���� ������ ����")]
    public bool isAvailableItem;
    public float itemCooldown;
    public int maxStack = 99; 
    
    [TextArea(3, 5)]
    public string itemInfo;

    [Header("��� ������ ȿ��")]
    public float recoveryHP; 
    public float recoveryMP;
    public GameObject recoveryItemEffect;
    public Vector3 itemEffectPos;
    public float startItemEffectDuration;
    public float itemEffectDuration;
    public string itemPoolKey;

    [Header("���Ⱥ���")]
    public List<StatModifier> statModifiers;
}
