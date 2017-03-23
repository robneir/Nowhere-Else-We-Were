using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Cutscene : MonoBehaviour {

    private List<CutsceneImage> cutsceneImages = new List<CutsceneImage>();

	public AudioSource music;
    public AudioClip typeSound;

    public Image CurrentImage;
	public Image SpeakPortrait;

    public Text textUI;

    private CutsceneImage currrentScene;

    public int startSceneIndex = 0;

	public float startSceneDelay = 1.0f;
	public float endSceneDelay = 1.0f;
    public float typeSoundVolume = 0.2f;

    public string nextScene = "";

    private enum FadeType { FadeOut, FadeIn };

    [HideInInspector]
    public bool canSkipScene;
    [HideInInspector]
    public bool hasSkippedScene;

    [HideInInspector]
    public bool fire1Pressed;

    private Coroutine typeTextCoroutine;
    private Coroutine fadeOutCoroutine;
    private Coroutine fadeInCoroutine;
    private Coroutine playSceneCoroutine;

    [SerializeField]
    private Image triangleImage;

    // Use this for initialization
    void Start () {
        fire1Pressed = false;
        if(triangleImage != null)
        {
            triangleImage.gameObject.SetActive(false);
        }
        // Get all cutscenes that are children of this object
        int childs = transform.childCount;
        for(int i = 0; i < childs; i++)
        {
            GameObject go = transform.GetChild(i).gameObject;
            CutsceneImage scene = go.GetComponent<CutsceneImage>();
            if(scene != null)
            {
                cutsceneImages.Add(scene);
            }
        }
        // Disable all the scenes so they are not showing on top of each other
        for (int i = 0; i < cutsceneImages.Count; i ++)
        {
            CutsceneImage scene = cutsceneImages[i];
            if(scene != null)
            {
                scene.gameObject.SetActive(false);
            }
        }
        playSceneCoroutine = StartCoroutine(playScenes(false));
	}

    void Update()
    {
        // Type dialogue text or skip to end if pres Fire1
        if (canSkipScene && Input.GetButtonDown("Fire1"))
        {
            hasSkippedScene = true;
        }
    }

    public IEnumerator playScenes(bool delayStartScene)
    {
        if(delayStartScene)
        {
            yield return new WaitForSeconds(startSceneDelay);
        }
        
        // Run through all the scenes
        for (int i = startSceneIndex; i < cutsceneImages.Count; i++)
        {
            currrentScene = cutsceneImages[i];
            // Make sure current scene gameobject is showing.
            currrentScene.Load();
            currrentScene.gameObject.SetActive(true);

            if (currrentScene != null)
            {
                // Fade image in if specified.
                fadeInCoroutine = StartCoroutine(fadeCutsceneImage(FadeType.FadeIn, currrentScene));
                yield return fadeInCoroutine;

                // Type text
                typeTextCoroutine = StartCoroutine(currrentScene.TypeText());
                yield return typeTextCoroutine;

                // Fade image out if specified.
                canSkipScene = true;
                if(triangleImage != null)
                {
                    triangleImage.gameObject.SetActive(true);
                }
                fadeOutCoroutine = StartCoroutine(fadeCutsceneImage(FadeType.FadeOut, currrentScene));
                yield return fadeOutCoroutine;

                yield return StartCoroutine(waitUntilSkipScene());
            }
        }
        float soundVolume = 0.1f;
        while (soundVolume > 0.0f)
        {
            soundVolume -= (1.0f / endSceneDelay) * Time.deltaTime;
            music.volume = soundVolume;
            yield return null;
        }

        // At end of scenes load the level
        SceneManager.LoadScene(nextScene);
    }

    private IEnumerator waitUntilSkipScene()
    {
        // Wait until we have pressed to continue.
        while (!hasSkippedScene)
        {
            yield return null;
        }
        if(triangleImage != null)
        {
            triangleImage.gameObject.SetActive(false);
        }
        hasSkippedScene = false;
        canSkipScene = false;
    }

    private IEnumerator fadeCutsceneImage(FadeType fadeType, CutsceneImage scene)
    {
        // Amount to subtract from alpha when fading.
        float alphaAddition = 0.0f;

        // Determine whether to fade in our out base on fadeType
        if (fadeType == FadeType.FadeOut)
        {
            alphaAddition = -1.0f / scene.fadeOutTime;
            scene.currentFadeTime = scene.fadeOutTime;
        }
        else
        {
            alphaAddition = 1.0f / scene.fadeInTime;
            scene.currentFadeTime = scene.fadeInTime;
            if(scene.shouldFadeImageIn)
            {
                CurrentImage.color = new Color(CurrentImage.color.r,
                    CurrentImage.color.g, CurrentImage.color.b, 0.0f);
            }
        }

        // Transition between scenes.
        while (scene.currentFadeTime > 0.0f)
        {
            scene.currentFadeTime -= Time.deltaTime;
            if (CurrentImage != null)
            {
                if(fadeType == FadeType.FadeOut && scene.shouldFadeImageOut ||
                    fadeType == FadeType.FadeIn && scene.shouldFadeImageIn)
                {
                    float actualAlphaAddition = alphaAddition * Time.deltaTime;
                    CurrentImage.color = new Color(CurrentImage.color.r,
                        CurrentImage.color.g, CurrentImage.color.b, CurrentImage.color.a + actualAlphaAddition);
                }
            }
            else
            {
                Debug.Log("Current image public member variable not set in cutscene.");
            }
            yield return null;

            // Check to see if we want to skip fadeout of scene.
            if (hasSkippedScene)
            {
                yield break;
            }
        }
    }
}
