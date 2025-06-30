using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// NPC와의 대화를 관리하고 퀘스트 수락 및 완료 기능을 처리하는 클래스
///
/// <para>사용 변수</para>
/// <para>public GameObject dialoguePanel, questPrefab</para>
/// <para>public TextMeshProUGUI npcNameText, dialogueText</para>
/// <para>public Button nextButton, endButton, acceptButton, questCompleteButton</para>
/// <para>public Transform questListParent</para>
/// 
/// <para>사용 메서드</para>
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
    //public GameObject questItem; //추가

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
            Debug.LogWarning("[PlayerDialogue] dialogueText가 자동으로 연결되었습니다.");
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
            Debug.LogWarning("[PlayerDialogue] 중복 인스턴스 감지, 기존 인스턴스를 유지합니다.");
            Destroy(gameObject);
            return;
        }
        /*if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }*/

        // 여기서 컴포넌트 연결 자동화
        /*if (dialogueText == null)
        {
            dialogueText = dialoguePanel.GetComponentInChildren<TextMeshProUGUI>();
        }*/

        // 다시 비활성화
        //dialoguePanel?.SetActive(false);
    }
    /// <summary>
    /// NPC와의 대화를 시작하고 대사 라인을 초기화합니다.
    /// </summary>
    public void StartDialogue(string npcName, List<string> lines, NPC_InteractionTrigger trigger)
    {
        if (lines == null || lines.Count == 0)
        {
            Debug.LogError("[PlayerDialogue] 대사가 비어 있습니다. NPCData에 대사 입력 필요!");
            return;
        }
        /*if (dialoguePanel == null) Debug.LogError("dialoguePanel is NULL!");
        if (npcNameText == null) Debug.LogError("npcNameText is NULL!");
        if (dialogueText == null) Debug.LogError("dialogueText is NULL!");
        if (nextButton == null) Debug.LogError("nextButton is NULL!");
        if (endButton == null) Debug.LogError("endButton is NULL!");
        if (acceptButton == null) Debug.LogError("acceptButton is NULL!");*/
        //Debug.Log($"[PlayerDialogue] StartDialogue 호출됨 - npcName: {npcName}");
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
        dialogueText.text = dialogueLines[currentLineIndex] ?? "{대사가 비었습니다.}";
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
    /// 다음 대사를 출력하거나 마지막 대사에 따른 UI 상태를 전환합니다.
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
    /// 대화를 종료합니다.
    /// </summary>
    public void EndClick()
    {
        EndDialogue();
    }
    /// <summary>
    /// 퀘스트 수락 버튼 클릭 시 호출됩니다.
    /// </summary>
    public void OnAcceptClick()
    {
        AcceptQuest();
    }
    /// <summary>
    /// 퀘스트 완료 버튼 클릭 시 호출되어 퀘스트 상태를 초기화하고 UI를 갱신합니다.
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
            // 퀘스트 완료 처리
            currentTrigger.npc_data.isQuestTurnedContents = true;

            // 만약 반복 퀘스트가 있다면 다시 초기 대화로 돌아가게 설정
            //bool hasRepeatable = false;

            foreach (var quest in currentTrigger.npc_data.questContents)
            {
                if (quest.questType != QuestType.Repeatable)
                {
                    /* hasRepeatable = true;
                     quest.isCompleted = false; // 다시 받을 수 있게
                     quest.currentCount = 0;                    */
                    QuestInfoPoolingManager.Instance.ReturnQuestInfo(quest);
                }
            }
            QuestManager.instance.activeQuests.Clear();
            // 반복 퀘스트가 있으면 대화 상태 되돌리기
            /*if (hasRepeatable)
            {
                currentTrigger.npc_data.isQuestTurnedContents = false;
                
                // 퀘스트 아이콘 다시 활성화
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
    /// 대화 종료 시 UI를 비활성화하고 상호작용을 종료합니다.
    /// </summary>
    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        skillKeyDown.SetActive(true);
        itemSlot.SetActive(true);   
        currentTrigger?.EndInteraction();
    }
    /// <summary>
    /// 퀘스트 UI 항목들을 오브젝트 풀로 반환합니다.
    /// </summary>
    private void ReturnQuestUiToPool()
    {
        foreach (Transform child in questListParent)
        {
            ObjectPoolingManager.Instance.ReturnToPool(NPC_QuestUI.QuestPoolKey, child.gameObject);
        }
    }
    /// <summary>
    /// 퀘스트를 수락하고 관련 UI 및 데이터를 초기화합니다.
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
            Debug.Log("[PlayerDialogue] 퀘스트 수락 시도");
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
                Debug.Log($"[퀘스트 확인] - {quest.content[i]}");
                if (quest.questType == QuestType.Ontime && quest.isCompleted)
                {
                    continue;
                }

                //Quest_Info runTimeQuest = ScriptableObject.CreateInstance<Quest_Info>();//런타임용 퀘스트 복사, 생성              
                //runTimeQuest.content = quest.content;
                //runTimeQuest.questType = quest.questType;
                //runTimeQuest.targetName = quest.targetName; 
                //runTimeQuest.targetCount = quest.targetCount;   
                //runTimeQuest.isKillQuest = quest.isKillQuest;
                //runTimeQuest.currentCount = quest.currentCount;
                //runTimeQuest.isCompleted = false; 
                Quest_Info runTimeQuest = QuestInfoPoolingManager.Instance.GetQuestInfo(quest); 
                runTimeQuest.isAcceptQuest = true;
                Debug.Log($"[PlayerDialogue] 런타임 퀘스트 '{runTimeQuest.content}' 등록됨");
                /*if (!QuestManager.instance.activeQuests.Contains(quest))
                {
                    QuestManager.instance.activeQuests.Add(quest);
                    Debug.Log($"[PlayerDialogue] 퀘스트 '{quest.content}' 가 QuestManager에 등록됨");
                }*/
                QuestManager.instance.activeQuests.Add(runTimeQuest);
                currentTrigger.npc_data.questContents[i] = runTimeQuest;

                //GameObject questItem = Instantiate(questPrefab, questListParent);
                GameObject questItem = ObjectPoolingManager.Instance.SpawnFromPool(NPC_QuestUI.QuestPoolKey, Vector3.zero, Quaternion.identity);
                if(questItem == null)
                {
                    Debug.Log("[Player Dialogue] QuestUi를 풀에서 가져오지 못했습니다.");
                    continue;
                }
                else
                {
                    Debug.Log($"[PlayerDialogue] 풀에서 퀘스트 UI 오브젝트를 가져왔습니다: {questItem.name}");
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

                //Debug.Log($"[퀘스트 생성] {questItem.name} 위치: {questItem.transform.localPosition}");

                //TextMeshProUGUI questText = questItem.GetComponentInChildren<TextMeshProUGUI>();

                 
                NPC_QuestUI npc_QuestUI = questItem.GetComponent<NPC_QuestUI>();
                
                
                if(npc_QuestUI != null)
                {
                    npc_QuestUI.QuestInitilize(runTimeQuest, currentTrigger.npc_data);                    
                }
                else
                {
                    Debug.LogWarning("[퀘스트 생성] TextMeshProUGUI 컴포넌트를 찾을 수 없습니다.");
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
