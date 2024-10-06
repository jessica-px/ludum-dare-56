using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgMusicController : MonoBehaviour
{
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        StartCoroutine(StartFade(0, audioSource.volume, 1));
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
