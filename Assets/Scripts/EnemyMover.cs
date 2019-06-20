using System.Collections;
using System.Collections.Generic;
using JTUtility;
using UnityEngine;

public class EnemyMover : MonoBehaviour, ICanMove
{
	[Header("Basic movement")]
	[SerializeField] float _baseSpeed;
	[SerializeField] float _jumpHeight;
	[SerializeField] float _knockbackSpeedFactor;

	[Header("Events")]
	[SerializeField] BoolEvent _onChangedFacingDirection;

	[Header("Debug")]
	[SerializeField] Vector2 _velocity;
	[SerializeField] Vector2 _movementVector;
	[SerializeField] Vector2 _knockbackVector;
	[SerializeField] MovingState _currentMovingState;

	EnemyState _state;
	Rigidbody2D _rigidbody;
	ICanDetectGround _groundDetector;
	List<ContactPoint2D> _contacts;

	public event Action<bool> OnChangeDirection;
	public event Action<ICanChangeMoveState, MovingEventArgs> OnMovingStateChanged;

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

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody2D>();
		_state = GetComponent<EnemyState>();
		_contacts = new List<ContactPoint2D>();
	}

	private void FixedUpdate()
	{
		_rigidbody.GetContacts(_contacts);

		Moving(_movementVector);

		CurrentMovingState =
			(_velocity.sqrMagnitude > 0.1f || _movementVector.sqrMagnitude > 0.1f) ?
			MovingState.Move :
			MovingState.Idle;
	}

	public void Jump()
	{
		if (!_groundDetector.IsOnGround) return;
		
		_velocity.y = Mathf.Sqrt(19.62f * _jumpHeight * _rigidbody.gravityScale);

		CurrentMovingState = MovingState.Airborne;
	}

	public void Move(Vector2 direction)
	{
		_movementVector = direction;

		if (_knockbackVector.sqrMagnitude > 0)
		{
			var knockback = _knockbackVector.normalized * _knockbackSpeedFactor;
			if (_knockbackVector.sqrMagnitude < knockback.sqrMagnitude)
			{
				_knockbackVector = Vector2.zero;
			}
			else
			{
				_knockbackVector -= knockback;
			}

			_movementVector += knockback;
		}
		
		if (direction.x != 0)
		{
			_state._facingRight = direction.x > 0;
			OnChangeDirection?.Invoke(direction.x > 0);
			_onChangedFacingDirection.Invoke(direction.x > 0);
		}
	}

	public void Knockback(Vector2 direction)
	{
		_knockbackVector = Vector2.right * (direction.magnitude * Mathf.Sign(direction.x));
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
		
		_rigidbody.MovePosition((Vector2)transform.position + _velocity * Time.deltaTime);
		
		ResetPhysicalContacts();
	}

	private void Knockbacking()
	{

	}

	private Vector2 ProcessVelocity(Vector2 velocity, Vector2 movingDirection)
	{
		velocity.y += _rigidbody.gravityScale * Physics2D.gravity.y * Time.deltaTime;
		velocity = ApplyPhysicalContactEffects(velocity);
		
		velocity.x = movingDirection.x * _baseSpeed;

		return velocity;
	}

	private void ResetPhysicalContacts()
	{
		_contacts.Clear();
	}
}
