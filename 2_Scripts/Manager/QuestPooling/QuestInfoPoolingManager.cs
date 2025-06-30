using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����Ʈ ������ Ǯ�� ������� �����ϴ� �Ŵ��� Ŭ����.
/// Quest_Info(ScriptableObject)�� �̸� �����ϰ� �����Ͽ� �޸� �Ҵ� ����� ����. 
/// <para>��� ����</para>
/// <para>private List&lt;Quest_Info&gt; quest_Info = new List&lt;Quest_Info&gt;()</para>
/// <para>private int poolSize</para>
/// <para>public static QuestInfoPoolingManager Instance { get; private set; }</para>
/// <para>��� �ż���</para>
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
    /// ����Ʈ ���� ��ü�� Ǯ���� �������ų�, ���� ��� ���� ����
    /// </summary>
    /// <param name="quest_info">������ �� ����Ʈ ����</param>
    /// <returns>����ǰų� ���� ������ Quest_Info ��ü</returns>
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
            Debug.Log($"[QuestInfoPoolingManager] Ǯ ���� �Ǵ� ����: {quest_info.content} �� ���� ����");
            var newQuest = ScriptableObject.CreateInstance<Quest_Info>();
            CopyFromTemplate(newQuest, quest_info);
            return newQuest;
        }
    }
    /// <summary>
    /// ����� ���� ����Ʈ ������ Ǯ�� ��ȯ
    /// </summary>
    /// <param name="quest">��ȯ�� ����Ʈ ��ü</param>
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
    /// ���ø����κ��� ����Ʈ ������ �����Ͽ� Ÿ�� ��ü�� �ʱ�ȭ
    /// </summary>
    /// <param name="target">�ʱ�ȭ�� ��ü</param>
    /// <param name="source">������ ���� ���ø�</param>
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
    /// ����Ʈ ������ �ʱ� ���·� ����
    /// </summary>
    /// <param name="quest">�ʱ�ȭ�� ����Ʈ</param>
    public void ResetQuest(Quest_Info quest)
    {
        quest.currentCount = 0;
        quest.isCompleted = false;
        quest.isAcceptQuest = false;    
    }
}
