using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public class InputManager : MonoSingleton<InputManager>
{
	[SerializeField] string[] commandList;
	[SerializeField] string[] keyBoardInputList;
	[SerializeField] string[] controllerInputList;

	const int k_playerCount = 1;

	bool[] modelType = new bool[k_playerCount];

	XboxInputModel[] joystickInput = new XboxInputModel[k_playerCount];
	ControllerInputModel[] controllers = new ControllerInputModel[k_playerCount];
	KeyboardInputModel keyboard;

	Action<IInputModel>[] InputModelChanged = new Action<IInputModel>[k_playerCount];

	protected override void Awake()
	{
		keyboard = new KeyboardInputModel(commandList, keyBoardInputList);

		for (int i = 0; i < k_playerCount; i++)
		{
			joystickInput[i] = new XboxInputModel(i + 1);
			controllers[i] = new ControllerInputModel(joystickInput[i], commandList, controllerInputList);
		}

		StartCoroutine(DetectGameController());
		base.Awake();
	}

	public void RegisterPluggable(int id, IInputModelPlugable plugable)
	{
		InputModelChanged[id] += plugable.SetInputModel;
		if (modelType[id])
		{
			plugable.SetInputModel(controllers[id]);
		}
		else
		{
			plugable.SetInputModel(keyboard);
		}
	}

	public void UnregisterPluggable(int id, IInputModelPlugable plugable)
	{
		InputModelChanged[id] -= plugable.SetInputModel;
	}

	IEnumerator DetectGameController()
	{
		WaitForSeconds wait = new WaitForSeconds(2);
		string[] controllerNames;

		while (true)
		{
			controllerNames = Input.GetJoystickNames();

			for (int i = 0; i < k_playerCount && i < controllerNames.Length; i++)
			{
				if (InputModelChanged[i] == null)
					continue;

				if (string.IsNullOrEmpty(controllerNames[i]))
				{
					// If controller isn't connected and using controller before
					if (modelType[i])
					{
						InputModelChanged[i].Invoke(keyboard);
						modelType[i] = false;
					}
				}
				else
				{
					// If controller is connected and using keyboard before
					if (!modelType[i])
					{
						InputModelChanged[i].Invoke(controllers[i]);
						modelType[i] = true;
					}
				}
			}

			yield return wait;
		}
	}
}
