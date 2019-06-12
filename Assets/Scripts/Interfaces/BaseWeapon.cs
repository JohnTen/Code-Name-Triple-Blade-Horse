using System.Collections;
using System.Collections.Generic;
using JTUtility;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour, IAttacker
{
	[SerializeField] protected float _baseHitPointDamage;
	[SerializeField] protected float _baseEnduranceDamage;

	public event Action<IAttackable, AttackResult, AttackPackage> OnHit;

	protected void RaiseOnHitEvent(IAttackable attackable, AttackResult result, AttackPackage attack)
	{
		OnHit?.Invoke(attackable, result, attack);
	}

	public abstract void Activate(AttackPackage attack);
	public abstract void Deactivate();
	public abstract AttackPackage Process(AttackPackage target);
}
