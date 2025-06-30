using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���Ͱ� ����� �� �ִ� ������ ����� �����ϴ� ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "DropTable", menuName = "Data/DropTable", order = int.MaxValue)]
public class DropItemTable : ScriptableObject
{
    public List<DropItem> dropItems = new List<DropItem>();
}
