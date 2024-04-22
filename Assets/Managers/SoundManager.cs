using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource sfxSource;


    public float BGMVol { get { return bgmSource.volume; } set{bgmSource.volume = value; } }
    public float sfxBol { get { return sfxSource.volume; } set {  sfxSource.volume = value; } }

    public void PlayBGM(AudioClip clip )
    {
        if ( bgmSource.isPlaying )
        {
            bgmSource.Play();
        }
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if ( bgmSource.isPlaying == false )
            return;
        bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip )
    {
        sfxSource.PlayOneShot(clip);
    }

    public void StopSFX()
    {
        if ( sfxSource.isPlaying == false )
            return;

        sfxSource.Stop();
    }
}
