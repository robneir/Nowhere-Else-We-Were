using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LumiusManager : MonoBehaviour {

	public enum LumiusStats { Damage, Hit, Dodge, DamageReduction, Movement };

	private Dictionary<WorldCharacter, ParticleSystem> particleMap = new Dictionary<WorldCharacter, ParticleSystem>();

    public bool CanUseLumius = false;

    [SerializeField]
    private Text LumiusText;

	[SerializeField]
	private BouncingText LumiusIndicator;

	[SerializeField]
	private uint DamageBuff = 0;

	public uint GetDamageBuff() { return DamageBuff; }

	[SerializeField]
	private ParticleSystem DamageBuffParticlePrefab;

	[SerializeField]
	private KeyCode DamageBuffKey;

	[SerializeField]
	private uint HitBuff = 0;

	public uint GetHitBuff() { return HitBuff; }

	[SerializeField]
	private ParticleSystem HitBuffParticlePrefab;

	[SerializeField]
	private KeyCode HitBuffKey;

	[SerializeField]
	private uint DodgeBuff = 0;

	public uint GetDodgeBuff() { return DodgeBuff; }

	[SerializeField]
	private ParticleSystem DodgeBuffParticlePrefab;

	[SerializeField]
	private KeyCode DodgeBuffKey;

	[SerializeField]
	private uint DamageReductionBuff = 0;

	public uint GetDamageReductionBuff() { return DamageReductionBuff; }

	[SerializeField]
	private ParticleSystem DamageReductionBuffParticlePrefab;

	[SerializeField]
	private KeyCode DamageReductionBuffKey;

	[SerializeField]
	private uint MovementBuff = 0;

	public uint GetMovementBuff() { return MovementBuff; }

	[SerializeField]
	private ParticleSystem MovementBuffParticlePrefab;

	[SerializeField]
	private KeyCode MovementBuffKey;

	[SerializeField]
	private uint LumiusMax = 0;

	private uint currLumius = 0;

	[SerializeField]
	private GameManager gm;

	// Use this for initialization
	void Start () {
		currLumius = LumiusMax;
        LumiusText.text = "Lumius: " + currLumius.ToString();
    }
	
	// Update is called once per frame
	void Update () {
        if(!CanUseLumius)
        {
            return;
        }
		if(Input.GetKeyDown(DamageBuffKey))
		{
			ApplyBuff(LumiusStats.Damage);
		}
		if (Input.GetKeyDown(HitBuffKey))
		{
			ApplyBuff(LumiusStats.Hit);
		}
		if (Input.GetKeyDown(DodgeBuffKey))
		{
			ApplyBuff(LumiusStats.Dodge);
		}
		if (Input.GetKeyDown(DamageReductionBuffKey))
		{
			ApplyBuff(LumiusStats.DamageReduction);
		}
		if (Input.GetKeyDown(MovementBuffKey))
		{
			ApplyBuff(LumiusStats.Movement);
		}
	}

    private void ApplyDamageBuff()
    {

    }

	private bool SpendLumius(uint l)
	{
		int newLumius = (int)currLumius - (int)l;
		if(newLumius < 0)
		{
			newLumius = 0;
			return false;
		}
		currLumius = (uint)newLumius;
		return true;
	}

	public uint GetCurrLumius()
	{
		return currLumius;
	}

	public uint GetLumiusMax()
	{
		return LumiusMax;
	}

	private bool SpendLumius(WorldCharacter w, LumiusStats stat)
	{
		bool output = SpendLumius(1);
		if(!output)
		{
			return false;
		}
		
		switch (stat)
		{
			case LumiusStats.Damage:
				w.SetDamageBuff(DamageBuff);
				ParticleSystem bpDamage = (ParticleSystem)Instantiate(DamageBuffParticlePrefab, w.transform);
				bpDamage.gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
				particleMap.Add(w, bpDamage);
				LumiusIndicator.gameObject.SetActive(true);
				TextMesh tm = LumiusIndicator.GetComponent<TextMesh>();
				if(tm != null)
				{
					tm.color = bpDamage.startColor;
				}
				LumiusIndicator.ShowText("More Damage!", w.gameObject.transform.position);
				break;
			case LumiusStats.Hit:
				w.SetHitBuff(HitBuff);
				ParticleSystem bpHit = (ParticleSystem)Instantiate(HitBuffParticlePrefab, w.transform);
				bpHit.gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
				particleMap.Add(w, bpHit);
				LumiusIndicator.gameObject.SetActive(true);
				tm = LumiusIndicator.GetComponent<TextMesh>();
				if (tm != null)
				{
					tm.color = bpHit.startColor;
				}
				LumiusIndicator.ShowText("More Hit Chance!", w.gameObject.transform.position);
				break;
			case LumiusStats.Dodge:
				w.SetDodgeBuff(DodgeBuff);
				ParticleSystem bpDodge = (ParticleSystem)Instantiate(DodgeBuffParticlePrefab, w.transform);
				bpDodge.gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
				particleMap.Add(w, bpDodge);
				LumiusIndicator.gameObject.SetActive(true);
				tm = LumiusIndicator.GetComponent<TextMesh>();
				if (tm != null)
				{
					tm.color = bpDodge.startColor;
				}
				LumiusIndicator.ShowText("Easier To Dodge!", w.gameObject.transform.position);
				break;
			case LumiusStats.DamageReduction:
				w.SetDamageReductionBuff(DamageReductionBuff);
				ParticleSystem bpDamageReduction = (ParticleSystem)Instantiate(DamageReductionBuffParticlePrefab, w.transform);
				bpDamageReduction.gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
				particleMap.Add(w, bpDamageReduction);
				LumiusIndicator.gameObject.SetActive(true);
				tm = LumiusIndicator.GetComponent<TextMesh>();
				if (tm != null)
				{
					tm.color = bpDamageReduction.startColor;
				}
				LumiusIndicator.ShowText("Take Less Damage!", w.gameObject.transform.position);
				break;
			case LumiusStats.Movement:
				w.SetMovementBuff(MovementBuff);
				ParticleSystem bpMovement = (ParticleSystem)Instantiate(MovementBuffParticlePrefab, w.transform);
				bpMovement.gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
				particleMap.Add(w, bpMovement);
				LumiusIndicator.gameObject.SetActive(true);
				tm = LumiusIndicator.GetComponent<TextMesh>();
				if (tm != null)
				{
					tm.color = bpMovement.startColor;
				}
				LumiusIndicator.ShowText("Move Farther!", w.gameObject.transform.position);
				break;
		}
		w.SetIsBuffed(true);

		return true;
	}

	public void ApplyBuff(LumiusStats stat)
    {
        WorldCharacter w = gm.selectionManager.GetCurrentSelectedCharacter();
        if (w != null && gm.playableCharacters.Contains(w) && w.bIsAlive)
		{
			if (w.IsBuffed())
			{
				gm.audioManager.Play2DSound(Constants.instance.ErrorSound, 0.75f, false);
			}
			else
			{
				if (SpendLumius(w, stat))
				{
					gm.audioManager.Play2DSound(Constants.instance.LumiusActivated, 0.75f, false);
					gm.battleManager.ShowBattleParameters();
					w.ComputeRangeCubes(gm.cubeManager);
					w.ShowRangeCubes(gm.cubeManager, gm.selectionManager);
					gm.uiManager.getCharacterPanel().Populate(w);
					
                    LumiusText.text = "Lumius: " + currLumius.ToString();

					//METRICS
					if (MetricManager.instance)
					{
						MetricManager.instance.AddLumiusUsed(1);
					}
				}
				else
				{
					gm.audioManager.Play2DSound(Constants.instance.ErrorSound, 0.75f, false);
				}
			}
		}
	}

	public void ResetBuffs(WorldCharacter w)
	{
		w.SetDamageBuff(0);
		w.SetHitBuff(0);
		w.SetDodgeBuff(0);
		w.SetDamageReductionBuff(0);
		w.SetMovementBuff(0);
		if(particleMap.ContainsKey(w))
		{
			GameObject.Destroy(particleMap[w]);
			particleMap.Remove(w);
		}
		w.SetIsBuffed(false);
	}
}
