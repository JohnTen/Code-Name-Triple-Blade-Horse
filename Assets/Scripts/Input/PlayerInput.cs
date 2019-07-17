using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse
{
	public enum PlayerInputCommand
	{
		Null,
		Dash,
		JumpBegin,
		Jump,
		RangeBegin,
		RangeAttack,
		RangeChargeAttack,
		MeleeBegin,
		MeleeAttack,
		MeleeChargeBegin,
		MeleeChargeBreak,
		MeleeChargeAttack,
		WithdrawAll,
		WithdrawOne,
		Regenerate,
	}

	public class PlayerInput : MonoBehaviour, IInputModelPlugable, ICharacterInput<PlayerInputCommand>
	{
		[Header("Melee")]
		[SerializeField] float _enterMeleeChargeTime = 0.3f;
		[SerializeField] float _minimumMeleeChargeTime = 0.2f;
		[SerializeField] float _meleeMaxChargeTime = 2f;

		[Header("Range")]
		[SerializeField] float _rangeChargeTime = 0.3f;
		[SerializeField] float _withdrawTime = 0.3f;

		[Header("Action")]
		[SerializeField] float _jumpTime = 0.2f;

		[Header("Reference")]
		[SerializeField] Transform _aimingPivot;
		[SerializeField] LineRenderer _aimingLine;
		[SerializeField] bool _delayingInput;
		[SerializeField] bool _blockInput;

		[Header("Debug")]
		[SerializeField] float _enterMeleeChargeTimer;
		[SerializeField] float _meleeChargeTimer;
		[SerializeField] float _jumpTimer;
		[SerializeField] float _rangeChargeTimer;
		[SerializeField] float _withdrawTimer;

		bool _triggerInput;
		bool _usingController;
		bool _throwPressedBefore;
		bool _withdrawPressedBefore;
		IInputModel _input;
		InputEventArg<PlayerInputCommand> _delayedInput;

		public bool DelayInput
		{
			get => _delayingInput;
			set
			{
				if (_delayingInput == value)
					return;

				_delayingInput = value;
				if (!value && _delayedInput._command != PlayerInputCommand.Null)
				{
					OnReceivedInput?.Invoke(_delayedInput);
				}

				if (_delayingInput)
				{
					_delayedInput._command = PlayerInputCommand.Null;
					_enterMeleeChargeTimer = 0;
					_meleeChargeTimer = 0;
					_rangeChargeTimer = 0;
					_withdrawTimer = 0;
				}
			}
		}

		public bool BlockInput
		{
			get => _blockInput;
			set
			{
				_blockInput = value;
				if (_blockInput)
				{
					_delayedInput._command = PlayerInputCommand.Null;
					_enterMeleeChargeTimer = float.NegativeInfinity;
					_rangeChargeTimer = 0;
					_withdrawTimer = 0;
				}
			}
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
			{
				_aimingLine.enabled = false;
				return Vector2.zero;
			}

			var aim = Vector2.zero;
			if (_usingController)
			{
				aim = new Vector2(_input.GetAxis("LookX"), _input.GetAxis("LookY"));
				if (aim.sqrMagnitude > 0.025f)
				{
					_aimingLine.enabled = true;
					_aimingLine.SetPosition(0, _aimingPivot.position + (Vector3)aim.normalized * 2);
					_aimingLine.SetPosition(1, _aimingPivot.position + (Vector3)aim.normalized * 2.1f);
					_aimingLine.SetPosition(2, _aimingPivot.position + (Vector3)aim.normalized * 2.2f);
					_aimingLine.SetPosition(3, _aimingPivot.position + (Vector3)aim.normalized * 2.3f);
					_aimingLine.SetPosition(4, _aimingPivot.position + (Vector3)aim.normalized * 2.4f);
					_aimingLine.SetPosition(5, _aimingPivot.position + (Vector3)aim.normalized * 2.5f);
				}
				else
				{
					_aimingLine.enabled = false;
				}
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

		public void ResetDelayInput()
		{
			_delayedInput._command = PlayerInputCommand.Null;
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
			_triggerInput = false;

			HandleDashInput();
			if (_triggerInput) return;

			HandleJumpInput();
			if (_triggerInput) return;

			HandleMeleeInput();
			if (_triggerInput) return;

			HandleRegeneration();
			if (_triggerInput) return;

			HandleRangeInput();
			HandleWithdraw();
		}

		private void InvokeInputEvent(PlayerInputCommand command)
		{
			_triggerInput = true;
			if (DelayInput)
			{
				_delayedInput._command = command;
				return;
			}

			OnReceivedInput?.Invoke(new InputEventArg<PlayerInputCommand>(command));
		}

		private void InvokeInputEvent(PlayerInputCommand command, Func<float> timer)
		{
			_triggerInput = true;
			if (DelayInput)
			{
				_delayedInput._command = command;
				return;
			}

			OnReceivedInput?.Invoke(new InputEventArg<PlayerInputCommand>(command, timer));
		}

		private void InvokeInputEvent(PlayerInputCommand command, float value)
		{
			_triggerInput = true;
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
				_enterMeleeChargeTimer = 0;
				_meleeChargeTimer = 0;
				InvokeInputEvent(PlayerInputCommand.MeleeBegin);
			}

			if (_input.GetButton("Melee"))
			{
				bool charged = _enterMeleeChargeTimer > _enterMeleeChargeTime;
				_enterMeleeChargeTimer += TimeManager.PlayerDeltaTime;
				if (_enterMeleeChargeTimer > _enterMeleeChargeTime)
				{
					if (!charged)
						InvokeInputEvent(PlayerInputCommand.MeleeChargeBegin);

					_meleeChargeTimer += TimeManager.PlayerDeltaTime;
				}

				if (_meleeChargeTimer > _meleeMaxChargeTime)
				{
					InvokeInputEvent(PlayerInputCommand.MeleeChargeAttack, 1);
					_meleeChargeTimer = float.NegativeInfinity;
					_enterMeleeChargeTimer = float.NegativeInfinity;
				}
			}

			if (_input.GetButtonUp("Melee"))
			{
				if (_meleeChargeTimer > _minimumMeleeChargeTime)
				{
					var chargedPercent = _meleeChargeTimer / _meleeMaxChargeTime;

					InvokeInputEvent(PlayerInputCommand.MeleeChargeAttack, chargedPercent);
				}
				else if (_meleeChargeTimer > 0)
				{
					InvokeInputEvent(PlayerInputCommand.MeleeChargeBreak);
				}
				else if (_enterMeleeChargeTimer > 0)
				{
					InvokeInputEvent(PlayerInputCommand.MeleeAttack);
				}
				_meleeChargeTimer = float.NegativeInfinity;
				_enterMeleeChargeTimer = float.NegativeInfinity;
			}

			if (!_input.GetButton("Melee") && _enterMeleeChargeTimer > _enterMeleeChargeTime)
			{
				InvokeInputEvent(PlayerInputCommand.MeleeChargeBreak);
				_meleeChargeTimer = float.NegativeInfinity;
				_enterMeleeChargeTimer = float.NegativeInfinity;
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
				InvokeInputEvent(PlayerInputCommand.RangeBegin, 
					() => 
					{
						var percent = _rangeChargeTimer / _rangeChargeTime;
						percent = float.IsNaN(percent) ? 0 : percent;
						percent = float.IsInfinity(percent) ? 1 : percent;
						return percent;
					});
			}

			if (_input.GetButton("Throw"))
			{
				_rangeChargeTimer += TimeManager.PlayerDeltaTime;
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

			if (GetAimingDirection().sqrMagnitude > 0.025f)
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
					InvokeInputEvent(PlayerInputCommand.RangeBegin,
						() =>
						{
							var percent = _rangeChargeTimer / _rangeChargeTime;
							percent = float.IsNaN(percent) ? 0 : percent;
							percent = float.IsInfinity(percent) ? 1 : percent;
							return percent;
						});
				}

				// OnButton
				_rangeChargeTimer += TimeManager.PlayerDeltaTime;
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
				_jumpTimer = 0;
				InvokeInputEvent(PlayerInputCommand.JumpBegin);
			}

			if (_input.GetButton("Jump"))
			{
				_jumpTimer += TimeManager.PlayerDeltaTime;
				if (_jumpTime <= _jumpTimer)
				{
					InvokeInputEvent(PlayerInputCommand.Jump, 1);
					_jumpTimer = float.NegativeInfinity;
				}
			}

			if (_input.GetButtonUp("Jump"))
			{
				InvokeInputEvent(PlayerInputCommand.Jump, _jumpTimer / _jumpTime);
				_jumpTimer = float.NegativeInfinity;
			}
		}

		private void HandleDashInput()
		{
			if(_input.GetButtonDown("Dash"))
			{
				InvokeInputEvent(PlayerInputCommand.Dash);
			}
		}

		private void HandleWithdraw()
		{
			var onButton = false; 
			var onButtonUp = false; 

			if (_usingController)
			{
				onButton = _input.GetAxis("WithdrawOnAir") > 0.4f || _input.GetButton("WithdrawStuck");

				if (onButton)
					_withdrawPressedBefore = true;

				if ((_input.GetAxis("WithdrawOnAir") <= 0.4f && _withdrawPressedBefore) || _input.GetButtonUp("WithdrawStuck"))
				{
					_withdrawPressedBefore = false;
					onButtonUp = true;
				}
			}
			else
			{
				onButton = _input.GetButton("WithdrawOnAir") || _input.GetButton("WithdrawStuck");
				onButtonUp = _input.GetButtonUp("WithdrawOnAir") || _input.GetButtonUp("WithdrawStuck");
			}

			if (onButton)
			{
				_withdrawTimer += TimeManager.PlayerDeltaTime;
				if (_withdrawTimer >= _withdrawTime)
				{
					InvokeInputEvent(PlayerInputCommand.WithdrawAll);
					_withdrawTimer = float.NegativeInfinity;
				}
			}
			else if (onButtonUp)
			{
				if (_withdrawTimer < _withdrawTime)
				{
					InvokeInputEvent(PlayerInputCommand.WithdrawOne);
					_withdrawTimer = float.NegativeInfinity;
				}
				_withdrawTimer = 0;
			}
		}

		private void HandleRegeneration()
		{
			if (_input.GetButtonDown("Regenerate"))
			{
				InvokeInputEvent(PlayerInputCommand.Regenerate);
			}
		}
	}
}
