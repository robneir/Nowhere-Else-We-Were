using UnityEngine;
using System.Collections;

public class LookAtComp : MonoBehaviour
{
	public float awayDistanceSpeed = 1.0f;
    public float awayDistanceScale = 1.0f;
	public float awayDistance = 5.0f;

	public float angleUpSpeed = 10.0f;
	public float angleUp = 60.0f;

    public enum dirs { PosZ, PosZPosX, PosX, PosXNegZ, NegZ, NegZNegX, NegX, NegXPosZ};
    public dirs dir;
	public float neededToRotate = 250.0f;
    float[] angleDirs = { 0.0f, 45.0f, 90.0f, 135.0f, 180.0f, 225.0f, 270.0f, 315.0f };

    public GameObject LookAt;

    public float awaySpring = 1.0f;
    public float awayMinVel = 0.0f;
    public float awayCloseEnough = 0.0f;
    public float angleUpSpring = 1.0f;
    public float angleUpMinVel = 0.0f;
    public float angleUpCloseEnough = 0.0f;
    public float angleDirSpring = 1.0f;
    public float angleDirMinVel = 0.0f;
    public float angleDirCloseEnough = 0.0f;
    public float lookAtSpring = 1.0f;
    public float lookAtMinVel = 0.0f;
    public float lookAtCloseEnough = 0.0f;

    bool interpingAway = false;
    bool interpingAngleUp = false;
    bool interpingAngleDir = false;
    bool interpingLookAt = false;

	float horizAxis = 0.0f;
	float verticAxis = 0.0f;

	public float horizSpeed = 500.0f;
	public float verticSpeed = 500.0f;

	public float awayPanScale = 0.05f;

	public CameraBounds cameraBounds = null;

	public bool disabled = false;

    Vector3 wantedPos;

	public void SetHorizAxis(float axis)
	{
		horizAxis = axis;
	}

	public void SetVerticAxis(float axis)
	{
		verticAxis = axis;
	}

	private float initAwayDistance;
	private float initAngleUp;
	private dirs initDir;

    // Use this for initialization
    void Start()
    {
		initAwayDistance = awayDistance;
		initAngleUp = angleUp;
		initDir = dir;
        UpdateImmediate();
    }

    // Update is called once per frame
    void Update()
    {
		if(disabled)
		{
			return;
		}

        if (interpingAway)
        {
            InterpAway();
        }

        if (interpingAngleUp)
        {
            InterpAngleUp();
        }

        if (interpingAngleDir)
        {
            InterpAngleDir();
        }

		if(Input.GetButtonDown("Recenter"))
		{
			UpdateImmediate();
		}

		if(!Mathf.Approximately(horizAxis, 0.0f))
		{
			Vector3 newPos = this.LookAt.transform.position + horizAxis * horizSpeed * awayDistance * awayPanScale * Time.deltaTime * LookAt.transform.right;
			if(cameraBounds == null || (cameraBounds != null && cameraBounds.IsWithin(newPos)))
			{
				this.LookAt.transform.position = newPos;
			}
		}

		if(!Mathf.Approximately(verticAxis, 0.0f))
		{
			Vector3 v = LookAt.transform.position - this.transform.position;
			Vector3 y = Vector3.Dot(v, Vector3.down) * Vector3.down;
			Vector3 forward = v - y;
			forward.Normalize();
			Vector3 newPos = this.LookAt.transform.position + verticAxis * verticSpeed * awayDistance * awayPanScale * Time.deltaTime * forward;
			if (cameraBounds == null || (cameraBounds != null && cameraBounds.IsWithin(newPos)))
			{
				this.LookAt.transform.position = newPos;
			}
		}

        if (interpingLookAt)
        {
            InterpLookAt();
        }
    }

    public void UpdateLookAtPos(Vector3 newPos)
    {
		if(disabled)
		{
			return;
		}
        wantedPos = newPos;
        interpingLookAt = true;
    }

    private void InterpLookAt()
    {
        Vector3 oldPos = this.LookAt.transform.position;
        Vector3 dist = wantedPos - oldPos;
        if (dist.sqrMagnitude < lookAtCloseEnough * lookAtCloseEnough)
        {
            this.LookAt.transform.position = wantedPos;
            interpingLookAt = false;
            return;
        }

        Vector3 vel = dist * lookAtSpring;

        if (vel.sqrMagnitude < lookAtMinVel)
        {
            vel.Normalize();
            vel *= lookAtMinVel;
        }

        Vector3 newPos = oldPos + vel * Time.deltaTime;
        this.LookAt.transform.position = newPos;

    }

    private void InterpAway()
    {
        float awayDist = -(this.gameObject.transform.localPosition.z);
        float awayWantedDist = awayDistance * awayDistanceScale;
        float vel = awaySpring * (awayWantedDist - awayDist);
        if (vel != 0 && Mathf.Abs(vel) < awayMinVel)
        {
            vel = awayMinVel * (vel / Mathf.Abs(vel));
        }
        float newDist = awayDist + vel * Time.deltaTime;
        if (Mathf.Abs(newDist - awayWantedDist) <= awayCloseEnough)
        {
            newDist = awayWantedDist;
            interpingAway = false;
        }
        this.gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, -newDist);
    }

    private void InterpAngleUp()
    {
        float angleUpOld = this.LookAt.transform.eulerAngles.x;
		float angleUpWanted = angleUp;
        float vel = angleUpSpring * (angleUpWanted - angleUpOld);
        if (vel != 0 && Mathf.Abs(vel) < angleUpMinVel)
        {
            vel = angleUpMinVel * (vel / Mathf.Abs(vel));
        }
        float newAngleUp = angleUpOld + vel * Time.deltaTime;
        if (Mathf.Abs(newAngleUp - angleUpWanted) <= angleUpCloseEnough)
        {
            newAngleUp = angleUpWanted;
            interpingAngleUp = false;
        }
        this.LookAt.transform.eulerAngles = new Vector3(newAngleUp, this.LookAt.transform.eulerAngles.y, 0.0f);
    }

    private void InterpAngleDir()
    {
        float angleDirOld = this.LookAt.transform.eulerAngles.y;
        float angleDirWanted = angleDirs[(int)dir];
        float dist = angleDirWanted - angleDirOld;
        if (Mathf.Abs(dist) > 180.0f)
        {
            if (dist > 0)
            {
                dist = 180.0f - dist;
            }
            else if (dist < 0)
            {
                dist = dist + 360.0f;
            }
        }
        float vel = angleDirSpring * dist;
        if (vel != 0 && Mathf.Abs(vel) < angleDirMinVel)
        {
            vel = angleDirMinVel * (vel / Mathf.Abs(vel));
        }
        float newAngleDir = angleDirOld + vel * Time.deltaTime;
        if (Mathf.Abs(newAngleDir - angleDirWanted) <= angleDirCloseEnough)
        {
            newAngleDir = angleDirWanted;
            interpingAngleDir = false;
        }
        this.LookAt.transform.eulerAngles = new Vector3(this.LookAt.transform.eulerAngles.x, newAngleDir, 0.0f);
    }

    public void UpdateAway(float realAway)
    {
		awayDistance = realAway;
        interpingAway = true;
    }

    public void UpdateAngleUp(float realAngleUp)
    {
		angleUp = realAngleUp;
        interpingAngleUp = true;
    }

    public void UpdateAngleDir(int newAngleDir)
    {
        if (newAngleDir < 0)
        {
            newAngleDir += 8;
        }
        dir = (dirs)(((int)dir + newAngleDir) % 8);
        interpingAngleDir = true;
    }

    public void UpdateImmediate()
    {
		//this.gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, -initAwayDistance * awayDistanceScale);
        //this.LookAt.transform.eulerAngles = new Vector3(initAngleUp, angleDirs[(int)initDir], 0.0f);
		awayDistance = initAwayDistance;
		angleUp = initAngleUp;
		dir = initDir;
		interpingAngleUp = true;
		interpingAngleDir = true;
		interpingAway = true;
    }

	public void StopInterpingLookAt()
	{
		interpingLookAt = false;
	}

	public float GetAwayDist()
	{
		return awayDistance;
	}
}
