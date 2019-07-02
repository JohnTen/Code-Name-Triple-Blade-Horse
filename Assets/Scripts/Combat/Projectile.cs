using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse.Combat
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class Projectile : BaseWeapon
	{
		[SerializeField] float _existTime;
		[SerializeField] ParticleSystem.MinMaxCurve _speed;
		[SerializeField] ParticleSystem.MinMaxCurve _torque;
		[SerializeField] bool _followTarget;
		[SerializeField] bool _rotateTowardsFlyingDirection;
		[SerializeField] GameObject _destroyEffectPrefab;

		[Header("Debug")]
		[SerializeField] Transform _target;
		[SerializeField] float _existTimer;
		[SerializeField] Vector2 _direction;
		Rigidbody2D _rigidbody;

		public event Action<Projectile, Collider2D> OnDestorying;

		public Transform Target
		{
			get => _target;
			set => _target = value;
		}

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody2D>();
		}

		private void FixedUpdate()
		{
			_existTimer += Time.deltaTime;

			Move();

			if (_existTimer >= _existTime)
			{
				DestroySelf(null);
			}
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			var attackable = collision.collider.GetComponentInParent<IAttackable>();
			var toTarget = collision.transform.position - this.transform.position;
			var direction = DirectionalHelper.NormalizeHorizonalDirection(toTarget);

			if (attackable != null && attackable.Faction == Faction)
			{
				Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
				return;
			}

			TryAttack(attackable, direction);

			DestroySelf(collision);
			return;
		}

		private void Move()
		{
			var existPercent = _existTimer / _existTime;
			var currentSpeed = _speed.Evaluate(existPercent);

			if (_followTarget && Target != null)
			{
				_direction = UpdateDirection(_direction);
			}

			Vector3 currentVelocity = _direction * currentSpeed;

			_rigidbody.MovePosition(transform.position + currentVelocity * Time.deltaTime);
		}

		private void DestroySelf(Collision2D collision)
		{
			Deactivate();

			if (_destroyEffectPrefab)
			{
				Instantiate(_destroyEffectPrefab, this.transform.position, this.transform.rotation);
			}

			OnDestorying?.Invoke(this, collision.collider);
			Destroy(this.gameObject);
		}

		Vector2 UpdateDirection(Vector2 direction)
		{
			var existPercent = _existTimer / _existTime;
			var toTarget = Target.position - transform.position;
			var currentTorque = _torque.Evaluate(existPercent) * Time.deltaTime;

			toTarget.z = 0;
			direction = Vector3.RotateTowards(direction, toTarget, currentTorque * Time.deltaTime, 0);

			return direction;
		}

		public void InitializeDirection(Vector2 direction)
		{
			_direction = direction.normalized;
			if (_rotateTowardsFlyingDirection)
				transform.right = _direction;
		}
	}
}
