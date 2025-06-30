using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// WaitForSeconds를 재사용하기 위한 캐시 클래스 
/// 
/// <para>사용 변수</para>
/// <para>private static readonly Dictionary<float, WaitForSeconds> cache = new Dictionary<float, WaitForSeconds>();</para>
/// 
/// <para>사용 매서드</para>
/// <para> public static : WaitForSeconds Get(float seconds), void Clear()</para>
/// </summary>
public class WaitForSecondsCache 
{
    private static readonly Dictionary<float, WaitForSeconds> cache = new Dictionary<float, WaitForSeconds>();

    /// <summary>
    /// 주어진 시간에 대한 WaitForSeconds 인스턴스를 캐싱하여 반환 
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static WaitForSeconds Get(float seconds)
    {
        if(seconds < 0f)
        {
            return new WaitForSeconds(0f);
        }

        if(!cache.TryGetValue(seconds, out var wait))
        {
            wait = new WaitForSeconds(seconds);
            cache[seconds] = wait;  
        }

        return wait;
    }

    /// <summary>
    /// 캐시 수동 초기화
    /// </summary>
    public static void Clear() 
    {
        cache.Clear();
    }
}
