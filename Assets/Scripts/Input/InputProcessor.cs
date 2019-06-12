using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputCommoand
{
	Idle,
	Jump,
	MeleePrepare,
	LightMelee,
	HeavyMelee,
	HeavyMeleeMax,
	ThrowPrepare,
	ThrowKnife,
	Withdraw,
	WithdrawJump,
}

public class PlayerInputProcessor : MonoBehaviour
{
	public float HeavyAttackCharge { get; }
	public bool Block { get; set; }

	public Vector3 GetMoveInput()
	{
		return Vector3.zero;
	}

	public Vector3 GetAimInput()
	{
		return Vector3.zero;
	}

	public InputCommoand GetActionInput()
	{
		return InputCommoand.Idle;
	}

	public void Clear()
	{

	}
}

