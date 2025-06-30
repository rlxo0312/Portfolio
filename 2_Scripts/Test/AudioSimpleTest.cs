using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSimpleTest : MonoBehaviour
{
    public AudioClip testClip;

    void Start()
    {
        var source = gameObject.AddComponent<AudioSource>();
        source.clip = testClip;
        source.loop = true;
        source.volume = 1f;
        source.spatialBlend = 0f;
        source.Play();
        Debug.Log("[AudioSimpleTest] 내장 사운드 테스트");
    }
}
