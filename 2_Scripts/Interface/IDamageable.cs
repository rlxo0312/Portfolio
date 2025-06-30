using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 데미지를 가할 수 있는 object를 선정하는 interface
/// <para>프로퍼티</para>
/// <para> bool IsAlive { get; }</para>
/// <para>사용 매서드</para>
/// <para> void BeDamaged(float damage, Vector3 contactPos, GameObject hitEffectPrefabs = null)</para>
/// </summary>
public interface IDamageable 
{
    bool IsAlive { get; }
    
    void BeDamaged(float damage, Vector3 contactPos, GameObject hitEffectPrefabs = null);
}
