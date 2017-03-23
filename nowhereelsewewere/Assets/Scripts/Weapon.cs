using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Weapon: MonoBehaviour
{
	public enum DAMAGE_TYPE { PHYSICAL, MAGICAL};
	public enum GROUP {  MELEE, RANGED, MAGIC };
	public enum SPECIFIC_TYPE { BLUGEONER, PIERCER, SLASHER, BOWS, CROSSBOWS, THROWN, NATURE, LIGHT, DARK };

	public string Name;
	public DAMAGE_TYPE DamageType;
	public GROUP Group;
	public SPECIFIC_TYPE SpecificType;
	public uint Tier;
    public uint Damage;
	public uint Hit;
	public uint Crit;
    public uint[] Range;

    public Sprite portrait;
    public Vector3 rotationOffset;
    public Vector3 positionOffset;
    public void SnapToOffset()
    {
        transform.localRotation = Quaternion.Euler(rotationOffset.x, rotationOffset.y, rotationOffset.z);
        transform.localPosition = positionOffset;
    }

    public void SetRange(uint[] r) { Range = r; }
    public uint[] GetRange()
    {
		uint[] r = new uint[Range.Length];
		for (uint i = 0; i < r.Length; i++)
		{
			r[i] = Range[i];
		}
        return r;
    }
};
