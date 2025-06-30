using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �÷��̾ ������ ��ų�� �����ϰ� �Է¿� ���� �����ϴ� �Ŵ��� Ŭ����.
/// Ű �Է¿� ���� ��ϵ� ��ų�� ����ϸ�, �� ��ų�� ���� ���� �� �ִϸ��̼��� ó����.
///
/// <para>��� ����</para>
/// <para>public PlayerManagerReference playerManagerRef</para>
/// <para>public List&lt;SkillSlot&gt; skillSlot</para>
/// <para>private List&lt;PlayerSkill&gt; skillList</para>
///
/// <para>��� �޼���</para>
/// <para>private void Awake(), private void Start(), private void Update(), private void UseSkill(int index)</para>
/// </summary>
public class PlayerSkillManager : MonoBehaviour
{

    //public PlayerManager playerManager;
    public PlayerManagerReference playerManagerRef;

    /// <summary>
    /// ��ų ���� ����ü: �� ��ų�� ������, Ű, UI�� ����
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
    /// PlayerManagerReference�� ��������, �� ��ų �����͸� ������� PlayerSkill �ν��Ͻ��� �ʱ�ȭ��
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
    /// �� �����Ӹ��� �Է��� Ȯ���ϰ� ��ų Ű�� ������ ��� �ش� ��ų ����
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
    /// �ε����� �ش��ϴ� ��ų�� ������. ��ų�� �غ�Ǿ� �ְ�, �÷��̾ �ٸ� �ൿ ���� �ƴ� ��쿡�� ����
    /// </summary>
    /// <param name="index">������ ��ų�� �ε���</param>
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
