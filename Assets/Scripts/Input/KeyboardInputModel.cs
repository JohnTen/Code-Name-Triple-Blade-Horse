using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInputModel : IInputModel
{
	Dictionary<string, string> commandMap;
	Dictionary<string, string> commandAlternativeMap;

	public KeyboardInputModel(string[] commandList, string[] keyboardKey)
	{
		if (commandList.Length != keyboardKey.Length)
			throw new System.ArgumentException("Length of the arraies don't match");

		commandMap = new Dictionary<string, string>();
		commandAlternativeMap = new Dictionary<string, string>();
		for (int i = 0; i < commandList.Length; i++)
		{
			if (!commandMap.ContainsKey(commandList[i]))
				commandMap.Add(commandList[i], keyboardKey[i]);
			else
				commandAlternativeMap.Add(commandList[i], keyboardKey[i]);
		}
	}

	public float GetAxis(string key)
	{
		var standard = Input.GetAxis(commandMap[key]);
		var alternative = 0.0f;

		if (commandAlternativeMap.ContainsKey(key))
			alternative = Input.GetAxis(commandAlternativeMap[key]);

		return Mathf.Abs(standard) > Mathf.Abs(alternative) ? standard : alternative;
	}

	public bool GetButtonDown(string key)
	{
		var standard = Input.GetButtonDown(commandMap[key]);
		var alternative = false;

		if (commandMap.ContainsKey(key))
			alternative = Input.GetButtonDown(commandMap[key]);

		return standard ? standard : alternative;
	}

	public bool GetButton(string key)
	{
		var standard = Input.GetButton(commandMap[key]);
		var alternative = false;

		if (commandMap.ContainsKey(key))
			alternative = Input.GetButton(commandMap[key]);

		return standard ? standard : alternative;
	}

	public bool GetButtonUp(string key)
	{
		var standard = Input.GetButtonUp(commandMap[key]);
		var alternative = false;

		if (commandMap.ContainsKey(key))
			alternative = Input.GetButtonUp(commandMap[key]);

		return standard ? standard : alternative;
	}
}
