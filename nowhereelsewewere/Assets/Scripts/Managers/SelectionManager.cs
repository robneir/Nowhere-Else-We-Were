using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{

	//reference to the game manager
	[SerializeField]
	private GameManager gm;

	// Layer that colliders that raycast can hit are on
	[SerializeField]
	private LayerMask raycastLayerMask;

    // All the cubes that are in range and able to move to.
    private List<Cube> cubesInMoveRange = new List<Cube>();

    // All the cubes that are in the current shortest path being displayed to the player.
    private List<Cube> cubesInShortestPath = new List<Cube>();

    // All the cubes that are in range and able to move to.
    private List<Cube> cubesInAttackRange = new List<Cube>();

    [HideInInspector]
    public bool readyToMoveAlongShortestPath = false;

    // Cube that was last hovered on in CheckForHover().
    private Cube lastHoveredCube;

    // Cube that was last clicked on
    private Cube lastClickedCube;

    // This cube is important and should always point to current cube selected. 
    // The camera follows this cube and will look at it.
    private Cube currentCubeSelected;

    // Character standing on current cube if there is one.
    private WorldCharacter currentCharacterSelected;

	private WorldCharacter previousCharacterHovered = null;

    [Tooltip("The sprite put over a cube that is currently highlighted or selected. The color of this sprite changes whether selection or highlight.")]
    public SpriteRenderer hoverSelectSprite;
    private SpriteRenderer hoverSprite;
    private SpriteRenderer selectionSprite;

    [SerializeField]
    [Tooltip("The offset of the highlighted cube sprite over the highlighted cube.")]
    private Vector3 hoverSelectSpriteOffset = new Vector3(0, 1.8f, 0.0f);

    [SerializeField]
    [Tooltip("The sound played when player reverses their action.")]
    private AudioClip reverseActionSound;
    [SerializeField]
    [Tooltip("The sound played when player selects cube to possibly move to.")]
    private AudioClip selectCubeForMoveSound;

	public SpriteRenderer footprintSprite;

    void Start()
    {
        lastHoveredCube = null;
        lastClickedCube = null;

        // Instantiate the highlight and selction sprite.
        hoverSprite = (SpriteRenderer)Instantiate(hoverSelectSprite, transform.root, false);
        hoverSprite.color = Constants.instance.hoverSpriteColor;
        selectionSprite = (SpriteRenderer)Instantiate(hoverSelectSprite, transform.root, false);
        selectionSprite.color = Constants.instance.selectionSpriteColor;
    }

	public void SetCubesInMoveRange(List<Cube> canMove)
	{
		cubesInMoveRange = new List<Cube>();
		foreach(Cube c in canMove)
		{
			cubesInMoveRange.Add(c);
		}
	}

	public void SetCubesInAttackRange(List<Cube> canAttack)
	{
		cubesInAttackRange = new List<Cube>();
		foreach(Cube c in canAttack)
		{
			cubesInAttackRange.Add(c);
		}
	}

    // Checks to see if user presses left click to select cube or player...
    public void CheckForNewSelection()
    {
        // If the cursor is not visible then return.
        if (!Cursor.visible)
        {
            return;
        }

        // If the player left clicks then raycast to try to select cube.
        if (Input.GetButtonUp("Fire1"))
        {
            // Check to see if click hit a UI component.
            bool clickedUI = gm.uiManager.CheckUIContainsPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            if (!clickedUI)
            {
                GameObject objHit = GetObjectWithRaycast();
                if (objHit != null)
                {
                    Cube cubeHit = objHit.GetComponentInParent<Cube>();
                    // Check to see if we hit a character with raycast if so select the cube.
                    if (objHit.transform.parent.parent.gameObject.tag == "Character")
                    {
                        Movement movement = objHit.transform.parent.GetComponentInParent<Movement>();
                        if (movement != null)
                        {
                            cubeHit = movement.GetCurrentCube();
                        }
                    }

                    // Check to make sure that the cube is not null.
                    if (cubeHit != null)
                    {
                        // Check to see if cube hit has a playable character on it. If so then select it and go to move phase.
                        GameObject occupant = cubeHit.GetOccupant();
                        if (occupant != null)
                        {
                            // Check to see if character is a playable character.
                            WorldCharacter character = occupant.GetComponent<WorldCharacter>();
                            if (character)
                            {
                                List<WorldCharacter> playableCharacters = gm.playableCharacters;
								List<WorldCharacter> enemyCharacters = gm.enemyCharacters;
                                if (playableCharacters.Contains(character) && character.bIsAlive)
								{
									if (!gm.turnManager.getCharactersWhoFinishedTurn().Contains(character))
									{
										SetCurrentSelectedCharacter(character);
										ClearCubesInShortestPath();
										gm.turnManager.SetPlayerTurnPhase(TurnManager.PlayerTurnPhase.Move);
										gm.audioManager.Play2DSound(Constants.instance.UIVintageHardClick, 0.6f, false);
									}
									else
									{
										// clicked on a character that already moved
										gm.audioManager.Play2DSound(Constants.instance.ErrorSound, 0.6f, false);
									}
								}
								else if(enemyCharacters.Contains(character) && character.bIsAlive)
								{
									//clicked on an enemy character
									gm.audioManager.Play2DSound(Constants.instance.ErrorSound, 0.6f, false);
                                }
                                
                                if(gm.uiManager.getCharacterPanel().isInLevel)
                                {
                                    // Populate the UI showing the information about the character selected even if it isn't a playable character.
                                    gm.uiManager.getCharacterPanel().Populate(character);
                                }
                            }
                        }
                        else
                        {
                            gm.audioManager.Play2DSound(Constants.instance.UISoftPlasticClick, 0.45f, false);
                        }
                    }
                }
            }
        }
    }

	public Cube GetCurrentHoveredCube()
	{
		return lastHoveredCube;
	}

    public void CheckForHover()
    {
        // If the cursor is not visible then return.
        if(!Cursor.visible)
        {
            return;
        }

        // activate highlight sprite.
        if(hoverSprite.gameObject.activeSelf == false)
        {
            hoverSprite.gameObject.SetActive(true);
        }
        GameObject go = GetObjectWithRaycast();
        if(go != null)
        {
            GameObject parent = go.transform.parent.gameObject;
            Cube cubeHit = parent.GetComponent<Cube>();
            if (cubeHit != null && cubeHit != lastHoveredCube)
            {
                lastHoveredCube = cubeHit;
				gm.uiManager.getCubePanel().Populate(lastHoveredCube);
                // Parent the highlight sprite to this cube.
                hoverSprite.transform.parent = cubeHit.transform;
                hoverSprite.transform.localPosition = Vector3.zero;
                hoverSprite.transform.position += cubeHit.GetCubeOffset() + hoverSelectSpriteOffset;
				if(gm.turnManager.getPlayerTurnPhase() == TurnManager.PlayerTurnPhase.SelectPlayer)
				{
					if (previousCharacterHovered != null)
					{
						previousCharacterHovered.HideRangeCubes(gm.cubeManager, this);
					}

					GameObject occupant = cubeHit.GetOccupant();
					if (occupant != null)
					{
						WorldCharacter w = occupant.GetComponent<WorldCharacter>();
						if (w)
						{
							w.ShowRangeCubes(gm.cubeManager, this);
							previousCharacterHovered = w;
						}
					}
				}
            }
        }
    }

    public void DisableHoverAndSelectionSprites()
    {
        if(hoverSprite.gameObject.activeSelf == true)
        {
            hoverSprite.gameObject.SetActive(false);
        }
        if (selectionSprite.gameObject.activeSelf == true)
        {
            selectionSprite.gameObject.SetActive(false);
        }
    }

    // Important because we need to set this in order for camera to know where to look.
    public void SetCurrentSelectedCube(Cube cube)
    {
        // Deselect previous selected cube if there is one.
        if(currentCubeSelected != null)
        {
            currentCubeSelected.SetSelectedState(Cube.SelectionState.NotSelected);
        }
        // Select and show selection of newly selected cube.
        currentCubeSelected = cube;
    }
    public Cube GetCurrentSelectedCube()
    {
        return currentCubeSelected;
    }

    // Sets the current selected character and will also set current cube to be the cube character is on.
    // This is so the camera will move to that cube.
    public WorldCharacter SetCurrentSelectedCharacter(WorldCharacter character)
    {
        currentCharacterSelected = character;
        if(currentCharacterSelected != null)
        {
            // Set the current cube to the cube the character is standing on.
            Movement moveComp = currentCharacterSelected.GetComponent<Movement>();
            if (moveComp != null)
            {
                Cube newCurrCube = moveComp.GetCurrentCube();
                SetCurrentSelectedCube(newCurrCube);
                // Enable the selection sprite 
                selectionSprite.gameObject.SetActive(true);
                // Parent the selectionSprite to this cube.
                selectionSprite.transform.parent = newCurrCube.transform;
                selectionSprite.transform.localPosition = Vector3.zero;
                selectionSprite.transform.position += newCurrCube.GetCubeOffset() + hoverSelectSpriteOffset;
            }
            // Populate inventory panel
            Inventory inventory = currentCharacterSelected.GetComponent<Inventory>();
            if(inventory  != null)
            {
                gm.uiManager.getInventoryPanel().Populate(inventory);
            }
        }
        else
        {
            // Hide selection if no character selected.
            selectionSprite.gameObject.SetActive(false);
            // Depopulate the character panel information since we are no longer selecting a character.
            gm.uiManager.getCharacterPanel().gameObject.SetActive(false);
            gm.uiManager.getInventoryPanel().gameObject.SetActive(false);
        }
        return currentCharacterSelected;
    }
    public WorldCharacter GetCurrentSelectedCharacter()
    {
        return currentCharacterSelected;
    }

    public void ShowCubesInShortestPath(Cube.SelectionState selectionState = Cube.SelectionState.InPath)
    {
        //Check to see if the occupant is a character
        if (currentCharacterSelected != null)
        {
            // Show range that occupant can move
            CubeManager.instance.SetSelectionStateOfCubes(cubesInShortestPath, selectionState);
        }
        else
        {
            Debug.Log("Cannot show cubes in path because character occupying the cube has empty Character component or movement script.");
        }
    }
    public void ShowInRangeAttackCubes(Cube.SelectionState selectionState = Cube.SelectionState.InAttackRange)
    {
        //Check to see if the occupant is a character
        if (currentCharacterSelected != null)
        {
            Movement moveComp = currentCharacterSelected.GetComponent<Movement>();
            if (moveComp != null)
            {
                // Get the range of the weapon the character is holding.
                Weapon weapon = currentCharacterSelected.getInventory().getEquippedWeapon();
                if (weapon != null)
                {
                    uint[] weaponrange = weapon.GetRange();
                    cubesInAttackRange = CubeManager.instance.GetAllAdjacentCubesInWeaponRange(currentCharacterSelected.GetFutureCube(), weaponrange, weapon.Group);
					foreach(Cube c in cubesInAttackRange)
					{
						if(c.IsMovable())
						{
							c.SetSelectedState(Cube.SelectionState.InAttackRange);
						}
					}
                }
                else
                {
                    Debug.Log("Weapon is null so cannot show the attack range of current selected character.");
                }
            }
            else
            {
                Debug.Log("No move component on character.");
            }
        }
        else
        {
            Debug.Log("Cannot show cubes in attack range because character occupying the cube has empty Character component.");
        }
    }

    public List<Cube> GetCubesInMoveRange()
    {
        return cubesInMoveRange;
    }
    public List<Cube> GetCubesInAttackRange()
    {
        return cubesInAttackRange;
    }
    public List<Cube> GetCubesInShortestPath()
    {
        return cubesInShortestPath;
    }

    // Set the selection state of all previous cubes in range to not selected and clear the cubes in range.
    public void ClearInRangeMoveCubes()
    {
        //CubeManager.instance.SetSelectionStateOfCubes(cubesInMoveRange, Cube.SelectionState.NotSelected);
        cubesInMoveRange.Clear();
    }
    public void ClearCubesInShortestPath()
    {
		if(cubesInShortestPath == null)
		{
			cubesInShortestPath = new List<Cube>();
		}
        CubeManager.instance.SetSelectionStateOfCubes(cubesInShortestPath, Cube.SelectionState.NotSelected);
        cubesInShortestPath.Clear();
    }
    public void ClearInRangeAttackCubes()
    {
		if (cubesInAttackRange == null)
		{
			cubesInAttackRange = new List<Cube>();
		}
		CubeManager.instance.SetSelectionStateOfCubes(cubesInAttackRange, Cube.SelectionState.NotSelected);
        cubesInAttackRange.Clear();
    }
    // Checks for specific input related to player trying to move.
    // Will return a bool that tells the TurnManager.cs when this moving phase is done.
    public bool CheckMovingPhaseInput()
    {
        // Make sure we only check input if the left mouse button is visible. else we are probably dragging mouse.
        if(!Cursor.visible)
        {
            return false;
        }

        if (currentCharacterSelected != null)
        {
            // Get current Player movement script so we can move player around.
            Movement moveComp = currentCharacterSelected.GetComponent<Movement>();
            if (moveComp != null)
            {
                Movement.MoveState currMoveState = moveComp.GetMoveState();
                if (currMoveState == Movement.MoveState.Idle)
                {
                    // Released left  click so move along shortest path.
                    if (Input.GetButtonUp("Fire1"))
                    {
                        // Check to make sure that the lastRaycastedCube is not null becuase if it was then dont set readyToMoveAlongShortestPath to true.
                        if (currentCharacterSelected != null && lastHoveredCube != null && cubesInMoveRange.Contains(lastHoveredCube))
                        {
							if (lastHoveredCube.GetOccupant() == null || lastHoveredCube.GetOccupant() != null && lastHoveredCube.GetOccupant().GetComponent<WorldCharacter>() == currentCharacterSelected)
							{
								if (moveComp != null)
								{
									// Set to true so that we know we can move along path now. Solved issue with holding right click which immediately made character move.
									readyToMoveAlongShortestPath = true;
									// Hide the action panel until you get to the destination cube.
									gm.uiManager.getActionPanel().RemoveActionMenuButtons();
									// This sets the last raycasted cube to null so that we can raycast in the attack turn.
									lastHoveredCube = null;
                                    // Play select possible cube to move to noise.
                                    gm.audioManager.Play2DSound(selectCubeForMoveSound, 0.6f, false);
                                }
							}
							else
							{
								// clicked on a character when trying to move
								gm.audioManager.Play2DSound(Constants.instance.ErrorSound, 0.6f, false);
							}
                        }
                    }
                    else if(Input.GetButtonDown("Fire2")) // Check to see if player wants to cancel the current shortest path.
                    {
                        // Check to see if we should move back to select player phase
                        ClearCubesInShortestPath();
						//ClearInRangeMoveCubes();
						GetCurrentSelectedCharacter().HideRangeCubes(gm.cubeManager, gm.selectionManager);
                        SetCurrentSelectedCharacter(null);
                        SetCurrentSelectedCube(null);
                        gm.turnManager.SetPlayerTurnPhase(TurnManager.PlayerTurnPhase.SelectPlayer);

                        // Play reverse action noise.
                        gm.audioManager.Play2DSound(reverseActionSound, 1.0f, false);

						//METRICS
						if (MetricManager.instance)
						{
							MetricManager.instance.AddNumTimesWentBack(1);
						}
						return false;
                    }
                    else if(Input.GetButtonUp("Fire1"))
                    {
                        // Reset this ready bool so that we don't move immediately to cube if we cancel the previous shortest path.
                        readyToMoveAlongShortestPath = false;
                    }

                    // Show shortest path to hovered cube if it is not null.
                    if(lastHoveredCube != null && cubesInMoveRange.Contains(lastHoveredCube))
                    {
                        CubeManager.instance.SetSelectionStateOfCubes(cubesInShortestPath, Cube.SelectionState.InMoveRange);
                        cubesInShortestPath = CubeManager.instance.BFS(currentCubeSelected, (int)GetCurrentSelectedCharacter().GetMovement(), GetCurrentSelectedCharacter().isPlayable, lastHoveredCube);
                        if(currentCubeSelected == lastHoveredCube)
                        {
                            cubesInShortestPath.Add(currentCubeSelected);
                        }
                        // Change selection state of cubes in path
                        CubeManager.instance.SetSelectionStateOfCubes(cubesInShortestPath, Cube.SelectionState.InPath);
                        // Set the selection state of current cube character is on to in path.
                        GetCurrentSelectedCharacter().GetCurrentCube().SetSelectedState(Cube.SelectionState.InPath);
                    }
                    else
					{
						// Clear all cubes in past shortest path.
						if (cubesInShortestPath != null)
						{
							CubeManager.instance.SetSelectionStateOfCubes(cubesInShortestPath, Cube.SelectionState.InMoveRange);
							cubesInShortestPath.Clear();
						}
                    }
                }

                // return true if movement is done.
                if (cubesInShortestPath != null && cubesInShortestPath.Count > 0 && readyToMoveAlongShortestPath)
                {
					WorldCharacter mover = moveComp.GetComponent<WorldCharacter>();
					if(mover)
					{
						mover.HideRangeCubes(gm.cubeManager, this);
						gm.cubeManager.SetSelectionStateOfCubes(cubesInShortestPath, Cube.SelectionState.InPath);
						readyToMoveAlongShortestPath = false;
						gm.uiManager.getActionPanel().Populate();
						currentCharacterSelected.SetFutureCube(lastHoveredCube);
						footprintSprite.transform.position = lastHoveredCube.transform.position + lastHoveredCube.GetCubeOffset() + new Vector3(0.0f, 2.0f, 0.0f);
						/*
						Vector3 newDirection = lastHoveredCube.transform.position - cubesInShortestPath[cubesInShortestPath.Count - 2].transform.position;
						newDirection.y = 0.0f;
						newDirection.Normalize();
						float angle = Mathf.Acos(Vector3.Dot(Vector3.right, newDirection));
						Vector3 cross = Vector3.Cross(Vector3.right, newDirection);
						cross.Normalize();
						angle *= cross.y;
						footprintSprite.transform.eulerAngles = new Vector3(footprintSprite.transform.eulerAngles.x, footprintSprite.transform.eulerAngles.y, angle);
						*/
						footprintSprite.gameObject.SetActive(true);
                        return true;
					}
					/*
                    if (moveComp.MoveOnCubePath(cubesInShortestPath))
                    {
                        readyToMoveAlongShortestPath = false;
                        // Show the player's actions panel
                        gm.uiManager.getActionPanel().Populate();
                        return true;
                    }
					*/
                }
            }
        }
        return false;
    }
    public void CheckPreAttackPhaseInput()
    {
        if (Input.GetButtonUp("Fire1")) // Released right click for attacking
        {
            Cube cubeCharacterIsAttacking = lastClickedCube;
            // Go through null checks to see if the cube we are attacking has an enemy on it.
            if (cubeCharacterIsAttacking != null)
            {
                GameObject occupant = cubeCharacterIsAttacking.GetOccupant();
                if (occupant != null)
                {
                    WorldCharacter enemyCharacter = occupant.GetComponent<WorldCharacter>();
                    if (enemyCharacter != null)
                    {
                        //check for now that the 
                        if (gm.enemyCharacters.Contains(enemyCharacter))
                        {
                            // Then ATTACK!!!
                            gm.audioManager.Play2DSound(selectCubeForMoveSound, .8f, false);

                            gm.uiManager.getActionPanel().RemoveActionMenuButtons();

                            gm.battleManager.SetCombatants(currentCharacterSelected, enemyCharacter, BattleManager.whoAttacks.player);
                            gm.battleManager.ShowBattleParameters();
                            gm.turnManager.SetPlayerTurnPhase(TurnManager.PlayerTurnPhase.Attack);
                        }
                    }
                }
                else
                {
                    gm.audioManager.Play2DSound(Constants.instance.ErrorSound, .5f, false);
                }
            }
            // Reset the sprite to just be in attack range.
            if(cubeCharacterIsAttacking != null)
            {
                cubeCharacterIsAttacking.SetSelectedState(Cube.SelectionState.InAttackRange);
            }
            lastClickedCube = null; // This sets the last raycasted cube to null so that we can raycast in the move turn when finding shortest path (in CheckPlayerMoveInput())
        }
        else if (Input.GetButton("Fire1")) // Check to see if player presses left click which means they are trying to attack
        {
            GameObject objHit = GetObjectWithRaycast();
            if (objHit != null)
            {
                Cube cubeToAttack = objHit.GetComponentInParent<Cube>();
                bool cubeIsValid = (cubeToAttack != null) && (cubeToAttack != lastClickedCube) && (cubeToAttack.IsMovable()) && cubesInAttackRange.Contains(cubeToAttack);
                // Make sure we are not allowing pathfinding computation if the cube that got hit by raycast was the last raycasted cube
                if (cubeIsValid)
                {
                    if (lastClickedCube != null)
                    {
                        lastClickedCube.SetSelectedState(Cube.SelectionState.InAttackRange);
                    }
                    lastClickedCube = cubeToAttack;
                    // Change selection state of cubes in path
                    cubeToAttack.SetSelectedState(Cube.SelectionState.Attacking);
                }
            }
        }
        else if(Input.GetButtonDown("Fire2"))
        {
            foreach(Cube c in cubesInAttackRange)
            {
                if(cubesInShortestPath.Contains(c))
                {
                    c.SetSelectedState(Cube.SelectionState.InPath);
                }
                else if (cubesInMoveRange.Contains(c))
                {
                    c.SetSelectedState(Cube.SelectionState.InMoveRange);
                }
                else
                {
                    c.SetSelectedState(Cube.SelectionState.NotSelected);
                }
			}

            // Play reverse action noise.
            gm.audioManager.Play2DSound(reverseActionSound, 1.0f, false);

            //METRICS
            if (MetricManager.instance)
			{
				MetricManager.instance.AddNumTimesWentBack(1);
			}

			cubesInAttackRange.Clear();
            gm.turnManager.SetPlayerTurnPhase(TurnManager.PlayerTurnPhase.ChoosePhase);
            gm.uiManager.getActionPanel().Populate();
        }
    }
    public void CheckAttackPhaseInput()
    {
        // Tile begin combat to attack button
        if (gm.turnManager.beginCombat == true)
		{
			// hide the shadow mesh thing here plz //

			footprintSprite.gameObject.SetActive(false);
			gm.uiManager.getAttackPanel().gameObject.SetActive(false);
			gm.cameraManager.LockCamera(gm.selectionManager.GetCurrentSelectedCharacter());
			Movement moveComp = GetCurrentSelectedCharacter().GetComponent<Movement>();
			List<Cube> cubes = gm.cubeManager.BFS(GetCurrentSelectedCharacter().GetCurrentCube(), (int)GetCurrentSelectedCharacter().GetMovement(), GetCurrentSelectedCharacter().isPlayable, GetCurrentSelectedCharacter().GetFutureCube());
			ClearInRangeAttackCubes();
			gm.cubeManager.SetSelectionStateOfCubes(cubes, Cube.SelectionState.InPath);
			gm.battleManager.SetEnemyBeingAttacked(true);

            if(GetCurrentSelectedCharacter().GetCurrentCube() == GetCurrentSelectedCharacter().GetFutureCube())
            {
                gm.battleManager.SetEnemyBeingAttacked(false);
                GetCurrentSelectedCharacter().GetFutureCube().SetSelectedState(Cube.SelectionState.NotSelected);
                gm.battleManager.BeginCombat();
                gm.turnManager.beginCombat = false;
                gm.turnManager.SetPlayerTurnPhase(TurnManager.PlayerTurnPhase.SelectPlayer);
                return;
            }

			if (!moveComp.MoveOnCubePath(cubes))
			{
				return;
			}
			gm.battleManager.SetEnemyBeingAttacked(false);
			GetCurrentSelectedCharacter().GetFutureCube().SetSelectedState(Cube.SelectionState.NotSelected);
			gm.battleManager.BeginCombat();
            gm.turnManager.beginCombat = false;
            gm.turnManager.SetPlayerTurnPhase(TurnManager.PlayerTurnPhase.SelectPlayer);
        }
        // Check to see if we want to go back to the previous phase.
        if (Input.GetButtonDown("Fire2"))
        {
            // Play reverse action noise.
            gm.audioManager.Play2DSound(reverseActionSound, 1.0f, false);

            //METRICS
            if (MetricManager.instance)
			{
				MetricManager.instance.AddNumTimesWentBack(1);
			}

			gm.uiManager.getAttackPanel().gameObject.SetActive(false);
            gm.turnManager.SetPlayerTurnPhase(TurnManager.PlayerTurnPhase.PreAttack);
        }
    }
    public void FireBeginCombatEvent()
    {
        // This allows CheckAttackPhaseInput() function right above to continue when attack button pressed.
        gm.turnManager.beginCombat = true;
    }
    public void CheckChoosePhaseInput()
    {
        // Check to see if we want to go back to the previous phase.
        if (Input.GetButtonDown("Fire2") && (gm.turnManager.getPlayerTurnPhase() == TurnManager.PlayerTurnPhase.Move || gm.turnManager.getPlayerTurnPhase() == TurnManager.PlayerTurnPhase.ChoosePhase))
        {
            // Play reverse action noise.
            gm.audioManager.Play2DSound(reverseActionSound, 1.0f, false);

            //METRICS
            if (MetricManager.instance)
			{
				MetricManager.instance.AddNumTimesWentBack(1);
			}

			footprintSprite.gameObject.SetActive(false);
			readyToMoveAlongShortestPath = false;
            gm.turnManager.SetPlayerTurnPhase(TurnManager.PlayerTurnPhase.MoveBack);
        }
    }

    public GameObject GetObjectWithRaycast()
    {
        GameObject objectHit = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Did raycast hit something?
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayerMask))
        {
            // Check to make sure the raycast is not casting through any UI components.
            if(!gm.uiManager.CheckUIContainsPoint(Input.mousePosition))
            {
                objectHit = hit.collider.gameObject;
            }
        }
        return objectHit;
    }
}
