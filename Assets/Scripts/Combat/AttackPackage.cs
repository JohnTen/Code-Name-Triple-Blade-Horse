using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

/// <summary>
/// A data package struct that carries data of each attack
/// </summary>
[System.Serializable]
public struct AttackPackage
{
	public int _hashID;
	public ModifiableValue _hitPointDamage;
	public ModifiableValue _enduranceDamage;
	public ModifiableValue _hitBackDistance;
	public bool _triggerGapStun;

	public bool _isChargedAttack;
	public bool _isMeleeAttack;
	public Vector2 _fromDirection;

	public AttackPackage(int hashID, float hitPointDamage, float enduranceDamage, float hitBackDistance, bool triggerGapStun, bool isChargedAttack, bool isMeleeAttack, Vector2 fromDirection)
	{
		_hashID = hashID;
		_hitPointDamage = new ModifiableValue(hitPointDamage);
		_enduranceDamage = new ModifiableValue(enduranceDamage);
		_hitBackDistance = new ModifiableValue(hitBackDistance);
		_triggerGapStun = triggerGapStun;

		_isChargedAttack = isChargedAttack;
		_isMeleeAttack = isMeleeAttack;
		_fromDirection = fromDirection;
	}

	public static AttackPackage CreateNewPackage()
	{
		var hashID = System.Guid.NewGuid().GetHashCode();
		var package = new AttackPackage(hashID, 0, 0, 0, false, false, false, Vector2.zero);
		return package;
	}

	public static AttackPackage CreateNewPackage(AttackPackage template)
	{
		var hashID = System.Guid.NewGuid().GetHashCode();
		template._hashID = hashID;
		return template;
	}
}
