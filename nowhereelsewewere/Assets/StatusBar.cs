using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatusBar : MonoBehaviour {

    [SerializeField]
    private Image healthBar;

    [SerializeField]
    private Image experienceBar;

    [HideInInspector]
    private WorldCharacter character;

    public Image teamPointer;

    [HideInInspector]
    public Vector2 offset;

    private GameManager gm;

	// Use this for initialization
	void Start () {
        gm = GameObject.FindObjectOfType<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
	    if(character != null)
        {
            Vector3 screenSpaceCharacterPos = Camera.main.WorldToScreenPoint(character.transform.position + new Vector3(offset.x, offset.y, 0));
            this.transform.position = screenSpaceCharacterPos;
            if(healthBar != null && experienceBar != null)
            {
                healthBar.fillAmount = (float)character.getCurrHealth() / (float)character.getMaxHealth();
                experienceBar.fillAmount = (float)character.experience / 100.0f;
                
                // Update health bar color based on amount of health.
                if(healthBar.fillAmount > .65f)
                {
                    healthBar.color = Constants.instance.HighHealthColor;
                }
                else if(healthBar.fillAmount > .3f)
                {
                    healthBar.color = Constants.instance.MedHealthColor;
                }
                else if (healthBar.fillAmount > 0.0f)
                {
                    healthBar.color = Constants.instance.LowHealthColor;
                }
            }
        }
	}

    public void SetTeamColor(Color color)
    {
        teamPointer.color = color;
    }

    public void SetParentCharacter(WorldCharacter character)
    {
        this.character = character;
    }
}
