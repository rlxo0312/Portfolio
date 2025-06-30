using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// NPC�� ���, ����Ʈ, ����Ʈ �Ϸ� �� ��ȭ ���� �����ϴ� ScriptableObject
/// ����Ʈ ����, �Ϸ�, �ݺ� ��ȭ �帧�� NPC���� ������ �� �ֵ��� �����Ǿ� ����
///
/// <para>��� ����</para>
/// <para>public string npcName</para>
/// <para>public List&lt;string&gt; dialogueLines, questClearContents, dailyConversation</para>
/// <para>public List&lt;Quest_Info&gt; questContents</para>
/// <para>public bool isQuestTurnedContents</para>
/// </summary>
[CreateAssetMenu(fileName = "NPCData", menuName = "Data/NPCData",order = int.MaxValue)]
public class NPC_Data : ScriptableObject
{
    public string npcName;
    [Header("��ȭ ����")]
    [TextArea(2,5)]
    public List<string> dialogueLines;
    [Header("����Ʈ ����")]    
    public List<Quest_Info> questContents;
    [Header("����Ʈ ���� �� ��ȣ�ۿ�� ��ȭ")]
    [TextArea(2, 5)]
    public List<string> dontClearQusetContents;
    [Header("����Ʈ �Ϸ� �� ��ȭ")]
    [TextArea(2,5)]
    public List<string> questClearContents;
    [Header("����Ʈ �Ϸ� �� ��ȭ")]
    [TextArea(2,5)]
    public List<string> dailyConversation;

    [HideInInspector]
    public bool isQuestTurnedContents = false;
}
