using UnityEngine;
using System.Collections.Generic;

public class SoundManager
{
    Dictionary<Sounds, AudioSource> audioSource = new Dictionary<Sounds, AudioSource>();


    public void PlaySound(Sounds sound, float volume, bool isLoop)
    {
        if (!audioSource.ContainsKey(sound))
        {
            audioSource.Add(sound, AppManager.instance.gameObject.AddComponent<AudioSource>());
        }
            audioSource[sound].volume = volume;
            audioSource[sound].loop = isLoop;
            
            audioSource[sound].Stop();
            audioSource[sound].clip = AppManager.instance.resourceManager.GetSound(sound);
            audioSource[sound].Play();
    }

    public void StopSound(Sounds sound)
    {
        if (audioSource.ContainsKey(sound))
        {
            audioSource[sound].Stop();
        }
    }



}
