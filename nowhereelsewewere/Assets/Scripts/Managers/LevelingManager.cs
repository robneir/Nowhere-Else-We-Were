using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelingManager : MonoBehaviour {

    [SerializeField]
    GameManager gm;

	[SerializeField]
	private int MouseContinueButton = 0;

    [SerializeField]
    private GameObject levelUpPanel;
    [SerializeField]
    private Image characterPortrait;
    [SerializeField]
    private GridLayoutGroup statGridLayout;
    [SerializeField]
    private Text statTextPrefab;
    [SerializeField]
    private AudioClip levelUpMusic;


	// Use this for initialization
	void Start () {
        levelUpPanel.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public IEnumerator DisplayLevelUp(Sprite portrait, 
		uint MaxHPPrevious, uint MaxHPIncrease, 
		uint MightPrevious, uint MightIncrease, 
		uint MagicPrevious, uint MagicIncrease, 
		uint SkillPrevious, uint SkillIncrease,
		uint SpeedPrevious, uint SpeedIncrease,
		uint LuckPrevious, uint LuckIncrease,
		uint DefensePrevious, uint DefenseIncrease,
		uint ResistancePrevious, uint ResistanceIncrease)
	{
        //show level up image
        characterPortrait.sprite = portrait;
        //play level up music
        if(levelUpMusic != null)
        {
            gm.audioManager.Play2DSound(levelUpMusic, 1.0f, false);
        }
        // Destroy all children of level up stat panel before populating in case something got added to it accidentally.
        ClearLevelUpPanel();
        // Player level panel animation to bring into screen.
        Animator levelPanelAnim = levelUpPanel.GetComponent<Animator>();
        if(levelPanelAnim != null)
        {
            levelPanelAnim.SetBool("isCenter", true);
            levelPanelAnim.SetBool("isOut", false);
        }
        // Show panel
        levelUpPanel.SetActive(true);
        //show portrait, and all previous stats (passed as parameters)
        //per stat whose corresponding Increase (passed as parameters; for Magic, MagicIncrease) is greater than 0
        //show a +1 or whatever next to the stat to indicate that that stat has increased
        Text text = (Text)Instantiate(statTextPrefab, statGridLayout.transform);
        text.text = Constants.instance.HP + CreateFormattedStat(MaxHPPrevious, MaxHPIncrease);
        text = (Text)Instantiate(statTextPrefab, statGridLayout.transform);
        text.text = Constants.instance.MIGHT + CreateFormattedStat(MightPrevious, MightIncrease);
        text = (Text)Instantiate(statTextPrefab, statGridLayout.transform);
        text.text = Constants.instance.MAGIC + CreateFormattedStat(MagicPrevious, MagicIncrease);
        text = (Text)Instantiate(statTextPrefab, statGridLayout.transform);
        text.text = Constants.instance.SKILL + CreateFormattedStat(SkillPrevious, SkillIncrease);
        text = (Text)Instantiate(statTextPrefab, statGridLayout.transform);
        text.text = Constants.instance.SPEED + CreateFormattedStat(SpeedPrevious, SpeedIncrease);
        text = (Text)Instantiate(statTextPrefab, statGridLayout.transform);
        text.text = Constants.instance.LUCK + CreateFormattedStat(LuckPrevious, LuckIncrease);
        text = (Text)Instantiate(statTextPrefab, statGridLayout.transform);
        text.text = Constants.instance.DEF + CreateFormattedStat(DefensePrevious, DefenseIncrease);
        text = (Text)Instantiate(statTextPrefab, statGridLayout.transform);
        text.text = Constants.instance.RESIST + CreateFormattedStat(ResistancePrevious, ResistanceIncrease);

        // Normalize all text instantiated in level up panel.
        gm.uiManager.NormalizeChildrenScale(statGridLayout.gameObject);

        //must press mouse button to continue
        while (!Input.GetMouseButtonDown(MouseContinueButton))
		{
			yield return null;
		}
		yield return new WaitForSeconds(1.0f);

        if (levelPanelAnim != null)
        {
            levelPanelAnim.SetBool("isCenter", false);
            levelPanelAnim.SetBool("isOut", true);
        }
        // Hide the level up panel
        //levelUpPanel.SetActive(false);
        // Destroy all children of level up stat panel to clear for next time we populate it.
        ClearLevelUpPanel();
    }

    // Formats a stat string to look correct for level up displaying stat increase.
    private string CreateFormattedStat(uint previousStat, uint statIncrease)
    {
        string formattedString = ": "+ previousStat.ToString();
        if(statIncrease > 0)
        {
            formattedString += "(+" + statIncrease + ")";
        }
        return formattedString;
    }

    private void ClearLevelUpPanel()
    {
        int childs = statGridLayout.transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            GameObject.Destroy(statGridLayout.transform.GetChild(i).gameObject);
        }
    }
}
