using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public enum InputCommand
{
	Null,
	Dash,
	Jump,
	RangeBegin,
	RangeAttack,
	RangeChargeAttack,
	MeleeBegin,
	MeleeAttack,
	MeleeChargeAttack,
	WithdrawAll,
	WithdrawOne,
}

public struct InputEventArg
{
	public InputCommand _command;
	public float _additionalValue;

	public InputEventArg(InputCommand command)
	{
		_command = command;
		_additionalValue = 0;
	}

	public InputEventArg(InputCommand command, float additionalValue)
	{
		_command = command;
		_additionalValue = additionalValue;
	}
}

public class PlayerInput : MonoBehaviour, IInputModelPlugable
{
	[SerializeField] float _meleeChargeTime;
	[SerializeField] float _meleeMaxChargeTime;
	[SerializeField] float _rangeChargeTime;
	[SerializeField] float _withdrawTime;
	[SerializeField] Transform _aimingPivot;

	float _meleeChargeTimer;
	float _rangeChargeTimer;
	float _withdrawTimer;

	bool _delayingInput;
	bool _usingController;
	bool _throwPressedBefore;
	IInputModel _input;
	InputEventArg _delayedInput;

	public bool DelayInput
	{
		get => _delayingInput;
		set
		{
			if (!value && _delayingInput && _delayedInput._command != InputCommand.Null)
			{
				OnReceivedInput?.Invoke(_delayedInput);
			}

			if (_delayingInput == value)
				return;

			_delayingInput = value;
			if (_delayingInput)
			{
				_delayedInput._command = InputCommand.Null;
			}
		}
	}

	public event Action<InputEventArg> OnReceivedInput;

	public Vector2 GetMovingDirection()
	{
		return new Vector2(_input.GetAxis("MoveX"), _input.GetAxis("MoveY"));
	}

	public Vector2 GetAimingDirection()
	{
		var aim = Vector2.zero;
		if (_usingController)
		{
			aim = new Vector2(_input.GetAxis("LookX"), _input.GetAxis("LookY"));
		}
		else
		{
			aim = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - _aimingPivot.position);
			if (aim.SqrMagnitude() > 1)
				aim.Normalize();
		}

		return aim;
	}

	public void SetInputModel(IInputModel model)
	{
		_input = model;
		_usingController = model is ControllerInputModel;
	}

	private void Start()
	{
		InputManager.Instance.RegisterPluggable(0, this);
	}

	private void Update()
	{
		HandleMeleeInput();
		HandleRangeInput();
		HandleJumpInput();
		HandleDashInput();
		HandleWithdraw();
	}

	private void InvokeInputEvent(InputCommand command)
	{
		if (DelayInput)
		{
			_delayedInput._command = command;
			return;
		}
		OnReceivedInput?.Invoke(new InputEventArg(command));
	}

	private void InvokeInputEvent(InputCommand command, float value)
	{
		if (DelayInput)
		{
			_delayedInput._command = command;
			_delayedInput._additionalValue = value;
			return;
		}

		OnReceivedInput?.Invoke(new InputEventArg(command, value));
	}

	private void HandleMeleeInput()
	{
		if (_input.GetButtonDown("Melee"))
		{
			_meleeChargeTimer = 0;
			InvokeInputEvent(InputCommand.MeleeBegin);
		}

		if (_input.GetButton("Melee"))
		{
			_meleeChargeTimer += Time.deltaTime;
			if (_meleeChargeTimer > _meleeMaxChargeTime)
			{
				InvokeInputEvent(InputCommand.MeleeChargeAttack, 1);
				_meleeChargeTimer = float.NegativeInfinity;
			}
		}

		if (_input.GetButtonUp("Melee"))
		{
			if (_meleeChargeTimer > _meleeChargeTime)
			{
				var chargedPercent =
					(_meleeChargeTimer - _meleeChargeTime) /
					(_meleeMaxChargeTime - _meleeChargeTime);

				InvokeInputEvent(InputCommand.MeleeChargeAttack, chargedPercent);
			}
			else if (_meleeChargeTimer > 0)
			{
				InvokeInputEvent(InputCommand.MeleeAttack);
			}
		}
	}

	private void HandleRangeInput()
	{
		if (_usingController)
		{
			HandleControllerRangeInput();
			return;
		}

		if (_input.GetButtonDown("Throw"))
		{
			_rangeChargeTimer = 0;
			InvokeInputEvent(InputCommand.RangeBegin);
		}

		if (_input.GetButton("Throw"))
		{
			_rangeChargeTimer += Time.deltaTime;
			if (_rangeChargeTimer > _rangeChargeTime)
			{
				InvokeInputEvent(InputCommand.RangeChargeAttack);
				_rangeChargeTimer = float.NegativeInfinity;
			}
		}

		if (_input.GetButtonUp("Throw"))
		{
			if (_rangeChargeTimer > 0)
				InvokeInputEvent(InputCommand.RangeAttack);
		}
	}

	private void HandleControllerRangeInput()
	{
		var throwPressed = false;

		if (GetAimingDirection().sqrMagnitude > 0.25f)
		{
			if (_input.GetAxis("Throw") > 0.3f)
			{
				throwPressed = true;
			}
		}

		if (throwPressed)
		{
			// OnButtonDown
			if (!_throwPressedBefore)
			{
				_throwPressedBefore = true;
				_rangeChargeTimer = 0;
				InvokeInputEvent(InputCommand.RangeBegin);
			}

			// OnButton
			_rangeChargeTimer += Time.deltaTime;
			if (_rangeChargeTimer > _rangeChargeTime)
			{
				InvokeInputEvent(InputCommand.RangeChargeAttack);
				_rangeChargeTimer = float.NegativeInfinity;
			}
		}
		// OnButtonUp
		else if (_throwPressedBefore)
		{
			if (_rangeChargeTimer > 0)
				InvokeInputEvent(InputCommand.RangeAttack);
		}
	}

	private void HandleJumpInput()
	{
		if (_input.GetButtonDown("Jump"))
		{
			InvokeInputEvent(InputCommand.Jump);
		}
	}

	private void HandleDashInput()
	{
		if (_input.GetButtonDown("Dash"))
		{
			InvokeInputEvent(InputCommand.Dash);
		}
	}

	private void HandleWithdraw()
	{
		if (_input.GetButton("WithdrawOnAir") || _input.GetButton("WithdrawStuck"))
		{
			_withdrawTimer += Time.deltaTime;
			if (_withdrawTimer >= _withdrawTime)
			{
				InvokeInputEvent(InputCommand.WithdrawAll);
			}
		}
		else if (_input.GetButtonUp("WithdrawOnAir") || _input.GetButtonUp("WithdrawStuck"))
		{
			if (_withdrawTimer < _withdrawTime)
			{
				InvokeInputEvent(InputCommand.WithdrawOne);
			}
			_withdrawTimer = 0;
		}
	}
}
