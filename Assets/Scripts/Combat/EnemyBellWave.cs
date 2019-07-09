using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TripleBladeHorse.Combat
{
	public class EnemyBellWave : BaseWeapon
	{
		[SerializeField] float _attackDuration;
		[SerializeField] float _maxSize;
		[SerializeField] SpriteRenderer _attackShape;
		[SerializeField] Gradient _color;

		float _timer;

		public override void Activate(AttackPackage attack, AttackMove move)
		{
			base.Activate(attack, move);
			this.transform.localScale = Vector3.zero;
		}

		public override void Deactivate()
		{
			base.Deactivate();
			Destroy(this.gameObject);
		}

		private void Update()
		{
			_timer += TimeManager.DeltaTime;
			this.transform.localScale = Vector3.one * (Mathf.Lerp(0, _maxSize, _timer / _attackDuration));
			_attackShape.color = _color.Evaluate(_timer / _attackDuration);

			if (_timer >= _attackDuration)
				Deactivate();
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			var attackable = collision.GetComponentInParent<IAttackable>();
			var toTarget = collision.transform.position - this.transform.position;
			var direction = DirectionalHelper.NormalizeHorizonalDirection(toTarget);
			TryAttack(attackable, direction);
		}
	}
}
