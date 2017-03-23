using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Menu : MonoBehaviour {

    public AudioClip hoverSound;
    public AudioClip clickSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClickSound()
    {
        if (clickSound != null)
        {
            audioSource.clip = clickSound;
            audioSource.Play();
        }
    }

    public void PlayHoverSound()
    {
        if (hoverSound != null)
        {
            audioSource.clip = hoverSound;
            audioSource.Play();
        }
    }
}
