using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 몬스터 스텟 데이터를 생성하는 scriptable object 
/// <para>사용 변수</para>
/// <para>public float enemyName, HP, attackPower, defense(범위 0 ~50), attackRange</para>
/// <para>public string poolKey</para>
/// </summary>
[CreateAssetMenu(fileName = "EnemyData", menuName = "Data/EnemyData", order = int.MaxValue)]
public class Enemy_Data : ScriptableObject
{
    public string enemyName;
    public float HP;
    public float attackPower;
    [Header("몬스터의 기본 방어율(%) 최대 50%까지")]
    [Range(0,50)]
    public float defense;
    public float attackRange;
    /// <summary>
    /// 주의!! Hierarchy창의 enemy 이름을 그대로 붙여서 사용하기
    /// </summary>
    [Header("ObjectPool KeyName(Hierarchy창의 enmeyName)")]
    public string poolKey;
#if UNITY_EDITOR
    /// <summary>
    /// 에디터에서 defense 수치가 0~50 범위를 벗어나면 경고 메시지를 출력
    /// </summary>
    public void CheckEnemyDefenseArea()
    {
        if(defense < 0 || defense > 50)
        {
            Debug.LogWarning("[Enemy_Data] defense의 범위를 초과 또는 음수로 설정하셨습니다. 재설정 부탁드립니다.");
        }
    }
    /// <summary>
    /// ScriptableObject가 에디터에서 값이 변경될 때 자동 호출됨
    /// </summary>
    private void OnValidate()
    {
        CheckEnemyDefenseArea();
    }
#endif
}
