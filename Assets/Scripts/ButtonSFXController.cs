using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSFXController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickClip;

    public void PlayClick()
    {
        audioSource.PlayOneShot(clickClip);
    }

}
