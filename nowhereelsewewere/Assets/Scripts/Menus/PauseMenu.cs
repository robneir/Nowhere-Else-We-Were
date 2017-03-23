using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : Menu {

    [SerializeField]
    private GameManager gm;

	public void ResumeClicked()
    {
        if(gm != null)
        {
            gm.ResumeGame();
        }
    }

    public void RestartClicked()
    {
        gm.GameOver();
    }

    public void SettingsClicked()
    {

    }

    public void MainMenuQuitClicked()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void QuitClicked()
    {
        Application.Quit();
    }
}
