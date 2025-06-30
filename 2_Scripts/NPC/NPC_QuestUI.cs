using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// 퀘스트 정보를 UI로 표시하는 클래스
/// 
/// <para>사용 변수</para>
/// <para>private: quest_Info, questText, npc_Data, cachedText</para>
/// <para>public static QuestPoolKey</para>
/// 
/// <para>사용 메서드</para>
/// <para>Update(), QuestInitilize(Quest_Info, NPC_Data)</para>
/// </summary>
public class NPC_QuestUI : MonoBehaviour
{
    private Quest_Info quest_Info;
    private TextMeshProUGUI questText;
    private NPC_Data npc_Data;
    private string cachedText = "";
    //[SerializeField] private string poolKey = "QuestItem";
    public static string QuestPoolKey = "QuestItem";
    private static readonly StringBuilder sb = new StringBuilder(); 
    // 퀘스트 상태를 실시간으로 업데이트하여 텍스트 변경
    private void Update()
    {
        if(quest_Info == null || questText == null)
        {
            return;
        }
        sb.Clear();
        sb.Append(quest_Info.content + "(" + quest_Info.currentCount + "/" + quest_Info.targetCount + ")");

        if(quest_Info.currentCount >= quest_Info.targetCount)
        {
            sb.Append("\n" + npc_Data.npcName + "에게 돌아가 퀘스트를 완료해 주세요");
        }

        string newText = sb.ToString();

        if(newText != cachedText)
        {
            questText.text = newText;   
            cachedText = newText;   
        }
    }
    /// <summary>
    /// 퀘스트 UI 초기화
    /// </summary>
    /// <param name="quest">퀘스트 정보</param>
    /// <param name="data">NPC 정보</param>
    public void QuestInitilize(Quest_Info quest, NPC_Data data)
    {
        quest_Info = quest;
        npc_Data = data;
           
        if (questText == null)
        {
            questText = GetComponentInChildren<TextMeshProUGUI>();
        }
        if(questText.text != null)
        {
            questText.text = "";
            cachedText = "";
        } 
    }
}
