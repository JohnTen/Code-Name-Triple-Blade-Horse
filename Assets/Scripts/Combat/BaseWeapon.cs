using System.Collections;
using System.Collections.Generic;
using JTUtility;
using UnityEngine;

namespace TripleBladeHorse.Combat
{
	public abstract class BaseWeapon : MonoBehaviour, IAttacker
	{
		[SerializeField] protected float _baseHitPointDamage;
		[SerializeField] protected float _baseEnduranceDamage;
		[SerializeField] protected float _baseAttackRate = 1;
		[SerializeField] protected float _knockback;
		[SerializeField] protected AttackType _defaultType;
		[SerializeField] protected Faction _faction;

		protected bool _activated;
		protected AttackPackage _baseAttack;
		protected AttackMove _attackMove;

		public Faction Faction
		{
			get => _faction;
			set => _faction = value;
		}

		public event Action<IAttackable, AttackResult, AttackPackage> OnHit;

		protected void RaiseOnHitEvent(IAttackable attackable, AttackResult result, AttackPackage attack)
		{
			OnHit?.Invoke(attackable, result, attack);
		}

		public virtual void Activate(AttackPackage attack, AttackMove move)
		{
			_activated = true;
			_baseAttack = AttackPackage.CreateNewPackage(attack);
			_attackMove = move;
		}

		public virtual void Deactivate()
		{
			_activated = false;
		}

		protected virtual AttackPackage Process(AttackPackage target)
		{
			target._hitPointDamage.Base += _baseHitPointDamage;
			target._enduranceDamage.Base += _baseEnduranceDamage;
			target._knockback.Base += _knockback;
			target._attackRate = _baseAttackRate;
			target._attackType = _defaultType;
			target._faction = Faction;

			print(target._hitPointDamage);
			if (_attackMove != null)
			{
				target = _attackMove.Process(target);
			}

			print(target._hitPointDamage);
			return target;
		}

		protected virtual bool TryAttack(IAttackable attackable, Vector2 attackDirection)
		{
			if (!IsAttackable(attackable)) return false;

			var package = Process(_baseAttack);
			package._fromDirection = attackDirection;

			var result = attackable.ReceiveAttack(package);
			RaiseOnHitEvent(attackable, result, package);

			return true;
		}

		protected virtual bool IsAttackable(IAttackable attackable)
		{
			return
				_activated &&
				attackable != null &&
				attackable.Faction != Faction &&
				attackable.Faction != Faction.Null;
		}
	}
}
