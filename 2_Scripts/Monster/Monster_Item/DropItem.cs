using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 드롭 가능한 아이템과 그 확률 및 수량 정보를 담는 클래스
/// </summary>
[System.Serializable]
public class DropItem 
{
    public Item_Data itemData;
    [Range(0f,1f)]
    public float dropProbability; //드랍확률 
    public int minAmount = 1; 
    public int maxAmount = 1;
}


