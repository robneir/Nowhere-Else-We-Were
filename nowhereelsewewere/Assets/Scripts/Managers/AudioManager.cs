using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

	[SerializeField]
	[Tooltip("The audio listener for the game, this will usually be on the camera")]
	private AudioListener audioListener;

	[SerializeField]
	private AudioSource audioSourcePrefab;

	public AudioSource gameMusicSource;
	public AudioSource envMusicSource;

    public AudioClip uiFailSound;
    public AudioClip uiDownwardSound;
    public AudioClip uiUpwardSound;

    private float previousMaxVolume = -1;

	public enum MusicType{
		Game,
		Environment
	}

	public enum FadeType
	{
		FadeOut,
		FadeIn
	}

	// This should just get called at start of game to load some basic music and environment sounds for now.
	public void BeginGameAudio()
	{
		// Play game music
		if (Constants.instance.rainForestSounds != null) 
		{
			StartCoroutine(TransitionGameMusic(envMusicSource, Constants.instance.rainForestSounds, .7f, 8.0f, .2f));
		} 
		else 
		{
			Debug.Log ("Trying to transition to null music");
		}
	}

	public void PlayMusic(MusicType musicType, AudioClip clip, float volume, bool isLooping)
	{
		AudioSource audioMusicSource;
		if (musicType == MusicType.Game) {
			audioMusicSource = gameMusicSource;
		} else {
			audioMusicSource = envMusicSource;
		}
		audioMusicSource.clip = clip;
		audioMusicSource.volume = volume;
		audioMusicSource.loop = isLooping;
		audioMusicSource.Play ();
	}

	public IEnumerator TransitionGameMusic(AudioSource audioSource, AudioClip newGameMusic, float fadeOutTime , float fadeInTime, float maxVolume)
	{
		if (fadeOutTime <= 0) {
			Debug.Log ("Cannot make fade in/out time 0 for music transitions");
			fadeOutTime = 1.0f;
		}
		if (fadeInTime <= 0) {
			Debug.Log ("Cannot make fade in/out time 0 for music transitions");
			fadeInTime = 1.0f;
		}
		// Check to see if we need to fadeout first, which is basically checking if there is game music playing right now.
		if (audioSource.clip != null && audioSource.isPlaying) 
		{
			yield return StartCoroutine (FadeMusic (audioSource, FadeType.FadeOut, fadeOutTime));
		}
		audioSource.clip = newGameMusic;
		audioSource.loop = true;
		audioSource.Play ();
		StartCoroutine (FadeMusic (audioSource, FadeType.FadeIn, fadeInTime, maxVolume));
	}

	public void PlayEnvMusic(AudioClip clip, float volume, bool isLooping)
	{
		envMusicSource.clip = clip;
		envMusicSource.volume = volume;
		envMusicSource.loop = isLooping;
		envMusicSource.Play ();
	}

	private IEnumerator FadeMusic(AudioSource audioSource, FadeType fadeType, float fadeTime, float maxVolume = 1.0f)
	{
		if (audioSource != null) {
			// Set previous max volume so we can figure out what to subtract to get to 0 on time.
			if (previousMaxVolume == -1) {
				previousMaxVolume = maxVolume;
			}

			// Figure out subtraction amount
			float volumeAdded = previousMaxVolume / fadeTime * Time.deltaTime; 
			float targetVolume = (fadeType == FadeType.FadeIn) ? maxVolume : 0.0f;
			while (audioSource.volume != targetVolume) 
			{
				// Choose whether to fade in or out.
				if(fadeType == FadeType.FadeIn)
				{
					//Debug.Log ("adding volume");
					audioSource.volume += volumeAdded;
					if (audioSource.volume >= maxVolume) 
					{
						audioSource.volume = maxVolume;
					}
				}
				else if(fadeType == FadeType.FadeOut)
				{
					//Debug.Log ("subtracting volume");
					audioSource.volume -= volumeAdded;
					if (audioSource.volume <= 0.0f) 
					{
						audioSource.volume = 0.0f;
					}
				}
				yield return null;
			}

			// Set previous max volume to -1 for next time we call this function.
			previousMaxVolume = -1;
		}
	}

    public void Play3DSound(AudioClip clip, Transform parent, bool isLooping)
    {
        StartCoroutine(Play3DSoundCoroutine(clip, parent, isLooping));
    }

    public void Play2DSound(AudioClip clip, float volume, bool isLooping)
    {
        StartCoroutine(Play2DSoundCoroutine(clip, volume, isLooping));
    }

    // Plays a sound that is in 3D space and can be parented to object.
    private IEnumerator Play3DSoundCoroutine(AudioClip clip, Transform parent, bool isLooping)
	{
		AudioSource audInstance = (AudioSource)Instantiate (audioSourcePrefab, parent); 
		audInstance.clip = clip;
		audInstance.spatialBlend = 1.0f;
		audInstance.loop = isLooping;
		audInstance.Play ();
		if (!isLooping)
        {
            // Destroy audio source after it has completely played.
            yield return new WaitForSeconds(audInstance.clip.length);
            Destroy(audInstance.gameObject);
        }
	}

	// Plays sound that is in 2D space so no depth/falloff to sound.
	private IEnumerator Play2DSoundCoroutine(AudioClip clip, float volume, bool isLooping)
	{
		AudioSource audInstance = (AudioSource)Instantiate (audioSourcePrefab); 
		audInstance.clip = clip;
		audInstance.volume = volume;
		audInstance.spatialBlend = 0.0f;
		audInstance.loop = isLooping;
		audInstance.Play ();
		if (!isLooping)
        {
            // Destroy audio source after it has completely played.
            yield return new WaitForSeconds(audInstance.clip.length);
            Destroy(audInstance.gameObject);
		}
	}
}
