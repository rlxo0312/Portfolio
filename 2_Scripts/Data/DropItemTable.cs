using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터가 드롭할 수 있는 아이템 목록을 저장하는 ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "DropTable", menuName = "Data/DropTable", order = int.MaxValue)]
public class DropItemTable : ScriptableObject
{
    public List<DropItem> dropItems = new List<DropItem>();
}
