using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �������� ���� �� �ִ� object�� �����ϴ� interface
/// <para>������Ƽ</para>
/// <para> bool IsAlive { get; }</para>
/// <para>��� �ż���</para>
/// <para> void BeDamaged(float damage, Vector3 contactPos, GameObject hitEffectPrefabs = null)</para>
/// </summary>
public interface IDamageable 
{
    bool IsAlive { get; }
    
    void BeDamaged(float damage, Vector3 contactPos, GameObject hitEffectPrefabs = null);
}
