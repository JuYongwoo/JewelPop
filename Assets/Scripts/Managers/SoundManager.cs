using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    //private Dictionary<Skills, AudioClip> skillSoundsMap;

    private Dictionary<AudioClip, AudioSource> audioSources = new Dictionary<AudioClip, AudioSource>();
    private float masterVolume = 1f; //��� ���忡 �������� ��

    public void OnStart()
    {
        GameManager.instance.eventManager.PlayAudioClipEvent -= PlayAudioClip;
        GameManager.instance.eventManager.PlayAudioClipEvent += PlayAudioClip;
        GameManager.instance.eventManager.StopAudioClipEvent -= StopAudioClip;
        GameManager.instance.eventManager.StopAudioClipEvent += StopAudioClip;
        GameManager.instance.eventManager.StopAllAudioClipEvent -= StopAllAudioClip;
        GameManager.instance.eventManager.StopAllAudioClipEvent += StopAllAudioClip;
        GameManager.instance.eventManager.SetMasterVolumeEvent -= SetMasterVolume;
        GameManager.instance.eventManager.SetMasterVolumeEvent += SetMasterVolume;
    }
    public void OnDestroy()
    {
        GameManager.instance.eventManager.PlayAudioClipEvent -= PlayAudioClip;
        GameManager.instance.eventManager.StopAudioClipEvent -= StopAudioClip;
        GameManager.instance.eventManager.StopAllAudioClipEvent -= StopAllAudioClip;
        GameManager.instance.eventManager.SetMasterVolumeEvent -= SetMasterVolume;

    }

    private void PlayAudioClip(AudioClip ac, float volume, bool isLoop)
    {
        if (!audioSources.ContainsKey(ac)) //�������� �ʴ� ����� Ŭ�� ���� �ҽ�
        {
            KeyValuePair<AudioClip, AudioSource> removeCandi = new KeyValuePair<AudioClip, AudioSource>(null, null);



            //���� ����Ѵٴ� ���� �����ε� ����� �� O, �����ִ� ������ҽ��� �̿��ϵ��� ��ųʸ��� key�� �ٲ��ֵ��� �Ѵ�.
            foreach (var pair in audioSources)
            {
                if (!pair.Value.isPlaying) //�����ִ� ������ҽ��� �ִ°�
                {
                    removeCandi = new KeyValuePair<AudioClip, AudioSource>(pair.Key, pair.Value); //���ο� �����Ŭ�� & �̹� ������ ������ҽ� ���
                    break; // �����ִ� ù ������ҽ��� �����ϰ� ���´�.
                }
            }


            if (removeCandi.Key != null) //���� �ִ� ������ҽ��� ã�Ҵ�
            {
                audioSources.Remove(removeCandi.Key); //���� ��ųʸ� ����(���� ������Ʈ�� �������� ����)
                audioSources.Add(ac, removeCandi.Value); //Ű�� �ٲ㼭 ����Ѵ�
            }
            else//���� �ִ� ������ҽ��� ��ã�Ҵ�.
            {
                var src = GameManager.instance.gameObject.AddComponent<AudioSource>(); //���� ����
                src.playOnAwake = false;
                src.spatialBlend = 0f;
                src.priority = 128;
                audioSources.Add(ac, src); //���� ���� ��ųʸ� �߰�
            }

        }

        if (isLoop)
        {
            var s = audioSources[ac];
            if (s.isPlaying && s.clip == ac) return; // �̹� ���� ������ ��� ���̸� ����
        }
        audioSources[ac].volume = volume * masterVolume;
        audioSources[ac].loop = isLoop;

        audioSources[ac].Stop();
        audioSources[ac].clip = ac;
        audioSources[ac].Play();

    }

    private void StopAudioClip(AudioClip ac)
    {
        if (audioSources.ContainsKey(ac))
        {
            audioSources[ac].Stop();
        }
    }

    private void StopAllAudioClip()
    {
        foreach (var source in audioSources.Values)
        {
            source.Stop();
        }
    }

    private void SetMasterVolume(float vol)
    {
        masterVolume = vol;
    }


}
