using System.Collections;
using System.Collections.Generic;
using JTUtility;
using UnityEngine;

public class HitBox : MonoBehaviour, IAttackable
{
	[SerializeField] float _meleeDamageMultiplier;
	[SerializeField] float _rangeDamageMultiplier;
	[SerializeField] float _meleeChargeDamageMultiplier;
	[SerializeField] float _rangeChargeDamageMultiplier;
	[SerializeField] float _floatBladeDamageMultiplier;
	[SerializeField] float _staminaRecover;
	[SerializeField] bool _isWeakSpot;

	public event Action<AttackPackage> OnHit;

	public AttackResult ReceiveAttack(ref AttackPackage attack)
	{
		OnHit?.Invoke(attack);
		return AttackResult.Success;
	}
}
