using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    [SerializeField]
    GameManager gm;

    [Tooltip("All the weapons this character has.")]
    public List<Weapon> Weapons;

    [Tooltip("Limit to how many weapons player can carry.")]
    public uint weaponLimit = 3;

    [Tooltip("Hand socket that has the current weapon equipped.")]
    public Transform weaponHand;

    private uint equippedIndex = 0;

    private WorldCharacter character;

    // Use this for initialization
    void Start ()
    {
        character = GetComponent<WorldCharacter>();
        // Create a temp list to throw all instantiated weapons into before overriding the Weapons list that contains PREFABS.
        // Doing this because PREFABS should not and cannot be editted but are used to determine which weapons player starts with.
        List<Weapon> weaponsTemp = new List<Weapon>();
        // Create all weapons that were initially given to the character and only show the equipped one.
        for(int i = 0; i < Weapons.Count; i++)
        {
            Weapon weaponPrefab = Weapons[i];
            if(weaponPrefab != null)
            {
                Weapon weapon = (Weapon)Instantiate(weaponPrefab, weaponHand.transform, false);
                weapon.SnapToOffset();
                weaponsTemp.Add(weapon);
                // Set all weapons that are not the first one to not be visible because first one is the equipped one.
                if(i != equippedIndex) 
                {
                    weapon.gameObject.SetActive(false);
                }
            }
        }
        // Replace the weapons list that is built of prefabs to the runtime instantiated weapons.
        Weapons = weaponsTemp;
    }
	
	// Update is called once per frame
	void Update () {
        if(character != null)
        {
                uint elementToSwap = equippedIndex;
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    equipWeapon(0);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    equipWeapon(1);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    equipWeapon(2);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    equipWeapon(3);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    equipWeapon(4);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    equipWeapon(5);
                }
        }
    }

    public void equipWeapon(uint elementToSwap)
    {
        if (gm.selectionManager.GetCurrentSelectedCharacter() != null && character == gm.selectionManager.GetCurrentSelectedCharacter())
        {
            if (elementToSwap != equippedIndex && elementToSwap < Weapons.Count && !gm.battleManager.InCombat && gm.turnManager.getPlayerTurnPhase() != TurnManager.PlayerTurnPhase.Attack)
            {
                if (!setEquippedWeapon(elementToSwap))
                {
                    gm.audioManager.Play2DSound(Constants.instance.ErrorSound, 1.0f, false);
                }
                else
                {
                    gm.audioManager.Play2DSound(gm.audioManager.uiDownwardSound, 1.0f, false);
                    gm.uiManager.getInventoryPanel().Populate(this);
                    gm.uiManager.getCharacterPanel().Populate(character);

                    if (gm.turnManager.getPlayerTurnPhase() == TurnManager.PlayerTurnPhase.Move)
                    {
                        character.HideRangeCubes(gm.cubeManager, gm.selectionManager);
                        character.ComputeRangeCubes(gm.cubeManager);
                        character.ShowRangeCubes(gm.cubeManager, gm.selectionManager);
                    }

                    else if (gm.turnManager.getPlayerTurnPhase() == TurnManager.PlayerTurnPhase.PreAttack)
                    {
                        gm.selectionManager.ClearInRangeAttackCubes();
                        gm.selectionManager.ShowCubesInShortestPath();
                        gm.selectionManager.ShowInRangeAttackCubes();
                    }
                    // Update attack panel if the weapon changes during showing the attack panel.
                    else if (gm.turnManager.getPlayerTurnPhase() == TurnManager.PlayerTurnPhase.Attack)
                    {
                        gm.battleManager.ShowBattleParameters();
                    }
                }
            }
        }
    }

    private void swapWeaponElements(uint i, uint j)
    {
        if (i == j) return;
        Weapon temp = Weapons[(int)i];
        Weapons[(int)i] = Weapons[(int)j];
        Weapons[(int)j] = temp;
    }

    private bool setEquippedWeapon(uint newWeaponIndex)
    {
        // No reason to swap equipped if selected the same index that is already equipped.
        if(newWeaponIndex == equippedIndex)
        {
            return true;
        }

        if(newWeaponIndex >= Weapons.Count)
        {
            Debug.Log("Equip weapon request failed because index out of bounds");
            return false;
        }



		Weapon newWeapon = Weapons[(int)newWeaponIndex];

		if (character && character.getMyClass() && character.getMyClass().getWeaponTypes() != null && newWeapon != null && !character.getMyClass().getWeaponTypes().Contains(newWeapon.SpecificType))
		{
			return false;
		}


		// Hide the weapon that was previously equipped.
		Weapon currWeapon = getEquippedWeapon();
		if (currWeapon != null)
        {
            currWeapon.gameObject.SetActive(false);
        }

        equippedIndex = newWeaponIndex;
        // Show the new weapon by loading into character hand and set active cause object is just hidden at this point.
        if (newWeapon != null)
        {
            // Snap the weapon to the hand correctly.
            newWeapon.gameObject.transform.parent = weaponHand;
            newWeapon.SnapToOffset();
            newWeapon.gameObject.SetActive(true);
        }



		return true;
    }

    public Weapon getEquippedWeapon()
    {
        if(Weapons.Count <= 0)
        {
            return null;
        }
        return Weapons[(int)equippedIndex];
    }

    public void storeObject(Pickup pickUp)
    {
        //Check to see what kind of pickup we picked up.
        Weapon weapon = pickUp.GetComponent<Weapon>();
        if(weapon != null)
        {
            // Go through weapons and place new weapon in first open slot.
            if (Weapons.Count < weaponLimit)
            {
                Weapons.Add(weapon);
                //gm.audioManager.Play2DSound(gm.audioManager.pickUpSound, 1.0f, false);
                // Update the inventory if we are the current character.
                if(gm.selectionManager.GetCurrentSelectedCharacter().gameObject == this.gameObject)
                {
                    gm.uiManager.getInventoryPanel().Populate(this);
                }
            }
            else
            {
                Debug.Log("cannot equip weapon since character weapons is full");
            }
        }
    }
}
