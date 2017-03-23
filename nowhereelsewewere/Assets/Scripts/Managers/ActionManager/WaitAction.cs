using UnityEngine;
using System.Collections;

public class WaitAction : Action {

    public WaitAction(GameManager gm, string actionName, AudioClip actionSound)
        : base(actionName, actionSound)
    {
        this.gm = gm;
        this.actionName = "Wait";
    }

    public override void DoAction()
    {
        base.DoAction();
        TurnManager turnManager = gm.turnManager;
        if (turnManager != null)
        {
            turnManager.SetPlayerTurnPhase(TurnManager.PlayerTurnPhase.EndTurn);
        }
    }
}
