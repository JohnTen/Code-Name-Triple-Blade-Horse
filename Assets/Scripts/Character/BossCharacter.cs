using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleBladeHorse.Animation;
using TripleBladeHorse.Combat;
using TripleBladeHorse.Movement;
using TripleBladeHorse.AI;

namespace TripleBladeHorse
{
	public class BossCharacter : MonoBehaviour
	{
		[SerializeField] BaseWeapon _weapon;
		[SerializeField] HitBox[] _hitboxes;

		[Header("Retreat")]
		[SerializeField] float _retreatTime;
		[SerializeField] ParticleSystem.MinMaxCurve _retreatSpeed;

		[Header("Slash")]
		[SerializeField] float _slashTime;
		[SerializeField] ParticleSystem.MinMaxCurve _slashSpeed;
		[SerializeField] AttackMove _slashMove;

		[Header("Combo2")]
		[SerializeField] float _combo2RaiseTime;
		[SerializeField] ParticleSystem.MinMaxCurve _combo2RaiseSpeed;
		[SerializeField] float _combo2PauseTime;
		[SerializeField] ParticleSystem.MinMaxCurve _combo2PauseSpeed;
		[SerializeField] float _combo2CrushTime;
		[SerializeField] ParticleSystem.MinMaxCurve _combo2CrushSpeed;
		[SerializeField] float _combo2SpeedScaler;
		[SerializeField] Vector2 _combo2CrushOffset;
		[SerializeField] AttackMove _crushMove;

		[Header("Combo3/thrust")]
		[SerializeField] float _thrustTime;
		[SerializeField] ParticleSystem.MinMaxCurve _thrustSpeed;
		[SerializeField] AttackMove _thrustMove;

		FSM _animator;
		CharacterState _state;
		ICharacterInput<BossInput> _input;
		ICanDetectGround _groundDetector;
		HitFlash _hitFlash;
		BossMover _mover;
		AttackMove _currentMove;

		private void Awake()
		{
			_animator = GetComponent<FSM>();
			_state = GetComponent<CharacterState>();
			_input = GetComponent<ICharacterInput<BossInput>>();
			_hitFlash = GetComponent<HitFlash>();
			_mover = GetComponent<BossMover>();
			_groundDetector = GetComponent<ICanDetectGround>();
			
			_input.OnReceivedInput += HandleReceivedInput;
			_groundDetector.OnLandingStateChanged += HandleLandingStateChange;
			_animator.Subscribe(Animation.AnimationState.FadingIn, HandleFadeInAnimation);
			_animator.OnReceiveFrameEvent += HandleFrameEvent;
			foreach (var hitbox in _hitboxes)
			{
				hitbox.OnHit += HandleOnHit;
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
				_animator.SetBool(BossAnimationData.Stat.Death, true);
				print("Boss Dead");
			}
		}

		private void HandleFrameEvent(FrameEventEventArg eventArgs)
		{
			if (eventArgs._name == "AttackBegin")
			{
				var attack = AttackPackage.CreateNewPackage();
				attack._hitPointDamage.Base = _state._hitPointDamage;
				attack._enduranceDamage.Base = _state._enduranceDamage;

				_weapon.Activate(attack, _currentMove);
			}
			else if (eventArgs._name == "AttackEnd")
			{
				_weapon.Deactivate();
			}
		}

		private void HandleLandingStateChange(ICanDetectGround detector, LandingEventArgs eventArgs)
		{
			if (eventArgs.currentLandingState == LandingState.OnGround 
			 && _animator.GetCurrentAnimation().name == BossAnimationData.Anim.Combo2_3)
			{
				_mover.InterruptContantMove();
			}
		}

		private void Update()
		{
			if (!_mover.IsConstantMoving && _animator.GetBool(BossAnimationData.Stat.Retreat))
			{
				_animator.SetBool(BossAnimationData.Stat.Retreat, false);
			}

			_state._frozen = _animator.GetBool(BossAnimationData.Stat.Frozen);
			_input.DelayInput = _animator.GetBool(BossAnimationData.Stat.DelayInput);
			_input.BlockInput = _animator.GetBool(BossAnimationData.Stat.BlockInput);

			var aimInput = _state._frozen ? Vector2.zero : _input.GetAimingDirection().normalized;
			var moveInput = _state._frozen ? Vector2.zero : _input.GetMovingDirection().normalized;
			if (moveInput.x != 0)
				moveInput = NormalizeHorizonalDirection(moveInput);
			_mover.Move(moveInput);

			if (_state._frozen)
			{
				_animator.SetFloat(BossAnimationData.Stat.XSpeed, 0);
			}
			else
			{
				_animator.SetFloat(BossAnimationData.Stat.XSpeed, moveInput.x);
			}

			UpdateFacingDirection(aimInput);
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
				case BossAnimationData.Anim.Slash1:
				case BossAnimationData.Anim.Slash2:
					UpdateFacingDirection(aim);
					_mover.InvokeConstantMovement(moveDirection, _slashSpeed, _slashTime);
					break;

				case BossAnimationData.Anim.Combo2_1:
					UpdateFacingDirection(aim);
					_mover.InvokeConstantMovement(Vector2.up, _combo2RaiseSpeed, _combo2RaiseTime);
					break;

				case BossAnimationData.Anim.Combo2_2:
					UpdateFacingDirection(aim);
					_mover.InvokeConstantMovement(Vector2.up, _combo2PauseSpeed, _combo2PauseTime);
					break;

				case BossAnimationData.Anim.Combo2_3:
					var player = GameManager.PlayerInstance;
					Vector2 toPlayer = player.transform.position - transform.position;

					UpdateFacingDirection(toPlayer);
					toPlayer += _state._facingRight ? _combo2CrushOffset : -_combo2CrushOffset;
					var speed = (toPlayer.magnitude / _combo2CrushTime) * _combo2SpeedScaler;

					_combo2CrushSpeed.constant = speed;
					_combo2CrushSpeed.curveMultiplier = speed;
					_mover.InvokeConstantMovement(toPlayer, _combo2CrushSpeed, _combo2CrushTime);
					break;

				case BossAnimationData.Anim.Combo3_2:
					UpdateFacingDirection(aim);
					_mover.InvokeConstantMovement(moveDirection, _thrustSpeed, _thrustTime);
					break;

				case BossAnimationData.Anim.Retreat:
					UpdateFacingDirection(aim);
					_mover.InvokeConstantMovement(-moveDirection, _retreatSpeed, _retreatTime);

					break;
			}
		}

		private void HandleReceivedInput(InputEventArg<BossInput> input)
		{
			switch (input._command)
			{
				case BossInput.Slash:
					_animator.SetToggle(BossAnimationData.Stat.Slash, true);
					_currentMove = _slashMove;
					SetBlockInput(true);
					SetFrozen(true);
					break;

				case BossInput.DashAttack:
					_animator.SetToggle(BossAnimationData.Stat.Thrust, true);
					_currentMove = _thrustMove;
					SetBlockInput(true);
					SetFrozen(true);
					break;

				case BossInput.Dodge:
					_animator.SetBool(BossAnimationData.Stat.Retreat, true);

					var aim = _input.GetAimingDirection();
					var moveDirection = NormalizeHorizonalDirection(aim);

					UpdateFacingDirection(aim);
					_mover.InvokeConstantMovement(-moveDirection, _retreatSpeed, _retreatTime);
					SetBlockInput(true);
					SetFrozen(true);
					break;

				case BossInput.JumpAttack:
					_animator.SetToggle(BossAnimationData.Stat.Combo2, true);
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

		void SetDelayInput(bool value)
		{
			_animator.SetBool(BossAnimationData.Stat.DelayInput, value);
			_input.DelayInput = value;
		}

		void SetBlockInput(bool value)
		{
			_animator.SetBool(BossAnimationData.Stat.BlockInput, value);
			_input.BlockInput = value;
		}

		void SetFrozen(bool value)
		{
			_animator.SetBool(BossAnimationData.Stat.Frozen, value);
			_state._frozen = value;
		}

		void UpdateFacingDirection(Vector2 aimInput)
		{
			if (aimInput.x != 0)
			{
				_state._facingRight = aimInput.x > 0;
				if (_animator.GetBool(BossAnimationData.Stat.Backward))
				{
					_state._facingRight = !_state._facingRight;
				}

				_animator.FlipX = _state._facingRight;
			}
		}
	}
}
