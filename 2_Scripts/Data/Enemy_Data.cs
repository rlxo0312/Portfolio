using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���� ���� �����͸� �����ϴ� scriptable object 
/// <para>��� ����</para>
/// <para>public float enemyName, HP, attackPower, defense(���� 0 ~50), attackRange</para>
/// <para>public string poolKey</para>
/// </summary>
[CreateAssetMenu(fileName = "EnemyData", menuName = "Data/EnemyData", order = int.MaxValue)]
public class Enemy_Data : ScriptableObject
{
    public string enemyName;
    public float HP;
    public float attackPower;
    [Header("������ �⺻ �����(%) �ִ� 50%����")]
    [Range(0,50)]
    public float defense;
    public float attackRange;
    /// <summary>
    /// ����!! Hierarchyâ�� enemy �̸��� �״�� �ٿ��� ����ϱ�
    /// </summary>
    [Header("ObjectPool KeyName(Hierarchyâ�� enmeyName)")]
    public string poolKey;
#if UNITY_EDITOR
    /// <summary>
    /// �����Ϳ��� defense ��ġ�� 0~50 ������ ����� ��� �޽����� ���
    /// </summary>
    public void CheckEnemyDefenseArea()
    {
        if(defense < 0 || defense > 50)
        {
            Debug.LogWarning("[Enemy_Data] defense�� ������ �ʰ� �Ǵ� ������ �����ϼ̽��ϴ�. �缳�� ��Ź�帳�ϴ�.");
        }
    }
    /// <summary>
    /// ScriptableObject�� �����Ϳ��� ���� ����� �� �ڵ� ȣ���
    /// </summary>
    private void OnValidate()
    {
        CheckEnemyDefenseArea();
    }
#endif
}
