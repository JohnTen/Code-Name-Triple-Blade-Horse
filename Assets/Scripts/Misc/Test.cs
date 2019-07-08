using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JTUtility;
using TripleBladeHorse;

public class Test : MonoBehaviour, ICharacterInput<PlayerInputCommand>
{
	[SerializeField] bool activateAction;
	[SerializeField] bool activateMovement;
	[SerializeField] bool repeatAction;
	[SerializeField] bool repeatMovement;
	[SerializeField] bool delayingInput;
	[SerializeField] bool blockInput;
	[SerializeField, Range(-1, 1)] float xMovement;
	[SerializeField, Range(-1, 1)] float yMovement;
	[SerializeField] Vector2 pullingDirection;
	[SerializeField] List<PlayerInputCommand> _commands1;
	[SerializeField] List<PlayerInputCommand> _commands2;
	[SerializeField] List<PlayerInputCommand> _commands3;
	[Header("Debug")]
	[SerializeField] int nextQueue;
	[SerializeField] PlayerInputCommand delayedInput;

	public event Action<Vector3> OnPull;

	int loop;
	InputEventArg<PlayerInputCommand> _delayedInput;
	Vector2 movement;

	public bool DelayInput
	{
		get => delayingInput;
		set
		{
			if (delayingInput == value)
				return;
			
			delayingInput = value;
			if (!value && _delayedInput._command != PlayerInputCommand.Null)
			{
				OnReceivedInput?.Invoke(_delayedInput);
			}

			if (delayingInput)
			{
				_delayedInput._command = PlayerInputCommand.Null;
				delayedInput = _delayedInput._command;
				_delayedInput._additionalValue = 0;
			}
		}
	}

	public bool BlockInput
	{
		get => blockInput;
		set => blockInput = value;
	}

	public event Action<InputEventArg<PlayerInputCommand>> OnReceivedInput;

	public Vector2 GetAimingDirection()
	{
		if (BlockInput)
			return Vector2.zero;
		return movement;
	}

	public Vector2 GetMovingDirection()
	{
		if (BlockInput)
			return Vector2.zero;
		return movement;
	}

	private void Update()
	{
		if (BlockInput) return;

		if (activateAction)
		{
			if (!repeatAction) activateAction = false;

			loop = 0;
			var queue = ChoseCommandQueue();
			foreach (var command in queue)
			{
				if (command == PlayerInputCommand.WithdrawOne || command == PlayerInputCommand.WithdrawAll)
				{
					OnPull?.Invoke(pullingDirection);
				}
				else
					InvokeInputEvent(command);
			}
		}

		if (activateMovement)
		{
			if (!repeatMovement) activateMovement = false;
			movement.x = xMovement;
			movement.y = yMovement;
		}
		else
		{
			movement = Vector2.zero;
		}
	}

	private void InvokeInputEvent(PlayerInputCommand command)
	{
		if (DelayInput)
		{
			_delayedInput._command = command;
			delayedInput = _delayedInput._command;
			return;
		}

		OnReceivedInput?.Invoke(new InputEventArg<PlayerInputCommand>(command));
	}

	private void InvokeInputEvent(PlayerInputCommand command, float value)
	{
		if (DelayInput)
		{
			_delayedInput._command = command;
			delayedInput = _delayedInput._command;
			_delayedInput._additionalValue = value;
			return;
		}

		OnReceivedInput?.Invoke(new InputEventArg<PlayerInputCommand>(command, value));
	}

	private List<PlayerInputCommand> ChoseCommandQueue()
	{
		List<PlayerInputCommand> queue;

		switch (nextQueue)
		{
			case 0: queue = _commands1; break;
			case 1: queue = _commands2; break;
			case 2: queue = _commands3; break;
			default: queue = _commands1; break;
		}
		loop++;
		nextQueue++;
		nextQueue %= 3;

		if (loop > 3)
			return queue;

		if (queue.Count <= 0)
			return ChoseCommandQueue();
		
		return queue;
	}
}
