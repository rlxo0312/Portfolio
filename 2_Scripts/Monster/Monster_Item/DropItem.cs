using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��� ������ �����۰� �� Ȯ�� �� ���� ������ ��� Ŭ����
/// </summary>
[System.Serializable]
public class DropItem 
{
    public Item_Data itemData;
    [Range(0f,1f)]
    public float dropProbability; //���Ȯ�� 
    public int minAmount = 1; 
    public int maxAmount = 1;
}


