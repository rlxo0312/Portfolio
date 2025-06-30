using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 플레이어의 HP, MP 바를 관리하는 클래스
/// 
/// <para>사용 변수</para>
/// <para>public PlayerManager playerManager</para>
/// <para>public Slider hpSlider, mpSlider</para>
///
/// <para>사용 메서드</para>
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
    /// 슬라이더 초기화 함수. 현재 HP/MP 비율을 적용
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
    /// 플레이어 스탯이 변경될 때 호출되어 슬라이더 값을 갱신
    /// </summary>
    private void OnChAngedValue()
    {
        hpSlider.value = (float)playerManager.HP / (float)playerManager.MaxHP;
        mpSlider.value = (float)playerManager.MP / (float)playerManager.MaxMP;          
    }
}
