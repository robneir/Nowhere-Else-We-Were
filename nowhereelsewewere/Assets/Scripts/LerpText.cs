using UnityEngine;
using System.Collections;

public class LerpText : MonoBehaviour {

	public float lerpSpeed = 10.0f;
	private float timer = 0.0f;
	private bool IsLerping = false;
	private uint current;
	private uint target;
	private uint max;
	public TextMesh text;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(IsLerping)
		{
			if(current == target)
			{
				text.text = ((int)current).ToString() + "/" + ((int)max).ToString();
				IsLerping = false;
			}
			else
			{
				timer -= Time.deltaTime;
				if(timer <= 0.0f)
				{
					timer = 1.0f / lerpSpeed;
					if((int)target - (int)current > 0)
					{
						current++;
					}
					else if((int)target - (int)current < 0)
					{
						current--;
					}
					else
					{
						IsLerping = false;
					}
					text.text = ((int)current).ToString() + "/" + ((int)max).ToString();
				}
			}
		}
	}

	public void ChangeValue(uint currentText, uint newText, uint maxText)
	{
		current = currentText;
		target = newText;
		max = maxText;
		timer = 1.0f / lerpSpeed;
		IsLerping = true;
	}
}
