using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [HideInInspector]
    public List<WorldCharacter> playableCharacters =  new List<WorldCharacter>();

    [HideInInspector]
    public List<WorldCharacter> enemyCharacters = new List<WorldCharacter>();

    public Texture2D GameOverTexture;

	public Texture2D VictoryTexture;

	public Texture2D BlackTexture;

	public Texture2D RestartTexture;

    public CubeManager cubeManager;

    public TurnManager turnManager;

	public BattleManager battleManager;

	public AIManager aiManager;

	public SelectionManager selectionManager;

    public CameraManager cameraManager;

    public ActionManager actionManager;

    public UIManager uiManager;

	public AudioManager audioManager;

	public LevelingManager levelingManager;

    public TutorialManager tutorialManager;

	public LumiusManager lumiusManager;

	public Fade FadeToBlack;

	public string VictoryLevel = "";

	public string DefeatLevel = "";

	[SerializeField]
	private float IntroTime = 2.0f;
	private Texture2D fadeOutTexture;
	private int drawDepth = -1001;
	private float alpha = 0.0f;
	private int fadeDir = -1;
	private float fadeSpeed = 0.5f;

	private bool canRestart = false;
    private bool isPaused = false;

	// Use this for initialization
	void Start () {
		StartCoroutine(Init());
	}

	IEnumerator Init()
	{
		foreach (WorldCharacter w in playableCharacters)
		{
			if(w != null)
			{
				w.resetHealth();
			}
		}

		foreach (WorldCharacter e in enemyCharacters)
		{
			if(e != null)
			{
				e.resetHealth();
			}
		}
		FadeToBlack.fade(-1, IntroTime, BlackTexture);
		turnManager.BeginTurn(this, IntroTime);
		yield return new WaitForSeconds(IntroTime);

	}
	
	// Update is called once per frame
	void Update () {
		if(canRestart && Input.GetKeyDown(KeyCode.R))
		{
			SceneManager.LoadScene("Level1_Dream");
		}
        // Show the pause menu if 'p' pressed
        if(Input.GetButtonDown("Pause") || Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
	}

    public void PauseGame()
    {
        isPaused = true;
        uiManager.ShowPauseMenu();
    }

    public void ResumeGame()
    {
        isPaused = false;
        uiManager.HidePauseMenu();
    }

	public void GameOver()
	{
		StartCoroutine(audioManager.TransitionGameMusic(audioManager.gameMusicSource, Constants.instance.DefeatSound, 1.5f, 1.5f, 1.0f));
		StartCoroutine(EndOfGameHelper(3.0f, GameOverTexture, DefeatLevel));
	}


	public void Victory()
	{
		StartCoroutine(audioManager.TransitionGameMusic(audioManager.gameMusicSource, Constants.instance.VictorySound, 0.5f, 0.5f, 1.0f));
		StartCoroutine(EndOfGameHelper(1.0f, VictoryTexture, VictoryLevel));
	}

	private IEnumerator EndOfGameHelper(float time, Texture2D text, string newScene)
	{
		turnManager.disabled = true;
		battleManager.disabled = true;
		fade(1, time, text);
		yield return new WaitForSeconds(time + 5.0f);
		FadeToBlack.fade(1, 2.0f, BlackTexture);
		yield return new WaitForSeconds(2.0f);

		canRestart = false;
		//switch levels
		SceneManager.LoadScene(newScene);

	}
	void OnGUI()
	{
		if (fadeOutTexture == null)
		{
			return;
		}
		alpha += fadeDir * fadeSpeed * Time.deltaTime;

		alpha = Mathf.Clamp01(alpha);

		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
		GUI.depth = drawDepth;
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
	}

	private void fade(int fadeDir, float fadeTime, Texture2D text)
	{
		this.fadeOutTexture = text;
		this.fadeDir = fadeDir;
		this.fadeSpeed = 1.0f / fadeTime;
	}
}
