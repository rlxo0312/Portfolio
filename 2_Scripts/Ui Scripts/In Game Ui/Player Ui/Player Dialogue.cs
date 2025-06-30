using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// NPC���� ��ȭ�� �����ϰ� ����Ʈ ���� �� �Ϸ� ����� ó���ϴ� Ŭ����
///
/// <para>��� ����</para>
/// <para>public GameObject dialoguePanel, questPrefab</para>
/// <para>public TextMeshProUGUI npcNameText, dialogueText</para>
/// <para>public Button nextButton, endButton, acceptButton, questCompleteButton</para>
/// <para>public Transform questListParent</para>
/// 
/// <para>��� �޼���</para>
/// <para>public void StartDialogue(string, List&lt;string&gt;, NPC_InteractionTrigger)</para>
/// <para>public void NextClick()</para>
/// <para>public void EndClick()</para>
/// <para>public void OnAcceptClick()</para>
/// <para>public void OnQuestCompletedClick()</para>
/// <para>private void EndDialogue()</para>
/// <para>private void ReturnQuestUiToPool()</para>
/// <para>private void AcceptQuest()</para>
/// </summary>
public class PlayerDialogue : MonoBehaviour
{
    public static PlayerDialogue instance { get; private set; }
    [Header("Player Communication")]
    public GameObject dialoguePanel;
    public GameObject skillKeyDown;
    public GameObject itemSlot;
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI dialogueText;
    public Button nextButton;
    public Button endButton;
    public Button acceptButton;
    public Button questCompleteButton;

    [Header("Player Quest")]
    public Transform questListParent;
    public GameObject questPrefab;    
    //public GameObject questItem; //�߰�

    private List<string> dialogueLines;
    private int currentLineIndex = 0;
    private NPC_InteractionTrigger currentTrigger;

    private void Start()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        if(questListParent != null)
        {
            questListParent.gameObject.SetActive(false);
        }        
    }
    private void Awake()
    {
        /* if (dialoguePanel != null)
         {
             dialoguePanel.SetActive(false);
         }*/
        /*if (dialogueText == null)
        {
            dialogueText = GetComponentInChildren<TextMeshProUGUI>();
            Debug.LogWarning("[PlayerDialogue] dialogueText�� �ڵ����� ����Ǿ����ϴ�.");
        }*/

        /*if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }*/
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("[PlayerDialogue] �ߺ� �ν��Ͻ� ����, ���� �ν��Ͻ��� �����մϴ�.");
            Destroy(gameObject);
            return;
        }
        /*if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }*/

        // ���⼭ ������Ʈ ���� �ڵ�ȭ
        /*if (dialogueText == null)
        {
            dialogueText = dialoguePanel.GetComponentInChildren<TextMeshProUGUI>();
        }*/

        // �ٽ� ��Ȱ��ȭ
        //dialoguePanel?.SetActive(false);
    }
    /// <summary>
    /// NPC���� ��ȭ�� �����ϰ� ��� ������ �ʱ�ȭ�մϴ�.
    /// </summary>
    public void StartDialogue(string npcName, List<string> lines, NPC_InteractionTrigger trigger)
    {
        if (lines == null || lines.Count == 0)
        {
            Debug.LogError("[PlayerDialogue] ��簡 ��� �ֽ��ϴ�. NPCData�� ��� �Է� �ʿ�!");
            return;
        }
        /*if (dialoguePanel == null) Debug.LogError("dialoguePanel is NULL!");
        if (npcNameText == null) Debug.LogError("npcNameText is NULL!");
        if (dialogueText == null) Debug.LogError("dialogueText is NULL!");
        if (nextButton == null) Debug.LogError("nextButton is NULL!");
        if (endButton == null) Debug.LogError("endButton is NULL!");
        if (acceptButton == null) Debug.LogError("acceptButton is NULL!");*/
        //Debug.Log($"[PlayerDialogue] StartDialogue ȣ��� - npcName: {npcName}");
        dialoguePanel.SetActive(true);
        skillKeyDown.SetActive(false);
        itemSlot.SetActive(false);  
        currentTrigger = trigger;
        //dialogueLines = lines; 
        currentLineIndex = 0;

        bool isCompletedAllQuest = true;
        bool hasAcceptQuest = false;
        /*foreach(var quest in trigger.npc_data.questContents)
        {
            if (!quest.isCompleted)
            {
                isCompletedAllQuest = false;
                break;
            }
        }*/
        List<Quest_Info> quest = trigger.npc_data.questContents;
        for(int i = 0; i < quest.Count; i++)
        {
            if(quest[i].isAcceptQuest)
            {
                hasAcceptQuest = true;
                //quest[i].isAcceptQuest = true;
                
            }
            if (!quest[i].isCompleted)
            {
                isCompletedAllQuest = false;                
                break;
            }
        }
        if (trigger.npc_data.isQuestTurnedContents)
        {
            dialogueLines = trigger.npc_data.dailyConversation;
        }
        else if (hasAcceptQuest && !isCompletedAllQuest && trigger.npc_data.dontClearQusetContents != null && 
            trigger.npc_data.dontClearQusetContents.Count > 0)
        {
            dialogueLines = trigger.npc_data.dontClearQusetContents;
        }
        else if(isCompletedAllQuest && trigger.npc_data.questClearContents != null && trigger.npc_data.questClearContents.Count > 0)
        {
            dialogueLines = trigger.npc_data.questClearContents;
        }        
        else if(!hasAcceptQuest)
        {
            dialogueLines = trigger.npc_data.dialogueLines;
        }
        /*bool isDaily = (lines == trigger.npc_data.dailyConversation);
        questCompleteButton.gameObject.SetActive(!isDaily && isCompletedAllQuest && trigger.npc_data.questClearContents != null &&
            trigger.npc_data.questClearContents.Count > 0);

        dialogueLines = isCompletedAllQuest && trigger.npc_data.questClearContents != null && trigger.npc_data.questClearContents.Count > 0
            ? trigger.npc_data.questClearContents : lines; */

        //dialoguePanel.SetActive(true);
        npcNameText.text = npcName;
        dialogueText.text = dialogueLines[currentLineIndex] ?? "{��簡 ������ϴ�.}";
        //dialogueText.text = dialogueLines[currentLineIndex];

        //nextButton.gameObject.SetActive(true);
        //endButton.gameObject.SetActive(false);   
        //acceptButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(dialogueLines.Count > 1);
        endButton.gameObject.SetActive(dialogueLines.Count == 1);
        acceptButton.gameObject.SetActive(false);
        questCompleteButton.gameObject.SetActive(isCompletedAllQuest && !trigger.npc_data.isQuestTurnedContents);
    }
    /// <summary>
    /// ���� ��縦 ����ϰų� ������ ��翡 ���� UI ���¸� ��ȯ�մϴ�.
    /// </summary>
    public void NextClick()
    {
        currentLineIndex++;
        if(currentLineIndex < dialogueLines.Count - 1)
        {
            dialogueText.text = dialogueLines[currentLineIndex];
        }
        else 
        {
            dialogueText.text = dialogueLines[currentLineIndex];
            nextButton.gameObject.SetActive(false);

            var npcData = currentTrigger?.npc_data;
            bool hasQuest = npcData != null && npcData.questContents.Count > 0;
            bool isAllQuestComplete = npcData != null && npcData.questContents.TrueForAll(q => q.isCompleted);
            //bool isAllQuestComplete = npcData != null && npcData.questContents.
            bool isQuestTurned = npcData != null && npcData.isQuestTurnedContents;

            if(hasQuest && !isAllQuestComplete)
            {
                acceptButton.gameObject.SetActive(true);
                endButton.gameObject.SetActive(true);
                questCompleteButton.gameObject.SetActive(false);
            }
            else if(hasQuest && isAllQuestComplete && !isQuestTurned) 
            {
                acceptButton.gameObject.SetActive(false);
                endButton.gameObject.SetActive(false);
                questCompleteButton.gameObject.SetActive(true);
            }
            else
            {
                acceptButton.gameObject.SetActive(false);
                questCompleteButton.gameObject.SetActive(false);
                endButton.gameObject.SetActive(true);
            }
            /*endButton.gameObject.SetActive(true);
            acceptButton.gameObject.SetActive(true);*/
        }
    }
    /// <summary>
    /// ��ȭ�� �����մϴ�.
    /// </summary>
    public void EndClick()
    {
        EndDialogue();
    }
    /// <summary>
    /// ����Ʈ ���� ��ư Ŭ�� �� ȣ��˴ϴ�.
    /// </summary>
    public void OnAcceptClick()
    {
        AcceptQuest();
    }
    /// <summary>
    /// ����Ʈ �Ϸ� ��ư Ŭ�� �� ȣ��Ǿ� ����Ʈ ���¸� �ʱ�ȭ�ϰ� UI�� �����մϴ�.
    /// </summary>
    public void OnQuestCompletedClick()
    {
        /*questCompleteButton.gameObject.SetActive(false);        
        if(currentTrigger != null && currentTrigger.npc_data != null)
        {
            currentTrigger.npc_data.isQuestTurnedContents = true;
        }
        questPrefab.gameObject.SetActive(false);
        EndDialogue();     
        foreach(var quest in currentTrigger.npc_data.questContents)
        {
            if(quest.questType == QuestType.Repeatable)
            {
                currentTrigger.questIcon.gameObject.SetActive(true);
                quest.isCompleted = false;
            }
        }*/
        questCompleteButton.gameObject.SetActive(false);

        if (currentTrigger != null && currentTrigger.npc_data != null)
        {
            // ����Ʈ �Ϸ� ó��
            currentTrigger.npc_data.isQuestTurnedContents = true;

            // ���� �ݺ� ����Ʈ�� �ִٸ� �ٽ� �ʱ� ��ȭ�� ���ư��� ����
            //bool hasRepeatable = false;

            foreach (var quest in currentTrigger.npc_data.questContents)
            {
                if (quest.questType != QuestType.Repeatable)
                {
                    /* hasRepeatable = true;
                     quest.isCompleted = false; // �ٽ� ���� �� �ְ�
                     quest.currentCount = 0;                    */
                    QuestInfoPoolingManager.Instance.ReturnQuestInfo(quest);
                }
            }
            QuestManager.instance.activeQuests.Clear();
            // �ݺ� ����Ʈ�� ������ ��ȭ ���� �ǵ�����
            /*if (hasRepeatable)
            {
                currentTrigger.npc_data.isQuestTurnedContents = false;
                
                // ����Ʈ ������ �ٽ� Ȱ��ȭ
                currentTrigger.questIcon.gameObject.SetActive(true);
            }*/ 
            foreach(var quest in currentTrigger.npc_data.questContents)
            {
                if(quest.questType == QuestType.Repeatable)
                {
                    QuestInfoPoolingManager.Instance.ResetQuest(quest);
                    QuestManager.instance.activeQuests.Add(quest);
                    currentTrigger.questIcon.gameObject.SetActive(true);
                    currentTrigger.npc_data.isQuestTurnedContents = false;
                }
            }
        }
        //questPrefab.gameObject.SetActive(false);
        ReturnQuestUiToPool();
        EndDialogue();
    }
    /// <summary>
    /// ��ȭ ���� �� UI�� ��Ȱ��ȭ�ϰ� ��ȣ�ۿ��� �����մϴ�.
    /// </summary>
    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        skillKeyDown.SetActive(true);
        itemSlot.SetActive(true);   
        currentTrigger?.EndInteraction();
    }
    /// <summary>
    /// ����Ʈ UI �׸���� ������Ʈ Ǯ�� ��ȯ�մϴ�.
    /// </summary>
    private void ReturnQuestUiToPool()
    {
        foreach (Transform child in questListParent)
        {
            ObjectPoolingManager.Instance.ReturnToPool(NPC_QuestUI.QuestPoolKey, child.gameObject);
        }
    }
    /// <summary>
    /// ����Ʈ�� �����ϰ� ���� UI �� �����͸� �ʱ�ȭ�մϴ�.
    /// </summary>
    private void AcceptQuest()
    {
        
        dialoguePanel.SetActive(false);
        skillKeyDown.SetActive(true);
        itemSlot.SetActive(true);
        currentTrigger.questIcon.SetActive(false);
        //NPC_QuestUI npc_QuestUI = questItem.GetComponent<NPC_QuestUI>();
        /*foreach (Transform child in questListParent)
        {
            ObjectPoolingManager.Instance.ReturnToPool(NPC_QuestUI.QuestPoolKey, child.gameObject);
        }*/
        ReturnQuestUiToPool();
        if (currentTrigger != null && currentTrigger.npc_data != null&& questListParent != null 
            && questPrefab != null)
        {
            Debug.Log("[PlayerDialogue] ����Ʈ ���� �õ�");
            Debug.Log($"[PlayerDialogue] currentTrigger{currentTrigger}");
            Debug.Log($"[PlayerDialogue] currentTrigger.npc_data{currentTrigger.npc_data}");
            Debug.Log($"[PlayerDialogue] questListParent{questListParent}");
            Debug.Log($"[PlayerDialogue] questPrefab{questPrefab}");
            questListParent.gameObject.SetActive(true);
            bool hasNewQuest = false;
            for(int i = 0; i<currentTrigger.npc_data.questContents.Count; i++) 
            {
                var quest = currentTrigger.npc_data.questContents[i];
                //quest.isAcceptQuest = true; 
                Debug.Log($"[����Ʈ Ȯ��] - {quest.content[i]}");
                if (quest.questType == QuestType.Ontime && quest.isCompleted)
                {
                    continue;
                }

                //Quest_Info runTimeQuest = ScriptableObject.CreateInstance<Quest_Info>();//��Ÿ�ӿ� ����Ʈ ����, ����              
                //runTimeQuest.content = quest.content;
                //runTimeQuest.questType = quest.questType;
                //runTimeQuest.targetName = quest.targetName; 
                //runTimeQuest.targetCount = quest.targetCount;   
                //runTimeQuest.isKillQuest = quest.isKillQuest;
                //runTimeQuest.currentCount = quest.currentCount;
                //runTimeQuest.isCompleted = false; 
                Quest_Info runTimeQuest = QuestInfoPoolingManager.Instance.GetQuestInfo(quest); 
                runTimeQuest.isAcceptQuest = true;
                Debug.Log($"[PlayerDialogue] ��Ÿ�� ����Ʈ '{runTimeQuest.content}' ��ϵ�");
                /*if (!QuestManager.instance.activeQuests.Contains(quest))
                {
                    QuestManager.instance.activeQuests.Add(quest);
                    Debug.Log($"[PlayerDialogue] ����Ʈ '{quest.content}' �� QuestManager�� ��ϵ�");
                }*/
                QuestManager.instance.activeQuests.Add(runTimeQuest);
                currentTrigger.npc_data.questContents[i] = runTimeQuest;

                //GameObject questItem = Instantiate(questPrefab, questListParent);
                GameObject questItem = ObjectPoolingManager.Instance.SpawnFromPool(NPC_QuestUI.QuestPoolKey, Vector3.zero, Quaternion.identity);
                if(questItem == null)
                {
                    Debug.Log("[Player Dialogue] QuestUi�� Ǯ���� �������� ���߽��ϴ�.");
                    continue;
                }
                else
                {
                    Debug.Log($"[PlayerDialogue] Ǯ���� ����Ʈ UI ������Ʈ�� �����Խ��ϴ�: {questItem.name}");
                }
                questItem.transform.SetParent(questListParent, false);
                questItem.transform.localScale = Vector3.one;
                questItem.transform.localPosition = Vector3.zero;

                //GameObject questObj = Instantiate(questPrefab, questItem.transform);
                //questObj.name = $"QuestText_{runTimeQuest.content}";
                //questObj.transform.localScale = Vector3.one;

                /*var tmp = questObj.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    tmp.text = $"{runTimeQuest.content} ({runTimeQuest.currentCount}/{runTimeQuest.targetCount})";
                }*/

                //Debug.Log($"[����Ʈ ����] {questItem.name} ��ġ: {questItem.transform.localPosition}");

                //TextMeshProUGUI questText = questItem.GetComponentInChildren<TextMeshProUGUI>();

                 
                NPC_QuestUI npc_QuestUI = questItem.GetComponent<NPC_QuestUI>();
                
                
                if(npc_QuestUI != null)
                {
                    npc_QuestUI.QuestInitilize(runTimeQuest, currentTrigger.npc_data);                    
                }
                else
                {
                    Debug.LogWarning("[����Ʈ ����] TextMeshProUGUI ������Ʈ�� ã�� �� �����ϴ�.");
                }

                if (quest.questType == QuestType.Ontime)
                {
                    quest.isCompleted = true;
                }

                if (quest.questType == QuestType.Repeatable)
                {
                    quest.isCompleted = false;
                    quest.isAcceptQuest = false;
                }
                /* else
                 {
                     quest.isCompleted = false;
                 }*/
                hasNewQuest = true;
            }
            
            if (hasNewQuest)
            {
                Canvas.ForceUpdateCanvases();
                LayoutRebuilder.ForceRebuildLayoutImmediate(questListParent.GetComponent<RectTransform>());
            }
        }
        currentTrigger?.EndInteraction();        
    }
}
