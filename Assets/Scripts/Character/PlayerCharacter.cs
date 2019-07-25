using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleBladeHorse.Animation;
using TripleBladeHorse.Combat;
using TripleBladeHorse.Movement;
using TripleBladeHorse.VFX;
using JTUtility;

namespace TripleBladeHorse
{
	public class PlayerCharacter : MonoBehaviour, ICanHandlePullingKnife
	{
		#region Fields
		[SerializeField] Transform _hittingPoint;
		[SerializeField] PlayerState _state;
		[SerializeField] BulletTimeManager _btManager;
		[SerializeField] AimAssistant _aimAssistant;
		[SerializeField] InstanceDashEcho _airDashGhost;
		[SerializeField] InstanceDashEcho _groundDashGhost;
		[SerializeField] int _maxAirAttack;
		[SerializeField] float _healingStaminaCost;
		[SerializeField] float _healingAmount;
		[SerializeField] float _dashCooldown;

		[Header("Debug")]
		[SerializeField] bool _bulletTimeReady;
		[SerializeField] bool _extraJump;

		int _currentAirAttack;
		FSM _animator;
		ICanDetectGround _groundDetector;
		IAttackable _hitbox;
		HitFlash _hitFlash;
		ICharacterInput<PlayerInputCommand> _input;
		PlayerMover _mover;
		WeaponSystem _weaponSystem;
		Timer _dashCooldownTimer;

		List<int> colliderLayers;
		List<Collider2D> colliders;

		float _enduranceRefreshTimer;
		float _enduranceRecoverTimer;
		#endregion

		#region Properties
		public Transform HittingPoint => _hittingPoint;
		#endregion

		#region Unity Messages
		private void Awake()
		{
			_input = GetComponent<ICharacterInput<PlayerInputCommand>>();
			_mover = GetComponent<PlayerMover>();
			_animator = GetComponent<FSM>();
			_weaponSystem = GetComponent<WeaponSystem>();
			_groundDetector = GetComponent<ICanDetectGround>();
			_hitbox = GetComponentInChildren<IAttackable>();
			_hitFlash = GetComponent<HitFlash>();
			
			colliders = new List<Collider2D>();
			colliderLayers = new List<int>();
			_dashCooldownTimer = new Timer();

			_mover.OnMovingStateChanged += HandleMovingStateChanged;
			_mover.OnBeginDashingInvincible += HandleDashingInvincibleBegin;
			_mover.OnStopDashingInvincible += HandleDashingInvincibleStop;
			_groundDetector.OnLandingStateChanged += HandleLandingStateChanged;
			_input.OnReceivedInput += HandleReceivedInput;
			_animator.Subscribe(Animation.AnimationState.FadingIn, HandleAnimationFadeinEvent);
			_animator.Subscribe(Animation.AnimationState.FadingOut, HandleAnimationFadeoutEvent);
			_animator.Subscribe(Animation.AnimationState.Completed, HandleAnimationCompletedEvent);
			_animator.OnReceiveFrameEvent += HandleAnimationFrameEvent;
			_weaponSystem.OnPull += HandlePull;
			_hitbox.OnHit += HandleOnHit;

			TimeManager.Instance.OnBulletTimeBegin += OnBulletTimeBegin;
			TimeManager.Instance.OnBulletTimeEnd += OnBulletTimeEnd;

			// Debug
			if (GetComponent<Test>())
				GetComponent<Test>().OnPull += HandlePull;
		}

		private void Update()
		{
			var frozen =
				_animator.GetBool(PlayerFSMData.Stat.Frozen)
				|| _mover.CurrentMovingState == MovingState.Dash
				|| _mover.PullDelaying
				|| _weaponSystem.Frozen;

			SetFrozen(frozen);
			
			_input.DelayInput = _animator.GetBool(PlayerFSMData.Stat.DelayInput);
			_input.BlockInput = _mover.BlockInput || _animator.GetBool(PlayerFSMData.Stat.BlockInput);

			var moveInput = _state._frozen ? Vector2.zero : _input.GetMovingDirection().normalized;
			if (moveInput.x != 0)
				moveInput = DirectionalHelper.NormalizeHorizonalDirection(moveInput);
			_mover.Move(moveInput);

			if (_state._frozen)
			{
				_animator.SetFloat(PlayerFSMData.Stat.XSpeed, 0);
				_animator.SetFloat(PlayerFSMData.Stat.YSpeed, _mover.Velocity.y);
			}
			else
			{
				_animator.SetFloat(PlayerFSMData.Stat.XSpeed, moveInput.x);
				_animator.SetFloat(PlayerFSMData.Stat.YSpeed, _mover.Velocity.y);
				UpdateFacingDirection(moveInput);
			}

			HandleEndurance();
		}

		private void OnDestroy()
		{
			_dashCooldownTimer.Dispose();

			TimeManager.Instance.OnBulletTimeBegin -= OnBulletTimeBegin;
			TimeManager.Instance.OnBulletTimeEnd -= OnBulletTimeEnd;
		}
		#endregion

		#region Event Handlers
		private void HandleAnimationFadeinEvent(AnimationEventArg eventArgs)
		{
			SetBlockInput(eventArgs._animation.blockInputOnStart);
			SetDelayInput(eventArgs._animation.delayInputOnStart);
			SetFrozen(eventArgs._animation.frozenOnStart);
			_mover.AirAttacking = eventArgs._animation.airAttack;

			switch (eventArgs._animation.name)
			{
				case PlayerFSMData.Anim.ATK_Charge_Ground_ATK:
				case PlayerFSMData.Anim.ATK_Melee_Air_1:
				case PlayerFSMData.Anim.ATK_Melee_Air_2:
				case PlayerFSMData.Anim.ATK_Melee_Air_3:
				case PlayerFSMData.Anim.ATK_Melee_Ground_1:
				case PlayerFSMData.Anim.ATK_Melee_Ground_2:
				case PlayerFSMData.Anim.ATK_Melee_Ground_3:
					_state._attacking = true;
					break;

				default:
					_state._attacking = false;
					break;
			}

			switch (eventArgs._animation.name)
			{
				case PlayerFSMData.Anim.Healing:
					_state._stamina -= _healingStaminaCost;
					break;
			}
		}

		private void HandleAnimationFadeoutEvent(AnimationEventArg eventArgs)
		{
			_mover.AirAttacking = false;
		}

		private void HandleAnimationCompletedEvent(AnimationEventArg eventArgs)
		{
			if (eventArgs._animation.name == PlayerFSMData.Anim.Death)
			{
				CancelAnimation();
				_animator.PlayAnimation(PlayerFSMData.Anim.Idle_Ground, 0.05f);
				foreach (var handler in GetComponents<ICanHandleDeath>())
				{
					handler.OnDeath(_state);
				}
			}
		}

		private void HandleAnimationFrameEvent(FrameEventEventArg eventArgs)
		{
			if (eventArgs._name == AnimEventNames.AttackBegin)
			{
				if (eventArgs._animation.name != PlayerFSMData.Anim.ATK_Charge_Ground_ATK)
					_weaponSystem.MeleeAttack();
			}
			else if (eventArgs._name == AnimEventNames.AttackEnd)
			{
				_weaponSystem.MeleeAttackEnd();
				_animator.SetBool(PlayerFSMData.Stat.Charge, false);
			}
			else if (eventArgs._name == AnimEventNames.StepDistance)
			{
				_mover.SetStepDistance(eventArgs._floatData);
			}
			else if (eventArgs._name == AnimEventNames.StepSpeed)
			{
				_mover.SetStepSpeed(eventArgs._floatData);
			}
			else if (eventArgs._name == AnimEventNames.Regenerate)
			{
				_state._hitPoints += _healingAmount;
				_state._hitPoints.Clamp();
			}
		}

		private void HandleDashingInvincibleBegin()
		{
			colliderLayers.Clear();
			GetComponentsInChildren(colliders);

			foreach (var collider in colliders)
			{
				colliderLayers.Add(collider.gameObject.layer);
				collider.gameObject.layer = LayerMask.NameToLayer("PlayerDash");
			}
		}

		private void HandleDashingInvincibleStop()
		{
			if (colliders.Count == 0) return;

			for (int i = 0; i < colliders.Count; i++)
			{
				colliders[i].gameObject.layer = colliderLayers[i];
			}

			colliders.Clear();
		}

		private void OnBulletTimeEnd()
		{
			_animator.TimeScale = 1;
		}

		private void OnBulletTimeBegin()
		{
			var scale = TimeManager.PlayerDeltaTime / TimeManager.DeltaTime;
			_animator.TimeScale = scale;
		}

		private void HandleMovingStateChanged(ICanChangeMoveState sender, MovingEventArgs eventArgs)
		{
			if (eventArgs.lastMovingState == MovingState.Dash)
			{
				_animator.SetToggle(PlayerFSMData.Stat.DashEnd, true);
			}
		}

		private void HandlePull(Vector3 direction)
		{
			_currentAirAttack = 0;
			_animator.SetToggle(PlayerFSMData.Stat.Jump, true);
			_mover.Pull(direction);
			_bulletTimeReady = true;
			_extraJump = true;
			_btManager.StartWithdrawBTWindow();
		}

		private void HandleLandingStateChanged(ICanDetectGround sender, LandingEventArgs eventArgs)
		{
			if (eventArgs.currentLandingState == LandingState.OnGround)
			{
				_currentAirAttack = 0;
				_animator.SetBool(PlayerFSMData.Stat.Airborne, false);
				_animator.SetBool(PlayerFSMData.Stat.Charge, false);
				_bulletTimeReady = false;
				_extraJump = false;
				_state._airborne = false;
				if (_animator.GetCurrentAnimation().name == PlayerFSMData.Anim.Dropping)
				{
					SetBlockInput(true);
					SetFrozen(true);
				}
			}
			else
			{
				_animator.SetBool(PlayerFSMData.Stat.Airborne, true);
				_state._airborne = true;
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

			if (result._finalFatigue > 0)
			{
				_enduranceRecoverTimer = 0;
			}

			if (attack._knockback != 0)
			{
				_mover.Knockback(attack._fromDirection * attack._knockback);
			}

			if (_state._hitPoints <= 0)
			{
				CancelAnimation();
				_animator.SetToggle(PlayerFSMData.Stat.Death, true);
				print("Player Dead");
				_mover.ResetMovement();
				_weaponSystem.ResetWeapon();
			}

			if (_state._endurance <= 0)
			{
				var input = _input as PlayerInput;
				var mInput = _input as MPlayerInput;

				if (input)
					input.ResetDelayInput();
				else
					mInput.ResetDelayInput();

				CancelAnimation();
				_state._endurance.Current = 0;
				_animator.SetToggle(attack._staggerAnimation, true);
				SetBlockInput(true);
				SetFrozen(true);
			}
		}

		private void HandleReceivedInput(InputEventArg<PlayerInputCommand> input)
		{
			var pInput = _input as PlayerInput;
			Vector2 aimingDirection = pInput.GetAimingDirection();

			switch (input._command)
			{
				case PlayerInputCommand.JumpBegin:
					if (!_groundDetector.IsOnGround) break;
					CancelAnimation();
					_mover.Jump();
					_animator.SetToggle(PlayerFSMData.Stat.Jump, true);
					break;

				case PlayerInputCommand.Jump:
					if (!_groundDetector.IsOnGround 
					 && !_extraJump)
						break;

					CancelAnimation();
					if (_extraJump)
					{
						_mover.ExtraJump(input._additionalValue);
						_extraJump = false;
					}
					else
					{
						_mover.Jump();
					}

					TriggerBulletTimeIfPossible();
					_animator.SetToggle(PlayerFSMData.Stat.Jump, true);
					break;

				case PlayerInputCommand.Dash:
					if (!_dashCooldownTimer.IsReachedTime()) return;

					var moveInput = _input.GetMovingDirection();
					if (moveInput == Vector2.zero)
						moveInput = _state._facingRight ? Vector2.right : Vector2.left;
					else
						moveInput = DirectionalHelper.NormalizeOctadDirection(moveInput);

					var airDash = !_groundDetector.IsOnGround || moveInput.y > 0;
					
					UpdateFacingDirection(moveInput);

					if (airDash && _state._stamina > 0)
					{
						_airDashGhost.enabled = true;
						_groundDashGhost.enabled = false;
						CancelAnimation();
						_mover.Dash(moveInput);
						_state._stamina -= 1;
						_currentAirAttack = 0;
						TriggerBulletTimeIfPossible();

						_extraJump = true;
						_bulletTimeReady = true;
						_btManager.StartDashBTWindow();
					}
					else if (!airDash)
					{
						_airDashGhost.enabled = false;
						_groundDashGhost.enabled = true;
						CancelAnimation();
						_mover.Dash(moveInput);
						_dashCooldownTimer.Start(_dashCooldown);
					}
					else break;

					_animator.SetToggle(PlayerFSMData.Stat.DashBegin, true);
					_animator.SetFloat(PlayerFSMData.Stat.XSpeed, moveInput.x);
					_animator.SetFloat(PlayerFSMData.Stat.YSpeed, moveInput.y);
					SetBlockInput(true);
					SetFrozen(true);

					_animator.UpdateAnimationState();
					break;

				case PlayerInputCommand.MeleeBegin:
					if (!_groundDetector.IsOnGround && _currentAirAttack >= _maxAirAttack)
						break;
					CancelAnimation();
					break;
					
				case PlayerInputCommand.MeleeAttack:
					if (!_groundDetector.IsOnGround && _currentAirAttack >= _maxAirAttack)
						break;

					if (!_groundDetector.IsOnGround)
						_currentAirAttack++;
					_animator.SetToggle(PlayerFSMData.Stat.MeleeAttack, true);
					_mover.AirAttacking = !_groundDetector.IsOnGround;
					SetDelayInput(true);
					SetFrozen(true);

					TriggerBulletTimeIfPossible();
					break;

				case PlayerInputCommand.MeleeChargeBegin:
					CancelAnimation();
					_animator.SetBool(PlayerFSMData.Stat.Charge, true);

					TriggerBulletTimeIfPossible();
					break;

				case PlayerInputCommand.MeleeChargeBreak:
					CancelAnimation();
					_animator.SetBool(PlayerFSMData.Stat.Charge, false);
					break;

				case PlayerInputCommand.MeleeChargeAttack:
					_weaponSystem.ChargedMeleeAttack(input._additionalValue);
					_animator.SetToggle(PlayerFSMData.Stat.MeleeAttack, true);
					SetDelayInput(true);
					SetFrozen(true);
					break;

				case PlayerInputCommand.RangeBegin:
					_weaponSystem.StartRangeCharge(input._actionChargedPercent);
					_aimAssistant.StartAimingAssistant();
					break;

				case PlayerInputCommand.RangeAttack:
					if (!pInput.IsUsingController)
					{
						_weaponSystem.RangeAttack(aimingDirection);
						break;
					}

					if (aimingDirection.sqrMagnitude <= 0.025f)
					{
						aimingDirection = _aimAssistant.ToNearestTarget(_state._facingRight);
					}
					else
					{
						aimingDirection = _aimAssistant.ExcuteAimingAssistantance(aimingDirection);
					}
					_weaponSystem.RangeAttack(aimingDirection);
					break;

				case PlayerInputCommand.RangeChargeBreak:
					_weaponSystem.StopCharging();
					break;

				case PlayerInputCommand.RangeChargeAttack:
					if (!pInput.IsUsingController)
					{
						_weaponSystem.ChargedRangeAttack(aimingDirection);
						break;
					}
					
					if (aimingDirection.sqrMagnitude <= 0.025f)
					{
						aimingDirection = _aimAssistant.ToNearestTarget(_state._facingRight);
					}
					else
					{
						aimingDirection = _aimAssistant.ExcuteAimingAssistantance(aimingDirection);
					}
					_weaponSystem.ChargedRangeAttack(aimingDirection);
					break;

				case PlayerInputCommand.WithdrawAll:
					_weaponSystem.WithdrawAll();
					TriggerBulletTimeIfPossible();
					break;

				case PlayerInputCommand.WithdrawOne:
					_weaponSystem.WithdrawOne();
					TriggerBulletTimeIfPossible();
					break;

				case PlayerInputCommand.Regenerate:
					if (_state._stamina < _healingStaminaCost) break;
					_animator.SetToggle(PlayerFSMData.Stat.Healing, true);
					SetBlockInput(true);
					SetFrozen(true);
					break;
			}
		}
		#endregion

		#region Private Methods
		void CallDeathHandlers()
		{
			var handlers = new List<ICanHandleDeath>();
			GetComponentsInChildren(handlers);

			foreach (var handler in handlers)
			{
				handler.OnDeath(_state);
			}
		}

		private void CancelAnimation()
		{
			if (_mover.CurrentMovingState == MovingState.Dash)
				_mover.CancelDash();

			SetFrozen(false);
			SetDelayInput(false);
			SetBlockInput(false);
			_animator.SetBool(PlayerFSMData.Stat.Charge, false);
			_weaponSystem.MeleeAttackEnd();
		}

		private void HandleEndurance()
		{
			if (_state._endurance < _state._enduranceSafeThreshlod)
			{
				_enduranceRefreshTimer += TimeManager.PlayerDeltaTime;
			}
			else
			{
				_enduranceRefreshTimer = 0;
			}

			if (_enduranceRefreshTimer >= _state._enduranceRefreshDelay)
			{
				_state._endurance.ResetCurrentValue();
			}

			if (_enduranceRecoverTimer < _state._enduranceRecoverDelay)
			{
				_enduranceRecoverTimer += TimeManager.PlayerDeltaTime;
			}
			else if (!_state._endurance.IsFull())
			{
				_state._endurance += _state._enduranceRecoverRate * TimeManager.PlayerDeltaTime;
			}
		}

		void SetDelayInput(bool value)
		{
			_animator.SetBool(PlayerFSMData.Stat.DelayInput, value);
			_input.DelayInput = value;
		}

		void SetBlockInput(bool value)
		{
			_animator.SetBool(PlayerFSMData.Stat.BlockInput, value);
			_input.BlockInput = value;
		}

		void SetFrozen(bool value)
		{
			_animator.SetBool(PlayerFSMData.Stat.Frozen, value);
			_state._frozen = value;
		}

		void TriggerBulletTimeIfPossible()
		{
			if (_bulletTimeReady)
				_btManager.TriggerBulletTime();

			_bulletTimeReady = false;
		}
		
		void UpdateFacingDirection(Vector2 movementInput)
		{
			if (movementInput.x != 0)
			{
				_state._facingRight = movementInput.x > 0;
				_animator.FlipX = !_state._facingRight;
			}
		}
		#endregion

		#region Public Methods
		public void ReplenishStamina(int amount)
		{
			_state._stamina += amount;
			_state._stamina.Clamp();
		}

		public void ResetState()
		{
			_weaponSystem.ResetWeapon();
			CancelAnimation();
		}
		#endregion

		#region Interface Handler

		public void OnPullingKnife(ICanStickKnife canStick, ThrowingKnife knife)
		{
			//if (!_state._stamina.IsFull())
			//	_state._stamina += canStick.RestoredStamina;
		}

		#endregion
	}
}
