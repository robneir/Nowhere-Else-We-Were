using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AttackPanel : UIPanel {

    [SerializeField]
    GameManager gm;

    [SerializeField]
    private Vector3 attackStatsOffsetFromPlayer;
    [SerializeField]
    private Text statTextCentered;
    [SerializeField]
    private VerticalLayoutGroup playerStats;
    [SerializeField]
    private VerticalLayoutGroup enemyStats;
    [SerializeField]
    private Text playerName;
    [SerializeField]
    private Text enemyName;
    [SerializeField]
    private Text playerWeapon;
    [SerializeField]
    private Text enemyWeapon;
    [SerializeField]
    private Button attackButton;

    // Use this for initialization
    void Start ()
    {
        this.gameObject.SetActive(false);
        // Add UI elements to the list of UI that does not raycast go through.
        RectTransform rectTrans = attackButton.GetComponent<RectTransform>();
        gm.uiManager.raycastStoppingUI.Add(rectTrans);
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Snap the attack stats panel close to character.
        gm.uiManager.SnapUIToCharacterWithOffset(this.gameObject, attackStatsOffsetFromPlayer);
    }

    private string GetBuffString(uint baseVal, int buff)
    {
        if (buff > 0)
        {
            return baseVal.ToString() + " +" + buff.ToString();
        }
        else if (buff < 0)
        {
            return baseVal.ToString() + " " + buff.ToString();
        }
        else
        {
            return baseVal.ToString();
        }
    }

   // public void Populate(string playerName, string playerWeapon, uint playerHealth, uint playerDamage, int playerDamageBuff, uint playerHit, int playerHitBuff, uint playerCrit, bool playerAttacksTwice,
    //    string enemyName, string enemyWeapon, uint enemyHealth, uint enemyDamage, int enemyDamageBuff, uint enemyHit, int enemyHitBuff, uint enemyCrit, bool enemyAttacksTwice)
	public void Populate(BattleManager.BattleParameters playerParam, BattleManager.BattleParameters enemyParam)
    {
        // Destroy all children of player stat panel.
        int childs = playerStats.transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            GameObject.Destroy(playerStats.transform.GetChild(i).gameObject);
        }
        gm.uiManager.NormalizeChildrenScale(playerStats.gameObject);
        // Destroy all children of enemy stat panel.
        childs = enemyStats.transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            GameObject.Destroy(enemyStats.transform.GetChild(i).gameObject);
        }
        gm.uiManager.NormalizeChildrenScale(enemyStats.gameObject);

        populatePanel(playerStats, playerName, playerWeapon, playerParam);
		populatePanel(enemyStats, enemyName, enemyWeapon, enemyParam);

		// Bind the attack button to fire begin combat event
		attackButton.onClick.AddListener(() => gm.selectionManager.FireBeginCombatEvent());
        // Show the attack stats panel
        this.gameObject.SetActive(true);
    }

	private void populatePanel(VerticalLayoutGroup stats, Text name, Text WeaponName, BattleManager.BattleParameters battleParams)
	{
		Text stat = (Text)Instantiate(statTextCentered, stats.transform);
		stat.text = battleParams.currHealth.ToString();
		stat = (Text)Instantiate(statTextCentered, stats.transform);
		stat.text = battleParams.attacksAtAll ? GetBuffString(battleParams.damage, battleParams.damageBuff) + (battleParams.attacksTwice ? " (x2)" : "") : "---";
		stat = (Text)Instantiate(statTextCentered, stats.transform);
		stat.text = battleParams.attacksAtAll ? GetBuffString(battleParams.hit, battleParams.hitBuff) : "---";
		stat = (Text)Instantiate(statTextCentered, stats.transform);
		stat.text = battleParams.attacksAtAll ? battleParams.crit.ToString() : "---";
		name.text = battleParams.name;
		WeaponName.text = battleParams.weaponName;

        // Normalize the UI 
        gm.uiManager.NormalizeChildrenScale(stats.gameObject);
	}
}
