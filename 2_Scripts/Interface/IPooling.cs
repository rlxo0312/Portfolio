using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 오브젝트 풀링 시스템에서 공통으로 사용하는 인터페이스
/// 풀에서 GameObject를 가져오고, 다시 반환하는 기능을 제공
///
/// <para>사용 메서드</para>
/// <para>GameObject GetFromPool()</para>
/// <para>void ReturnToPool(GameObject obj)</para>
/// </summary>
public interface IPooling 
{
    GameObject GetFromPool();
    void ReturnToPool(GameObject obj);
}
