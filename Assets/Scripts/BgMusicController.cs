using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgMusicController : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip bgMusic;
    public AudioClip endTag;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        
    }

    public void StartMusic()
    {
        audioSource.clip = bgMusic;
        audioSource.loop = true;
        audioSource.Play();
        StartCoroutine(StartFade(0, audioSource.volume, 1));
    }

    public void StartEndTag()
    {
        audioSource.clip = endTag;
        audioSource.loop = false;
        audioSource.Play();
    }

    public IEnumerator StartFade(float startVolume, float targetVolume, float duration)
    {
        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }
}
