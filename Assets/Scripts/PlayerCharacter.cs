﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleBladeHorse.Animation;

public class PlayerCharacter : MonoBehaviour
{
	[SerializeField] PlayerState _state;
	[SerializeField] Transform _hittingPoint;

	ICharacterInput<PlayerInputCommand> _input;
	PlayerMover _mover;
	WeaponSystem _weaponSystem;
	ICanDetectGround _groundDetector;
	PlayerAnimation _animator;
	IAttackable _hitbox;
	HitFlash _hitFlash;

	PlayerInputCommand _lastInput;

	public Transform HittingPoint => _hittingPoint;

	private void Awake()
	{
		_input = GetComponent<ICharacterInput<PlayerInputCommand>>();
		_mover = GetComponent<PlayerMover>();
		_animator = GetComponent<PlayerAnimation>();
		_weaponSystem = GetComponent<WeaponSystem>();
		_groundDetector = GetComponent<ICanDetectGround>();
		_hitbox = GetComponentInChildren<IAttackable>();
		_hitFlash = GetComponent<HitFlash>();

		_mover.OnMovingStateChanged += HandleMovingStateChanged;
		_groundDetector.OnLandingStateChanged += HandleLandingStateChanged;
		_input.OnReceivedInput += OnReceivedInputHandler;
		_animator.OnRecievedFrameEvent += HandleAnimationFrameEvent;
		_weaponSystem.OnPull += PullHandler;
		_hitbox.OnHit += HandleOnHit;
	}

	private void HandleAnimationFrameEvent(string name, float value)
	{
		if (name == "AttackBegin")
		{
			_weaponSystem.MeleeAttack();
		}
		else if (name == "AttackEnd")
		{
			_weaponSystem.MeleeAttackEnd();
		}
		else if (name == "AttackStepDistance")
		{
			_mover.SetStepDistance(value);
		}
		else if (name == "AttackStepSpeed")
		{
			_mover.SetStepSpeed(value);
		}
	}

	private void HandleOnHit(AttackPackage attack, AttackResult result)
	{
		_hitFlash.Flash();
		_state._hitPoints -= result._finalDamage;
		_state._endurance -= result._finalFatigue;
		_mover.Knockback(attack._fromDirection * attack._knockback);

		if (_state._hitPoints <= 0)
		{
			print("Player Dead");
			_mover.ResetMovement();
			RecoverPoint.MainRespawn(true);
		}
	}

	private void HandleLandingStateChanged(ICanDetectGround sender, LandingEventArgs eventArgs)
	{
		if (eventArgs.currentLandingState == LandingState.OnGround)
		{
			_animator.SetBool("Landing", true);
		}
	}

	private void HandleMovingStateChanged(ICanChangeMoveState sender, MovingEventArgs eventArgs)
	{
		if (eventArgs.lastMovingState == MovingState.Dash)
		{
			_animator.SetBool("Dash", false);
		}
	}

	private void PullHandler(Vector3 direction)
	{
		_mover.Pull(direction);
		_animator.SetBool("Jump", true);
	}

	private void LandingHandler()
	{
		_animator.SetBool("Landing", true);
	}

	private void OnReceivedInputHandler(InputEventArg<PlayerInputCommand> input)
	{
		switch (input._command)
		{
			case PlayerInputCommand.Jump:
				Cancel();
				_mover.Jump();
				_animator.SetBool("Jump", true);
				break;

			case PlayerInputCommand.Dash:
				if (_state._stamina <= 0) break;

				Cancel();
				_state._stamina -= 1;
				_mover.Dash(_input.GetMovingDirection());
				_animator.SetBool("Dash", true);
				break;

			case PlayerInputCommand.MeleeBegin:
				Cancel();
				break;

			case PlayerInputCommand.MeleeAttack:
				_animator.SetBool("MeleeAttak", true);
                _animator.Attack();
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

	private void Update()
	{
		_state._frozen = 
			_animator.GetBool("Frozen")
			|| _mover.CurrentMovingState == MovingState.Dash
			|| _mover.PullDelaying
			|| _weaponSystem.Frozen;

		_input.DelayInput = _animator.GetBool("DelayInput");
		_input.BlockInput = _mover.BlockInput;

		var moveInput = _state._frozen ? Vector2.zero : _input.GetMovingDirection();
		_mover.Move(moveInput);

		if (_state._frozen)
		{
			_animator.SetFloat("XSpeed", 0);
			_animator.SetFloat("YSpeed", 0);
		}
		else
		{
			_animator.SetFloat("XSpeed", _mover.Velocity.x);
			_animator.SetFloat("YSpeed", _mover.Velocity.y);
		}
	}

	private void Cancel()
	{
		if (_mover.CurrentMovingState == MovingState.Dash)
			_mover.CancelDash();
	}
}
