using UnityEngine;
using System.Collections;
using System;

public class PickUpAction : Action {

    private Pickup pickUp;
    private WorldCharacter currCharacter;

    public PickUpAction(GameManager gm, Pickup pickUp, WorldCharacter currCharacter, string actionName, AudioClip actionSound)
        : base(actionName, actionSound)
    {
        this.gm = gm;
        // Get name of pickup from weapon because right now only works for weapons 
        Weapon weapon = pickUp.GetComponent<Weapon>();
        if(weapon != null)
        {
            this.actionName = "take " + weapon.Name;
        }
        else
        {
            Debug.Log(pickUp.name + " is not a weapon so no pickup name for it yet.");
        }
        this.pickUp = pickUp;
        this.currCharacter = currCharacter;
    }

    public override void DoAction()
    {
        base.DoAction();
        if (currCharacter != null)
        {
            //Remove pickup from parent cube
            pickUp.parentCube.pickUps.Remove(pickUp);
            pickUp.PickUp(currCharacter);

            // Disable object picked up but don't destroy because we need the object for later and we don't
            // want to have to spawn the object from scratch cause code...
            pickUp.gameObject.SetActive(false);

            // Remove possible action of picking up this object from action menu
            RemoveActionFromPossibleActions();

            // Make picking up count as a full action that will cause turn to end.
            TurnManager turnManager = gm.turnManager;
            if (turnManager != null)
            {
                turnManager.SetPlayerTurnPhase(TurnManager.PlayerTurnPhase.SelectPlayer);
                gm.turnManager.DisableCharacter();
            }
        }
    }

}
