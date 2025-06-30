using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// <summary>
/// �ΰ��� ������ ��� ����(BGM) �� �÷��̾��� ���� ȿ��(SFX)�� �����ϴ� �̱��� Ŭ����
///
/// <para>��� ����</para>
/// <para>public static SoundManager soundManager</para>
/// <para>public AudioClip BGM, footStepSFX, jumpSFX, runStepSFX, swordSFX</para>
///
/// <para>�޼���</para>
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
