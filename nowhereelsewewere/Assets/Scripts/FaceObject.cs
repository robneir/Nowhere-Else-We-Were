using UnityEngine;
using System.Collections;

public class FaceObject : MonoBehaviour {

    // Target to face
    public GameObject target;

	// Use this for initialization
	void Start () {
        // Make target camera if no target.
        if(target == null)
        {
            target = Camera.main.gameObject;
        }
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 vectToTarget = this.transform.position - target.transform.position;
        this.transform.rotation = Quaternion.LookRotation(vectToTarget, target.transform.up);
    }
}
