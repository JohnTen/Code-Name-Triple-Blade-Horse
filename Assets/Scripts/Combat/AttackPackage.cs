using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public enum AttackType
{
	Null,
	Melee,
	Range,
	ChargedMelee,
	ChargedRange,
	Float,
}

/// <summary>
/// A data package struct that carries data of each attack
/// </summary>
[System.Serializable]
public struct AttackPackage
{
	public int _hashID;
	public ModifiableValue _hitPointDamage;
	public ModifiableValue _enduranceDamage;
	public ModifiableValue _knockback;
	public ModifiableValue _chargedPercent;
	public bool _triggerGapStagger;
	public string _staggerAnimation;
	public string _gapStaggerAnimation;

	public AttackType _attackType;
	public Vector2 _fromDirection;

	AttackPackage(int hashID, float hitPointDamage, float enduranceDamage, float knockback, 
		float chargedPercent, bool triggerGapStun, AttackType attackType, Vector2 fromDirection,
		string staggerAnimation, string gapStaggerAnimation)
	{
		_hashID = hashID;
		_hitPointDamage = new ModifiableValue(hitPointDamage);
		_enduranceDamage = new ModifiableValue(enduranceDamage);
		_knockback = new ModifiableValue(knockback);
		_chargedPercent = new ModifiableValue(chargedPercent);
		_triggerGapStagger = triggerGapStun;

		_attackType = attackType;
		_staggerAnimation = staggerAnimation;
		_gapStaggerAnimation = gapStaggerAnimation;
		_fromDirection = fromDirection;
	}

	public static AttackPackage CreateNewPackage()
	{
		var hashID = System.Guid.NewGuid().GetHashCode();
		var package = 
			new AttackPackage(hashID, 1, 1, 0, 0, false, AttackType.Null, Vector2.zero, "", "");
		return package;
	}

	public static AttackPackage CreateNewPackage(AttackPackage template)
	{
		var hashID = System.Guid.NewGuid().GetHashCode();
		template._hashID = hashID;
		return template;
	}
}
