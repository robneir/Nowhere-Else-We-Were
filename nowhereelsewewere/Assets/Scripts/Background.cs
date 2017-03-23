using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {

	public Texture2D backgroundImage;

	private Texture2D fadeOutTexture;
	private int drawDepth = 1;
	private float alpha = 0.0f;
	private int fadeDir = -1;
	private float fadeSpeed = 0.5f;

	// Use this for initialization
	void Start () {
		fade(1, 1.0f / fadeSpeed, backgroundImage);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI()
	{
		if (fadeOutTexture == null)
		{
			return;
		}
		alpha += fadeDir * fadeSpeed * Time.deltaTime;

		alpha = Mathf.Clamp01(alpha);

		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
		GUI.depth = drawDepth;
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
	}

	private void fade(int fadeDir, float fadeTime, Texture2D text)
	{
		this.fadeOutTexture = text;
		this.fadeDir = fadeDir;
		this.fadeSpeed = 1.0f / fadeTime;
	}
}
