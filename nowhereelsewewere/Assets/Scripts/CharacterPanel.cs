using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterPanel : UIPanel {

    [SerializeField]
    private GameManager gm;

    [SerializeField]
    private Text statText;

    [SerializeField]
    private Text characterNameField;

    // Shows the stats of a specific character selected.
    public VerticalLayoutGroup characterStatsPanelLeft;
    public VerticalLayoutGroup characterStatsPanelRight;
    public Text health;
    public Text exp;
    public Text levelText;

    // Current character portrait.
    public Image portrait;

    private WorldCharacter currCharacter;

    // Use this for initialization
    void Start ()
    {
        // Make sure panel is invisible at the start of game.
        this.gameObject.SetActive(false);
        RectTransform rectTrans = this.GetComponent<RectTransform>();
        gm.uiManager.raycastStoppingUI.Add(rectTrans);
    }
	
	// Update is called once per frame
	void Update () {
        // If current character is not null then update the health bar and experience bar.
        if (currCharacter != null)
        {
            health.text = "HP: " + currCharacter.getCurrHealth() + "/" + currCharacter.getMaxHealth();
            exp.text = "EXP " + currCharacter.experience + "/" + 100;
        }
    }

    public void Populate(WorldCharacter currCharacter)
    {
        this.currCharacter = currCharacter;
        if (currCharacter == null)
        {
            return;
        }

        Depopulate();
        Text stat = (Text)Instantiate(statText, characterStatsPanelLeft.transform);
        stat.text = Constants.instance.MIGHT + ": " + currCharacter.might.ToString();
        stat = (Text)Instantiate(statText, characterStatsPanelLeft.transform);
        stat.text = Constants.instance.MAGIC + ": " + currCharacter.magic.ToString();
        stat = (Text)Instantiate(statText, characterStatsPanelLeft.transform);
        stat.text = Constants.instance.SKILL + ": " + currCharacter.skill.ToString();
        stat = (Text)Instantiate(statText, characterStatsPanelLeft.transform);
        stat.text = Constants.instance.SPEED + ": " + currCharacter.speed.ToString();
        stat = (Text)Instantiate(statText, characterStatsPanelLeft.transform);
        stat.text = Constants.instance.DEF + ": " + currCharacter.defense.ToString();
        stat = (Text)Instantiate(statText, characterStatsPanelLeft.transform);
        stat.text = Constants.instance.RESIST + ": " + currCharacter.resistance.ToString();
        stat = (Text)Instantiate(statText, characterStatsPanelLeft.transform);
        stat.text = Constants.instance.LUCK + ": " + currCharacter.luck.ToString();
        stat = (Text)Instantiate(statText, characterStatsPanelLeft.transform);
        stat.text = Constants.instance.MOVE + ": " + currCharacter.GetBaseMovement().ToString() + (currCharacter.GetMovementBuff() > 0 ? " +" + currCharacter.GetMovementBuff().ToString() : "");

        // Set portrait of current character.
        portrait.sprite = currCharacter.portrait;
        characterNameField.text = currCharacter.characterName;

        // Scale all children correctly cause unity is bad.
        gm.uiManager.NormalizeChildrenScale(characterStatsPanelLeft.gameObject);

        // Apply all buffs to extra stats
        string dmg = (currCharacter.DamageType() == Weapon.DAMAGE_TYPE.PHYSICAL ? currCharacter.PhysicalDamage().ToString() : currCharacter.MagicalDamage().ToString())+
            ((currCharacter.GetDamageBuff()) > 0 ? " +" + currCharacter.GetDamageBuff().ToString() : "");
        string hit = currCharacter.ToHit().ToString() + (currCharacter.GetHitBuff() > 0 ? " +" + currCharacter.GetHitBuff().ToString() : "");
        string dodge = currCharacter.ToDodge().ToString() + (currCharacter.GetDodgeBuff() > 0 ? " +" + currCharacter.GetDodgeBuff().ToString() : "");
        string crit = currCharacter.CritChance().ToString();
        stat = (Text)Instantiate(statText, characterStatsPanelRight.transform);
        stat.text = Constants.instance.DAMAGE + ": " + dmg;
        stat.color = Constants.instance.damageColor;
        stat = (Text)Instantiate(statText, characterStatsPanelRight.transform);
        stat.text = Constants.instance.HIT + ": " + hit;
        stat.color = Constants.instance.hitColor;
        stat = (Text)Instantiate(statText, characterStatsPanelRight.transform);
        stat.text = Constants.instance.DODGE + ": " + dodge;
        stat.color = Constants.instance.dodgeColor;
        stat = (Text)Instantiate(statText, characterStatsPanelRight.transform);
        stat.text = Constants.instance.CRIT + ": " + crit;

        // Set the level text of the character.
        levelText.text = currCharacter.level.ToString();

        // Scale all children correctly cause unity is bad.
        gm.uiManager.NormalizeChildrenScale(characterStatsPanelRight.gameObject);

        // Make sure panel is visible.
        this.gameObject.SetActive(true);
    }

    public void Depopulate()
    {
        // Destroy all children of first stat panel.
        int childs = characterStatsPanelLeft.transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            GameObject.Destroy(characterStatsPanelLeft.transform.GetChild(i).gameObject);
        }
        childs = characterStatsPanelRight.transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            GameObject.Destroy(characterStatsPanelRight.transform.GetChild(i).gameObject);
        }
    }
}
