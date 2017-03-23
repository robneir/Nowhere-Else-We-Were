using UnityEngine;
using System.Collections;

public class BackAction : Action
{
    public BackAction(GameManager gm, string actionName, AudioClip actionSound)
        : base(actionName, actionSound)
    {
        this.gm = gm;
    }

    public override void DoAction()
    {
        base.DoAction();
        SelectionManager selectionManager = gm.selectionManager;
        TurnManager turnManager = gm.turnManager;
        if (turnManager != null && selectionManager != null)
        {
            // Do different things based on if we are in attack or choose phase.
            if (turnManager.getPlayerTurnPhase() == TurnManager.PlayerTurnPhase.PreAttack)
            {
                //attackButton.gameObject.SetActive(true);
               // waitButton.gameObject.SetActive(true);
                selectionManager.ClearInRangeAttackCubes();
				selectionManager.GetCurrentSelectedCharacter().ShowRangeCubes(gm.cubeManager, gm.selectionManager);
                selectionManager.ShowCubesInShortestPath();
                turnManager.SetPlayerTurnPhase(TurnManager.PlayerTurnPhase.ChoosePhase);
            }
            else if (turnManager.getPlayerTurnPhase() == TurnManager.PlayerTurnPhase.ChoosePhase)
            {
                gm.uiManager.getActionPanel().RemoveActionMenuButtons();

                // Move the character back to where they were before moving.
                Movement currCharMoveComp = selectionManager.GetCurrentSelectedCharacter().GetComponent<Movement>();
                if (currCharMoveComp != null)
                {
                    currCharMoveComp.SnapToCube(currCharMoveComp.GetPreviousCube());
                }
                else
                {
                    Debug.Log("Character move component is null!");
                }
                selectionManager.ClearCubesInShortestPath();
				selectionManager.GetCurrentSelectedCharacter().ShowRangeCubes(gm.cubeManager, gm.selectionManager);
                turnManager.SetPlayerTurnPhase(TurnManager.PlayerTurnPhase.Move);
            }
        }
    }
}