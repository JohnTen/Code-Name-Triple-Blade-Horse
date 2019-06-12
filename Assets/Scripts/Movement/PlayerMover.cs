using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMover : MonoBehaviour, ICanMove, ICanJump, ICanDash
{
	#region Fields
	[SerializeField] bool _frozen;
	[Header("Basic movement")]
	[SerializeField] float _baseSpeed;
	[SerializeField] float _maxXSpeed;
	[SerializeField] float _maxYSpeed;
	[SerializeField] float _jumpHeight;

	[Header("Dashing")]
	[SerializeField] float _airDashingDistance;
	[SerializeField] float _airDashingDuration;
	[SerializeField] float _airDashingDelay;
	[SerializeField] float _groundDashingDistance;
	[SerializeField] float _groundDashingDuration;
	[SerializeField] float _groundDashingDelay;
	[SerializeField] Vector2 _dashingDirection;

	[Header("Attacking Step(Unadjustable, Debug only)")]
	[SerializeField] float _attackStepSpeed;
	[SerializeField] Vector2 _attackStepDirection;

	[Header("Airborne Movement")]
	[SerializeField] float _airborneSpeedFactor;
	[SerializeField] float _pullingForce;
	[SerializeField] float _pullingMoveDelay;
	[SerializeField] float _airborneXDecayRate = 0.02f;
	[SerializeField] float _airborneXMovementTimeFactor = 0.5f;
	[SerializeField] AnimationCurve _airborneXMovementCurve;

	[Header("Events")]
	[SerializeField] BoolEvent _onChangedFacingDirection;

	Rigidbody2D _rigidbody;
	ICanDetectGround _groundDetector;
	Vector2 _movementVector;

	Vector2 _velocity;
	bool _dashing;
	float _gravityScale;
	float _airborneTime;
	float _deservedDashingDistance;
	float _dashingDelayTimer;
	float _dashingSpeed;

	List<ContactPoint2D> _contacts;

	readonly Vector2[] _normalizedDirections =
	{
		new Vector2( 0,  1),
		new Vector2( 1,  1).normalized,
		new Vector2( 1,  0),
		new Vector2( 1, -1).normalized,
		new Vector2( 0, -1),
		new Vector2(-1, -1).normalized,
		new Vector2(-1,  0),
		new Vector2(-1,  1).normalized,
	};
	#endregion

	#region Properties
	public Vector2 FacingDirection { get; private set; } = Vector2.right;
	public bool IsOnGround => _groundDetector.IsOnGround;
	public bool IsDashing => _dashing;
	public Vector2 Velocity => _velocity;

	#endregion

	#region Events
	public event Action<bool> OnChangeDirection;
	public event Action OnBlocked;
	public event Action OnJump;
	public event Action OnDashingBegin;
	public event Action OnDashingDelayBegin;
	public event Action OnDashingFinished;
	#endregion

	#region Event Handlers

	private void GroundDetectorOnLandingHandler()
	{
		_airborneTime = 0;
	}

	#endregion

	#region Unity Messages

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody2D>();
		_groundDetector = GetComponent<ICanDetectGround>();
		_groundDetector.OnLanding += GroundDetectorOnLandingHandler;
		_contacts = new List<ContactPoint2D>();
	}

	private void FixedUpdate()
	{
		if (!_groundDetector.IsOnGround)
			_airborneTime += Time.fixedDeltaTime;

		Moving(_movementVector);
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		UpdatePhysicalContacts(collision.contacts);
	}

	#endregion

	#region Interfaces

	public void Dash(Vector3 direction)
	{
		_dashing = true;
		_dashingDirection = NormalizeMovingDirection(direction);
		_gravityScale = _rigidbody.gravityScale;
		_rigidbody.gravityScale = 0;

		if (!_groundDetector.IsOnGround || _dashingDirection.y > 0)
		{
			_dashingDelayTimer = _airDashingDelay;
			_deservedDashingDistance = _airDashingDistance;
			_dashingSpeed = _airDashingDistance / _airDashingDuration;
		}
		else
		{
			_dashingDelayTimer = _groundDashingDelay;
			_deservedDashingDistance = _groundDashingDistance;
			_dashingSpeed = _groundDashingDistance / _groundDashingDuration;
		}

		OnDashingBegin?.Invoke();
	}

	public void Jump()
	{
		if (!_groundDetector.IsOnGround) return;

		OnJump?.Invoke();
		_velocity.y = Mathf.Sqrt(19.62f * _jumpHeight * _rigidbody.gravityScale);
	}

	public void Move(Vector3 direction)
	{
		_movementVector = direction;
		if (direction.x != 0)
		{
			FacingDirection = direction.x > 0 ? Vector2.right : Vector2.left;
			OnChangeDirection?.Invoke(direction.x > 0);
			_onChangedFacingDirection.Invoke(direction.x > 0);
		}
	}

	public void Pull(Vector3 direction)
	{
		_velocity = direction.normalized * _pullingForce;
		_airborneTime = 0;
	}

	public void SetStepDistance(float stepLength)
	{
		_attackStepDirection = FacingDirection * stepLength;
	}

	public void SetStepSpeed(float speed)
	{
		_attackStepSpeed = speed;
	}

	#endregion

	#region Private Methods

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

	private Vector2 ApplySpeedLimit(Vector2 velocity)
	{
		velocity.x = Mathf.Clamp(velocity.x, -_maxXSpeed, _maxXSpeed);
		velocity.y = Mathf.Clamp(velocity.y, -_maxYSpeed, _maxYSpeed);

		return velocity;
	}

	private void HandleAttackStepping()
	{
		if (_attackStepDirection.sqrMagnitude <= 0) return;

		var targetPos = Vector2.MoveTowards(
			transform.position,
			(Vector2)transform.position + _attackStepDirection,
			_attackStepSpeed * Time.deltaTime);

		_rigidbody.MovePosition(targetPos);
		if (targetPos == (Vector2)transform.position + _attackStepDirection)
		{
			_attackStepDirection = Vector3.zero;
		}
		else
		{
			_attackStepDirection -= _attackStepDirection.normalized *
				_attackStepSpeed * Time.deltaTime;
		}
	}

	private void HandleDashing()
	{
		if (_dashingDirection == Vector2.zero) return;

		_velocity = Vector2.zero;
		_rigidbody.velocity = Vector2.zero;
		if (_deservedDashingDistance > float.Epsilon)
		{
			var dashingDistance = _dashingSpeed * Time.deltaTime;
			var velocity = ApplyPhysicalContactEffects(_dashingDirection);

			if (dashingDistance > _deservedDashingDistance)
				dashingDistance = _deservedDashingDistance;

			velocity *= dashingDistance;
			_deservedDashingDistance -= dashingDistance;

			_rigidbody.MovePosition((Vector2)transform.position + velocity);
			
			if (_deservedDashingDistance <= float.Epsilon)
			{
				OnDashingDelayBegin?.Invoke();
			}

			return;
		}

		if (_dashingDelayTimer > 0)
		{
			_dashingDelayTimer -= Time.deltaTime;
			return;
		}

		_rigidbody.gravityScale = _gravityScale;
		_dashingDirection = Vector2.zero;
		OnDashingFinished?.Invoke();
	}

	private void Moving(Vector3 direction)
	{
		_velocity = ProcessVelocity(_velocity, direction);

		_rigidbody.MovePosition((Vector2)transform.position + _velocity * Time.deltaTime);

		HandleAttackStepping();
		HandleDashing();

		ResetPhysicalContacts();
	}

	private Vector2 NormalizeMovingDirection(Vector2 direction)
	{
		if (direction.sqrMagnitude < 0.03f)
			return FacingDirection;
		direction.Normalize();

		var dotResult = float.NegativeInfinity;
		var closestDirection = direction;

		foreach (var dir in _normalizedDirections)
		{
			var dot = Vector2.Dot(direction, dir);

			if (dot > dotResult)
			{
				dotResult = dot;
				closestDirection = dir;
			}
		}

		return closestDirection;
	}

	private Vector2 ProcessVelocity(Vector2 velocity, Vector2 movingDirection)
	{
		velocity.y += _rigidbody.gravityScale * Physics2D.gravity.y * Time.deltaTime;
		velocity = ApplyPhysicalContactEffects(velocity);

		if (_groundDetector.IsOnGround && velocity.y <= 0)
		{
			velocity.x = movingDirection.x * _baseSpeed;
		}
		else
		{
			velocity.x *= 1 - _airborneXDecayRate;
			if (Mathf.Sign(velocity.x) != Mathf.Sign(movingDirection.x) ||
				Mathf.Abs(velocity.x) < _baseSpeed)
			{
				velocity.x += movingDirection.x * _baseSpeed * _airborneSpeedFactor *
					_airborneXMovementCurve.Evaluate(_airborneTime / _airborneXMovementTimeFactor);
			}
		}

		velocity = ApplySpeedLimit(velocity);

		return velocity;
	}

	private void ResetPhysicalContacts()
	{
		_contacts.Clear();
	}

	private void UpdatePhysicalContacts(ContactPoint2D[] contactPoints)
	{
		_contacts.AddRange(contactPoints);
	}
	
	#endregion
}
