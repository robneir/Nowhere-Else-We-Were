using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExperiencePanel : MonoBehaviour {

	public Text expText;

	private WorldCharacter player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(player)
		{
			expText.text = "EXP: " + player.experience.ToString();
		}
	}

	public IEnumerator Show(WorldCharacter player)
	{
		this.player = player;
		expText.enabled = true;
		expText.text = "EXP: " + player.experience.ToString();
		yield return new WaitForSeconds(0.5f);
	}

	public IEnumerator Hide()
	{
		yield return new WaitForSeconds(0.5f);
		this.player = null;
		expText.text = "";
		expText.enabled = false;
	}
}
