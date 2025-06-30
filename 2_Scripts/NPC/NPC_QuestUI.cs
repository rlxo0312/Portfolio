using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// ����Ʈ ������ UI�� ǥ���ϴ� Ŭ����
/// 
/// <para>��� ����</para>
/// <para>private: quest_Info, questText, npc_Data, cachedText</para>
/// <para>public static QuestPoolKey</para>
/// 
/// <para>��� �޼���</para>
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
    // ����Ʈ ���¸� �ǽð����� ������Ʈ�Ͽ� �ؽ�Ʈ ����
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
            sb.Append("\n" + npc_Data.npcName + "���� ���ư� ����Ʈ�� �Ϸ��� �ּ���");
        }

        string newText = sb.ToString();

        if(newText != cachedText)
        {
            questText.text = newText;   
            cachedText = newText;   
        }
    }
    /// <summary>
    /// ����Ʈ UI �ʱ�ȭ
    /// </summary>
    /// <param name="quest">����Ʈ ����</param>
    /// <param name="data">NPC ����</param>
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
