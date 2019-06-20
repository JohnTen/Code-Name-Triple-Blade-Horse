using System.Collections;
using System.Collections.Generic;
using JTUtility;
using UnityEngine;

public class HitBox : MonoBehaviour, IAttackable
{
	[SerializeField] protected Faction _faction;
	[SerializeField] protected float _meleeDamageMultiplier;
	[SerializeField] protected float _rangeDamageMultiplier;
	[SerializeField] protected float _meleeChargeDamageMultiplier;
	[SerializeField] protected float _rangeChargeDamageMultiplier;
	[SerializeField] protected float _floatBladeDamageMultiplier;
	[SerializeField] protected float _knifeStabDamageMultiplier;
	[SerializeField] protected float _staminaRecover;
	[SerializeField] protected bool _isWeakSpot;

	[Header("Debug")]
	[SerializeField] protected Dictionary<int, float> attacksFreqTimer;

	public Faction Faction => _faction;
	public event Action<AttackPackage, AttackResult> OnHit;

	public HitBox()
	{
		attacksFreqTimer = new Dictionary<int, float>();
	}

	public virtual AttackResult ReceiveAttack(AttackPackage attack)
	{
		print(attack._faction);
		if (attacksFreqTimer.ContainsKey(attack._hashID)
		 || attack._attackType == AttackType.Null
		 || attack._faction == this.Faction)
			return AttackResult.Failed;

		attacksFreqTimer.Add(attack._hashID, attack._attackRate);
		StartCoroutine(RemoveTimer(attack._hashID));

		var result = new AttackResult
		{
			_attackSuccess = true,
			_finalDamage = attack._hitPointDamage,
			_finalFatigue = attack._enduranceDamage,
			_isWeakspot = _isWeakSpot
		};

		ApplyDamageMultiplier(ref result, attack);

		RaiseOnHit(attack, result);
		return result;
	}

	protected void RaiseOnHit(AttackPackage attack, AttackResult result)
	{
		OnHit?.Invoke(attack, result);
	}

	protected virtual void ApplyDamageMultiplier(ref AttackResult result, AttackPackage attack)
	{
		switch (attack._attackType)
		{
			case AttackType.Melee:
				result._finalDamage *= _meleeDamageMultiplier;
				break;
			case AttackType.Range:
				result._finalDamage *= _rangeDamageMultiplier;
				break;
			case AttackType.ChargedMelee:
				result._finalDamage *= _meleeChargeDamageMultiplier;
				break;
			case AttackType.ChargedRange:
				result._finalDamage *= _rangeChargeDamageMultiplier;
				break;
			case AttackType.Float:
				result._finalDamage *= _floatBladeDamageMultiplier;
				break;
		}
	}

	protected virtual IEnumerator RemoveTimer(int id)
	{
		while (attacksFreqTimer[id] > 0)
		{
			attacksFreqTimer[id] -= Time.deltaTime;
			yield return null;
		}

		attacksFreqTimer.Remove(id);
	}
}
