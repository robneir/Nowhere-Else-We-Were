using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour {

	public float speed = 10.0f;
	public float speedUp = 2.0f;
	public string NextScene = "";
	public Text LastText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 LastTextPos = LastText.transform.position;
		if(LastTextPos.y < Screen.height / 2)
		{
			float newSpeed = speed;
			if (Input.GetButton("Fire1"))
			{
				newSpeed *= speedUp;
			}
			Vector3 newPos = this.transform.position;
			newPos += Vector3.up * newSpeed * Time.deltaTime;
			this.transform.position = newPos;
		}
		else
		{
			if(Input.GetButtonDown("Fire1"))
			{
				SceneManager.LoadScene(NextScene);
			}
		}
	}
}
