using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{

	public GameManager gm;

	public BouncingText damageText;

	public ExperiencePanel expPanel;

	public bool InCombat = false;

	private uint PlayerExperienceGain = 0;

	[SerializeField]
	private uint expGainBase = 10;

	[SerializeField]
	private uint heightAdvantagePerBlock = 10;

	private class combat
	{
		public combat(WorldCharacter a, WorldCharacter d, bool b)
		{
			attacker = a;
			defender = d;
			PlayerCombat = b;
		}
		public WorldCharacter attacker;
		public WorldCharacter defender;
		public bool PlayerCombat;
	}

	private WorldCharacter player;
	private WorldCharacter enemy;

	public enum whoAttacks { player, enemy };
	private whoAttacks initiator;

	private Queue<combat> combatOrder = new Queue<combat>();

	public volatile bool disabled = false;



	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	private void AddCombatTurn(WorldCharacter attacker, WorldCharacter defender)
	{
		if(GetIfAttacksAtAll(attacker, defender))
		{
			combatOrder.Enqueue(new combat(attacker, defender, attacker == player));
		}
	}

	public void SetCombatants(WorldCharacter player, WorldCharacter enemy, whoAttacks attacker)
	{
		this.player = player;
		this.enemy = enemy;
		this.initiator = attacker;
		combatOrder.Clear();
		if (initiator == whoAttacks.player)
		{
			AddCombatTurn(player, enemy);
			AddCombatTurn(enemy, player);
		}
		else if (initiator == whoAttacks.enemy)
		{
			AddCombatTurn(enemy, player);
			AddCombatTurn(player, enemy);

		}

		if (GetIfAttackTwice(player, enemy))
		{
			AddCombatTurn(player, enemy);
		}
		else if (GetIfAttackTwice(enemy, player))
		{
			AddCombatTurn(enemy, player);
		}
	}

	public class BattleParameters
	{
		public readonly string name;
		public readonly string weaponName;
		public readonly uint currHealth;
		public readonly uint damage;
		public readonly int damageBuff;
		public readonly uint hit;
		public readonly int hitBuff;
		public readonly uint crit;
		public readonly bool attacksTwice;
		public readonly bool attacksAtAll;
		public BattleParameters(string name, string weaponName, uint currHealth, uint damage, int damageBuff, uint hit, int hitBuff, uint crit, bool attacksTwice, bool attacksAtAll)
		{
			this.name = name;
			this.weaponName = weaponName;
			this.currHealth = currHealth;
			this.damage = damage;
			this.damageBuff = damageBuff;
			this.hit = hit;
			this.hitBuff = hitBuff;
			this.crit = crit;
			this.attacksTwice = attacksTwice;
			this.attacksAtAll = attacksAtAll;
		}
	};

	public void ShowBattleParameters()
	{
		if(player == null || enemy == null)
		{
			return;
		}
		BattleParameters playerParams = new BattleParameters(player.characterName, player.getInventory().getEquippedWeapon().Name, player.getCurrHealth(), GetDamage(player, enemy), GetAttackerDamageBuff(player, enemy), GetToHit(player, enemy), GetAttackerHitBuff(player, enemy), GetToCrit(player, enemy), GetIfAttackTwice(player, enemy), GetIfAttacksAtAll(player, enemy));
		BattleParameters enemyParams = new BattleParameters(enemy.characterName, enemy.getInventory().getEquippedWeapon().Name, enemy.getCurrHealth(), GetDamage(enemy, player), GetAttackerDamageBuff(enemy, player), GetToHit(enemy, player), GetAttackerHitBuff(enemy, player), GetToCrit(enemy, player), GetIfAttackTwice(enemy, player), GetIfAttacksAtAll(enemy, player));
		gm.uiManager.getAttackPanel().Populate(playerParams, enemyParams);
	}

	public void RemoveCombatants()
	{
		this.player = null;
		this.enemy = null;
	}

	public void BeginCombat()
	{

		InCombat = true;
		PlayerExperienceGain = 0;
		gm.selectionManager.ClearInRangeAttackCubes();
		player.Face(enemy);
		enemy.Face(player);
		player.HideRangeCubes(gm.cubeManager, gm.selectionManager);
		enemy.HideRangeCubes(gm.cubeManager, gm.selectionManager);

		StartCoroutine(Combat());

	}

	private IEnumerator WaitForAnimationWhilePlaying(WorldCharacter w, AnimationController.AnimState a)
	{
		AnimationController cont = w.GetComponent<AnimationController>();
		cont.PlayAnimation(a);
		float waitTime = cont.GetAnimationLength(a);
		yield return new WaitForSeconds(waitTime);
	}

	private IEnumerator Combat()
	{
		if (combatOrder.Count == 0)
		{
			yield return StartCoroutine(ToEndCombat(1.0f));
            yield break;
		}

		combat curr = combatOrder.Dequeue();
		WorldCharacter attacker = curr.attacker;
		WorldCharacter defender = curr.defender;

		gm.cameraManager.LockCamera(attacker);
		bool defenderDied = false;
		if (PercentageChance(GetToCrit(attacker, defender)))
		{
			yield return StartCoroutine(WaitForAnimationWhilePlaying(attacker, AnimationController.AnimState.HeavyAttack));

			gm.cameraManager.LockCamera(defender);

			uint damage = GetDamage(attacker, defender);
			int damageBuff = GetAttackerDamageBuff(attacker, defender);
			int realDamage = (int)damage + damageBuff;
			realDamage = Mathf.Clamp(realDamage, 0, 100);
			damage = (uint)realDamage;
			damage *= 3;
			if (damage == 0)
			{
				gm.audioManager.Play2DSound(Constants.instance.blockSound, 0.8f, false);
				damageText.ShowText("No Damage", defender.transform.position);
				yield return StartCoroutine(WaitForAnimationWhilePlaying(defender, AnimationController.AnimState.Block));
			}
			else
			{
				gm.audioManager.Play2DSound(Constants.instance.heavyDamageSound, 0.8f, false);
				damageText.ShowText(damage.ToString() + "!", defender.transform.position);
				defenderDied = defender.TakeDamage(damage);
				if (curr.PlayerCombat)
				{
					PlayerExperienceGain = expGainBase;
				}
			}

		}
		else if (PercentageChance((uint)Mathf.Clamp((int)GetToHit(attacker, defender) + GetAttackerHitBuff(attacker, defender), 0, 100)))
		{
			yield return StartCoroutine(WaitForAnimationWhilePlaying(attacker, AnimationController.AnimState.Attack));

			gm.cameraManager.LockCamera(defender);

			uint damage = GetDamage(attacker, defender);
			int damageBuff = GetAttackerDamageBuff(attacker, defender);
			int realDamage = (int)damage + damageBuff;
			realDamage = Mathf.Clamp(realDamage, 0, 100);
			damage = (uint)realDamage;
			if (damage == 0)
			{
				gm.audioManager.Play2DSound(Constants.instance.blockSound, 0.8f, false);
				damageText.ShowText("No Damage", defender.transform.position);
			}
			else
			{
				gm.audioManager.Play2DSound(Constants.instance.damageSound, 0.8f, false);
				damageText.ShowText(damage.ToString(), defender.transform.position);
				defenderDied = defender.TakeDamage(damage);
				if (curr.PlayerCombat)
				{
					PlayerExperienceGain = expGainBase;
				}
			}
		}
		else
		{
			yield return StartCoroutine(WaitForAnimationWhilePlaying(attacker, AnimationController.AnimState.Attack));

			gm.cameraManager.LockCamera(defender);

			gm.audioManager.Play2DSound(Constants.instance.dodgeSound, 0.8f, false);
			damageText.ShowText("Miss", defender.transform.position);
			yield return StartCoroutine(WaitForAnimationWhilePlaying(defender, AnimationController.AnimState.Dodge));
		}

		if (defenderDied)
		{
			PlayerExperienceGain *= 3;
			yield return StartCoroutine(WaitForAnimationWhilePlaying(defender, AnimationController.AnimState.Die));
			defender.Die();
			combatOrder.Clear();
		}

		StartCoroutine(Combat());

	}

	private IEnumerator GainExperience(uint exp)
	{
		expPanel.enabled = true;
		yield return StartCoroutine(expPanel.Show(player));
		uint initLevel = player.level;
		uint levelsGained = 0;
		for(uint i = 0; i < exp; i++)
		{
			levelsGained += player.gainExperience(1);
			yield return new WaitForSeconds(0.01f);
		}
		yield return StartCoroutine(expPanel.Hide());
		expPanel.enabled = false;
		for(uint i = 0; i < levelsGained; i++)
		{
			LevelingUp l = player.gameObject.GetComponent<LevelingUp>();
			yield return StartCoroutine(l.LevelUp(initLevel + i));
		}
	}

	private IEnumerator ToEndCombat(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		if(PlayerExperienceGain > 0 && player.bIsAlive)
		{
			yield return StartCoroutine(GainExperience(PlayerExperienceGain));
		}
		EndCombat();
	}

	private bool PercentageChance(uint chance)
	{
		int rand = UnityEngine.Random.Range(1, 100);
		return rand <= (int)chance;
	}

	private void EndCombat()
	{

		if (player.bIsAlive)
		{
			if (initiator == whoAttacks.player)
			{
				gm.turnManager.DisableCharacter();
			}
		}
		if (enemy.bIsAlive)
		{
			if (initiator == whoAttacks.enemy)
			{
				enemy.ShowDisabled();
			}
		}

		if(gm.turnManager.PlayablesLeft() && gm.turnManager.EnemiesLeft())
		{
			gm.cameraManager.FreeCamera();
		}
		InCombat = false;
	}

	private uint GetDamage(WorldCharacter attacker, WorldCharacter defender)
	{
		if (attacker.getInventory().getEquippedWeapon() == null)
		{
			return 0;
		}
		int damage = 0;
		if (attacker.DamageType() == Weapon.DAMAGE_TYPE.PHYSICAL)
		{
			damage = (int)attacker.PhysicalDamage() - (int)defender.defense;
		}
		else if (attacker.DamageType() == Weapon.DAMAGE_TYPE.MAGICAL)
		{
			damage = (int)attacker.MagicalDamage() - (int)defender.resistance;
		}
		//damage = damage + (int)attacker.GetDamageBuff() - (int)defender.GetDamageReductionBuff();
		if (damage < 0)
		{
			damage = 0;
		}
		return (uint)damage;
	}

	private int GetAttackerDamageBuff(WorldCharacter attacker, WorldCharacter defender)
	{
		int damage = (int)attacker.GetDamageBuff() - (int)defender.GetDamageReductionBuff();

		return damage;
	}

	private uint GetToHit(WorldCharacter attacker, WorldCharacter defender)
	{
		if (attacker.getInventory().getEquippedWeapon() == null)
		{
			return 0;
		}
		int ToHit = (int)attacker.ToHit() - (int)defender.ToDodge();
		int heightAdvantage = gm.cubeManager.GetHeightDifference(attacker.GetFutureCube(), defender.GetFutureCube());
		ToHit += (int)heightAdvantagePerBlock * heightAdvantage;
		int terrainAdvantage = (int)defender.GetFutureCube().hitDifficulty;
		ToHit -= terrainAdvantage;
		ToHit = Mathf.Clamp(ToHit, 0, 100);
		return (uint)ToHit;
	}

	private int GetAttackerHitBuff(WorldCharacter attacker, WorldCharacter defender)
	{
		int hit = (int)attacker.GetHitBuff() - (int)defender.GetDodgeBuff();
		return hit;
	}

	private uint GetToCrit(WorldCharacter attacker, WorldCharacter defender)
	{
		if (attacker.getInventory().getEquippedWeapon() == null)
		{
			return 0;
		}
		int ToCrit = (int)attacker.CritChance() - (int)defender.CritEvade();
		ToCrit = Mathf.Clamp(ToCrit, 0, 100);
		return (uint)ToCrit;
	}

	private bool GetIfAttackTwice(WorldCharacter attacker, WorldCharacter defender)
	{
		int speedDiff = (int)attacker.speed - (int)defender.speed;
		return speedDiff >= 5;
	}

	private bool GetIfAttacksAtAll(WorldCharacter attacker, WorldCharacter defender)
	{
		List<Cube> cubesInAttackRange = gm.cubeManager.GetAllAdjacentCubesInWeaponRange(attacker.GetFutureCube(), attacker.getInventory().getEquippedWeapon().GetRange(), attacker.getInventory().getEquippedWeapon().Group);
		return cubesInAttackRange.Contains(defender.GetFutureCube());
	}

	public void SetEnemyBeingAttacked(bool b)
	{
		if(b)
		{
			enemy.GetCurrentCube().SetSelectedState(Cube.SelectionState.Attacking);
		}
		else
		{
			enemy.GetCurrentCube().SetSelectedState(Cube.SelectionState.NotSelected);
		}
	}
}
