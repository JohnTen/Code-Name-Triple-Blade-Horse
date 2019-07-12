using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JTUtility;
using TripleBladeHorse;
using TripleBladeHorse.AI;

public class BossTest : MonoBehaviour, ICharacterInput<BossInput>
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
	[SerializeField] List<BossInput> _commands1;
	[SerializeField] List<BossInput> _commands2;
	[SerializeField] List<BossInput> _commands3;
	[Header("Debug")]
	[SerializeField] int nextQueue;
	[SerializeField] BossInput delayedInput;

	public event Action<Vector3> OnPull;

	int loop;
	InputEventArg<BossInput> _delayedInput;
	Vector2 movement;

	public bool DelayInput
	{
		get => delayingInput;
		set
		{
			if (delayingInput == value)
				return;

			delayingInput = value;
			if (!value && _delayedInput._command != BossInput.Null)
			{
				OnReceivedInput?.Invoke(_delayedInput);
			}

			if (delayingInput)
			{
				_delayedInput._command = BossInput.Null;
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

	public event Action<InputEventArg<BossInput>> OnReceivedInput;

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

	private void InvokeInputEvent(BossInput command)
	{
		if (DelayInput)
		{
			_delayedInput._command = command;
			delayedInput = _delayedInput._command;
			return;
		}

		OnReceivedInput?.Invoke(new InputEventArg<BossInput>(command));
	}

	private void InvokeInputEvent(BossInput command, float value)
	{
		if (DelayInput)
		{
			_delayedInput._command = command;
			delayedInput = _delayedInput._command;
			_delayedInput._additionalValue = value;
			return;
		}

		OnReceivedInput?.Invoke(new InputEventArg<BossInput>(command, value));
	}

	private List<BossInput> ChoseCommandQueue()
	{
		List<BossInput> queue;

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
