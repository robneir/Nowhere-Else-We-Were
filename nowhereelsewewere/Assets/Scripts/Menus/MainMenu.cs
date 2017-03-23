using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : Menu {

	public string startingLevel = "";
	public string CreditsLevel = "";

    public void NewGameClicked()
    {
        SceneManager.LoadScene(startingLevel);
    }

    public void SettingsClicked()
    {

    }

	public void CreditsClicked()
	{
		SceneManager.LoadScene(CreditsLevel);
	}

    public void LoadGameClicked()
    {
        SceneManager.LoadScene("CutScene_WakeUp");
    }

    public void QuitClicked()
    {
        Application.Quit();
    }
}
