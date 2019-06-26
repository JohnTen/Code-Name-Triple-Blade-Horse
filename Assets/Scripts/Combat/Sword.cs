using System.Collections;
using System.Collections.Generic;
using JTUtility;
using UnityEngine;

namespace TripleBladeHorse.Combat
{
	public class Sword : BaseWeapon
	{
		[SerializeField] Collider2D _triggerBox;
		[SerializeField] PlayerState _state;

		public override void Activate(AttackPackage attack, AttackMove move)
		{
			base.Activate(attack, move);
			_triggerBox.enabled = true;
		}

		public override void Deactivate()
		{
			base.Deactivate();
			_triggerBox.enabled = false;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			var attackable = other.GetComponent<IAttackable>();
			var attackDirection = _state._facingRight ? Vector2.right : Vector2.left;
			TryAttack(attackable, attackDirection);
		}
	}
}
