using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 플레이어 오디어 매니저 클래스 
/// </summary>
public class PlayerAudioManager : MonoBehaviour
{
    AudioSource audioSource;
    

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();  
    }
    public void PlaySwordSoundSFX()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(SoundManager.soundManager.swordSFX);
        }
    } 
    public void PlayFootSoundSFX()
    {
        //&& !SoundManager.soundManager.footStepSFX
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(SoundManager.soundManager.footStepSFX);
        }
    }
    public void PlayJumpSFX()
    {
        //&& !SoundManager.soundManager.jumpSFX
        if (!audioSource.isPlaying)
        {

            audioSource.PlayOneShot(SoundManager.soundManager.jumpSFX);
        }
    }
    public void RunStepSFX()
    {
        // && !SoundManager.soundManager.runStepSFX
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(SoundManager.soundManager.runStepSFX);
        }
    }

    public void PlayAllStop()
    {
        audioSource.Stop();
    }
}
