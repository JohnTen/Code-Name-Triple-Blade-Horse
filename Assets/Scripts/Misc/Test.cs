using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour, IInputModelPlugable
{
	[SerializeField] Text text;

	bool usingController;
	IInputModel input;

	private void Start()
	{
		InputManager.Instance.RegisterPluggable(0, this);
	}

	// Update is called once per frame
	void Update()
    {
		var move = new Vector2(input.GetAxis("MoveX"), input.GetAxis("MoveY"));

		var aim = Vector2.zero;
		if (usingController)
		{
			aim = new Vector2(input.GetAxis("LookX"), input.GetAxis("LookY"));
		}
		else
		{
			aim = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
			if (aim.SqrMagnitude() > 1)
				aim.Normalize();
		}

		if (input.GetButtonDown("Melee"))
		{
			print("Melee");
		}

		if (usingController)
		{
			if (input.GetAxis("Throw") > 0.8f)
			{
				if (aim.magnitude > 0.5f)
					print("Throw");
			}
		}
		else
		{
			if (input.GetButtonDown("Throw"))
			{
				print("Throw");
			}
		}
		

		if (input.GetButtonDown("Dash"))
		{
			print("Dash");
		}

		if (input.GetButtonDown("Jump"))
		{
			print("Jump");
		}

		if (input.GetButtonDown("WithdrawOnAir"))
		{
			print("WithdrawOnAir");
		}

		if (input.GetButtonDown("WithdrawStuck"))
		{
			print("WithdrawStuck");
		}

		Debug.DrawRay(transform.position, move, Color.blue);
		Debug.DrawRay(transform.position, aim, Color.red);
	}

	public void SetInputModel(IInputModel model)
	{
		input = model;
		usingController = model is ControllerInputModel;

		if (usingController)
		{
			print("Using Controller");
		}
		else
		{
			print("Using keyboard & mouse");
		}
	}
}
