using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public struct InputEventArg<T> where T : struct, System.IConvertible
{
	public T _command;
	public float _additionalValue;
	public Func<float> _actionChargedPercent;

	public InputEventArg(T command)
	{
		_command = command;
		_additionalValue = 0;
		_actionChargedPercent = null;
	}

	public InputEventArg(T command, Func<float> timer)
	{
		_command = command;
		_additionalValue = 0;
		_actionChargedPercent = timer;
	}

	public InputEventArg(T command, float additionalValue)
	{
		_command = command;
		_additionalValue = additionalValue;
		_actionChargedPercent = null;
	}

	public InputEventArg(T command, float additionalValue, Func<float> timer)
	{
		_command = command;
		_additionalValue = additionalValue;
		_actionChargedPercent = timer;
	}
}
