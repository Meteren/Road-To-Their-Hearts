using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<AudioSource> bossMusics;
    [SerializeField] private List<AudioSource> audios;

    public void PlayBossMusic(int i)
    {
        bossMusics[i].Play();
    }
    
    public void StopBossMusic(int i)
    {
        bossMusics[i].Stop();
    }

    public void PlaySFX(int i)
    {
        audios[i].Play();   
    }

    public void StopSFX(int i)
    {
        audios[i].Stop();
    }

    public AudioSource BossMusicAudioSource(int i)
    {
        return bossMusics[i];
    }

    public AudioSource GetSFX(int i)
    {
        return audios[i];
    }

}
