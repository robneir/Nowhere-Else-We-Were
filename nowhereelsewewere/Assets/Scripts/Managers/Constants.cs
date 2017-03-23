using UnityEngine;
using System.Collections;

public class Constants: MonoBehaviour { 
    public static Constants instance = null;

    public Color inMoveRangeColor;
    public Color notSelectedColor;
    public Color selectedColor;
    public Color inPathColor;
    public Color inAttackRangeColor;
    public Color attackingColor;

	public AudioClip playerPhaseGameMusic;
	public AudioClip enemyMusic;
	public AudioClip combatMusic;
	public AudioClip rainForestSounds;

	public AudioClip damageSound;
	public AudioClip blockSound;
	public AudioClip heavyDamageSound;
	public AudioClip dodgeSound;

    public AudioClip UISoftPlasticClick;
    public AudioClip UIVintageHardClick;

	public AudioClip silence;

	public AudioClip LumiusActivated;
	public AudioClip ErrorSound;
	public AudioClip VictorySound;
	public AudioClip DefeatSound;

    public Color HighHealthColor;
    public Color MedHealthColor;
    public Color LowHealthColor;

    public Color hoverSpriteColor;
    public Color selectionSpriteColor;

    // String representations of stats to maintain consistency throughout game.
    public string MIGHT = "MIGHT";
    public string MAGIC = "MAGIC";
    public string SKILL = "SKILL";
    public string SPEED = "SPD";
    public string DEF = "DEF";
    public string RESIST = "RESIST";
    public string LUCK = "LUCK";
    public string MOVE = "MOVE";
    public string HP = "HP";
    public string DODGE = "DODGE";

    //Weapon stats
    public string NAME = "NAME";
    public string DAMAGE = "DMG";
    public string RANGE = "RNG";
    public string CRIT = "CRIT";
    public string HIT = "HIT";

    // For the inventory panel
    public Color equippedBackgroundColor;
    public Color notEquippedBackgroundColor;

    public Color enemyHealthColor;
    public Color playerHealthColor;

    public Color damageColor;
    public Color hitColor;
    public Color dodgeColor;
    public Color damagerReduceColor;
    public Color moveColor;

    public Color enemyTeamColor;
    public Color allyTeamColor;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else if(instance != this)
        {
            Destroy(gameObject);
        }
    }
}
