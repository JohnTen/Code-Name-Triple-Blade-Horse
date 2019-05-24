using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A data package struct that carries data of each attack
/// </summary>
[System.Serializable]
public struct AttackPackage
{
	public AttackPackage(float hitPointDamage, float enduranceDamage, float hitBackDistance, bool triggerGapStun)
	{
		_hitPointDamage = hitPointDamage;
		_enduranceDamage = enduranceDamage;
		_hitBackDistance = hitBackDistance;
		_triggerGapStun = triggerGapStun;
	}

	public float _hitPointDamage;
	public float _enduranceDamage;
	public float _hitBackDistance;
	public bool _triggerGapStun;
}
