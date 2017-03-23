using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class WorldCharacter : MonoBehaviour {

	// Name
	[Tooltip("Name")]
	public string characterName = "";

	// Levels
	[Tooltip("Levels.")]
	public uint level = 1;

	// How much experience they have
	[Tooltip("How much physcical damage this character deals.")]
	public uint experience;

	//how much experience they have accrued throughout the entire game.
	private uint realExperience;

	// How much physcical damage this character deals.
	[Tooltip("How much physcical damage this character deals.")]
	public uint might;

	// How much magical damage this character deals.
	[Tooltip("How much magical damage this character deals.")]
	public uint magic;

	// How easy this character can land hits.
	[Tooltip("How easy this character can land hits.")]
	public uint skill;

	// How easy this character can dodge attacks.
	[Tooltip("How easy this character can dodge attacks.")]
	public uint speed;

	// How much phyiscal damage this character prevents.
	[Tooltip("How much phyiscal damage this character prevents.")]
	public uint defense;

	// How much magical damage this character prevents.
	[Tooltip("How much magical damage this character prevents.")]
	public uint resistance;

	// How easy this character can land critical hits.
	[Tooltip("How easy this character can land critical hits.")]
	public uint luck;

	private Cube futureCube;
	public Cube GetFutureCube() { return futureCube; }
	public void SetFutureCube(Cube c) { futureCube = c; }

	//Get Base Movement
	public uint GetBaseMovement()
	{
		if(MyClass != null)
		{
			return MyClass.getMovement();
		}
		Debug.Log(characterName + " has no class!!!!!!");
		return 5;
	}

	// Calculates how far the character can move based on its class.
	public uint GetMovement()
	{
		return GetBaseMovement() + GetMovementBuff();
	}

	// How much health this character has.
	[Tooltip("How much health this character has.")]
	public uint MaxHealth;

	[Tooltip("Image of this character to show in UI when selected.")]
	public Sprite portrait;

	[Tooltip("Offset of this character and the cube the character is on.")]
	public Vector3 cubeOffset;

	[SerializeField]
	[Tooltip("character's inventory reference goes here.")]
	private Inventory inventory;

    [SerializeField]
	private Class MyClass;
	
    public bool isPlayable = true;

    [SerializeField]
    private StatusBar statusBarPrefab;
    private StatusBar statusBarInstance;
    [SerializeField]
    private Vector3 statusBarOffset = new Vector3(0,9, 0);
    public StatusBar getStatusBar() { return statusBarInstance; }

    public Class getMyClass()
	{
		return MyClass;
	}

	private uint currHealth;

    volatile public bool bIsAlive = true;

	volatile private bool buffed = false;

	public bool IsBuffed() { return buffed; }
	public void SetIsBuffed(bool b) { buffed = b; }

	// Used to change material on character when disabled.
	private MaterialSetter materialSetter;

    private Movement movementComp;

	private uint DamageBuff = 0;
	public void SetDamageBuff(uint b) { DamageBuff = b; }
	public uint GetDamageBuff() { return DamageBuff; }

	private uint HitBuff = 0;
	public void SetHitBuff(uint b) { HitBuff = b; }
	public uint GetHitBuff() { return HitBuff; }

	private uint DodgeBuff = 0;
	public void SetDodgeBuff(uint b) { DodgeBuff = b; }
	public uint GetDodgeBuff() { return DodgeBuff; }

	private uint DamageReductionBuff = 0;
	public void SetDamageReductionBuff(uint b) { DamageReductionBuff = b; }
	public uint GetDamageReductionBuff() { return DamageReductionBuff; }

	private uint MovementBuff = 0;
	public void SetMovementBuff(uint b) { MovementBuff = b; }
	public uint GetMovementBuff() { return MovementBuff; }

	private List<Cube> CanAttackCubes = new List<Cube>();
	private List<Cube> CanMoveCubes = new List<Cube>();

    private GameManager gm;

    void Awake()
    {
        // Add character to playable characters or enemy based on kind of character.
        gm = GameObject.FindObjectOfType<GameManager>();
        if (gm != null)
        {
            if (isPlayable && bIsAlive && !gm.playableCharacters.Contains(this))
            {
                gm.playableCharacters.Add(this);
            }
            else if(!isPlayable && bIsAlive && !gm.enemyCharacters.Contains(this))
            {
                gm.enemyCharacters.Add(this);
            }
        }
    }

	// Use this for initialization
	void Start()
    {
        resetHealth();
        initExperience();
        materialSetter = GetComponent<MaterialSetter>();
        movementComp = GetComponent<Movement>();

        Canvas canvas = GameObject.FindObjectOfType<Canvas>();
        if(canvas != null)
        {
            statusBarPrefab = Resources.Load<StatusBar>("StatusBars");
            if(statusBarPrefab)
            {
                statusBarInstance = Instantiate(statusBarPrefab, canvas.transform, true) as StatusBar;
                statusBarInstance.SetParentCharacter(this);
                statusBarInstance.offset = statusBarOffset;
                // Set color of status bar to correct team color
                statusBarInstance.teamPointer.color = (isPlayable) ? Constants.instance.allyTeamColor : Constants.instance.enemyTeamColor;
                statusBarInstance.transform.SetAsFirstSibling();
            }
            else
            {
                Debug.Log("Status bar resource not loaded.");
            }
        }
    }

	public void ComputeRangeCubes(CubeManager cm)
	{
        if(this.gameObject.activeSelf == false)
        {
            return;
        }

		CanMoveCubes.Clear();
		AIBehaviour ai = GetComponent<AIBehaviour>();
		if(ai != null)
		{
			if(ai.behaviour == AIBehaviour.Behaviour.Stationary)
			{
				CanMoveCubes.Add(GetCurrentCube());
				CanAttackCubes.Clear();
				CanAttackCubes = cm.GetAllAdjacentCubesInWeaponRange(GetCurrentCube(), getInventory().getEquippedWeapon().GetRange(), getInventory().getEquippedWeapon().Group);
				return;
			}
		}
		CanMoveCubes.Add(GetCurrentCube());
		List<Cube> newCubes = cm.BFS(GetCurrentCube(), (int)GetMovement(), isPlayable);
		foreach(Cube c in newCubes)
		{
			if(c.IsMovable())
			{
				CanMoveCubes.Add(c);
			}
		}
		CanAttackCubes.Clear();
		if(CanMoveCubes != null)
		{
			foreach(Cube c in CanMoveCubes)
			{
				if (c.GetOccupant() == null || (c.GetOccupant().GetComponent<WorldCharacter>() == this))
				{
					Weapon weaponEquipped = getInventory().getEquippedWeapon();
					if (weaponEquipped != null)
					{
						List<Cube> attackCubes = cm.GetAllAdjacentCubesInWeaponRange(c, weaponEquipped.GetRange(), getInventory().getEquippedWeapon().Group);
						foreach (Cube aC in attackCubes)
						{
							if(aC.IsMovable())
							{
								CanAttackCubes.Add(aC);
							}
						}
					}
				}
			}
		}
	}

	public void ShowRangeCubes(CubeManager cm, SelectionManager sm)
	{
		cm.SetSelectionStateOfCubes(CanAttackCubes, Cube.SelectionState.InAttackRange);
		cm.SetSelectionStateOfCubes(CanMoveCubes, Cube.SelectionState.InMoveRange);
		sm.SetCubesInMoveRange(CanMoveCubes);
	}

	public void HideRangeCubes(CubeManager cm, SelectionManager sm)
	{
		cm.SetSelectionStateOfCubes(CanMoveCubes, Cube.SelectionState.NotSelected);
		cm.SetSelectionStateOfCubes(CanAttackCubes, Cube.SelectionState.NotSelected);
		sm.ClearInRangeMoveCubes();
	}

	public List<Cube> GetCanMoveCubes()
	{
		return CanMoveCubes;
	}

	public List<Cube> GetCanAttackCubes()
	{
		return CanAttackCubes;
	}

    public Inventory getInventory()
    {
        return inventory;
    }

    public uint getCurrHealth()
	{
		return currHealth;
	}

    public uint getMaxHealth()
    {
        return MaxHealth;
    }

	public void resetHealth()
	{
		currHealth = MaxHealth;
	}

	public bool TakeDamage(uint dam)
	{
		int newHealth = (int)currHealth - (int)dam;
		if(newHealth <= 0)
		{
			currHealth = 0;
			return true;
		}
		currHealth = (uint)newHealth;
		return false;
	}

	public void HealDamage(uint heal)
	{
		uint newHealth = currHealth + heal;
		if(newHealth >= MaxHealth)
		{
			currHealth = MaxHealth;
		}
		else
		{
			currHealth = MaxHealth;
		}
	}

	public Weapon.DAMAGE_TYPE DamageType()
	{
		return inventory.getEquippedWeapon().DamageType;
	}

	public uint PhysicalDamage()
	{
		return this.might + inventory.getEquippedWeapon().Damage;
	}

	public uint MagicalDamage()
	{
		return this.magic + inventory.getEquippedWeapon().Damage;
	}

	public uint ToHit()
	{
		uint weaponHit = this.inventory.getEquippedWeapon().Hit;
		uint output = weaponHit + 2*this.skill + this.luck;
		return output;
	}

	public uint ToDodge()
	{
		return 2 * this.speed + this.luck;
	}

	public uint CritChance()
	{
		return this.skill / 2 + inventory.getEquippedWeapon().Crit;
	}

	public uint CritEvade()
	{
		return this.luck;
	}

    // Get current cube that the character is on.
    public Cube GetCurrentCube()
    {
        Cube currCube = null;
        if(movementComp != null)
        {
            currCube = movementComp.GetCurrentCube();
        }
        return currCube;
    }

	public void Die()
	{
        if(movementComp != null)
        {
            Destroy(statusBarInstance.gameObject);
            movementComp.GetCurrentCube().SetOccupant(null);
        }else
        {
            Debug.Log("Movement component null");
        }
		this.gameObject.SetActive(false);
        bIsAlive = false;

		if(isPlayable)
		{
			gm = GameObject.FindObjectOfType<GameManager>();
			if(!gm.playableCharacters.Remove(this))
			{
				Debug.Log("How in the hell");
			}
		}
		else
		{
			gm = GameObject.FindObjectOfType<GameManager>();
			if (!gm.enemyCharacters.Remove(this))
			{
				Debug.Log("How in the hell");
			}
		}
	}

	public void RestorePosition()
	{
        if(movementComp != null)
        {
            movementComp.SnapToCube(movementComp.GetCurrentCube());
        }
        else
        {
            Debug.Log("Movement component null");
        }
    }

	// Update is called once per frame
	void Update () {

    }

	private void initExperience()
	{
		realExperience = 100 * (level - 1) + experience;
	}

	public uint gainExperience(uint exp)
	{
		realExperience += exp;

		uint levelsGained = (realExperience / 100) + 1 - level;
		level = (realExperience / 100) + 1;
		experience = realExperience % 100;
		return levelsGained;
	}

	public void Reset()
	{
		this.gameObject.GetComponent<Movement>().ResetPrevCube();
	}

    public void ShowDisabled()
    {
        if (materialSetter != null)
        {
            materialSetter.ShowDisabledMaterial();
        }
    }

    public void ShowEnabled()
    {
        if (materialSetter != null)
        {
            materialSetter.ShowEnabledMaterial();
        }
    }

    public bool GetEnabled()
    {
        if (materialSetter != null)
        {
            return materialSetter.isEnabled;
        }
        return true;
    }

	public void Face(WorldCharacter other)
	{
		Vector3 towards = other.gameObject.transform.position - this.gameObject.transform.position;
		towards.y = 0.0f;
		towards.Normalize();
		float angle = Mathf.Acos(Vector3.Dot(towards, Vector3.forward)) * Mathf.Rad2Deg;
		Vector3 cross = Vector3.Cross(towards, Vector3.forward);
		cross.Normalize();
		float dir = -cross.y;
		if(dir < 0)
		{
			angle = -angle;
		}
		this.gameObject.transform.eulerAngles = new Vector3(0.0f, angle, 0.0f);
	}
}
