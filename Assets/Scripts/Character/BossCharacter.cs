using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleBladeHorse.Animation;
using TripleBladeHorse.Combat;
using TripleBladeHorse.Movement;
using TripleBladeHorse.AI;
using JTUtility;

namespace TripleBladeHorse
{
	public class BossCharacter : MonoBehaviour, ICanDash
	{
		[SerializeField] BaseWeapon _weapon;
		[SerializeField] HitBox[] _hitboxes;

		[Header("Weakpoint Movemnt")]
		[SerializeField] Transform[] _weakpoints;
		[SerializeField] Transform[] _pivots;

		[Header("Retreat")]
		[SerializeField] float _retreatTime;
		[SerializeField] ParticleSystem.MinMaxCurve _retreatSpeed;

		[Header("Slash")]
		[SerializeField] float _slashTime;
		[SerializeField] ParticleSystem.MinMaxCurve _slashSpeed;
		[SerializeField] AttackMove _slashMove;
		[SerializeField] List<Collider2D> _slashAttackBoxes;

		[Header("Combo2")]
		[SerializeField] float _combo2RaiseTime;
		[SerializeField] ParticleSystem.MinMaxCurve _combo2RaiseSpeed;
		[SerializeField] float _combo2PauseTime;
		[SerializeField] ParticleSystem.MinMaxCurve _combo2PauseSpeed;
		[SerializeField] float _combo2CrushTime;
		[SerializeField] ParticleSystem.MinMaxCurve _combo2CrushSpeed;
		[SerializeField] float _minCrushSpeed;
		[SerializeField] float _maxCrushSpeed;
		[SerializeField] float _combo2SpeedScaler;
		[SerializeField] float _stalkingSpeed = 4;
		[SerializeField] float _stalkingSpeed2 = 0.2f;
		[SerializeField] float _stalkingBackwardSpeedOffset = 0.3f;
		[SerializeField] Vector2 _combo2CrushOffset;
		[SerializeField] AttackMove _crushMove;
		[SerializeField] List<Collider2D> _combo2AttackBoxes;

		[Header("Combo3/thrust")]
		[SerializeField] float _thrustTime;
		[SerializeField] ParticleSystem.MinMaxCurve _thrustSpeed;
		[SerializeField] AttackMove _thrustMove;
		[SerializeField] List<Collider2D> _thrustAttackBoxes;

		FSM _animator;
		EnemyState _state;
		ICharacterInput<BossInput> _input;
		ICanDetectGround _groundDetector;
		HitFlash _hitFlash;
		BossMover _mover;
		AttackMove _currentMove;
		Collider2D _collider;
		BezierTracker _stalkTracker;
		bool _stalking;

		public event Action OnBeginDashingInvincible;
		public event Action OnStopDashingInvincible;
		public event Action<ICanChangeMoveState, MovingEventArgs> OnMovingStateChanged;

		private void Awake()
		{
			_animator = GetComponent<FSM>();
			_state = GetComponent<EnemyState>();
			_input = GetComponent<ICharacterInput<BossInput>>();
			_hitFlash = GetComponent<HitFlash>();
			_mover = GetComponent<BossMover>();
			_groundDetector = GetComponent<ICanDetectGround>();
			_collider = GetComponent<Collider2D>();
			_stalkTracker = new BezierTracker();

			_input.OnReceivedInput += HandleReceivedInput;
			_groundDetector.OnLandingStateChanged += HandleLandingStateChange;
			_animator.Subscribe(Animation.AnimationState.FadingIn, HandleFadeInAnimation);
			_animator.Subscribe(Animation.AnimationState.Completed, HandleCompletedAnimation);
			_animator.OnReceiveFrameEvent += HandleFrameEvent;
			foreach (var hitbox in _hitboxes)
			{
				hitbox.OnHit += HandleOnHit;
			}

			foreach (var weakpoint in _weakpoints)
			{
				weakpoint.SetParent(null);
			}
		}

		private void Update()
		{
			if (!_mover.IsConstantMoving && _animator.GetBool(BossFSMData.Stat.Retreat))
			{
				_animator.SetBool(BossFSMData.Stat.Retreat, false);
			}

			_state._frozen = _animator.GetBool(BossFSMData.Stat.Frozen);
			_input.DelayInput = _animator.GetBool(BossFSMData.Stat.DelayInput);
			_input.BlockInput = _animator.GetBool(BossFSMData.Stat.BlockInput);

			var rawMoveInput = _input.GetMovingDirection();
			var aimInput = _state._frozen ? Vector2.zero : _input.GetAimingDirection().normalized;
			var moveInput = _state._frozen ? Vector2.zero : _input.GetMovingDirection().normalized;

			UpdateMovingMode(aimInput, rawMoveInput);
			UpdateFacingDirection(aimInput);
			UpdateStates();

			if (moveInput.x != 0)
				moveInput = NormalizeHorizonalDirection(moveInput);
			_mover.Move(moveInput);

			if (_state._frozen)
			{
				_animator.SetFloat(BossFSMData.Stat.XSpeed, 0);
			}
			else
			{
				_animator.SetFloat(BossFSMData.Stat.XSpeed, moveInput.x);
			}
		}

		private void FixedUpdate()
		{
			if (_stalking)
			{
				Stalking();
			}
		}

		private void HandleOnHit(AttackPackage attack, AttackResult result)
		{
			_state._hitPoints -= result._finalDamage;
			_state._endurance -= result._finalFatigue;

			if (result._finalDamage > 0)
			{
				_hitFlash.Flash();
			}

			if (_state._hitPoints <= 0)
			{
				_animator.SetBool(BossFSMData.Stat.Death, true);
			}
		}

		private void HandleLandingStateChange(ICanDetectGround detector, LandingEventArgs eventArgs)
		{
			if (eventArgs.currentLandingState == LandingState.OnGround
			 && _animator.GetCurrentAnimation().name == BossFSMData.Anim.Combo2_3)
			{
				_mover.InterruptContantMove();
			}
		}

		private void HandleFrameEvent(FrameEventEventArg eventArgs)
		{
			if (eventArgs._name == AnimEventNames.AttackBegin)
			{
				var attack = AttackPackage.CreateNewPackage();
				attack._hitPointDamage.Base = _state._hitPointDamage;
				attack._enduranceDamage.Base = _state._enduranceDamage;

				_weapon.Activate(attack, _currentMove);

				if (eventArgs._animation.name == BossFSMData.Anim.Slash1
				 || eventArgs._animation.name == BossFSMData.Anim.Slash2)
				{
					foreach (var collider in _slashAttackBoxes)
					{
						collider.enabled = true;
					}
					_collider.gameObject.layer = LayerMask.NameToLayer("EnemyDash");
					OnBeginDashingInvincible?.Invoke();
				}

				if (eventArgs._animation.name == BossFSMData.Anim.Combo2_3)
				{
					foreach (var collider in _combo2AttackBoxes)
					{
						collider.enabled = true;
					}
					_collider.gameObject.layer = LayerMask.NameToLayer("EnemyDash");
					OnBeginDashingInvincible?.Invoke();
				}

				if (eventArgs._animation.name == BossFSMData.Anim.Combo3_2)
				{
					foreach (var collider in _thrustAttackBoxes)
					{
						collider.enabled = true;
					}
					_collider.gameObject.layer = LayerMask.NameToLayer("EnemyDash");
					OnBeginDashingInvincible?.Invoke();
				}
			}
			else if (eventArgs._name == AnimEventNames.AttackEnd)
			{
				_weapon.Deactivate();

				if (eventArgs._animation.name == BossFSMData.Anim.Slash1
				 || eventArgs._animation.name == BossFSMData.Anim.Slash2)
				{
					foreach (var collider in _slashAttackBoxes)
					{
						collider.enabled = false;
					}
					_collider.gameObject.layer = LayerMask.NameToLayer("Enemy");
					OnStopDashingInvincible?.Invoke();
				}

				if (eventArgs._animation.name == BossFSMData.Anim.Combo2_3)
				{
					foreach (var collider in _combo2AttackBoxes)
					{
						collider.enabled = false;
					}
					_collider.gameObject.layer = LayerMask.NameToLayer("Enemy");
					OnStopDashingInvincible?.Invoke();
				}

				if (eventArgs._animation.name == BossFSMData.Anim.Combo3_2)
				{
					foreach (var collider in _thrustAttackBoxes)
					{
						collider.enabled = false;
					}
					_collider.gameObject.layer = LayerMask.NameToLayer("Enemy");
					OnStopDashingInvincible?.Invoke();
				}
			}
		}

		private void HandleFadeInAnimation(AnimationEventArg eventArgs)
		{
			SetFrozen(eventArgs._animation.frozenOnStart);
			SetBlockInput(eventArgs._animation.blockInputOnStart);
			SetDelayInput(eventArgs._animation.delayInputOnStart);

			var aim = _input.GetAimingDirection();
			var moveDirection = NormalizeHorizonalDirection(aim);

			switch (eventArgs._animation.name)
			{
				case BossFSMData.Anim.Slash1:
				case BossFSMData.Anim.Slash2:
					UpdateFacingDirection(aim);
					_mover.InvokeConstantMovement(moveDirection, _slashSpeed, _slashTime);
					break;

				case BossFSMData.Anim.Combo2_1:
					UpdateFacingDirection(aim);
					_mover.InvokeConstantMovement(Vector2.up, _combo2RaiseSpeed, _combo2RaiseTime);
					_stalkTracker.Initialize(transform.position);
					_stalking = true;
					break;

				case BossFSMData.Anim.Combo2_2:
					UpdateFacingDirection(aim);
					_mover.InvokeConstantMovement(Vector2.up, _combo2PauseSpeed, _combo2PauseTime);
					break;

				case BossFSMData.Anim.Combo2_3:
					var player = GameManager.PlayerInstance;
					Vector2 toPlayer = player.transform.position - transform.position;

					UpdateFacingDirection(toPlayer);
					toPlayer += _state._facingRight ? _combo2CrushOffset : -_combo2CrushOffset;
					var speed = (toPlayer.magnitude / _combo2CrushTime) * _combo2SpeedScaler;

					speed = Mathf.Clamp(speed, _minCrushSpeed, _maxCrushSpeed);

					_combo2CrushSpeed.constant = speed;
					_combo2CrushSpeed.curveMultiplier = speed;
					_mover.InvokeConstantMovement(toPlayer, _combo2CrushSpeed, _combo2CrushTime);

					_stalking = false;
					break;

				case BossFSMData.Anim.Combo3_2:
					UpdateFacingDirection(aim);
					print(aim);
					_mover.InvokeConstantMovement(moveDirection, _thrustSpeed, _thrustTime);
					break;

				case BossFSMData.Anim.Retreat:
					UpdateFacingDirection(aim);
					_mover.InvokeConstantMovement(-moveDirection, _retreatSpeed, _retreatTime);

					break;
			}
		}

		private void HandleCompletedAnimation(AnimationEventArg eventArgs)
		{
			if (eventArgs._animation.name != BossFSMData.Anim.Death) return;
			foreach (var handler in GetComponents<ICanHandleDeath>())
			{
				handler.OnDeath(_state);
			}
		}

		private void HandleReceivedInput(InputEventArg<BossInput> input)
		{
			switch (input._command)
			{
				case BossInput.Slash:
					_animator.SetToggle(BossFSMData.Stat.Slash, true);
					_currentMove = _slashMove;
					SetBlockInput(true);
					SetFrozen(true);
					break;

				case BossInput.DashAttack:
					UpdateFacingDirection(_input.GetAimingDirection());
					_animator.SetToggle(BossFSMData.Stat.Thrust, true);
					_currentMove = _thrustMove;
					SetBlockInput(true);
					SetFrozen(true);
					break;

				case BossInput.Dodge:
					_animator.SetBool(BossFSMData.Stat.Retreat, true);

					var aim = _input.GetAimingDirection();
					var moveDirection = NormalizeHorizonalDirection(aim);

					UpdateFacingDirection(aim);
					_mover.InvokeConstantMovement(-moveDirection, _retreatSpeed, _retreatTime);
					SetBlockInput(true);
					SetFrozen(true);
					break;

				case BossInput.JumpAttack:
					_animator.SetToggle(BossFSMData.Stat.Combo2, true);
					_currentMove = _crushMove;
					SetBlockInput(true);
					SetFrozen(true);
					break;
			}
		}

		Vector2 NormalizeHorizonalDirection(Vector2 direction)
		{
			if (direction.x == 0)
			{
				return _state._facingRight ? Vector2.right : Vector2.left;
			}

			return DirectionalHelper.NormalizeHorizonalDirection(direction);
		}

		void Stalking()
		{
			var player = GameManager.PlayerInstance;
			Vector2 fromPlayer = transform.position - player.transform.position;
			var facingDirection = fromPlayer.x < 0;
			var idealPlayerPosition = (Vector2)player.transform.position +
				(facingDirection ? _combo2CrushOffset : -_combo2CrushOffset);
			var idealPosition = idealPlayerPosition;
			idealPosition.y += fromPlayer.y;
			idealPosition.x += facingDirection ? -fromPlayer.y : fromPlayer.y;
			UpdateFacingDirection(-fromPlayer);

			Debug.DrawLine(idealPosition, idealPlayerPosition);

			idealPosition.y = transform.position.y;
			var movement = _stalkTracker.Berp((Vector2)transform.position, idealPosition, _stalkingSpeed2);
			movement.y = transform.position.y;
			movement = Vector2.MoveTowards(transform.position, idealPosition, _stalkingSpeed * TimeManager.DeltaTime);
			movement -= (Vector2)transform.position;

			if (movement.x > 0 != facingDirection)
			{
				movement *= _stalkingBackwardSpeedOffset;
			}

			_mover.ManualMove(movement);
		}

		void SetDelayInput(bool value)
		{
			_animator.SetBool(BossFSMData.Stat.DelayInput, value);
			_input.DelayInput = value;
		}

		void SetBlockInput(bool value)
		{
			_animator.SetBool(BossFSMData.Stat.BlockInput, value);
			_input.BlockInput = value;
		}

		void SetFrozen(bool value)
		{
			_animator.SetBool(BossFSMData.Stat.Frozen, value);
			_state._frozen = value;
		}

		void UpdateStates()
		{
			if (_state._currentComboInterval > 0)
			{
				_state._currentComboInterval -= TimeManager.DeltaTime;
				if (_state._currentComboInterval <= 0)
				{
					_state._currentComboTimes = 0;
				}
			}
		}

		void UpdateMovingMode(Vector2 aimInput, Vector2 moveInput)
		{
			var backward = false;
			if (Mathf.Abs(aimInput.x) > 0 && Mathf.Abs(moveInput.x) > 0)
			{
				backward = Mathf.Sign(aimInput.x) != Mathf.Sign(moveInput.x);
			}

			_animator.SetBool(BossFSMData.Stat.Backward, backward);

			_animator.SetBool(BossFSMData.Stat.QucikMove,
				moveInput.sqrMagnitude > 1);

			if (_animator.GetBool(BossFSMData.Stat.Backward))
				_mover.Movemode = BossMover.MoveMode.Back;
			else if (_animator.GetBool(BossFSMData.Stat.QucikMove))
				_mover.Movemode = BossMover.MoveMode.Quick;
			else
				_mover.Movemode = BossMover.MoveMode.Slow;
		}

		void UpdateFacingDirection(Vector2 aimInput)
		{
			if (aimInput.x != 0)
			{
				_state._facingRight = aimInput.x > 0;
				if (_animator.GetBool(BossFSMData.Stat.Backward))
				{
					_state._facingRight = !_state._facingRight;
				}

				_animator.FlipX = _state._facingRight;

				foreach (var pivot in _pivots)
				{
					pivot.transform.localEulerAngles = _state._facingRight ? Vector3.zero : Vector3.up * 180;
				}
				print("Updated direction");
			}
		}

		public void Dash(Vector2 direction) { }
	}
}
