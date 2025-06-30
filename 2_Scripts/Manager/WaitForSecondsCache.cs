using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// WaitForSeconds�� �����ϱ� ���� ĳ�� Ŭ���� 
/// 
/// <para>��� ����</para>
/// <para>private static readonly Dictionary<float, WaitForSeconds> cache = new Dictionary<float, WaitForSeconds>();</para>
/// 
/// <para>��� �ż���</para>
/// <para> public static : WaitForSeconds Get(float seconds), void Clear()</para>
/// </summary>
public class WaitForSecondsCache 
{
    private static readonly Dictionary<float, WaitForSeconds> cache = new Dictionary<float, WaitForSeconds>();

    /// <summary>
    /// �־��� �ð��� ���� WaitForSeconds �ν��Ͻ��� ĳ���Ͽ� ��ȯ 
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
    /// ĳ�� ���� �ʱ�ȭ
    /// </summary>
    public static void Clear() 
    {
        cache.Clear();
    }
}
