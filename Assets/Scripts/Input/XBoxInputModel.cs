using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum XboxControllerInputButton
{
	A,
	B,
	X,
	Y,
	L1,
	R1,
	Share,
	Options,
	L3,
	R3,
}

public enum XboxControllerInputAxis
{
	LeftStickX,
	LeftStickY,
	RightStickX,
	RightStickY,
	DPadX,
	DPadY,
	L2Axis,
	R2Axis,
}

[Serializable]
public class XboxInputModel
{
	const int k_buttonCount = 10;
	const string k_basePrefix = "Joystick";

	[SerializeField] int joyStickNumber;

	[SerializeField] string[] axisNames;
	[SerializeField] KeyCode[] buttonCodes;

	public int JoyStickNumber => joyStickNumber;

	public XboxInputModel(int joyStickNumber)
	{
		this.joyStickNumber = joyStickNumber;
		BuildButtonCodes();
		BuildAxisNames();
	}

	public float GetAxis(XboxControllerInputAxis axis)
	{
		return Input.GetAxis(axisNames[(int)axis]);
	}

	public bool GetButtonDown(XboxControllerInputButton button)
	{
		return Input.GetKeyDown(buttonCodes[(int)button]);
	}

	public bool GetButton(XboxControllerInputButton button)
	{
		return Input.GetKey(buttonCodes[(int)button]);
	}

	public bool GetButtonUp(XboxControllerInputButton button)
	{
		return Input.GetKeyUp(buttonCodes[(int)button]);
	}

	private void BuildAxisNames()
	{
		string prefix = k_basePrefix + joyStickNumber + "_";
		var baseName = Enum.GetNames(typeof(XboxControllerInputAxis));
		axisNames = new string[baseName.Length];

		StringBuilder nameBuilder = new StringBuilder();

		for (int i = 0; i < baseName.Length; i ++)
		{
			nameBuilder.Append(prefix);
			nameBuilder.Append(baseName[i]);
			axisNames[i] = nameBuilder.ToString();
			nameBuilder.Clear();
		}
	}

	private void BuildButtonCodes()
	{
		string prefix = k_basePrefix + "Button";
		StringBuilder nameBuilder = new StringBuilder();
		buttonCodes = new KeyCode[k_buttonCount];

		for (int i = 0; i < k_buttonCount; i++)
		{
			nameBuilder.Append(prefix);
			nameBuilder.Append(i.ToString());
			buttonCodes[i] = (KeyCode)Enum.Parse(typeof(KeyCode), nameBuilder.ToString());
			nameBuilder.Clear();
		}
	}
}
