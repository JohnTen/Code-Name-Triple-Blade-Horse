using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleBladeHorse.Animation;
using TripleBladeHorse.Combat;
using TripleBladeHorse.Movement;

namespace TripleBladeHorse
{
	public class PlayerCharacter : MonoBehaviour
	{
		#region Fields
		[SerializeField] Transform _hittingPoint;
		[SerializeField] PlayerState _state;
		
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
			_animator.OnReceiveFrameEvent += HandleAnimationFrameEvent;
			_weaponSystem.OnPull += HandlePull;
			_hitbox.OnHit += HandleOnHit;
		}

		private void Update()
		{
			_state._frozen =
				_animator.GetBool("Frozen")
				|| _mover.CurrentMovingState == MovingState.Dash
				|| _mover.PullDelaying
				|| _weaponSystem.Frozen;

			_input.DelayInput = _animator.GetBool("DelayInput");
			_input.BlockInput = _mover.BlockInput || _animator.GetBool("BlockInput");

			var moveInput = _state._frozen ? Vector2.zero : _input.GetMovingDirection().normalized;
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
			if (eventArgs._animation.name == "ATK_Melee_Ground_1" ||
				eventArgs._animation.name == "ATK_Melee_Ground_2" ||
				eventArgs._animation.name == "ATK_Melee_Ground_3")
			{
				_animator.SetBool("DelayInput", true);
				_input.DelayInput = true;
			}
			else
			{
				_animator.SetBool("DelayInput", false);
				_input.DelayInput = false;
			}

			if (eventArgs._animation.name == "Droping_Buffering" ||
				eventArgs._animation.name == "Hitten_Ground_Small" ||
				eventArgs._animation.name == "Hitten_Ground_Normal" ||
				eventArgs._animation.name == "Hitten_Ground_Big")
			{
				_animator.SetBool("BlockInput", true);
				_input.BlockInput = true;
			}
			else
			{
				_animator.SetBool("BlockInput", false);
				_input.BlockInput = false;
			}
		}

		private void HandleAnimationFrameEvent(FrameEventEventArg eventArgs)
		{
			print(eventArgs._name);
			if (eventArgs._name == "AttackBegin")
			{
				_weaponSystem.MeleeAttack();
			}
			else if (eventArgs._name == "AttackEnd")
			{
				_weaponSystem.MeleeAttackEnd();
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
			_animator.SetToggle("Jump", true);
			_mover.Pull(direction);
		}

		private void HandleLanding()
		{
			_animator.SetBool("BlockInput", true);
		}

		private void HandleLandingStateChanged(ICanDetectGround sender, LandingEventArgs eventArgs)
		{
			if (eventArgs.currentLandingState == LandingState.OnGround)
			{
				_animator.SetBool("Airborne", false);
			}
			else
			{
				_animator.SetBool("Airborne", true);
			}
		}

		private void HandleOnHit(AttackPackage attack, AttackResult result)
		{
			_hitFlash.Flash();
			_state._hitPoints -= result._finalDamage;
			_state._endurance -= result._finalFatigue;
			_enduranceRecoverTimer = 0;
			_mover.Knockback(attack._fromDirection * attack._knockback);
			
			if (_state._hitPoints <= 0)
			{
				print("Player Dead");
				_mover.ResetMovement();
				_weaponSystem.ResetWeapon();
				RecoverPoint.MainRespawn(true);
			}

			if (_state._endurance <= 0)
			{
				_state._endurance.Current = 0;
				_animator.SetToggle(attack._staggerAnimation, true);
			}
		}

		private void HandleReceivedInput(InputEventArg<PlayerInputCommand> input)
		{
			switch (input._command)
			{
				case PlayerInputCommand.Jump:
					if (!_groundDetector.IsOnGround) break;
					Cancel();
					_mover.Jump();
					_animator.SetToggle("Jump", true);
					print("Jump");
					break;

				case PlayerInputCommand.Dash:

					Cancel();
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
					_animator.SetToggle("DashBegin", true);
					_animator.SetFloat("XSpeed", moveInput.x);
					_animator.SetFloat("YSpeed", moveInput.y);

					_animator.UpdateAnimationState();
					break;

				case PlayerInputCommand.MeleeBegin:
					Cancel();
					break;

				case PlayerInputCommand.MeleeAttack:
					_animator.SetToggle("MeleeAttack", true);
					print("Attack");
					break;

				case PlayerInputCommand.MeleeChargeAttack:
					_weaponSystem.ChargedMeleeAttack(input._additionalValue);
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
		private void Cancel()
		{
			if (_mover.CurrentMovingState == MovingState.Dash)
				_mover.CancelDash();

			_animator.SetBool("Frozen", false);
			_animator.SetBool("DelayInput", false);
			_animator.SetBool("BlockInput", false);
		}

		private void HandleEndurance()
		{
			if (_state._endurance < _state._enduranceSafeThreshlod)
			{
				_enduranceRefreshTimer += Time.deltaTime;
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
				_enduranceRecoverTimer += Time.deltaTime;
			}
			else if (!_state._endurance.IsFull())
			{
				_state._endurance += _state._enduranceRecoverRate * Time.deltaTime;
			}
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
	}
}
