using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType
{
    Ontime,
    Repeatable
}
/// <summary>
/// NPC와 연동되는 퀘스트 데이터를 정의하는 ScriptableObject입니다.
/// 퀘스트 내용, 조건 여부, 진행 상태 등을 저장하며 조건형 퀘스트(예: 몬스터 처치)에 활용됩니다.
///
/// <para> 사용 변수</para>
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
    [Header("questKey는 몬스터의 이름으로 설정하기")]
    public string questKey;

    [Header("조건형 퀘스트")]
    //public string questTitle;
    public string targetName;
    public bool isKillQuest;
    public bool isAcceptQuest;
    public int currentCount;
    public int targetCount;
}



