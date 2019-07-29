using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse.Movement
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class PlayerMover : MonoBehaviour, ICanMove, ICanJump, ICanDash, ICanBlockInput, ICanStandOnMovingPlatform
	{
		#region Fields
		[Header("Basic movement")]
		[SerializeField] float _baseSpeed;
		[SerializeField] float _maxXSpeed;
		[SerializeField] float _maxYSpeed;
		[SerializeField] float _jumpHeight;
		[SerializeField] float _gravityScale;
		[SerializeField] float _defaultKnockBackSpeed;
		[SerializeField] float _airAttackMaxXSpeed;
		[SerializeField] float _airAttackMaxYSpeed;
		[SerializeField] float _airAttackgravityScale;

		[Header("Dashing")]
		[SerializeField] float _dashCancelPercent;
		[SerializeField] float _dashInvincibleBeginPercent;
		[SerializeField] float _dashInvincibleStopPercent;
		[SerializeField] float _airDashingDistance;
		[SerializeField] float _airDashingDuration;
		[SerializeField] float _airDashingDelay;
		[SerializeField] float _groundDashingDistance;
		[SerializeField] float _groundDashingDuration;
		[SerializeField] float _groundDashingDelay;
		[SerializeField] float _shortGroundDashingDistance;
		[SerializeField] float _shortGroundDashingDuration;
		[SerializeField] float _shortGroundDashingDelay;
		[SerializeField] Vector2 _dashingDirection;

		[Header("Airborne Movement")]
		[SerializeField] float _minExtraJumpHeightScale;
		[SerializeField] float _maxExtraJumpHeightScale;
		[SerializeField] float _airborneSpeedFactor;
		[SerializeField] float _pullingForce;
		[SerializeField] float _pullingMoveDelay;
		[SerializeField] float _airborneXDecayRate = 0.02f;
		[SerializeField] float _airborneXMovementTimeFactor = 0.5f;
		[SerializeField] AnimationCurve _airborneXMovementCurve;

		[Header("Events")]
		[SerializeField] BoolEvent _onChangedFacingDirection;

		[Header("Debug")]
		[SerializeField] float _currentDashingPercent;
		[SerializeField] float _attackStepSpeed;
		[SerializeField] float _pullDelayTimer;
		[SerializeField] Vector2 _attackStepDirection;
		[SerializeField] Vector2 _velocity;
		[SerializeField] Vector2 _movementVector;
		[SerializeField] Vector2 _knockbackVector;
		[SerializeField] MovingState _currentMovingState;
		[SerializeField] bool onPlatform;
		[SerializeField] bool _blockInput;
		[SerializeField] List<Collider2D> _ignoredColliders;
		[SerializeField] float _knockbackSpeed;


		PlayerState _state;
		Rigidbody2D _rigidbody;
		ICanDetectGround _groundDetector;

		bool _airAttacking;
		bool _dashing;
		float _airborneTime;
		float _deservedDashingDistance;
		float _leftDashingDistance;
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
		public bool BlockInput => _blockInput;
		public bool DelayInput { get; private set; }
		public bool AirAttacking
		{
			get => _airAttacking;
			set
			{
				_airAttacking = value;
				_rigidbody.gravityScale = _airAttacking ? _airAttackgravityScale : _gravityScale;
			}
		}
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

		float GravityScale
		{
			get => AirAttacking ? _airAttackgravityScale : _gravityScale;
		}

		#endregion

		#region Events
		public event Action<bool> OnChangeDirection;
		public event Action OnBeginDashingInvincible;
		public event Action OnStopDashingInvincible;
		public event Action<ICanChangeMoveState, MovingEventArgs> OnMovingStateChanged;
		public event Action<ICanChangeMoveState, LandingEventArgs> OnLandingStateChanged;
		#endregion

		#region Event Handlers

		private void HandleLanding(ICanDetectGround detector, LandingEventArgs eventArgs)
		{
			if (eventArgs.lastLandingState != eventArgs.currentLandingState &&
				eventArgs.currentLandingState == LandingState.OnGround)
            {
                _airborneTime = 0;
            }
			
		}

		#endregion

		#region Unity Messages

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody2D>();
			_state = GetComponent<PlayerState>();
			_groundDetector = GetComponent<ICanDetectGround>();
			_groundDetector.OnLandingStateChanged += HandleLanding;
			_contacts = new List<ContactPoint2D>();
			_ignoredColliders = new List<Collider2D>();
		}

		private void FixedUpdate()
		{
			if (!_groundDetector.IsOnGround)
				_airborneTime += Time.fixedDeltaTime;

			if (_pullDelayTimer > 0)
				_pullDelayTimer -= TimeManager.PlayerDeltaTime;

			UpdateContacts();

			Moving(_movementVector);

			CurrentMovingState = UpdateState();
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (_ignoredColliders.Contains(collision.collider)) return;

			var effector = collision.collider.GetComponent<PlatformEffector2D>();
			if (effector == null) return;

			if (!IsEffectivePlatformContact(effector, collision.contacts))
			{
				_ignoredColliders.Add(collision.collider);
			}
		}

		private void OnCollisionExit2D(Collision2D collision)
		{
			if (!_ignoredColliders.Contains(collision.collider)) return;
			_ignoredColliders.Remove(collision.collider);
		}

		#endregion

		#region Public Methods

		public void CancelDash()
		{
			if (!_dashing) return;

			_dashingDirection = Vector2.zero;
			if (_currentDashingPercent > _dashInvincibleBeginPercent)
				OnStopDashingInvincible?.Invoke();

			_rigidbody.gravityScale = GravityScale;
			_deservedDashingDistance = 0;
			_dashingDelayTimer = 0;
			_dashingDirection = Vector2.zero;
			_dashing = false;
			_airborneTime = 0;
			_blockInput = false;
		}

		public void Dash(Vector2 direction)
		{
			if (!_dashing)
			{
				_rigidbody.gravityScale = 0;
			}

			_dashing = true;
			_dashingDirection = GetOctadDirection(direction);
			_currentDashingPercent = 0;

			if (!_groundDetector.IsOnGround || _dashingDirection.y > 0)
			{
				_dashingDelayTimer = _airDashingDelay;
				_deservedDashingDistance = _airDashingDistance;
				_leftDashingDistance = _airDashingDistance;
				_dashingSpeed = _airDashingDistance / _airDashingDuration;
			}
			else
			{
				_dashingDelayTimer = _groundDashingDelay;
				_deservedDashingDistance = _groundDashingDistance;
				_leftDashingDistance = _groundDashingDistance;
				_dashingSpeed = _groundDashingDistance / _groundDashingDuration;
			}
		}

		public void ShortDash(Vector2 direction)
		{
			if (!_dashing)
			{
				_rigidbody.gravityScale = 0;
			}

			_dashing = true;
			_dashingDirection = GetHorizontalDirection(direction);
			_currentDashingPercent = 0;

			_dashingDelayTimer = _shortGroundDashingDelay;
			_deservedDashingDistance = _shortGroundDashingDistance;
			_leftDashingDistance = _shortGroundDashingDistance;
			_dashingSpeed = _shortGroundDashingDistance / _shortGroundDashingDuration;
		}

		public void Jump()
		{
			_velocity.y = Mathf.Sqrt(19.62f * _jumpHeight * GravityScale);
			_airborneTime = 0;
			CurrentMovingState = MovingState.Airborne;
		}

		public void ExtraJump(float percent)
		{
			var scale = Mathf.Lerp(_minExtraJumpHeightScale, _maxExtraJumpHeightScale, percent);
			_velocity.y = Mathf.Sqrt(19.62f * _jumpHeight * GravityScale * scale);
			_airborneTime = 0;
			CurrentMovingState = MovingState.Airborne;
		}

		public void Knockback(Vector2 direction)
		{
			_knockbackVector = Vector2.right * (direction.magnitude * Mathf.Sign(direction.x));
			_knockbackSpeed = _defaultKnockBackSpeed;
		}

		public void Knockback(Vector2 direction, float speed)
		{
			_knockbackVector = Vector2.right * (direction.magnitude * Mathf.Sign(direction.x));
			_knockbackSpeed = speed;
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

		public void EnterPlatform()
		{
			onPlatform = true;
		}

		public void LeavePlatform()
		{
			onPlatform = false;
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
			var maxXSpeed = AirAttacking ? _airAttackMaxXSpeed : _maxXSpeed;
			var maxYSpeed = AirAttacking ? _airAttackMaxYSpeed : _maxYSpeed;
			velocity.x = Mathf.Clamp(velocity.x, -maxXSpeed, maxXSpeed);
			velocity.y = Mathf.Clamp(velocity.y, -maxYSpeed, maxYSpeed);

			return velocity;
		}

		private void HandleAttackStepping()
		{
			if (_attackStepDirection.sqrMagnitude <= 0) return;

			var movingDistance = _attackStepSpeed * TimeManager.PlayerDeltaTime;
			var leftDistance = _attackStepDirection.magnitude;

			if (movingDistance > leftDistance)
			{
				movingDistance = leftDistance;
			}

			var movingVector = movingDistance / leftDistance * _attackStepDirection;

			var targetPos = (Vector2)transform.position + movingVector;

			MoveTo(targetPos);
			_attackStepDirection -= movingVector;
		}

		private void HandleDashing()
		{
			if (_dashingDirection == Vector2.zero) return;

			_velocity = Vector2.zero;
			_rigidbody.velocity = Vector2.zero;
			if (_leftDashingDistance > float.Epsilon)
			{
				var previousPercent = _currentDashingPercent;
				var dashingDistance = _dashingSpeed * TimeManager.PlayerDeltaTime;
				var velocity = ApplyPhysicalContactEffects(_dashingDirection);

				if (dashingDistance > _leftDashingDistance)
					dashingDistance = _leftDashingDistance;

				velocity *= dashingDistance;
				_leftDashingDistance -= dashingDistance;

				MoveTo((Vector2)transform.position + velocity);

				_currentDashingPercent = 1 - _leftDashingDistance / _deservedDashingDistance;

				_blockInput = _currentDashingPercent < _dashCancelPercent;

				if (previousPercent < _dashInvincibleBeginPercent &&
					_currentDashingPercent > _dashInvincibleBeginPercent)
				{
					OnBeginDashingInvincible?.Invoke();
				}

				if (previousPercent < _dashInvincibleStopPercent &&
					_currentDashingPercent >= _dashInvincibleStopPercent)
				{
					OnStopDashingInvincible?.Invoke();
				}

				return;
			}

			if (_dashingDelayTimer > 0)
			{
				_dashingDelayTimer -= TimeManager.PlayerDeltaTime;
				return;
			}

			_rigidbody.gravityScale = GravityScale;
			_dashingDirection = Vector2.zero;
			_dashing = false;
			_airborneTime = 0;
			_blockInput = false;
		}

		private void Moving(Vector3 direction)
		{
			_velocity = ProcessVelocity(_velocity, direction);

			MoveTo((Vector2)transform.position + _velocity * TimeManager.PlayerDeltaTime);

			HandleAttackStepping();
			HandleDashing();

			ResetPhysicalContacts();
		}

		private void MoveTo(Vector2 position)
		{
			if (onPlatform)
			{
				transform.position = (Vector3)position;
			}
			else
			{
				_rigidbody.MovePosition(position);
			}
		}

		private Vector2 GetHorizontalDirection(Vector2 direction)
		{
			if (direction.sqrMagnitude < 0.03f || direction.x < 0.03f)
				return _state._facingRight ? Vector2.right : Vector2.left;

			print(direction);
			return DirectionalHelper.NormalizeHorizonalDirection(direction);
		}

		private Vector2 GetOctadDirection(Vector2 direction)
		{
			if (direction.sqrMagnitude < 0.03f)
				return _state._facingRight ? Vector2.right : Vector2.left;

			return DirectionalHelper.NormalizeOctadDirection(direction);
		}

		private bool IsEffectivePlatformContact(PlatformEffector2D effector, ContactPoint2D[] contacts)
		{
			for (int i = 0; i < contacts.Length; i++)
			{
				var minAngle = -effector.surfaceArc * 0.5f + effector.rotationalOffset;
				var maxAngle = effector.surfaceArc * 0.5f + effector.rotationalOffset;
				var contactAngle = Vector2.SignedAngle(Vector2.up, contacts[i].normal);
				if (contactAngle > minAngle && contactAngle < maxAngle) return true;

				if (effector.sideArc > 0)
				{
					minAngle = -effector.sideArc * 0.5f + effector.rotationalOffset + 90;
					maxAngle = -effector.sideArc * 0.5f + effector.rotationalOffset + 90;
					if (contactAngle > minAngle && contactAngle < maxAngle) return true;

					minAngle -= 180;
					maxAngle -= 180;
					if (contactAngle > minAngle && contactAngle < maxAngle) return true;
				}
			}

			return false;
		}

		private Vector2 ProcessVelocity(Vector2 velocity, Vector2 movingDirection)
		{
			velocity.y += GravityScale * Physics2D.gravity.y * TimeManager.PlayerDeltaTime;
			velocity = ApplyPhysicalContactEffects(velocity);

			if (_knockbackVector.sqrMagnitude > 0)
			{
				var knockback = _knockbackVector.normalized * _knockbackSpeed;
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

		private void UpdateContacts()
		{
			_rigidbody.GetContacts(_contacts);
			for (int i = 0; i < _contacts.Count; i++)
			{
				if (!_ignoredColliders.Contains(_contacts[i].collider)) continue;

				_contacts.RemoveAt(i);
				i--;
			}
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
}
