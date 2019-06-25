using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public enum PlayerInputCommand
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

public class PlayerInput : MonoBehaviour, IInputModelPlugable, ICharacterInput<PlayerInputCommand>
{
	[SerializeField] float _meleeChargeTime;
	[SerializeField] float _meleeMaxChargeTime;
	[SerializeField] float _rangeChargeTime;
	[SerializeField] float _withdrawTime;
	[SerializeField] Transform _aimingPivot;
	[SerializeField] bool _delayingInput;
	[SerializeField] bool blockInput;

	float _meleeChargeTimer;
	float _rangeChargeTimer;
	float _withdrawTimer;

	//bool _delayingInput;
	bool _usingController;
	bool _throwPressedBefore;
	IInputModel _input;
	InputEventArg<PlayerInputCommand> _delayedInput;

	public bool DelayInput
	{
		get => _delayingInput;
		set
		{
			if (_delayingInput == value)
				return;

			if (!value && _delayingInput && _delayedInput._command != PlayerInputCommand.Null)
			{
				OnReceivedInput?.Invoke(_delayedInput);
			}

			_delayingInput = value;
			if (_delayingInput)
			{
				_delayedInput._command = PlayerInputCommand.Null;
			}
		}
	}

	public bool BlockInput
	{
		get => blockInput;
		set => blockInput = value;
	}

	public event Action<InputEventArg<PlayerInputCommand>> OnReceivedInput;

	public Vector2 GetMovingDirection()
	{
		if (BlockInput)
			return Vector2.zero;
		return new Vector2(_input.GetAxis("MoveX"), _input.GetAxis("MoveY")).normalized;
	}

	public Vector2 GetAimingDirection()
	{
		if (BlockInput)
			return Vector2.zero;
		var aim = Vector2.zero;
		if (_usingController)
		{
			aim = new Vector2(_input.GetAxis("LookX"), _input.GetAxis("LookY"));
		}
		else
		{
			var diff = _aimingPivot.position.z - Camera.main.transform.position.z;
			var screenPos = Input.mousePosition + Vector3.forward * diff;

			aim = (Camera.main.ScreenToWorldPoint(screenPos) - _aimingPivot.position);
			Debug.DrawLine(Camera.main.ScreenToWorldPoint(screenPos), _aimingPivot.position);
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
		if (BlockInput) return;

		HandleMeleeInput();
		HandleRangeInput();
		HandleJumpInput();
		HandleDashInput();
		HandleWithdraw();
	}

	private void InvokeInputEvent(PlayerInputCommand command)
	{
		if (DelayInput)
		{
			_delayedInput._command = command;
			return;
		}
		OnReceivedInput?.Invoke(new InputEventArg<PlayerInputCommand>(command));
	}

	private void InvokeInputEvent(PlayerInputCommand command, float value)
	{
		if (DelayInput)
		{
			_delayedInput._command = command;
			_delayedInput._additionalValue = value;
			return;
		}

		OnReceivedInput?.Invoke(new InputEventArg<PlayerInputCommand>(command, value));
	}

	private void HandleMeleeInput()
	{
		if (_input.GetButtonDown("Melee"))
		{
			_meleeChargeTimer = 0;
			InvokeInputEvent(PlayerInputCommand.MeleeBegin);
		}

		if (_input.GetButton("Melee"))
		{
			_meleeChargeTimer += Time.deltaTime;
			if (_meleeChargeTimer > _meleeMaxChargeTime)
			{
				InvokeInputEvent(PlayerInputCommand.MeleeChargeAttack, 1);
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

				InvokeInputEvent(PlayerInputCommand.MeleeChargeAttack, chargedPercent);
			}
			else if (_meleeChargeTimer > 0)
			{
				InvokeInputEvent(PlayerInputCommand.MeleeAttack);
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
			InvokeInputEvent(PlayerInputCommand.RangeBegin);
		}

		if (_input.GetButton("Throw"))
		{
			_rangeChargeTimer += Time.deltaTime;
			if (_rangeChargeTimer > _rangeChargeTime)
			{
				InvokeInputEvent(PlayerInputCommand.RangeChargeAttack);
				_rangeChargeTimer = float.NegativeInfinity;
			}
		}

		if (_input.GetButtonUp("Throw"))
		{
			if (_rangeChargeTimer > 0)
				InvokeInputEvent(PlayerInputCommand.RangeAttack);
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
				InvokeInputEvent(PlayerInputCommand.RangeBegin);
			}

			// OnButton
			_rangeChargeTimer += Time.deltaTime;
			if (_rangeChargeTimer > _rangeChargeTime)
			{
				InvokeInputEvent(PlayerInputCommand.RangeChargeAttack);
				_rangeChargeTimer = float.NegativeInfinity;
			}
		}
		// OnButtonUp
		else if (_throwPressedBefore)
		{
			_throwPressedBefore = false;
			if (_rangeChargeTimer > 0)
				InvokeInputEvent(PlayerInputCommand.RangeAttack);
		}
	}

	private void HandleJumpInput()
	{
		if (_input.GetButtonDown("Jump"))
		{
			InvokeInputEvent(PlayerInputCommand.Jump);
		}
	}

	private void HandleDashInput()
	{
		if (_input.GetButtonDown("Dash"))
		{
			InvokeInputEvent(PlayerInputCommand.Dash);
		}
	}

	private void HandleWithdraw()
	{
		if (_input.GetButton("WithdrawOnAir") || _input.GetButton("WithdrawStuck"))
		{
			_withdrawTimer += Time.deltaTime;
			if (_withdrawTimer >= _withdrawTime)
			{
				InvokeInputEvent(PlayerInputCommand.WithdrawAll);
			}
		}
		else if (_input.GetButtonUp("WithdrawOnAir") || _input.GetButtonUp("WithdrawStuck"))
		{
			if (_withdrawTimer < _withdrawTime)
			{
				InvokeInputEvent(PlayerInputCommand.WithdrawOne);
			}
			_withdrawTimer = 0;
		}
	}
}
