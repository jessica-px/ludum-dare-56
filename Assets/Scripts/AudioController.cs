using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SoundEffect
{
    Success,
    Miss,
    Death
}

public class AudioController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip successAudio;
    public AudioClip missAudio;

    public List<AudioClip> squishAudioClips = new List<AudioClip>();


    public void PlaySoundEffect(SoundEffect soundEffect, float delay)
    {
        audioSource.clip = GetAudioClip(soundEffect);
        audioSource.PlayDelayed(delay);
    }

    AudioClip GetAudioClip(SoundEffect soundEffect)
    {
        switch (soundEffect)
        {
            case SoundEffect.Success:
                return successAudio;
            case SoundEffect.Miss:
                return missAudio;
            case SoundEffect.Death:
                return GetRandomClip(squishAudioClips);
            default:
                return null;
        }
    }

    AudioClip GetRandomClip(List<AudioClip> list)
    {
        int index = Random.Range(0, list.Count);
        return list[index];
    }
}
