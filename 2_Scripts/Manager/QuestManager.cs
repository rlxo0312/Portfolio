using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����Ʈ�� ���� ���¸� �����ϴ� �̱��� Ŭ����
/// ���� óġ ���� ���� ���� ���θ� Ȯ���Ͽ� ����Ʈ �Ϸ� ���θ� ����
///
/// <para>��� ����</para>
/// <para>public static QuestManager instance</para>
/// <para>public List&lt;Quest_Info&gt; activeQuests</para>
///
/// <para>�޼���</para>
/// <para>private void Awake()</para>
/// <para>public void OnMOnsterKilled(string enemyTargetName)</para>
/// </summary>
public class QuestManager : MonoBehaviour
{
    public static QuestManager instance { get; private set; } 
    public List<Quest_Info> activeQuests = new List<Quest_Info>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// ���Ͱ� óġ�Ǿ��� �� ȣ��Ǿ� ���� ����Ʈ�� ���൵�� ������Ʈ    
    /// </summary>
    /// /// <param name="enemyTargetName">óġ�� ������ �̸�</param>
    public void OnMOnsterKilled(string enemyTargetName)
    {
        foreach(Quest_Info quest in activeQuests)
        {
            if(!quest.isCompleted && quest.targetName == enemyTargetName)
            {
                Debug.Log($"[QuestManager] OnMonsterKilled ��� Quest Hash: {quest.GetHashCode()}");
                quest.currentCount++;
                Debug.Log($"[QuestManager] {quest.targetName} - {quest.currentCount}/{quest.targetCount}");

                if(quest.currentCount >= quest.targetCount)
                {
                    quest.isCompleted = true;
                    Debug.Log($"[QuestManager] {quest.content} �Ϸ�!");                    
                }
            }

        }
    }
}
