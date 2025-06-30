using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType
{
    Ontime,
    Repeatable
}
/// <summary>
/// NPC�� �����Ǵ� ����Ʈ �����͸� �����ϴ� ScriptableObject�Դϴ�.
/// ����Ʈ ����, ���� ����, ���� ���� ���� �����ϸ� ������ ����Ʈ(��: ���� óġ)�� Ȱ��˴ϴ�.
///
/// <para> ��� ����</para>
/// <para>public int currentCount, targetCount</para>
/// <para>public string content, targetName</para>
/// <para>public bool isCompleted, isAcceptQuest ,isKillQuest</para>
/// <para>public QuestType questType</para>
/// <para>public enum QuestType (Ontime, Repeatable)</para>
/// 
/// </summary>
[CreateAssetMenu(fileName = "QuestData", menuName = "Data/NPCQuestData", order = int.MaxValue)]
public class Quest_Info : ScriptableObject
{
    [TextArea(2, 5)]
    public string content;
    public QuestType questType;
    public bool isCompleted;
    [Header("questKey�� ������ �̸����� �����ϱ�")]
    public string questKey;

    [Header("������ ����Ʈ")]
    //public string questTitle;
    public string targetName;
    public bool isKillQuest;
    public bool isAcceptQuest;
    public int currentCount;
    public int targetCount;
}



