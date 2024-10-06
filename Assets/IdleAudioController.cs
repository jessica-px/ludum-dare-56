using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAudioController : MonoBehaviour
{
    public AudioSource audioSource;
    public List<AudioClip> idleAudioClips = new List<AudioClip>();

    float playCountdown = 1;

    private void Update()
    {
        // playCountdown -= Time.deltaTime;
        // if (playCountdown <= 0)
        // {
            // PlayIdleClip();
            // playCountdown = Random.Range(1f, 3f);
        // }
    }

    public void PlayIdleClip()
    {
        AudioClip clip = GetRandomClip(idleAudioClips);
        audioSource.panStereo = Random.Range(-1f, 1f);
        audioSource.PlayOneShot(clip);
    }
 
    AudioClip GetRandomClip(List<AudioClip> list)
    {
        int index = Random.Range(0, list.Count);
        return list[index];
    }
}
