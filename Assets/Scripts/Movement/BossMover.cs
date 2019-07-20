using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse.Movement
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class BossMover : MonoBehaviour, ICanMove
	{
		public enum MoveMode
		{
			Slow,
			Quick,
			Back
		}

		[SerializeField] float _slowSpeed = 8;
		[SerializeField] float _quickSpeed = 8;
		[SerializeField] float _backSpeed = 8;

		[Header("Debug")]
		[SerializeField] float _baseSpeed = 8;
		[SerializeField] bool _constantMoving;
		[SerializeField] Vector2 _velocity;
		[SerializeField] Vector2 _movementVector;
		[SerializeField] MovingState _currentMovingState;

		Rigidbody2D _rigidbody;

		ParticleSystem.MinMaxCurve _constantMovingSpeedCurve;
		Vector2 _constantMoveVelocity;
		bool _usingCurveForConstantMoving;
		float _originalGravityScale;
		MoveMode _moveMode;
		Vector2 _extraMovement;

		CharacterState _state;
		List<ContactPoint2D> _contacts;

		public Vector2 Velocity => _velocity;
		public bool IsConstantMoving => _constantMoving;
		public MoveMode Movemode
		{
			get => _moveMode;
			set
			{
				_moveMode = value;
				switch (value)
				{
					case MoveMode.Back: _baseSpeed = _backSpeed; break;
					case MoveMode.Slow: _baseSpeed = _slowSpeed; break;
					case MoveMode.Quick: _baseSpeed = _quickSpeed; break;
				}
			}
		}

		public MovingState CurrentMovingState
		{
			get => _currentMovingState;
			protected set
			{
				if (value == _currentMovingState) return;

				var last = _currentMovingState;
				_currentMovingState = value;

				OnMovingStateChanged?.
					Invoke(this, new MovingEventArgs(
						_state._facingRight,
						transform.position,
						_velocity,
						value,
						last));
			}
		}
		
		public event Action<ICanChangeMoveState, MovingEventArgs> OnMovingStateChanged;

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody2D>();
			_state = GetComponent<EnemyState>();
			_contacts = new List<ContactPoint2D>();
		}

		private void FixedUpdate()
		{
			_rigidbody.GetContacts(_contacts);

			if (!IsConstantMoving)
				Moving(_movementVector);

			CurrentMovingState =
				(_velocity.sqrMagnitude > 0.1f || _movementVector.sqrMagnitude > 0.1f) ?
				MovingState.Move :
				MovingState.Idle;
		}

		public void Move(Vector2 direction)
		{
			if (!IsConstantMoving)
			_movementVector = direction;
		}

		public void ManualMove(Vector2 toward)
		{
			_extraMovement += toward;
		}

		public void InvokeConstantMovement(Vector3 velocity, float time)
		{
			if (IsConstantMoving) return;

			_originalGravityScale = _rigidbody.gravityScale;
			_rigidbody.gravityScale = 0;

			_usingCurveForConstantMoving = false;
			_constantMoveVelocity = velocity;
			StartCoroutine(ConstantMoving(time));
		}

		public void InvokeConstantMovement(Vector3 direction, ParticleSystem.MinMaxCurve speed, float time)
		{
			if (IsConstantMoving) return;

			_originalGravityScale = _rigidbody.gravityScale;
			_rigidbody.gravityScale = 0;

			_usingCurveForConstantMoving = true;
			_constantMoveVelocity = direction.normalized;
			_constantMovingSpeedCurve = speed;
			StartCoroutine(ConstantMoving(time));
		}

		public void InterruptContantMove()
		{
			if (!IsConstantMoving) return;

			StopAllCoroutines();
			_velocity = Vector3.zero;
			_constantMoving = false;
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.gravityScale = _originalGravityScale;
		}

		private Vector2 ApplyPhysicalContactEffects(Vector2 velocity)
		{
			foreach (var contact in _contacts)
			{
				var d = contact.normal * velocity;
				if (d.x < 0)
					velocity.x -= d.x * Mathf.Sign(contact.normal.x);
				if (d.y < 0)
					velocity.y -= d.y * Mathf.Sign(contact.normal.y);
			}

			return velocity;
		}

		private void Moving(Vector3 direction)
		{
			_velocity = ProcessVelocity(_velocity, direction);

			_rigidbody.MovePosition((Vector2)transform.position + _velocity * TimeManager.DeltaTime);

			ResetPhysicalContacts();
		}

		private Vector2 ProcessVelocity(Vector2 velocity, Vector2 movingDirection)
		{
			velocity.y += _rigidbody.gravityScale * Physics2D.gravity.y * TimeManager.DeltaTime;
			velocity = ApplyPhysicalContactEffects(velocity);
			
			velocity.x = movingDirection.x * _baseSpeed;

			return velocity;
		}

		private void ResetPhysicalContacts()
		{
			_contacts.Clear();
		}

		IEnumerator ConstantMoving(float time)
		{
			var timer = 0f;
			var wait = new WaitForFixedUpdate();
			_constantMoving = true;

			while (timer < time)
			{
				var percentage = timer / time;
				_velocity = _constantMoveVelocity;

				if (_usingCurveForConstantMoving)
				{
					_velocity *= _constantMovingSpeedCurve.Evaluate(percentage);
				}

				_velocity = ApplyPhysicalContactEffects(_velocity);
				_extraMovement = ApplyPhysicalContactEffects(_extraMovement);
				var movement = (Vector2)transform.position + _velocity * TimeManager.DeltaTime;
				movement += _extraMovement;
				_rigidbody.MovePosition(movement);
				ResetPhysicalContacts();

				timer += TimeManager.DeltaTime;
				_extraMovement = Vector2.zero;
				yield return wait;
			}

			_rigidbody.gravityScale = _originalGravityScale;
			_constantMoving = false;
		}
	}
}
