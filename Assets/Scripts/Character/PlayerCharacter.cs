using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleBladeHorse.Animation;
using TripleBladeHorse.Combat;
using TripleBladeHorse.Movement;

namespace TripleBladeHorse
{
	public class PlayerCharacter : MonoBehaviour, ICanHandlePullingKnife
	{
		#region Fields
		[SerializeField] Transform _hittingPoint;
		[SerializeField] PlayerState _state;
		[SerializeField] int _maxAirAttack;

		[SerializeField] bool extraJump;
		int _currentAirAttack;
		FSM _animator;
		ICanDetectGround _groundDetector;
		IAttackable _hitbox;
		HitFlash _hitFlash;
		ICharacterInput<PlayerInputCommand> _input;
		PlayerMover _mover;
		WeaponSystem _weaponSystem;

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

			_mover.OnMovingStateChanged += HandleMovingStateChanged;
			_mover.OnBeginDashingInvincible += HandleDashingInvincibleBegin;
			_mover.OnStopDashingInvincible += HandleDashingInvincibleStop;
			_groundDetector.OnLandingStateChanged += HandleLandingStateChanged;
			_input.OnReceivedInput += HandleReceivedInput;
			_animator.Subscribe(Animation.AnimationState.FadingIn, HandleAnimationFadeinEvent);
			_animator.Subscribe(Animation.AnimationState.FadingOut, HandleAnimationFadeoutEvent);
			_animator.OnReceiveFrameEvent += HandleAnimationFrameEvent;
			_weaponSystem.OnPull += HandlePull;
			_hitbox.OnHit += HandleOnHit;

			// Debug
			GetComponent<Test>().OnPull += HandlePull;
		}

		private void Update()
		{
			var frozen =
				_animator.GetBool("Frozen")
				|| _mover.CurrentMovingState == MovingState.Dash
				|| _mover.PullDelaying
				|| _weaponSystem.Frozen;

			SetFrozen(frozen);
			
			_input.DelayInput = _animator.GetBool("DelayInput");
			_input.BlockInput = _mover.BlockInput || _animator.GetBool("BlockInput");

			var moveInput = _state._frozen ? Vector2.zero : _input.GetMovingDirection().normalized;
			if (moveInput.x != 0)
				moveInput = DirectionalHelper.NormalizeHorizonalDirection(moveInput);
			_mover.Move(moveInput);

			if (_state._frozen)
			{
				_animator.SetFloat("XSpeed", 0);
				_animator.SetFloat("YSpeed", _mover.Velocity.y);
			}
			else
			{
				_animator.SetFloat("XSpeed", moveInput.x);
				_animator.SetFloat("YSpeed", _mover.Velocity.y);
				UpdateFacingDirection(moveInput);
			}

			HandleEndurance();
		}
		#endregion

		#region Event Handlers
		private void HandleAnimationFadeinEvent(AnimationEventArg eventArgs)
		{
			SetBlockInput(eventArgs._animation.blockInputOnStart);
			SetDelayInput(eventArgs._animation.delayInputOnStart);
			SetFrozen(eventArgs._animation.frozenOnStart);
			if (eventArgs._animation.airAttack)
				_mover.AirAttacking = true;
		}

		private void HandleAnimationFadeoutEvent(AnimationEventArg eventArgs)
		{
			if (eventArgs._animation.airAttack)
				_mover.AirAttacking = false;
		}

		private void HandleAnimationFrameEvent(FrameEventEventArg eventArgs)
		{
			if (eventArgs._name == "AttackBegin")
			{
				_weaponSystem.MeleeAttack();
			}
			else if (eventArgs._name == "AttackEnd")
			{
				_weaponSystem.MeleeAttackEnd();
				_animator.SetBool("Charge", false);
			}
			else if (eventArgs._name == "AttackStepDistance")
			{
				_mover.SetStepDistance(eventArgs._floatData);
			}
			else if (eventArgs._name == "AttackStepSpeed")
			{
				_mover.SetStepSpeed(eventArgs._floatData);
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

		private void HandleMovingStateChanged(ICanChangeMoveState sender, MovingEventArgs eventArgs)
		{
			if (eventArgs.lastMovingState == MovingState.Dash)
			{
				_animator.SetToggle("DashEnd", true);
			}
		}

		private void HandlePull(Vector3 direction)
		{
			_currentAirAttack = 0;
			_animator.SetToggle("Jump", true);
			_mover.Pull(direction);
			extraJump = true;
		}

		private void HandleLandingStateChanged(ICanDetectGround sender, LandingEventArgs eventArgs)
		{
			if (eventArgs.currentLandingState == LandingState.OnGround)
			{
				_currentAirAttack = 0;
				_animator.SetBool("Airborne", false);
				_animator.SetBool("Charge", false);
				SetBlockInput(true);
				SetFrozen(true);
				extraJump = false;
			}
			else
			{
				_animator.SetBool("Airborne", true);
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
				print("Player Dead");
				_mover.ResetMovement();
				_weaponSystem.ResetWeapon();
				RecoverPoint.MainRespawn(true);
			}

			if (_state._endurance <= 0)
			{
				CancelAnimation();
				_state._endurance.Current = 0;
				_animator.SetToggle(attack._staggerAnimation, true);
			}
		}

		private void HandleReceivedInput(InputEventArg<PlayerInputCommand> input)
		{
			switch (input._command)
			{
				case PlayerInputCommand.Jump:
					if (!_groundDetector.IsOnGround && !extraJump) break;
					CancelAnimation();
					_mover.Jump();
					_animator.SetToggle("Jump", true);
					extraJump = false;
					break;

				case PlayerInputCommand.Dash:

					CancelAnimation();
					var moveInput = _input.GetMovingDirection();
					if (moveInput == Vector2.zero)
						moveInput = _state._facingRight ? Vector2.right : Vector2.left;
					else
						moveInput = DirectionalHelper.NormalizeOctadDirection(moveInput);

					var airDash = !_groundDetector.IsOnGround || moveInput.y > 0;
					
					UpdateFacingDirection(moveInput);

					if (_state._stamina > 0)
					{
						_mover.Dash(moveInput);
						_state._stamina -= 1;
					}
					else if (!airDash)
					{
						_mover.ShortDash(moveInput);
					}
					else break;
					
					extraJump = true;
					_animator.SetToggle("DashBegin", true);
					_animator.SetFloat("XSpeed", moveInput.x);
					_animator.SetFloat("YSpeed", moveInput.y);
					SetDelayInput(true);

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
					_animator.SetToggle("MeleeAttack", true);
					_mover.AirAttacking = !_groundDetector.IsOnGround;
					SetDelayInput(true);
					break;

				case PlayerInputCommand.MeleeChargeBegin:
					CancelAnimation();
					_animator.SetBool("Charge", true);
					break;

				case PlayerInputCommand.MeleeChargeBreak:
					CancelAnimation();
					_animator.SetBool("Charge", false);
					break;

				case PlayerInputCommand.MeleeChargeAttack:
					_weaponSystem.ChargedMeleeAttack(input._additionalValue);
					_animator.SetToggle("MeleeAttack", true);
					SetDelayInput(true);
					break;

				case PlayerInputCommand.RangeAttack:
					_weaponSystem.RangeAttack(_input.GetAimingDirection());
					break;

				case PlayerInputCommand.RangeChargeAttack:
					_weaponSystem.ChargedRangeAttack(_input.GetAimingDirection());
					break;

				case PlayerInputCommand.WithdrawAll:
					_weaponSystem.WithdrawAll();
					break;

				case PlayerInputCommand.WithdrawOne:
					_weaponSystem.WithdrawOne();
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

			_animator.SetBool("Charge", false);
			_animator.SetBool("Frozen", false);
			_animator.SetBool("DelayInput", false);
			_animator.SetBool("BlockInput", false);
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
			_animator.SetBool("DelayInput", value);
			_input.DelayInput = value;
		}

		void SetBlockInput(bool value)
		{
			_animator.SetBool("BlockInput", value);
			_input.BlockInput = value;
		}

		void SetFrozen(bool value)
		{
			_animator.SetBool("Frozen", value);
			_state._frozen = value;
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
		#endregion

		#region Interface Handler

		public void OnPullingKnife(ICanStickKnife canStick, ThrowingKnife knife)
		{
			if (!_state._stamina.IsFull())
				_state._stamina += canStick.RestoredStamina;
		}

		#endregion
	}
}
