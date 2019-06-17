using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility.Platformer;

[RequireComponent(typeof(Enemy))]
public class EnemyJumper : PhysicalJumper
{
	[SerializeField] Enemy mover;
	[SerializeField] float jumpRate;

	void Start ()
	{
		mover = GetComponent<Enemy>();
	}

	protected override Vector3 GetJumpingCommand()
	{
		if (!groundDetector.OnGround)
			return Vector3.zero;
		if (mover.Stuned || mover.State == 2)
			return Vector3.zero;

		return jumpRate > Random.value? Vector3.up: Vector3.zero;
	}
}
