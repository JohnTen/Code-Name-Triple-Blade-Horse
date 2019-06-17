using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
	[SerializeField] PlayerState _state;

	PlayerInput _input;
	PlayerMover _mover;
	WeaponSystem _weaponSystem;
	ICanDetectGround _groundDetector;
	PlayerAnimation _animation;

	InputCommand _lastInput;

	private void Awake()
	{
		_input = GetComponent<PlayerInput>();
		_mover = GetComponent<PlayerMover>();
		_animation = GetComponent<PlayerAnimation>();
		_weaponSystem = GetComponent<WeaponSystem>();
		_groundDetector = GetComponent<ICanDetectGround>();

		_mover.OnMovingStateChanged += HandleMovingStateChanged;
		_groundDetector.OnLandingStateChanged += HandleLandingStateChanged;
		_input.OnReceivedInput += OnReceivedInputHandler;
		_mover.OnDashingFinished += DashingFinishedHandler;
		_animation.OnRecievedFrameEvent += OnRecievedFrameEvent;
		_weaponSystem.OnPull += PullHandler;
	}

	private void HandleLandingStateChanged(ICanDetectGround sender, LandingEventArgs eventArgs)
	{
		if (eventArgs.currentLandingState == LandingState.OnGround)
			_animation.SetBool("Landing", true);
	}

	private void HandleMovingStateChanged(ICanChangeMoveState sender, MovingEventArgs eventArgs)
	{
		print(Time.frameCount + "Moving state changed, last " + eventArgs.lastMovingState
			+ "\ncurrent " + eventArgs.currentMovingState
			+ "\nvelocity " + eventArgs.velocity);
		if (eventArgs.lastMovingState == MovingState.Dash)
		{
			_animation.SetBool("Dash", false);
		}
	}

	private void DashingFinishedHandler()
	{
		print("Dashfinished2" + +Time.frameCount);
	}

	private void PullHandler(Vector3 direction)
	{
		_mover.Pull(direction);
		_animation.SetBool("Jump", true);
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

	private void LandingHandler()
	{
		_animation.SetBool("Landing", true);
	}

	private void OnReceivedInputHandler(InputEventArg input)
	{
		switch (input._command)
		{
			case InputCommand.Jump:
				_mover.Jump();
				_animation.SetBool("Jump", true);
				break;

			case InputCommand.Dash:
				_mover.Dash(_input.GetMovingDirection());
				_animation.SetBool("Dash", true);
				break;

			case InputCommand.MeleeBegin:
				print("Melee begin");
				break;

			case InputCommand.MeleeAttack:
				_animation.Attack();
				break;

			case InputCommand.MeleeChargeAttack:
				print("Charged melee  " + input._additionalValue);
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
		_state._frozen = 
			_animation.GetBool("Frozen")
			|| _mover.CurrentMovingState == MovingState.Dash
			|| _mover.PullDelaying
			|| _weaponSystem.Frozen;

		_input.DelayInput = _animation.GetBool("DelayInput");

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

	private void Cancel()
	{

	}
}
