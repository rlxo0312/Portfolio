using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Audio;

public class AudioTest : MonoBehaviour
{
    public UiVolumSlifer slider1;
    public AudioSource source;
    public AudioClip clip;
    public AudioMixerGroup group;
    //public float delay;
    void Start()
    {
        /*if (slider1 != null)
        {
            float value = slider1.slider.value;
            slider1.SliderValue(value);
        }*/
        /* if(source != null)
         {
             source.Play();
             Debug.Log("[AudioTest]�������");
         }
         else
         {
             Debug.Log("[AudioTest]������ ������� ����");
         }*/
        /*Debug.Log($"[AudioTest] AudioSource - {source.name}");
        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
            Debug.Log("[AudioTest] - AudioSource�� ��� ���Ҵ�");
        }
        source.outputAudioMixerGroup = group;
        Debug.Log($"[AudioTest] AudioMixerGroup - {group.name}");
        source.clip = clip;
        Debug.Log($"[AudioTest] AudioClip - {clip.name}");
        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f; // 2D
        source.volume = 1f;
        //yield return new WaitForSeconds(delay);

        source.Play();*/
        //Debug.Log("[AudioSimpleTest] ��� �õ�");
    }
}
