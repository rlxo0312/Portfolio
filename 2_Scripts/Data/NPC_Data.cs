using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// NPC의 대사, 퀘스트, 퀘스트 완료 후 대화 등을 저장하는 ScriptableObject
/// 퀘스트 수락, 완료, 반복 대화 흐름을 NPC별로 정의할 수 있도록 구성되어 있음
///
/// <para>사용 변수</para>
/// <para>public string npcName</para>
/// <para>public List&lt;string&gt; dialogueLines, questClearContents, dailyConversation</para>
/// <para>public List&lt;Quest_Info&gt; questContents</para>
/// <para>public bool isQuestTurnedContents</para>
/// </summary>
[CreateAssetMenu(fileName = "NPCData", menuName = "Data/NPCData",order = int.MaxValue)]
public class NPC_Data : ScriptableObject
{
    public string npcName;
    [Header("대화 내용")]
    [TextArea(2,5)]
    public List<string> dialogueLines;
    [Header("퀘스트 내용")]    
    public List<Quest_Info> questContents;
    [Header("퀘스트 진행 중 상호작용시 대화")]
    [TextArea(2, 5)]
    public List<string> dontClearQusetContents;
    [Header("퀘스트 완료 시 대화")]
    [TextArea(2,5)]
    public List<string> questClearContents;
    [Header("퀘스트 완료 후 대화")]
    [TextArea(2,5)]
    public List<string> dailyConversation;

    [HideInInspector]
    public bool isQuestTurnedContents = false;
}
