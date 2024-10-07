using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchSFXController : MonoBehaviour
{
    public AudioSource audioSource;
    public List<AudioClip> audioClips = new List<AudioClip>();
    public List<AudioClip> cackeClips = new List<AudioClip>();

    public void PlayRandomClip()
    {
        AudioClip clip = GetRandomClip(audioClips);
        audioSource.PlayOneShot(clip);

        float random = Random.Range(0, 1f);
        if (random < .4)
        {
            StartCoroutine(PlayRandomCackleAfterDelay(.2f));
        }
    }


    AudioClip GetRandomClip(List<AudioClip> list)
    {
        int index = Random.Range(0, list.Count);
        return list[index];
    }

    IEnumerator PlayRandomCackleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.PlayOneShot(GetRandomClip(cackeClips));
    }
}
