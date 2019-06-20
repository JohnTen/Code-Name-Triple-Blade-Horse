using System.Collections;
using System.Collections.Generic;
using JTUtility;
using UnityEngine;

public class TempEnemyAI : MonoBehaviour, ICharacterInput<EnemyInput>
{
	[SerializeField] float meleeRange;
	[SerializeField] float detectRange;

	[SerializeField] Vector2 moveVector;

	bool TowardsFirstTarget;

	public Transform Target { get; set; }
	public Vector2 FirstTarget { get; set; }
	public Vector2 LastTarget { get; set; }

	public bool DelayInput { get; set; }
	public bool BlockInput { get; set; }

	public event Action<InputEventArg<EnemyInput>> OnReceivedInput;

	public Vector2 GetAimingDirection()
	{
		return Vector2.zero;
	}

	public Vector2 GetMovingDirection()
	{
		if (BlockInput) return Vector2.zero;

		if (DetectTarget())
		{
			return Chasing();
		}
		else
		{
			return Patrol();
		}
	}

	Vector2 Chasing()
	{
		var toTarget = Target.position.x - transform.position.x;
		var moveVector = Vector2.zero;

		if (Mathf.Abs(toTarget) < meleeRange)
		{
			OnReceivedInput?.Invoke(new InputEventArg<EnemyInput>(EnemyInput.Attack));
		}
		else if (Mathf.Abs(toTarget) < detectRange)
		{
			moveVector = Vector3.right * Mathf.Sign(toTarget);
		}

		return moveVector;
	}

	bool DetectTarget()
	{
		var toTarget = Target.position.x - transform.position.x;
		return Mathf.Abs(toTarget) < detectRange;
	}

	Vector2 Patrol()
	{
		var towardsTarget = TowardsFirstTarget ? FirstTarget - (Vector2)transform.position : LastTarget - (Vector2)transform.position;
		if (towardsTarget.sqrMagnitude < 1)
			TowardsFirstTarget = !TowardsFirstTarget;

		return towardsTarget.normalized;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, new Vector3(detectRange * 2, 1, 1));
	}

	private void Awake()
	{
		Target = FindObjectOfType<PlayerCharacter>().transform;
	}

	private void Update()
	{
		moveVector = Vector2.zero;
		if (BlockInput) return;

		var toTarget = Target.position.x - transform.position.x;

		if (Mathf.Abs(toTarget) < meleeRange)
		{
			OnReceivedInput?.Invoke(new InputEventArg<EnemyInput>(EnemyInput.Attack));
		}
		else if (Mathf.Abs(toTarget) < detectRange)
		{
			moveVector = Vector3.right * Mathf.Sign(toTarget);
		}
	}
}
