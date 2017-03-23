using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CutsceneImage : MonoBehaviour {

    public string prompt;

	public Cutscene cs;

	public Sprite newBackground = null;

	public Sprite newPortrait = null;

	public AudioClip newBackgroundMusic = null;

    public bool shouldFadeImageOut = false;
    public bool shouldFadeImageIn = false;
    private bool firePressed;
    private bool canSkipText;

    public float fadeOutTime = 0.0f;
    public float fadeInTime = 0.0f;

    [HideInInspector]
    public float currentFadeTime = 1.0f;

    [SerializeField]
    private float timeBetweenLetters = 0.05f;
    private float startLetterTime = 0.0f;

    private AudioSource audioSource;

    private int currLetterIndex = 0;

    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
        firePressed = false;
        canSkipText = false;
    }

    void Update()
    {
        if(canSkipText && Input.GetButtonDown("Fire1"))
        {
            firePressed = true;
        }
    }

    public void Load()
    {
        // Clear the text that all scenes use because it could have text from previous scene.
        cs.textUI.text = "";

        // Set background image.
        if (newBackground != null)
        {
            cs.CurrentImage.sprite = newBackground;
        }

        // Make the background visible no matter what.
        if (cs.CurrentImage != null)
        {
            cs.CurrentImage.color = new Color(cs.CurrentImage.color.r, cs.CurrentImage.color.g,
                cs.CurrentImage.color.b, 1.0f);
        }

        // Set character portrait
        if (newPortrait != null)
        {
            cs.SpeakPortrait.sprite = newPortrait;
        }

        // Set the background music
        if (newBackgroundMusic != null)
        {
            cs.music.Stop();
            cs.music.clip = newBackgroundMusic;
            cs.music.Play();
        }

        // Set the type sound
        if (audioSource != null)
        {
            if (cs.typeSound != null)
            {
                audioSource.clip = cs.typeSound;
                audioSource.volume = cs.typeSoundVolume;
            }
        }
    }
	
	public IEnumerator TypeText()
    {
        canSkipText = true;
        // Type all of the text for this scene.
        for (int i = 0; i < prompt.Length; i++)
        {
            cs.textUI.text += prompt[i];
            // Play type sound if not null
            if(audioSource != null && audioSource.clip != null)
            {
                audioSource.Play();
            }
            
            startLetterTime = 0.0f;
            while(startLetterTime < timeBetweenLetters)
            {
                startLetterTime += Time.deltaTime;
                if(firePressed)
                {
                    cs.textUI.text = prompt;
                    canSkipText = false;
                    yield break;
                }
                else
                {
                    // Keep looping
                    yield return null;
                }
            }
        }
        canSkipText = false;
    }
}
