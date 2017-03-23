using UnityEngine;
using System.Collections;

public class BouncingText : MonoBehaviour {

	[SerializeField]
	private TextMesh text;

	[SerializeField]
	private float timeAllowed;

	[SerializeField]
	private float accelerationDown;

	[SerializeField]
	private float initSpeed;

	private bool IsBouncing = false;

	private float timer = 0.0f;

	private float vel = 0.0f;

	private float originalHeight;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(IsBouncing)
		{
			timer -= Time.deltaTime;
			if(timer <= 0.0f)
			{
				timer = 0;
				IsBouncing = false;
				text.text = "";
			}
			else
			{
				float height = text.transform.position.y;
				height += vel * Time.deltaTime;

				if(height < originalHeight)
				{
					height = originalHeight;
				}

				text.transform.position = new Vector3(text.transform.position.x, height, text.transform.position.z);
				vel -= accelerationDown * Time.deltaTime;
			}
		}
	}

	public void ShowText(string t, Vector3 loc)
	{
		text.transform.position = loc;
		originalHeight = loc.y;
		text.text = t;
		IsBouncing = true;
		timer = timeAllowed;
		vel = initSpeed;
	}
}
