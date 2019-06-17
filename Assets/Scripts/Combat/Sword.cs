using System.Collections;
using System.Collections.Generic;
using JTUtility;
using UnityEngine;

public class Sword : BaseWeapon
{
	[SerializeField] float _maxChargeAttack;
	[SerializeField] Collider2D _triggerBox;
	[SerializeField] PlayerState _state;
	[SerializeField] AttackMove _normalAttack;
	[SerializeField] AttackMove _chargedAttack;

	bool _activated;
	AttackPackage _basePackage;
	AttackMove _attackMove;

	public override void Activate(AttackPackage attack, AttackMove move)
	{
		_basePackage = attack;
		_attackMove = move;
		_activated = true;
		_triggerBox.enabled = true;
	}

	public override void Deactivate()
	{
		_activated = false;
		_triggerBox.enabled = false;
	}

	private AttackPackage Process(AttackPackage target)
	{
		target._hitPointDamage += _baseHitPointDamage;
		target._enduranceDamage += _baseEnduranceDamage;
		
		target = _attackMove.Process(target);

		return target;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		var attackable = other.GetComponent<IAttackable>();
		if (!_activated || attackable == null) return;

		var package = Process(_basePackage);
		package._fromDirection = _state._facingRight ? Vector2.right : Vector2.left;

		var result = attackable.ReceiveAttack(ref package);
		RaiseOnHitEvent(attackable, result, package);
	}
}
