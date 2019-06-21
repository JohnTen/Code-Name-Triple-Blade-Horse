using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBellWave : BaseWeapon
{
	[SerializeField] float _attackDuration;
	[SerializeField] float _maxSize;
	[SerializeField] float _knockback;
	[SerializeField] SpriteRenderer _attackShape;
	[SerializeField] Gradient _color;

	bool _activated;
	float _timer;
	AttackPackage _basePackage;

	public override void Activate(AttackPackage attack, AttackMove move)
	{
		_activated = true;
		_basePackage = attack;
		_basePackage._faction = Faction.Enemy;
		this.transform.localScale = Vector3.zero;
	}

	public override void Deactivate()
	{
		_activated = false;
		Destroy(this.gameObject);
	}

	private AttackPackage Process(AttackPackage target)
	{
		target._hitPointDamage += _baseHitPointDamage;
		target._enduranceDamage += _baseEnduranceDamage;
		target._attackRate = 1;
		target._attackType = AttackType.Melee;
		target._knockback += _knockback;

		return target;
	}

	private void Update()
	{
		_timer += Time.deltaTime;
		this.transform.localScale = Vector3.one * (Mathf.Lerp(0, _maxSize, _timer / _attackDuration));
		_attackShape.color = _color.Evaluate(_timer / _attackDuration);

		if (_timer >= _attackDuration)
			Deactivate();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		var attackable = collision.GetComponent<IAttackable>();
		if (attackable == null
		 || attackable.Faction == Faction.Enemy
		 || attackable.Faction == Faction.Null)
		{
			return;
		}

		var package = Process(_basePackage);
		var toTarget = collision.transform.position.x - this.transform.position.x;
		package._fromDirection = toTarget > 0 ? Vector2.right : Vector2.left;
		
		var result = attackable.ReceiveAttack(package);
		RaiseOnHitEvent(attackable, result, package);
	}
}
