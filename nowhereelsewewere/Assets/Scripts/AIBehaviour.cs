using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AIBehaviour : MonoBehaviour {
	public enum Behaviour { MoveTowardsTarget, IdleTillThreatened, Stationary}
	public Behaviour behaviour = Behaviour.IdleTillThreatened;
	public WorldCharacter EnemyCharacter;

	public class Actions
	{
		public enum Action { Move, Attack };
		public Queue<Action> ActionList = new Queue<Action>();
		public List<Cube> MovePath = new List<Cube>();
		public WorldCharacter attackTarget = null;
	}

	private class AICube
	{
		public WorldCharacter worldCharacter;
		public List<Cube> cubesThatCanAttackThis = new List<Cube>();
	}



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Actions GetActions(GameManager gm)
	{
		Actions a = new Actions();
		if(behaviour == Behaviour.MoveTowardsTarget || behaviour == Behaviour.IdleTillThreatened)
		{
			AICube PathToAttackTarget = GetMovePathAndAttackTargetIfTargetInRange(gm);
			if (PathToAttackTarget != null)
			{
				a.ActionList.Enqueue(Actions.Action.Move);
				a.MovePath = PathToAttackTarget.cubesThatCanAttackThis;
				a.ActionList.Enqueue(Actions.Action.Attack);
				a.attackTarget = PathToAttackTarget.worldCharacter;
			}
			else if(behaviour == Behaviour.MoveTowardsTarget)
			{
				List<Cube> MovePath = GetMovePathTowardsATarget(gm);
				if(MovePath != null)
				{
					a.ActionList.Enqueue(Actions.Action.Move);
					a.MovePath = MovePath;
				}
			}
		}
		else if(behaviour == Behaviour.Stationary)
		{
			WorldCharacter attackTarget = GetAdjacentCharacterToAttack(gm);
			if (attackTarget)
			{
				a.ActionList.Enqueue(Actions.Action.Attack);
				a.attackTarget = attackTarget;
			}
		}

		return a;
	}

	private WorldCharacter GetTargetCharacter(GameManager gm)
	{
		foreach(WorldCharacter w in gm.playableCharacters)
		{
			if(w.bIsAlive)
			{
				return w;
			}
		}
		return null;
	}

	private List<Cube> GetMovePathTowardsATarget(GameManager gm)
	{
		WorldCharacter w = GetTargetCharacter(gm);
		if(w)
		{
			List<Cube> ShortestPath = gm.cubeManager.FindPathTowardsTarget(EnemyCharacter, w);
			if(ShortestPath == null)
			{
				return null;
			}
			List<Cube> RealPath = new List<Cube>();
			int moveCounter = 0;
			int cubeCounter = 0;
			while(cubeCounter < ShortestPath.Count)
			{
				moveCounter += (int)ShortestPath[cubeCounter].difficulty;
				RealPath.Add(ShortestPath[cubeCounter]);
				if (moveCounter >= EnemyCharacter.GetMovement())
				{
					break;
				}
				cubeCounter++;
			}
			return RealPath;
		}
		return null;
	}

	private WorldCharacter GetAdjacentCharacterToAttack(GameManager gm)
	{
		List<Cube> cubesInAttackRange = new List<Cube>();
		cubesInAttackRange = gm.cubeManager.GetAllAdjacentCubesInWeaponRange(EnemyCharacter.GetCurrentCube(), EnemyCharacter.getInventory().getEquippedWeapon().GetRange(), EnemyCharacter.getInventory().getEquippedWeapon().Group);
		foreach (Cube c in cubesInAttackRange)
		{
			GameObject g = c.GetOccupant();
			if (g != null)
			{
				WorldCharacter worldCharacter = g.GetComponent<WorldCharacter>();
				if (worldCharacter)
				{
					if (gm.playableCharacters.Contains(worldCharacter))
					{
						return worldCharacter;
					}
				}
			}
		}
		return null;
	}

	private AICube GetMovePathAndAttackTargetIfTargetInRange(GameManager gameManager)
	{
		/*
		List<AICube> cubesInRange = new List<AICube>();

		//List<Cube> cubesToMove = gameManager.selectionManager.GetCubesInMoveRange();
		List<Cube> cubesToMove = new List<Cube>();
		cubesToMove.Add(EnemyCharacter.GetCurrentCube());
		List<Cube> newCubes = gameManager.cubeManager.BFS(EnemyCharacter.GetCurrentCube(), (int)EnemyCharacter.GetMovement(), EnemyCharacter.isPlayable);
		foreach(Cube c in newCubes)
		{
			if(c.IsMovable() && c.GetOccupant() != null)
			{
				cubesToMove.Add(c);
			}
		}
		foreach (Cube c in cubesToMove)
		{
			if(c.GetOccupant() != null)
			{
				break;
			}
			List<Cube> cubesCanAttackFromC = gameManager.cubeManager.GetAllAdjacentCubesInWeaponRange(c, EnemyCharacter.getInventory().getEquippedWeapon().GetRange(), EnemyCharacter.getInventory().getEquippedWeapon().Group);
			foreach(Cube curr in cubesCanAttackFromC)
			{
				GameObject g = curr.GetOccupant();
				if(g != null)
				{
					WorldCharacter worldCharacter = g.GetComponent<WorldCharacter>();
					if (worldCharacter)
					{
						if (gameManager.playableCharacters.Contains(worldCharacter))
						{
							bool WorldCharacterExists = false;
							foreach(AICube aic in cubesInRange)
							{
								if(aic.worldCharacter == worldCharacter)
								{
									aic.cubesThatCanAttackThis.Add(c);
									WorldCharacterExists = true;
									break;
								}
							}
							if (!WorldCharacterExists)
							{
								AICube aic = new AICube();
								aic.worldCharacter = worldCharacter;
								aic.cubesThatCanAttackThis.Add(c);
								cubesInRange.Add(aic);
							}
						}
					}
				}
			}
		}

		if (cubesInRange.Count > 0)
		{
			AICube aiCube = new AICube();
			const int charToSelect = 0;
			const int cubeToSelect = 0;
			aiCube.worldCharacter = cubesInRange[charToSelect].worldCharacter;
			aiCube.cubesThatCanAttackThis = gameManager.cubeManager.BFS(EnemyCharacter.GetComponent<Movement>().GetCurrentCube(), (int)EnemyCharacter.GetMovement(), EnemyCharacter.isPlayable, cubesInRange[charToSelect].cubesThatCanAttackThis[cubeToSelect]);
			return aiCube;
		}
		*/
		WorldCharacter targetChar = null;
		List<Cube> canAttackCubes = EnemyCharacter.GetCanAttackCubes();
		foreach(Cube canAttack in canAttackCubes)
		{
			if(canAttack != null)
			{
				GameObject occ = canAttack.GetOccupant();
				if(occ != null)
				{
					WorldCharacter occChar = occ.GetComponent<WorldCharacter>();
					if(occChar != null)
					{
						if(occChar.isPlayable)
						{
							targetChar = occChar;
							break;
						}
					}
				}
			}
		}
		if(targetChar != null)
		{
			Cube targetCube = null;
			List<Cube> canMoveCubes = EnemyCharacter.GetCanMoveCubes();
			foreach(Cube canMove in canMoveCubes)
			{
				if(canMove != null && canMove.IsMovable() && (canMove.GetOccupant() == null || canMove.GetOccupant().GetComponent<WorldCharacter>() == EnemyCharacter))
				{
					bool doneSearching = false;
					List<Cube> canAttackFromCanMove = gameManager.cubeManager.GetAllAdjacentCubesInWeaponRange(canMove, EnemyCharacter.getInventory().getEquippedWeapon().GetRange(), EnemyCharacter.getInventory().getEquippedWeapon().Group);
					
					if (canAttackFromCanMove != null)
					{
						Debug.Log(EnemyCharacter.name + " can attack from " + canMove.name + ". Trying to find " + targetChar.name);
						foreach (Cube canAttack in canAttackFromCanMove)
						{
							if(canAttack != null)
							{
								GameObject occ = canAttack.GetOccupant();
								if (occ != null)
								{
									WorldCharacter occChar = occ.GetComponent<WorldCharacter>();
									if (occChar == targetChar)
									{
										targetCube = canMove;
										doneSearching = true;
										break;
									}
								}
							}
						}
					}
					if(doneSearching)
					{
						Debug.Log("Done Searching");
						break;
					}
				}
			}
			if(targetCube != null)
			{
				AICube aiCube = new AICube();
				aiCube.worldCharacter = targetChar;
				aiCube.cubesThatCanAttackThis = gameManager.cubeManager.BFS(EnemyCharacter.GetComponent<Movement>().GetCurrentCube(), (int)EnemyCharacter.GetMovement(), EnemyCharacter.isPlayable, targetCube);
				return aiCube;
			}
		}
		return null;
	}

}
