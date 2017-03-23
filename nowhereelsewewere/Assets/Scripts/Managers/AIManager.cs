using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIManager : MonoBehaviour {

	public GameManager gameManager;

	private WorldCharacter currCharacter;

	public bool WeShallMove = false;
	private List<Cube> MovePath = new List<Cube>();
	public bool WeShallAttack = false;
	private WorldCharacter AttackTarget = null;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public WorldCharacter GetCurrentSelectedCharacter()
	{
		return currCharacter;
	}

	public void SetCurrentSelectedCharacter(WorldCharacter w)
	{
		currCharacter = w;
		WeShallMove = false;
		MovePath.Clear();
		WeShallAttack = false;
		AttackTarget = null;

		AIBehaviour ai = currCharacter.GetComponent<AIBehaviour>();
		if(ai)
		{
			AIBehaviour.Actions actions = ai.GetActions(gameManager);
			while(actions.ActionList.Count != 0)
			{
				AIBehaviour.Actions.Action a = actions.ActionList.Dequeue();
				if(a == AIBehaviour.Actions.Action.Move)
				{
					List<Cube> path = actions.MovePath;
					if((path != null) && (path.Count != 0))
					{
						MovePath = path;
						WeShallMove = true;
					}
				}
				else if(a == AIBehaviour.Actions.Action.Attack)
				{
					WorldCharacter target = actions.attackTarget;
					if(target)
					{
						AttackTarget = target;
						WeShallAttack = true;
					}
				}
			}
		}
	}

	public bool AIMoveAlongPath()
	{
		return currCharacter.GetComponent<Movement>().MoveOnCubePath(MovePath);
	}

	public void HideMovePath()
	{
		gameManager.cubeManager.SetSelectionStateOfCubes(MovePath, Cube.SelectionState.NotSelected);
	}

	public void Act()
	{
		currCharacter.Face(AttackTarget);
		AttackTarget.Face(currCharacter);
		gameManager.battleManager.SetCombatants(AttackTarget, currCharacter, BattleManager.whoAttacks.enemy);
		gameManager.battleManager.BeginCombat();
	}

	public void ShowMovePath()
	{
		gameManager.cubeManager.SetSelectionStateOfCubes(MovePath, Cube.SelectionState.InPath);
	}

	public void ShowAttackTarget()
	{
		List<Cube> target = new List<Cube>();
		target.Add(AttackTarget.GetCurrentCube());
		gameManager.cubeManager.SetSelectionStateOfCubes(target, Cube.SelectionState.Attacking);
	}

	public void HideAttackTarget()
	{
		List<Cube> target = new List<Cube>();
		target.Add(AttackTarget.GetCurrentCube());
		gameManager.cubeManager.SetSelectionStateOfCubes(target, Cube.SelectionState.NotSelected);
	}
}
