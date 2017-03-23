using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Class : MonoBehaviour {
	[SerializeField]
	private string className = "";

	public string getName()
	{
		return className;
	}

	// How far this character can move per turn
	[SerializeField]
	private uint movement;

	public uint getMovement()
	{
		return movement;
	}

	[SerializeField]
	private List<Weapon.SPECIFIC_TYPE> weaponTypes = new List<Weapon.SPECIFIC_TYPE>();

	public List<Weapon.SPECIFIC_TYPE> getWeaponTypes()
	{
		List<Weapon.SPECIFIC_TYPE> output = new List<Weapon.SPECIFIC_TYPE>();
		foreach(Weapon.SPECIFIC_TYPE type in weaponTypes)
		{
			output.Add(type);
		}
		return output;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
