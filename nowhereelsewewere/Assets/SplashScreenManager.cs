using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashScreenManager : MonoBehaviour {

    public string mainMenuLevel = "";

    public bool doesLoadLevel = false;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LoadMainMenuLevel()
    {
        if (doesLoadLevel)
        {
            SceneManager.LoadScene(mainMenuLevel);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
