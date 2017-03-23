using UnityEngine;
using System.Collections;

public class LevelingUp : MonoBehaviour {

	[SerializeField]
	private GameManager gm;

	[SerializeField]
	private WorldCharacter character;

	[SerializeField]
	private uint MaxHPGrowth = 0;

	[SerializeField]
	private uint MightGrowth = 0;

	[SerializeField]
	private uint MagicGrowth = 0;

	[SerializeField]
	private uint SkillGrowth = 0;

	[SerializeField]
	private uint SpeedGrowth = 0;

	[SerializeField]
	private uint LuckGrowth = 0;

	[SerializeField]
	private uint DefenseGrowth = 0;

	[SerializeField]
	private uint ResistanceGrowth = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public IEnumerator LevelUp(uint prevLevel)
	{
		uint MaxHPPrevious = character.MaxHealth;
		uint MaxHPIncrease = StatIncreases(MaxHPGrowth);

		uint MightPrevious = character.might;
		uint MightIncrease = StatIncreases(MightGrowth);

		uint MagicPrevious = character.magic;
		uint MagicIncrease = StatIncreases(MagicGrowth);

		uint SkillPrevious = character.skill;
		uint SkillIncrease = StatIncreases(SkillGrowth);

		uint SpeedPrevious = character.speed;
		uint SpeedIncrease = StatIncreases(SpeedGrowth);

		uint LuckPrevious = character.luck;
		uint LuckIncrease = StatIncreases(LuckGrowth);

		uint DefensePrevious = character.defense;
		uint DefenseIncrease = StatIncreases(DefenseGrowth);

		uint ResistancePrevious = character.resistance;
		uint ResistanceIncrease = StatIncreases(ResistanceGrowth);


		character.MaxHealth += MaxHPIncrease;
		character.might += MightIncrease;
		character.magic += MagicIncrease;
		character.skill += SkillIncrease;
		character.speed += SpeedIncrease;
		character.luck += LuckIncrease;
		character.defense += DefenseIncrease;
		character.resistance += ResistanceIncrease;


		yield return StartCoroutine((gm.levelingManager.DisplayLevelUp(character.portrait,
			MaxHPPrevious, MaxHPIncrease, 
			MightPrevious, MightIncrease, 
			MagicPrevious, MagicIncrease, 
			SkillPrevious, SkillIncrease, 
			SpeedPrevious, SpeedIncrease, 
			LuckPrevious, LuckIncrease,
			DefensePrevious, DefenseIncrease,
			ResistancePrevious, ResistanceIncrease)));
	}

	private uint StatIncreases(uint statChance)
	{
		uint output = statChance / 100u;
		output += (PercentageChance(statChance % 100u) ? 1u : 0u);
		return output;
	}

	private bool PercentageChance(uint chance)
	{
		int rand = UnityEngine.Random.Range(1, 100);
		return rand <= (int)chance;
	}
}
