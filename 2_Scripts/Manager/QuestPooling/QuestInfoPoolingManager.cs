using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 퀘스트 정보를 풀링 방식으로 관리하는 매니저 클래스.
/// Quest_Info(ScriptableObject)를 미리 생성하고 재사용하여 메모리 할당 비용을 줄임. 
/// <para>사용 변수</para>
/// <para>private List&lt;Quest_Info&gt; quest_Info = new List&lt;Quest_Info&gt;()</para>
/// <para>private int poolSize</para>
/// <para>public static QuestInfoPoolingManager Instance { get; private set; }</para>
/// <para>사용 매서드</para>
/// <para> public Quest_Info GetQuestInfo(Quest_Info quest_info), public void ReturnQuestInfo(Quest_Info quest),
/// public void CopyFromTemplate(Quest_Info target, Quest_Info source), public void ResetQuest(Quest_Info quest)</para>
/// </summary>
public class QuestInfoPoolingManager : MonoBehaviour
{
    public static QuestInfoPoolingManager Instance { get; private set; }
    [SerializeField] private List<Quest_Info> quest_Info = new List<Quest_Info>();
    [SerializeField] private int poolSize;

    private Dictionary<string,Queue<Quest_Info>> pool = new Dictionary<string, Queue<Quest_Info>>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (var template in quest_Info)
        {
            if (template == null || string.IsNullOrEmpty(template.content)) continue;

            var pools = new Queue<Quest_Info>();
            for (int i = 0; i < poolSize; i++)
            {
                var quest = ScriptableObject.CreateInstance<Quest_Info>();
                CopyFromTemplate(quest, template);
                pools.Enqueue(quest);
            }
            pool[template.questKey] = pools;
        }
    }

    /// <summary>
    /// 퀘스트 정보 객체를 풀에서 가져오거나, 없을 경우 새로 생성
    /// </summary>
    /// <param name="quest_info">기준이 될 퀘스트 정보</param>
    /// <returns>재사용되거나 새로 생성된 Quest_Info 객체</returns>
    public Quest_Info GetQuestInfo(Quest_Info quest_info)
    {
        if(quest_info == null || string.IsNullOrEmpty(quest_info.content))
        {
            return null; 
        }
        if(pool.TryGetValue(quest_info.questKey, out var pools) && pools.Count > 0)
        {
            var quest = pools.Dequeue();
            CopyFromTemplate(quest, quest_info);
            return quest;
        }
        else
        {
            Debug.Log($"[QuestInfoPoolingManager] 풀 없음 또는 부족: {quest_info.content} → 새로 생성");
            var newQuest = ScriptableObject.CreateInstance<Quest_Info>();
            CopyFromTemplate(newQuest, quest_info);
            return newQuest;
        }
    }
    /// <summary>
    /// 사용이 끝난 퀘스트 정보를 풀에 반환
    /// </summary>
    /// <param name="quest">반환할 퀘스트 객체</param>
    public void ReturnQuestInfo(Quest_Info quest)
    {
        if(quest == null || string.IsNullOrEmpty(quest.questKey))
        {
            return;
        }
        ResetQuest(quest);
        if (!pool.ContainsKey(quest.questKey))
        {
            pool[quest.questKey] = new Queue<Quest_Info>();
        }
        pool[quest.questKey].Enqueue(quest); 
    }
    /// <summary>
    /// 템플릿으로부터 퀘스트 정보를 복사하여 타겟 객체를 초기화
    /// </summary>
    /// <param name="target">초기화할 객체</param>
    /// <param name="source">복사할 원본 템플릿</param>
    public void CopyFromTemplate(Quest_Info target, Quest_Info source)
    {
        target.questKey = source.questKey;  
        target.content = source.content;
        target.questType = source.questType;
        target.targetName = source.targetName;
        target.isKillQuest = source.isKillQuest;
        target.targetCount = source.targetCount;
        target.currentCount = 0;
        target.isCompleted = false;
        target.isAcceptQuest = false;
    }
    /// <summary>
    /// 퀘스트 정보를 초기 상태로 리셋
    /// </summary>
    /// <param name="quest">초기화할 퀘스트</param>
    public void ResetQuest(Quest_Info quest)
    {
        quest.currentCount = 0;
        quest.isCompleted = false;
        quest.isAcceptQuest = false;    
    }
}
