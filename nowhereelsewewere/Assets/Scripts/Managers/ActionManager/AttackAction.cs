using UnityEngine;
using System.Collections;

public class AttackAction : Action {

    public AttackAction(GameManager gm, string actionName, AudioClip actionSound) : 
        base(actionName, actionSound)
    {
        this.gm = gm;
    }

    public override void DoAction()
    {
        base.DoAction();
        TurnManager turnManager = gm.turnManager;
        if (turnManager != null)
        {
            turnManager.SetPlayerTurnPhase(TurnManager.PlayerTurnPhase.PreAttack);
            gm.selectionManager.ClearInRangeAttackCubes();
            gm.selectionManager.ShowInRangeAttackCubes();
            gm.uiManager.getActionPanel().RemoveActionMenuButtons();
        }
    }
}
