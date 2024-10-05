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
    public AudioClip deathAudio;


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
                return deathAudio;
            default:
                return null;
        }
    }
}
