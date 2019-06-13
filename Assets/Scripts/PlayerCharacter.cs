using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
	[SerializeField] PlayerState _state;

	PlayerInput _input;
	PlayerMover _mover;
	WeaponSystem _weaponSystem;
	ICanDetectGround groundDetector;
	PlayerAnimation _animation;

	InputCommand _lastInput;

	private void Awake()
	{
		_input = GetComponent<PlayerInput>();
		_mover = GetComponent<PlayerMover>();
		_animation = GetComponent<PlayerAnimation>();
		_weaponSystem = GetComponent<WeaponSystem>();
		groundDetector = GetComponent<ICanDetectGround>();

		_input.OnReceivedInput += OnReceivedInputHandler;
		groundDetector.OnLanding += GroundDetector_OnLanding;
		_animation.OnRecievedFrameEvent += OnRecievedFrameEvent;
	}

	private void OnRecievedFrameEvent(string name, float value)
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

	private void GroundDetector_OnLanding()
	{
		_animation.SetBool("Landing", true);
	}

	private void OnReceivedInputHandler(InputEventArg input)
	{
		if (_state._frozen) return;

		switch (input._command)
		{
			case InputCommand.Jump:
				_mover.Jump();
				_animation.SetBool("Jump", true);
				break;

			case InputCommand.Dash:
				_mover.Dash(_input.GetMovingDirection());
				break;

			case InputCommand.MeleeAttack:
				_animation.Attack();
				break;

			case InputCommand.MeleeChargeAttack:
				_weaponSystem.ChargedMeleeAttack(input._additionalValue);
				break;

			case InputCommand.RangeAttack:
				_weaponSystem.RangeAttack(_input.GetAimingDirection());
				break;

			case InputCommand.RangeChargeAttack:
				_weaponSystem.ChargedRangeAttack(_input.GetAimingDirection());
				break;

			case InputCommand.WithdrawAll:
				_weaponSystem.WithdrawAll();
				break;

			case InputCommand.WithdrawOne:
				_weaponSystem.WithdrawOne();
				break;
		}
	}

	private void Update()
	{
		_state._frozen = _animation.GetBool("Frozen") || _mover.IsDashing || _weaponSystem.Frozen;

		_input.DelayInput = _state._frozen;

		var moveInput = _state._frozen ? Vector2.zero : _input.GetMovingDirection();
		_mover.Move(moveInput);

		if (_state._frozen)
		{
			_animation.SetFloat("XSpeed", 0);
			_animation.SetFloat("YSpeed", 0);
		}
		else
		{
			_animation.SetFloat("XSpeed", _mover.Velocity.x);
			_animation.SetFloat("YSpeed", _mover.Velocity.y);
		}
	}
}
