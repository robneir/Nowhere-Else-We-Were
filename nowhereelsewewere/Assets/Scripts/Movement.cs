using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Movement : MonoBehaviour {

    private WorldCharacter characterComp;

    private AnimationController animationController;

    [SerializeField]
    private float lerpSpeed = 0.01f;

    [SerializeField]
    private float snapThreshold = 0.001f;

    private List<Cube> targetCubePath = new List<Cube>();

    private int pathProgressCounter = 0;

    [SerializeField]
    private Cube currentCube;

    // The last cube in the path.
    private Cube endCube;

    // Next cube we are lerping to.
    private Cube targetCube;

    // Cube character was on before moving to targetCube
    private Cube prevCube;

    public enum MoveState { DoneMoving, Moving, Idle, Turning }
    private MoveState moveState;

    // Use this for initialization
    void Start () {
        // Get character this script is attached to
        characterComp = GetComponent<WorldCharacter>();

        animationController = GetComponent<AnimationController>();

        // Set movestate to idle
        moveState = MoveState.Idle;

        // Set the occupant to this character.
        currentCube.SetOccupant(this.gameObject);

        SnapToCube(currentCube);
    }
	
	// Update is called once per frame
	void Update () {

    }

    // Gets previous cube character was on.
    public Cube GetPreviousCube()
    {
        return prevCube;
    }

    // Snap character to previous cube
    public void SnapToCube(Cube c)
    {
        if(c != null)
        {
            moveState = MoveState.DoneMoving;
            SetCurrentCube(c);
            endCube = null;
            moveState = MoveState.Idle;
            // Set the player's position to cube character is on.
            Vector3 targetPosWithOffset = c.transform.position;
            if (characterComp != null)
            {
                // Add the offset of the character so that the character stays above the cubes while moving.
                targetPosWithOffset += characterComp.cubeOffset + GetCurrentCube().GetCubeOffset();
            }
            transform.position = targetPosWithOffset;

            // Transition to Idling animation.
            AnimationController animController = this.GetComponent<AnimationController>();
            if(animController != null)
            {
                animController.PlayAnimation(AnimationController.AnimState.Idling);
            }
            else
            {
                Debug.Log("No animation controller attached to character.");
            }
        }
    }

    // Move along a path to cube orthogonally following the cubes passed in.
    public bool MoveOnCubePath(List<Cube> cubes)
    {
        // If we are in the idle state then setup the pathing.
        if(moveState == MoveState.Idle)
        {
            // Set Prev cube.
            prevCube = currentCube;
            // Reset the progress so we are starting from the first cube of this new path.
            pathProgressCounter = 0;
            // Specify the path that we want to travel.
            targetCubePath = cubes;
            // Specify ending cube so we know when to stop in update.
            if (targetCubePath.Count > 0)
            {
                endCube = targetCubePath[targetCubePath.Count - 1];
                // Change to moving state
                moveState = MoveState.Turning;
            }
        }

        // If we have a target cube and we have not exceeded the bounds of the path we are following...
        if (targetCubePath.Count > 0 && targetCubePath.Count > pathProgressCounter)
        {
            if(moveState == MoveState.Moving)
            {
                // Play the walking animation when moving.
                animationController.PlayAnimation(AnimationController.AnimState.Walking);
                // Change the target cube to the next one if pathProgressCounter increased.
                targetCube = targetCubePath[pathProgressCounter];
                if (targetCube != null)
                {
                    Vector3 targetPosWithOffset = targetCube.transform.position + targetCube.GetCubeOffset();
                    if (characterComp != null)
                    {
                        // Add the offset of the character so that the character stays above the cubes while moving.
                        targetPosWithOffset += characterComp.cubeOffset;
                    }
                    // Apply lerp to position.
                    transform.position = Vector3.MoveTowards(transform.position, targetPosWithOffset, lerpSpeed * Time.deltaTime);
                    // Set the currently moving bool to true so we can check outside of this script if the character is moving.
                    moveState = MoveState.Moving;
                    // Snap to target cube if close enough. 
                    if (Vector3.Distance(transform.position, targetPosWithOffset) < snapThreshold)
                    {
                        transform.position = targetPosWithOffset;
                        // Update target cube (this is needed so we can turn to face next cube, needed for calculation).
                        prevCube = targetCube;
                        pathProgressCounter++;
                        // Change to turning state
                        moveState = MoveState.Turning;
                        // Check to see if we have reached the end of the path.
                        if (targetCube == endCube)
                        {
                            animationController.PlayAnimation(AnimationController.AnimState.Idling);
							prevCube = null;
                            SnapToCube(endCube);
                            return true;
                        }
                    }
                }
            }
            else if(moveState == MoveState.Turning)
            {
                targetCube = targetCubePath[pathProgressCounter];
                // Rotate the character to the next cube it must travel to.
                if (targetCube != null)
                {
                    int cubeDirInt = CubeManager.instance.FindAdjacentDirectionOfCube(GetPreviousCube(), targetCube);
                    Cube.AdjacentDirections directionAsEnum = (Cube.AdjacentDirections)cubeDirInt;
                    if (Cube.AdjacentDirections.FORWARD == directionAsEnum)
                    {
                        // Stop the walking animation if we changing direction
                        this.transform.eulerAngles = new Vector3(0, 0, 0);
                    }
                    else if (Cube.AdjacentDirections.BACK == directionAsEnum)
                    {
                        this.transform.eulerAngles = new Vector3(0, 180, 0);
                    }
                    else if (Cube.AdjacentDirections.RIGHT == directionAsEnum)
                    {
                        this.transform.eulerAngles = new Vector3(0, 90, 0);
                    }
                    else if (Cube.AdjacentDirections.LEFT == directionAsEnum)
                    {
                        this.transform.eulerAngles = new Vector3(0, 270, 0);
                    }

                    // Once we have turned go back to moving.
                    moveState = MoveState.Moving;
                }
            }
        }
        return false;
    }

	public void StopMoving()
	{
		moveState = MoveState.Idle;
	}

    public void ResetPrevCube()
	{
		prevCube = null;
	}

	public MoveState GetMoveState()
    {
        return moveState;
    }

    private void SetMoveState(MoveState moveState)
    {
        this.moveState = moveState;
    }

    // Sets the current cube the character is on.
    public void SetCurrentCube(Cube cube)
    {
        // Clear the contents of the occupant of the cube this character is on.
        currentCube.SetOccupant(null);
        // Set the occupant of the end cube to be the character that just moved onto it.
        cube.SetOccupant(characterComp.gameObject);
        currentCube = cube;
		characterComp.SetFutureCube(currentCube);
    }

    // Gets the current cube the character is on.
    public Cube GetCurrentCube()
    {
        return currentCube;
    }
}
