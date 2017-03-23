using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionManager : MonoBehaviour {

    public GameManager gm;

    [HideInInspector]
    public List<Action> currentPossibleActions = new List<Action>();

    public AudioClip attackActionSound;
    public AudioClip waitActionSound;
    public AudioClip pickUpActionSound;

    public List<Action> GetAllPossibleActions()
    {
        currentPossibleActions.Clear();
        WaitAction wait = new WaitAction(gm, "Wait", waitActionSound);
        currentPossibleActions.Add(wait);
        AttackAction attack = new AttackAction(gm, "Attack", attackActionSound);
        currentPossibleActions.Add(attack);
        /*BackAction back = new BackAction(gm);
        currentPossibleActions.Add(back);*/

        // Get all possible things to pickup on the current cube we are on and display them as possible action.
        WorldCharacter currCharacter = gm.selectionManager.GetCurrentSelectedCharacter();
        Movement movement = currCharacter.GetComponent<Movement>();

        Cube destinationCube = null;
        List<Cube> shortestPath = gm.selectionManager.GetCubesInShortestPath();
        if(shortestPath != null)
        {
            destinationCube = shortestPath[shortestPath.Count - 1];
        }
        if(destinationCube != null && currCharacter != null)
        {
            List<Pickup> pickUps = destinationCube.pickUps;
            for(int i = 0; i < pickUps.Count; i++)
            {
                Pickup pickUp = pickUps[i];
                if(pickUp != null)
                {
                    // Create a button for each possible pickup
                    PickUpAction pickUpAciton = new PickUpAction(gm, pickUp, currCharacter, "Pickup ", pickUpActionSound);
                    currentPossibleActions.Add(pickUpAciton);
                }
            }
        }
        else
        {
            Debug.Log("Current cube or current character is null so we didn't list the possible pickups");
        }

        return currentPossibleActions;
    }
}
