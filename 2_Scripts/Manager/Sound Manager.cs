using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// <summary>
/// 인게임 내에서 배경 음악(BGM) 및 플레이어의 사운드 효과(SFX)를 관리하는 싱글톤 클래스
///
/// <para>사용 변수</para>
/// <para>public static SoundManager soundManager</para>
/// <para>public AudioClip BGM, footStepSFX, jumpSFX, runStepSFX, swordSFX</para>
///
/// <para>메서드</para>
/// <para>private void Awake()</para>
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager soundManager; 
    
    public AudioClip BGM;
    public AudioClip footStepSFX; 
    public AudioClip jumpSFX;
    public AudioClip runStepSFX;
    public AudioClip swordSFX;
   
    private void Awake()
    {
        if (soundManager == null)
        {
            soundManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}
