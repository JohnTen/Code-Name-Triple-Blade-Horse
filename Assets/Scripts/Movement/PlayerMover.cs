using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMover : MonoBehaviour, ICanMove, ICanJump, ICanDash, ICanBlockInput
{
	#region Fields
	[Header("Basic movement")]
	[SerializeField] float _baseSpeed;
	[SerializeField] float _maxXSpeed;
	[SerializeField] float _maxYSpeed;
	[SerializeField] float _jumpHeight;
	[SerializeField] float _knockbackSpeedFactor;

	[Header("Dashing")]
	[SerializeField] float _dashCancelRange;
	[SerializeField] float _airDashingDistance;
	[SerializeField] float _airDashingDuration;
	[SerializeField] float _airDashingDelay;
	[SerializeField] float _groundDashingDistance;
	[SerializeField] float _groundDashingDuration;
	[SerializeField] float _groundDashingDelay;
	[SerializeField] Vector2 _dashingDirection;

	[Header("Airborne Movement")]
	[SerializeField] float _airborneSpeedFactor;
	[SerializeField] float _pullingForce;
	[SerializeField] float _pullingMoveDelay;
	[SerializeField] float _airborneXDecayRate = 0.02f;
	[SerializeField] float _airborneXMovementTimeFactor = 0.5f;
	[SerializeField] AnimationCurve _airborneXMovementCurve;

	[Header("Events")]
	[SerializeField] BoolEvent _onChangedFacingDirection;

	[Header("Debug")]
	[SerializeField] float _attackStepSpeed;
	[SerializeField] float _pullDelayTimer;
	[SerializeField] Vector2 _attackStepDirection;
	[SerializeField] Vector2 _velocity;
	[SerializeField] Vector2 _movementVector;
	[SerializeField] Vector2 _knockbackVector;
	[SerializeField] MovingState _currentMovingState;


	CharacterState _state;
	Rigidbody2D _rigidbody;
	ICanDetectGround _groundDetector;
	
	bool _dashing;
	float _gravityScale;
	float _airborneTime;
	float _deservedDashingDistance;
	float _dashingDelayTimer;
	float _dashingSpeed;

	[SerializeField] List<ContactPoint2D> _contacts;

	readonly static Vector2[] _normalizedDirections =
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
	public bool FacingRight { get; private set; } = true;
	public bool IsOnGround => _groundDetector.IsOnGround;
	public bool IsDashing => _dashing;
	public bool PullDelaying => _pullDelayTimer > 0;
	public bool BlockInput { get; private set; }
	public bool DelayInput { get; private set; }
	public Vector2 Velocity => _velocity;

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

	#endregion

	#region Events
	public event Action<bool> OnChangeDirection;
	public event Action OnJump;
	public event Action OnDashingBegin;
	public event Action OnDashingDelayBegin;
	public event Action OnDashingFinished;
	public event Action<ICanChangeMoveState, MovingEventArgs> OnMovingStateChanged;
	public event Action<ICanChangeMoveState, LandingEventArgs> OnLandingStateChanged;
	#endregion

	#region Event Handlers

	private void HandleLanding(ICanDetectGround detector, LandingEventArgs eventArgs)
	{
		if (eventArgs.lastLandingState != eventArgs.currentLandingState &&
			eventArgs.currentLandingState == LandingState.OnGround)
			_airborneTime = 0;
	}

	#endregion

	#region Unity Messages

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody2D>();
		_state = GetComponent<CharacterState>();
		_groundDetector = GetComponent<ICanDetectGround>();
		_groundDetector.OnLandingStateChanged += HandleLanding;
		_contacts = new List<ContactPoint2D>();
	}

	private void FixedUpdate()
	{
		if (!_groundDetector.IsOnGround)
			_airborneTime += Time.fixedDeltaTime;

		if (_pullDelayTimer > 0)
			_pullDelayTimer -= Time.deltaTime;

		_rigidbody.GetContacts(_contacts);

		Moving(_movementVector);

		CurrentMovingState = UpdateState();
	}

	#endregion

	#region Interfaces

	public void CancelDash()
	{
		if (!_dashing) return;

		_dashingDirection = Vector2.zero;

		_rigidbody.gravityScale = _gravityScale;
		_deservedDashingDistance = 0;
		_dashingDelayTimer = 0;
		_dashingDirection = Vector2.zero;
		_dashing = false;
		_airborneTime = 0;
		OnDashingFinished?.Invoke();
	}

	public void Dash(Vector2 direction)
	{
		if (!_dashing)
		{
			_gravityScale = _rigidbody.gravityScale;
			_rigidbody.gravityScale = 0;
		}

		_dashing = true;
		_dashingDirection = GetOctadDirection(direction);

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

		CurrentMovingState = MovingState.Airborne;
	}

	public void Knockback(Vector2 direction)
	{
		_knockbackVector = Vector2.right * (direction.magnitude * Mathf.Sign(direction.x));
	}

	public void Move(Vector2 direction)
	{
		_movementVector = direction;

		if (direction.x != 0)
		{
			_state._facingRight = direction.x > 0;
			OnChangeDirection?.Invoke(direction.x > 0);
			_onChangedFacingDirection.Invoke(direction.x > 0);
		}
	}

	public void Pull(Vector2 direction)
	{
		_velocity = direction.normalized * _pullingForce;
		_pullDelayTimer = _pullingMoveDelay;
		_airborneTime = 0;
	}

	public void ResetMovement()
	{
		_velocity = Vector2.zero;
		_movementVector = Vector2.zero;

		CancelDash();
		_knockbackVector = Vector2.zero;
	}

	public void SetStepDistance(float stepLength)
	{
		_attackStepDirection
			= (_state._facingRight ? Vector2.right : Vector2.left)
			* stepLength;
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

			BlockInput = _deservedDashingDistance < _dashCancelRange;

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
		_dashing = false;
		_airborneTime = 0;
		BlockInput = false;
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

	private Vector2 GetOctadDirection(Vector2 direction)
	{
		if (direction.sqrMagnitude < 0.03f)
			return _state._facingRight? Vector2.right: Vector2.left;

		return DirectionalHelper.NormalizeOctadDirection(direction);
	}

	private Vector2 ProcessVelocity(Vector2 velocity, Vector2 movingDirection)
	{
		velocity.y += _rigidbody.gravityScale * Physics2D.gravity.y * Time.deltaTime;
		velocity = ApplyPhysicalContactEffects(velocity);

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

			movingDirection += knockback;
		}

		if (_groundDetector.IsOnGround && velocity.y <= float.Epsilon)
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

	private MovingState UpdateState()
	{
		var state = CurrentMovingState;
		var isMoving = _velocity.sqrMagnitude > 0.1f || _movementVector.sqrMagnitude > 0.1f;
		var isStepping = _attackStepDirection.sqrMagnitude > 0;

		if (isStepping)
			return MovingState.AttackStep;

		if (IsDashing)
			return MovingState.Dash;

		if (!_groundDetector.IsOnGround)
			return MovingState.Airborne;

		if (isMoving)
			return MovingState.Move;

		return MovingState.Idle;
	}
	
	#endregion
}
