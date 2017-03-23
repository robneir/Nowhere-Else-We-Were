using UnityEngine;
using System.Collections;

public class UIPanel : MonoBehaviour {

    public bool isInLevel;

	// Use this for initialization
	void Start () {
	    if(!isInLevel)
        {
            this.gameObject.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
