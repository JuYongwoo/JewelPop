using System;
using UnityEngine;
using System.Collections.Generic;

public class SoundManager
{
    GameObject soundManagerGameObject;
    Dictionary<Sounds, AudioSource> audioSource = new Dictionary<Sounds, AudioSource>();

    public void OnAwake()
    {
        soundManagerGameObject = new GameObject("SoundManager");

    }

    public void PlaySound(Sounds sound, float volume, bool isLoop)
    {
        if (!audioSource.ContainsKey(sound))
        {
            audioSource.Add(sound, soundManagerGameObject.AddComponent<AudioSource>());
        }
            audioSource[sound].volume = volume;
            audioSource[sound].loop = isLoop;
            
            audioSource[sound].Stop();
            audioSource[sound].clip = ManagerObject.instance.resourceManager.gameSoundClips[sound];
            audioSource[sound].Play();
    }



}
