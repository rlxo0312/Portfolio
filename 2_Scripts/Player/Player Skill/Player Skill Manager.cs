using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 플레이어가 보유한 스킬을 관리하고 입력에 따라 실행하는 매니저 클래스.
/// 키 입력에 따라 등록된 스킬을 사용하며, 각 스킬의 실행 조건 및 애니메이션을 처리함.
///
/// <para>사용 변수</para>
/// <para>public PlayerManagerReference playerManagerRef</para>
/// <para>public List&lt;SkillSlot&gt; skillSlot</para>
/// <para>private List&lt;PlayerSkill&gt; skillList</para>
///
/// <para>사용 메서드</para>
/// <para>private void Awake(), private void Start(), private void Update(), private void UseSkill(int index)</para>
/// </summary>
public class PlayerSkillManager : MonoBehaviour
{

    //public PlayerManager playerManager;
    public PlayerManagerReference playerManagerRef;

    /// <summary>
    /// 스킬 슬롯 구조체: 각 스킬의 데이터, 키, UI를 포함
    /// </summary>
    [System.Serializable]
    public class SkillSlot
    {
        public PlayerSkillData skillData;
        public KeyCode keyCode;
        public PlayerSkillUi skillUi;
    }

    public List<SkillSlot> skillSlot;
    [SerializeField] private List<PlayerSkill> skillList = new List<PlayerSkill>();

    private void Awake()
    {
        /*if(playerManagerRef == null)
        {
            playerManagerRef = GetComponent<PlayerManager>();  
        }*/
       
    }
    /// <summary>   
    /// PlayerManagerReference를 가져오고, 각 스킬 데이터를 기반으로 PlayerSkill 인스턴스를 초기화함
    /// </summary>
    private void Start()
    {
        if(playerManagerRef == null)
        {
            playerManagerRef = GetComponent<PlayerManagerReference>();
        }
        foreach (var slot in skillSlot) 
        {
            PlayerSkill skill = new PlayerSkill(slot.skillData, slot.keyCode, slot.skillUi, 
                playerManagerRef.PlayerManager.playerAnimationManager.animator, playerManagerRef.PlayerManager);
            skillList.Add(skill);
        }
    }
    /// <summary>    
    /// 매 프레임마다 입력을 확인하고 스킬 키가 눌렸을 경우 해당 스킬 실행
    /// </summary>
    private void Update()
    {
        for(int i =0; i < skillList.Count; i++)
        {
            if (Input.GetKeyDown(skillSlot[i].keyCode))
            {
                //Debug.Log($"[PlayerSkillManager]: {skillSlot[i].key}");
                UseSkill(i);
            }
        }
    }
    /// <summary>    
    /// 인덱스에 해당하는 스킬을 실행함. 스킬이 준비되어 있고, 플레이어가 다른 행동 중이 아닐 경우에만 실행
    /// </summary>
    /// <param name="index">실행할 스킬의 인덱스</param>
    private void UseSkill(int index)
    {
        if(index < 0 || index >= skillList.Count || playerManagerRef.PlayerManager.isPerformingAction)
        {
            return; 
        }

        PlayerSkill playerSkill = skillList[index];
        Debug.Log($"[PlayerSkillManager]:{skillList[index]}");
        if (playerSkill.isReady())
        {
            Debug.Log("[PlayerSkillManager] isReady ON");
            playerSkill.Active(this);
        }
    }
}
