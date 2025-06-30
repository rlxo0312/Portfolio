using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 퀘스트의 진행 상태를 관리하는 싱글톤 클래스
/// 몬스터 처치 등의 조건 충족 여부를 확인하여 퀘스트 완료 여부를 갱신
///
/// <para>사용 변수</para>
/// <para>public static QuestManager instance</para>
/// <para>public List&lt;Quest_Info&gt; activeQuests</para>
///
/// <para>메서드</para>
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
    /// 몬스터가 처치되었을 때 호출되어 관련 퀘스트의 진행도를 업데이트    
    /// </summary>
    /// /// <param name="enemyTargetName">처치된 몬스터의 이름</param>
    public void OnMOnsterKilled(string enemyTargetName)
    {
        foreach(Quest_Info quest in activeQuests)
        {
            if(!quest.isCompleted && quest.targetName == enemyTargetName)
            {
                Debug.Log($"[QuestManager] OnMonsterKilled 대상 Quest Hash: {quest.GetHashCode()}");
                quest.currentCount++;
                Debug.Log($"[QuestManager] {quest.targetName} - {quest.currentCount}/{quest.targetCount}");

                if(quest.currentCount >= quest.targetCount)
                {
                    quest.isCompleted = true;
                    Debug.Log($"[QuestManager] {quest.content} 완료!");                    
                }
            }

        }
    }
}
