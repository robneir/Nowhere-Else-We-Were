using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TurnManager : MonoBehaviour {

    public GameManager gm;

    // Keeps track of what characters have finished their turn.
    private List<WorldCharacter> charactersWhoFinishedTurn = new List<WorldCharacter>();

	private Queue<WorldCharacter> enemyCharacters = new Queue<WorldCharacter>();

    public enum TurnPhase{ Player, Enemy }
    public TurnPhase currTurnPhase;
    
    public enum PlayerTurnPhase{ Move, MoveBack, ChoosePhase, PreAttack, Attack, SelectPlayer, SwitchingRounds, EndTurn, GameOver}
    private PlayerTurnPhase currPlayerTurnPhase;

	bool waiting = false;
	float timer = 0.0f;

	public volatile bool disabled = false;

	public Texture2D playerTurnTexture;
	public Texture2D enemyTurnTexture;
	private Texture2D fadeOutTexture;
	private int drawDepth = -1000;
	private float alpha = 0.0f;
	private int fadeDir = -1;
	private float fadeSpeed = 0.5f;

    // bool to check to see if we should begin combat when attack button pressed
    public volatile bool beginCombat = false;

	// Use this for initialization
	void Start () {
        // Begin in the player phase.
        currTurnPhase = TurnPhase.Player;
		gm.audioManager.BeginGameAudio();
    }

    // Update is called once per frame
    void Update () {
		if (!gm || gm.battleManager.InCombat || disabled)
		{
			return;
		}
		if (waiting)
		{
			timer -= Time.deltaTime;
			if(timer <= 0.0f)
			{
				timer = 0.0f;
				waiting = false;
			}
			return;
		}
        if(currTurnPhase == TurnPhase.Player)
        {
            #region PlayerTurn
            gm.uiManager.phaseIndicator.transform.parent.gameObject.SetActive(true);
            // Show the highlight sprite for current hover 
            gm.selectionManager.CheckForHover();

            if (currPlayerTurnPhase == PlayerTurnPhase.SwitchingRounds)
			{
				ComputeAllRangeCubes(gm.playableCharacters);
				ComputeAllRangeCubes(gm.enemyCharacters);
				// Clear all character's disabled materials.
				foreach (WorldCharacter character in charactersWhoFinishedTurn)
                {
                    character.ShowEnabled();
					gm.lumiusManager.ResetBuffs(character);
                }

				foreach(WorldCharacter character in gm.enemyCharacters)
				{
					if(character != null && character.bIsAlive)
					{
						character.ShowEnabled();
					}
				}
				gm.battleManager.RemoveCombatants();
				this.charactersWhoFinishedTurn.Clear();
                gm.cameraManager.canInput = true;
				currPlayerTurnPhase = PlayerTurnPhase.SelectPlayer;
				if (!PlayablesLeft() || !EnemiesLeft()) { return; }
				StartCoroutine(gm.audioManager.TransitionGameMusic(gm.audioManager.gameMusicSource, Constants.instance.playerPhaseGameMusic, 2.0f, 0.5f, 0.25f));
				fade(1, 0.5f, playerTurnTexture);
				Wait(2.0f);
			}
            else if (currPlayerTurnPhase == PlayerTurnPhase.SelectPlayer)
			{
				if (!PlayablesLeft())
				{
					gm.GameOver();
					currPlayerTurnPhase = PlayerTurnPhase.GameOver;
					return;
				}
				else if (!EnemiesLeft())
				{
					gm.Victory();
					currPlayerTurnPhase = PlayerTurnPhase.GameOver;
					return;
				}
				fade(-1, 0.5f, playerTurnTexture);
                gm.lumiusManager.CanUseLumius = false;
                gm.uiManager.getActionPanel().RemoveActionMenuButtons();

                // Check to see if we should go to enemies turn
                bool playerPhaseOver = true;
                for(int i=0; i < gm.playableCharacters.Count; i++)
                {
                    WorldCharacter character = gm.playableCharacters[i];
                    if(character != null)
                    {
                        if(character.bIsAlive && !charactersWhoFinishedTurn.Contains(character))
                        {
                            playerPhaseOver = false;
							break;
                        }
                    }
                }
                // If we figured out the player phase was over above in for loop.
                if (playerPhaseOver)
                { 
                    BeginEnemyTurn();
                    return;
                }
                gm.cameraManager.FreeCamera();


                // Get and Set character whos turn is next.
                gm.selectionManager.CheckForNewSelection();
                WorldCharacter selectedCharacter = gm.selectionManager.GetCurrentSelectedCharacter();
                if (selectedCharacter != null && !charactersWhoFinishedTurn.Contains(selectedCharacter)) // If playable characters left (who havent done their turn).
                {
                    gm.selectionManager.SetCurrentSelectedCharacter(selectedCharacter);
                    gm.selectionManager.ClearCubesInShortestPath();
					selectedCharacter.ShowRangeCubes(gm.cubeManager, gm.selectionManager);
					currPlayerTurnPhase = PlayerTurnPhase.Move;
                }
            }
            else if (currPlayerTurnPhase == PlayerTurnPhase.Move) // Address the actions the current player can do.
			{
                gm.lumiusManager.CanUseLumius = true;
                gm.uiManager.getActionPanel().gameObject.SetActive(false);
                gm.battleManager.RemoveCombatants();

				// hide the shadow mesh thing here plz //

				gm.cameraManager.FreeCamera();

				bool movePhaseDone = gm.selectionManager.CheckMovingPhaseInput();
				WorldCharacter curr = gm.selectionManager.GetCurrentSelectedCharacter();
				if (curr)
				{
					Movement m = curr.GetComponent<Movement>();
					if(m)
					{
						if(m.GetMoveState() != Movement.MoveState.Idle)
						{
                            //lock the camera to the selected character
                            gm.lumiusManager.CanUseLumius = false;

							// show the shadow mesh thing here plz //

                            gm.cameraManager.LockCamera(gm.selectionManager.GetCurrentSelectedCharacter(), true);
						}
					}
				}
                if(movePhaseDone)
                {
                    gm.lumiusManager.CanUseLumius = false;
                    currPlayerTurnPhase = PlayerTurnPhase.ChoosePhase;
                }
				else
				{
					gm.selectionManager.CheckChoosePhaseInput();
				}
            }
            else if(currPlayerTurnPhase == PlayerTurnPhase.ChoosePhase)
            {
                gm.cameraManager.LockCamera(gm.selectionManager.GetCurrentSelectedCharacter(), true);
                gm.battleManager.RemoveCombatants();
                // Check to see if we want to move back to the move phase.
                gm.selectionManager.CheckChoosePhaseInput();
            }
            else if(currPlayerTurnPhase == PlayerTurnPhase.MoveBack)
            {
				MoveCurrCharacterBackToStartCube();
            }
            else if (currPlayerTurnPhase == PlayerTurnPhase.PreAttack)
			{
				gm.selectionManager.CheckPreAttackPhaseInput();
            }
            else if (currPlayerTurnPhase == PlayerTurnPhase.Attack) // show attack panel in here
            {
				gm.selectionManager.CheckAttackPhaseInput();
            }
			else if(currPlayerTurnPhase == PlayerTurnPhase.EndTurn)
			{
				// hide the shadow mesh thing here plz //

				gm.cameraManager.LockCamera(gm.selectionManager.GetCurrentSelectedCharacter());
				gm.uiManager.getActionPanel().RemoveActionMenuButtons();
				Movement moveComp = gm.selectionManager.GetCurrentSelectedCharacter().GetComponent<Movement>();
				List<Cube> cubes = gm.cubeManager.BFS(gm.selectionManager.GetCurrentSelectedCharacter().GetCurrentCube(), (int)gm.selectionManager.GetCurrentSelectedCharacter().GetMovement(), gm.selectionManager.GetCurrentSelectedCharacter().isPlayable, gm.selectionManager.GetCurrentSelectedCharacter().GetFutureCube());
                if(gm.selectionManager.GetCurrentSelectedCharacter().GetCurrentCube() == gm.selectionManager.GetCurrentSelectedCharacter().GetFutureCube())
                {
                    gm.selectionManager.footprintSprite.gameObject.SetActive(false);
                    DisableCharacter();
                    currPlayerTurnPhase = PlayerTurnPhase.SelectPlayer;
                    return;
                }
				if (moveComp.MoveOnCubePath(cubes))
                {
                    gm.selectionManager.footprintSprite.gameObject.SetActive(false);
                    DisableCharacter();
					currPlayerTurnPhase = PlayerTurnPhase.SelectPlayer;
				}
			}
            #endregion
        }
        else if(currTurnPhase == TurnPhase.Enemy)
        {
            #region EnemyTurn
            gm.selectionManager.DisableHoverAndSelectionSprites();
            if (currPlayerTurnPhase == PlayerTurnPhase.SwitchingRounds)
			{
                gm.cameraManager.canInput = false;
				// Show all enabled materials for playable characters before starting enemy turn.
				foreach (WorldCharacter character in charactersWhoFinishedTurn)
                {
                    character.ShowEnabled();
                }
				this.enemyCharacters.Clear();

				foreach(WorldCharacter w in this.gm.enemyCharacters)
                {
                    if (w != null && w.gameObject.activeSelf)
					{
						w.ShowEnabled();
						this.enemyCharacters.Enqueue(w);
						gm.lumiusManager.ResetBuffs(w);
					}
				}
				currPlayerTurnPhase = PlayerTurnPhase.SelectPlayer;
				if(!PlayablesLeft() || !EnemiesLeft()) { return; }
				StartCoroutine(gm.audioManager.TransitionGameMusic(gm.audioManager.gameMusicSource, Constants.instance.enemyMusic, 2.0f, 0.5f, .15f));
				fade(1, 0.5f, enemyTurnTexture);
				Wait(2.0f);
			}
			else if(currPlayerTurnPhase == PlayerTurnPhase.SelectPlayer)
			{
				if (!PlayablesLeft())
				{
					gm.GameOver();
					currPlayerTurnPhase = PlayerTurnPhase.GameOver;
					return;
				}
				else if (!EnemiesLeft())
				{
					gm.Victory();
					currPlayerTurnPhase = PlayerTurnPhase.GameOver;
					return;
				}
				fade(-1, 0.5f, enemyTurnTexture);
                if (this.enemyCharacters.Count <= 0)
				{
					BeginPlayerTurn();
					Wait(1.0f);
					return;
				}
				WorldCharacter currEnemy = this.enemyCharacters.Dequeue();
				gm.selectionManager.ClearCubesInShortestPath();
				gm.aiManager.SetCurrentSelectedCharacter(currEnemy);
				currPlayerTurnPhase = PlayerTurnPhase.Move;
				if(gm.aiManager.WeShallMove || gm.aiManager.WeShallAttack)
				{
					gm.cameraManager.LockCamera(currEnemy);
					if(gm.aiManager.WeShallMove)
					{
						gm.aiManager.ShowMovePath();
					}
					if(gm.aiManager.WeShallAttack)
					{
						gm.aiManager.ShowAttackTarget();
					}
					Wait(0.5f);
				}
			}
			else if(currPlayerTurnPhase == PlayerTurnPhase.Move)
			{
				if (gm.aiManager.WeShallMove)
				{
					bool doneMoving = gm.aiManager.AIMoveAlongPath();
					if (doneMoving)
					{
						gm.aiManager.HideMovePath();
						currPlayerTurnPhase = PlayerTurnPhase.Attack;
						Wait(0.1f);
					}
				}
				else
                {
					currPlayerTurnPhase = PlayerTurnPhase.Attack;
				}
			}
			else if(currPlayerTurnPhase == PlayerTurnPhase.Attack)
            {
				if(gm.aiManager.WeShallAttack)
				{
					gm.aiManager.HideAttackTarget();
					gm.aiManager.Act();
				}
				currPlayerTurnPhase = PlayerTurnPhase.SelectPlayer;
			}
            #endregion
        }
    }

    public void MoveCurrCharacterBackToStartCube()
    {
        //Make character move back to original cube position at start of turn.
        WorldCharacter currCharacter = gm.selectionManager.GetCurrentSelectedCharacter();
        if(currCharacter != null)
        {
            Movement moveComp = currCharacter.GetComponent<Movement>();
            if (moveComp != null)
            {
                moveComp.SnapToCube(moveComp.GetPreviousCube());
				currCharacter.ShowRangeCubes(gm.cubeManager, gm.selectionManager);
                SetPlayerTurnPhase(PlayerTurnPhase.Move);
            }
        }
    }

	public void DisableCharacter()
	{
		// Add the character that just finished to the list of characters who finished their turn. 
		WorldCharacter characterWhoFinishedTurn = gm.selectionManager.GetCurrentSelectedCharacter();
		if (characterWhoFinishedTurn != null && !charactersWhoFinishedTurn.Contains(characterWhoFinishedTurn))
		{
			characterWhoFinishedTurn.Reset();
			// Set material of character to disabled material.
			characterWhoFinishedTurn.ShowDisabled();
            gm.selectionManager.ClearCubesInShortestPath();
			//gm.selectionManager.ClearInRangeMoveCubes();
			characterWhoFinishedTurn.HideRangeCubes(gm.cubeManager, gm.selectionManager);
            charactersWhoFinishedTurn.Add(characterWhoFinishedTurn);
            gm.selectionManager.SetCurrentSelectedCharacter(null);
            gm.selectionManager.SetCurrentSelectedCube(null);
        }
		ComputeAllRangeCubes(gm.playableCharacters);
		ComputeAllRangeCubes(gm.enemyCharacters);
	}

	private void ComputeAllRangeCubes(List<WorldCharacter> characters)
	{
		foreach (WorldCharacter w in characters)
		{
			if (w != null && w.bIsAlive)
			{
				w.ComputeRangeCubes(gm.cubeManager);
			}
		}
	}

	public void BeginTurn(GameManager gm, float waitTime)
    {
        this.gm = gm;
        BeginPlayerTurn();
		WorldCharacter first = gm.playableCharacters[0];
		if(first)
		{
			gm.cameraManager.LockCamera(first);
		}
		Wait(waitTime);
    }

    public void BeginPlayerTurn()
    {
		currTurnPhase = TurnPhase.Player;
		currPlayerTurnPhase = PlayerTurnPhase.SwitchingRounds;
    }

    public void BeginEnemyTurn()
    {
		currTurnPhase = TurnPhase.Enemy;
		currPlayerTurnPhase = PlayerTurnPhase.SwitchingRounds;
    }

    // Called only when in select player state and will end player's turn even if not all playable characters have moved.
    public void SkipPlayerTurn()
    {
        if (currPlayerTurnPhase != PlayerTurnPhase.SelectPlayer ||
            currTurnPhase != TurnPhase.Player)
        {
            return;
        }
        BeginEnemyTurn();
    }

    // Gets the next playable character who has not finished their turn!
    public void CycleToNextPlayableCharacter()
    {
        // Only allow this to happen if the player is in move or player select state
        if(currPlayerTurnPhase != PlayerTurnPhase.SelectPlayer && 
            currPlayerTurnPhase != PlayerTurnPhase.Move &&
            currTurnPhase != TurnPhase.Player)
        {
            return;
        }

        WorldCharacter character = null;
        // If there are none left then move on to next player or go to enemy's turn.
        List<WorldCharacter> playableCharacters = gm.playableCharacters;
        WorldCharacter currSelectedCharacter = gm.selectionManager.GetCurrentSelectedCharacter();

        // Get index of current selected character if there is one
        int currCharacterIndex = 0;
        if(currSelectedCharacter != null)
        {
            currCharacterIndex = playableCharacters.IndexOf(currSelectedCharacter);
        }

        // Get next available character from playable list.
        int upperBound = currCharacterIndex + playableCharacters.Count;
        int modIndex = 0;
        for (int i = currCharacterIndex; i < upperBound; i++)
        {
            // force modIndex to loop around to 0 while i continues.
            modIndex = i % (playableCharacters.Count);
            WorldCharacter playableCharacter = playableCharacters[modIndex];
            if (!charactersWhoFinishedTurn.Contains(playableCharacter) && 
                playableCharacter.gameObject.activeSelf && 
                playableCharacter != gm.selectionManager.GetCurrentSelectedCharacter())
            {
                character = playableCharacter;
                gm.uiManager.getCharacterPanel().Populate(playableCharacter);
                break;
            }
        }

        // If character obtained not null then set this character to current selected.
        if(character != null)
        {
            // Hide all in range cubes of current selected character
            if (currSelectedCharacter != null)
            {
                MoveCurrCharacterBackToStartCube();
                currSelectedCharacter.HideRangeCubes(gm.cubeManager, gm.selectionManager);
            }
            // Set new selected character that we cycled to
            gm.selectionManager.SetCurrentSelectedCharacter(character);
            currPlayerTurnPhase = PlayerTurnPhase.SelectPlayer;
        }


        // THIS LINE BELOW IS JANK BEYOND BELIEF
        // Reason for it is because ready to move along shortest path is set to true because we jump straight 
        // to move phase through select player phase in this function call. If we didnt set this then when we hover over cube
        // We would move straight to that cube without clicking. Must be fixed in future. Dam jank.
        gm.selectionManager.readyToMoveAlongShortestPath = false;
    }

	public TurnPhase getCurrentTurnPhase()
	{
		return currTurnPhase;
	}

    public PlayerTurnPhase getPlayerTurnPhase()
    {
        return currPlayerTurnPhase;
    }

    public void SetPlayerTurnPhase(PlayerTurnPhase phase)
    {
        currPlayerTurnPhase = phase;
    }

    public List<WorldCharacter> getCharactersWhoFinishedTurn()
    {
        return charactersWhoFinishedTurn;
    }

    public void Wait(float timeToWait)
	{
		waiting = true;
		timer = timeToWait;
	}

	void OnGUI()
	{
		if(fadeOutTexture == null)
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

	public bool PlayablesLeft()
	{
		bool output = false;
		foreach (WorldCharacter e in gm.playableCharacters)
		{
			output = output || e.bIsAlive;
		}
		return output;
	}

	public bool EnemiesLeft()
	{
		bool output = false;
		foreach(WorldCharacter e in gm.enemyCharacters)
		{
			if(e != null)
			{
				output = output || e.bIsAlive;
			}
		}
		return output;
	}
}
