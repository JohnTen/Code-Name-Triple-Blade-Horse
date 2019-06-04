using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMover : MonoBehaviour, ICanMove, ICanJump, ICanDash
{
	#region Fields
	[Header("Basic movement")]
	[SerializeField] float _moveSpeed;
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

	[Header("Attacking Step(Unadjustable, Debug only)")]
	[SerializeField] float _attackStepSpeed;
	[SerializeField] Vector2 _attackStepDirection;

	[Header("Airborne Movement")]
	[SerializeField] float pullingForce;
	[SerializeField] float pullingMoveDelay;
	[SerializeField] float _airborneXDecayRate = 0.02f;
	[SerializeField] float _airborneXMovementTimeFactor = 0.5f;
	[SerializeField] AnimationCurve _airborneXMovementCurve;

	[Header("Events")]
	[SerializeField] BoolEvent _onChangedFacingDirection;

	Rigidbody2D _rigidbody;
	ICanDetectGround _groundDetector;

	Vector2 _velocity;
	bool _dashing;
	float _airborneTime;

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
	public Vector2 Velocity => _velocity;
	public bool IsOnGround => _groundDetector.IsOnGround;
	public bool IsDashing => _dashing;
	public bool Frozen { get; set; }
	#endregion

	#region Events
	public event Action<bool> OnChangeDirection;
	public event Action OnBlocked;
	public event Action OnJump;
	public event Action OnDashing;
	public event Action OnDashed;
	#endregion

	#region Unity Messages
	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody2D>();
		_groundDetector = GetComponent<ICanDetectGround>();
		_groundDetector.OnLanding += _groundDetector_OnLanding;
	}

	private void _groundDetector_OnLanding()
	{
		
	}

	private void FixedUpdate()
	{
		var movement = _movementVector + _airborneVector;

		_rigidbody.MovePosition((Vector2)transform.position + movement * Time.deltaTime);

		_airborneVector += Physics2D.gravity * Time.deltaTime * _rigidbody.gravityScale;
		if (_airborneVector.y < -_fallingSpeedLimit)
			_airborneVector.y = -_fallingSpeedLimit;
	}
	#endregion

	#region Interfaces
	public void Move(Vector3 direction)
	{
		_movementVector = direction * _moveSpeed;
	}

	public void Jump()
	{
		if (!_groundDetector.IsOnGround) return;

		OnJump?.Invoke();
		_airborneVector.y = Mathf.Sqrt(19.62f * _jumpHeight * _rigidbody.gravityScale);
	}

	public void Dash(Vector3 direction)
	{
		
	}
	#endregion

	#region Private Methods

	private Vector2 NormalizeMovingDirection(Vector2 direction)
	{
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
	#endregion
}
