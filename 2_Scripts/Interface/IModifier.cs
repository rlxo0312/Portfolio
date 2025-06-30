using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 현재 interface로 선정된 데이터를 저장하는 곳
/// <para>사용 매서드 </para>
/// <para> void AddValue(ref float Value)</para>
/// </summary>
public interface IModifier 
{    
    void AddValue(ref float Value);
}
