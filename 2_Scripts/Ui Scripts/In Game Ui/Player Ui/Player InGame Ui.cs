using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// �÷��̾��� HP, MP �ٸ� �����ϴ� Ŭ����
/// 
/// <para>��� ����</para>
/// <para>public PlayerManager playerManager</para>
/// <para>public Slider hpSlider, mpSlider</para>
///
/// <para>��� �޼���</para>
/// <para>public void InitializeSlider()</para>
/// <para>private void OnChAngedValue()</para>
/// </summary>
public class PlayerInGameUi : MonoBehaviour
{
    public PlayerManager playerManager;

    //public Image hpSlider;
    //public Image mpSlider;
    public UnityEngine.UI.Slider hpSlider;
    public UnityEngine.UI.Slider mpSlider;

    private void OnEnable()
    {
        playerManager.OnChangerStats += OnChAngedValue;
    }
    private void OnDisable()
    {
        playerManager.OnChangerStats -= OnChAngedValue;
    }
    // <summary>
    /// �����̴� �ʱ�ȭ �Լ�. ���� HP/MP ������ ����
    /// </summary>
    public void InitializeSlider()
    {
        hpSlider.maxValue = 1f;
        hpSlider.value = ((float)playerManager.HP / (float)playerManager.MaxHP);
        mpSlider.value = ((float)playerManager.MP / (float)playerManager.MaxMP);
    }
    private void Start()
    {
        
    }

    
    private void Update()
    {
        
    }

    /// <summary>
    /// �÷��̾� ������ ����� �� ȣ��Ǿ� �����̴� ���� ����
    /// </summary>
    private void OnChAngedValue()
    {
        hpSlider.value = (float)playerManager.HP / (float)playerManager.MaxHP;
        mpSlider.value = (float)playerManager.MP / (float)playerManager.MaxMP;          
    }
}
