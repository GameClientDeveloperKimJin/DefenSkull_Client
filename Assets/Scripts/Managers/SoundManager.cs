using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public void SetMusicVolume(float volume)
    {
        bgmPlayer.volume = volume;
        //DontDestroyOnLoad(musicSource);

    }
    public void SetButtonVolume(float volume)
    {

        for (int i = 0; i < channels; i++)
        {
            sfxPlayers[i].volume = volume;
        }


    }
    public static SoundManager sound;

    public static SoundManager Sound
    {
        get
        {
            if (sound == null)
            {
                sound = FindObjectOfType<SoundManager>();
                if (sound == null)
                {
                    GameObject managerObject = new GameObject("SoundManager");
                    sound = managerObject.AddComponent<SoundManager>();
                }
            }
            return sound;
        }
    }

    [Header("#BGM")]//배경음
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;

    [Header("#SFX")]//효과음
    public AudioClip[] sfxClips;
    public float sfxVolume = 1.0f;
    public int channels;
    AudioSource[] sfxPlayers;
    int channlIndex;

    public enum Sfx
    {
       Player_Basic_Attack,
       Player_Combo_Attack,
       Player_Curve_Attack,
       Player_Die,
       Skull_Die,
       Skull_Hit

    }

    private void Awake()
    {
       sound = this;
    }

    void Init()
    {
        //배경음 플레이어 초기화 
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;

        bgmPlayer.loop = true;
        //bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;


        //효과음 플레이어 초기화 
        GameObject sfxObject = new GameObject("sfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].volume = 0.3f;
            sfxPlayers[index].pitch = 3.0f;
        }
    }
    public void PlayBgm(bool isPlay)
    {

        if (bgmPlayer == null)
        {
            Init(); // bgmPlayer가 null이면 Init 호출로 초기화
        }

        if (isPlay)
        {
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }
    public void PlaySfx(Sfx sfx)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channlIndex) % sfxPlayers.Length;
            if (sfxPlayers[loopIndex].isPlaying)
            {
                continue;
            }

            channlIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }


    }

}
