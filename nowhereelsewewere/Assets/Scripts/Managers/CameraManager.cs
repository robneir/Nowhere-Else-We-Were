using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    [SerializeField]
    private GameManager gm;

    // Used to tell the camera what to look at in this script.
    [SerializeField]
    private LookAtComp lookAtComp;

    // Somewhat of and offset so the camera doesn't look at the center of a cube.
    // Chose 9 because that is half the size of a cube right now.
    [SerializeField]
    private Vector3 cameraLookAtOffset = new Vector3(0.0f, 3.0f, 0.0f);

	public enum CameraState { Panning, Locked, Midpoint };

	[SerializeField]
	private CameraState state = CameraState.Panning;
	
	private WorldCharacter MainCharacter;

	private bool IsMousePanning = false;
	private bool CanRotate = false;
	private float rotateAmount = 0.0f;

	private bool isFuture = false;

    public volatile bool canInput = false;

    [Tooltip("The threshold that is used to determin whether or not to hide the cursor when panning around the map.")]
    public float cursorHideThreshold = 0.1f;
    [HideInInspector]
    public bool cursorVisibleFlag = true;
    public bool cursorSnapToMiddleScreen = false;

	private Vector3 CalcLookAt(WorldCharacter w, Cube c)
	{
		Vector3 selectedLookAt = new Vector3();
		if (w != null)
		{
			selectedLookAt = w.transform.position + cameraLookAtOffset;
		}
		else
		{
			selectedLookAt = c.transform.position + c.GetCubeOffset();
		}
		return selectedLookAt;
	}

    // waits till next frame to show cursor.
    IEnumerator WaitToShowCursor()
    {
        yield return null;
        Cursor.visible = true;
        if(cursorSnapToMiddleScreen)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    // Update is called once per frame.
    void Update()
	{
		if (Input.GetButtonDown("Fire1") && !gm.uiManager.CheckUIContainsPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)))
		{
			IsMousePanning = true;
        }
		if (Input.GetButtonUp("Fire1"))
		{
			IsMousePanning = false;
            StartCoroutine(WaitToShowCursor());
        }
		switch (state)
		{
			case CameraState.Panning:
				float horizAxis = Input.GetAxis("Horizontal");
				float verticAxis = Input.GetAxis("Vertical");
				if(IsMousePanning)
				{
					horizAxis -= Input.GetAxis("Mouse X");
					verticAxis -= Input.GetAxis("Mouse Y");
                    // Hide the mouse
                    if (Mathf.Abs(verticAxis) > cursorHideThreshold || Mathf.Abs(horizAxis) > cursorHideThreshold)
                    {
                        Cursor.visible = false;
                        if (cursorSnapToMiddleScreen)
                        {
                            Cursor.lockState = CursorLockMode.Locked;
                        }
                    }
				}

                if(canInput)
                {
				    lookAtComp.SetHorizAxis(horizAxis);
				    lookAtComp.SetVerticAxis(verticAxis);
                }
                else
                {
                    lookAtComp.SetHorizAxis(0.0f);
                    lookAtComp.SetVerticAxis(0.0f);
                }

				break;
			case CameraState.Locked:
				lookAtComp.UpdateLookAtPos((isFuture ? MainCharacter.GetFutureCube().transform.position + MainCharacter.GetFutureCube().GetCubeOffset() : MainCharacter.transform.position) + MainCharacter.cubeOffset);
				break;

			case CameraState.Midpoint:
				Vector3 mainCharacterLookAt = MainCharacter.transform.position + MainCharacter.cubeOffset;
				SelectionManager sm = gm.selectionManager;
				if(sm)
				{
					Cube currentCubeHovered = sm.GetCurrentHoveredCube();
					if(currentCubeHovered)
					{
						GameObject go = currentCubeHovered.GetOccupant();
						WorldCharacter hoveredCharacter = null;
						if(go)
						{
							hoveredCharacter = go.GetComponent<WorldCharacter>();
						}
						Vector3 hoverLookAt = CalcLookAt(hoveredCharacter, currentCubeHovered);
						Vector3 Midpoint = 0.5f * (mainCharacterLookAt + hoverLookAt);
						lookAtComp.UpdateLookAtPos(Midpoint);
					}
				}
				break;
		}

		if(Input.GetButtonDown("SpinRight"))
		{
			lookAtComp.UpdateAngleDir(1);
		}
		if(Input.GetButtonDown("SpinLeft"))
		{
			lookAtComp.UpdateAngleDir(-1);
		}

		if(Input.GetButtonDown("Fire3"))
		{
			CanRotate = true;
		}
		else if(Input.GetButtonUp("Fire3"))
		{
			CanRotate = false;
			rotateAmount = 0.0f;
		}

		if(Input.GetButton("Fire3"))
		{
			float vertAxis = -Input.GetAxis("Mouse Y");
			float angle = lookAtComp.angleUp;
			angle += vertAxis * lookAtComp.angleUpSpeed * Time.deltaTime;
			angle = Mathf.Clamp(angle, 0.0001f, 89.999f);
			lookAtComp.UpdateAngleUp(angle);

			if(CanRotate)
			{
				rotateAmount += Input.GetAxis("Mouse X");
				bool rotateMore = rotateAmount >= lookAtComp.neededToRotate;
				bool rotateLess = rotateAmount <= -lookAtComp.neededToRotate;
				if (rotateMore || rotateLess)
				{
					float decrAmount = 0.0f;
					if(rotateMore)
					{
						lookAtComp.UpdateAngleDir(1);
						decrAmount = lookAtComp.neededToRotate;
					}
					else if(rotateLess)
					{
						lookAtComp.UpdateAngleDir(-1);
						decrAmount = -lookAtComp.neededToRotate;
					}
					rotateAmount -= decrAmount;
				}
			}
		}

		float scrollAxis = -Input.GetAxis("Mouse ScrollWheel");
		float away = lookAtComp.awayDistance;
		away += scrollAxis * lookAtComp.awayDistanceSpeed * Time.deltaTime;
		away = Mathf.Clamp(away, 1.0f, 15.0f);
		lookAtComp.UpdateAway(away);
	}

	public void FreeCamera()
	{
		lookAtComp.StopInterpingLookAt();
		state = CameraState.Panning;
		MainCharacter = null;
	}

	public void LockCamera(WorldCharacter w, bool isFuture = false)
	{
		state = CameraState.Locked;
		SetIsFuture(isFuture);
		MainCharacter = w;
	}
	/*
	public void MidpointCamera(WorldCharacter w)
	{
		state = CameraState.Midpoint;
		MainCharacter = w;
	}
	*/

	public CameraState GetState()
	{
		return state;
	}

    public LookAtComp GetLookAt()
    {
        return lookAtComp;
    }

	public void SetIsFuture(bool b)
	{
		isFuture = b;
	}
}
