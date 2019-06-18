using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InputEventArg<T> where T : struct, System.IConvertible
{
	public T _command;
	public float _additionalValue;

	public InputEventArg(T command)
	{
		_command = command;
		_additionalValue = 0;
	}

	public InputEventArg(T command, float additionalValue)
	{
		_command = command;
		_additionalValue = additionalValue;
	}
}
