using UnityEngine;
using System.Collections;

public class CameraBounds : MonoBehaviour {

	private float minX;
	private float minY;
	private float minZ;
	private float maxX;
	private float maxY;
	private float maxZ;

	// Use this for initialization
	void Start () {
		Vector3 pos = this.transform.position;
		Vector3 scale = this.transform.localScale;
		minX = pos.x - scale.x / 2.0f;
		minY = pos.y - scale.y / 2.0f;
		minZ = pos.z - scale.z / 2.0f;

		maxX = pos.x + scale.x / 2.0f;
		maxY = pos.y + scale.y / 2.0f;
		maxZ = pos.z + scale.z / 2.0f;
	}

	public bool IsWithin(Vector3 lookAtPos)
	{
		return lookAtPos.x >= minX && lookAtPos.y >= minY && lookAtPos.z >= minZ && lookAtPos.x <= maxX && lookAtPos.y <= maxY && lookAtPos.z <= maxZ;
	}
}
