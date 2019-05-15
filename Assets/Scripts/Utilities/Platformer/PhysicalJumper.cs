using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JTUtility.Platformer
{
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(BaseGroundDetector))]
	public class PhysicalJumper : BaseCharacterJumper
	{
		protected Rigidbody2D rigidBody;
		protected BaseGroundDetector groundDetector;

		public override event Action OnJump;

		protected override void Jumping(Vector3 direction)
		{
			var vel = rigidBody.velocity;

			// Concluded from S = Vi * t + 1/2 * a * t^2 and t = (Vf - Vi)/a
			vel.y = Mathf.Sqrt(19.62f * jumpHeight * rigidBody.gravityScale);
			rigidBody.velocity = vel;

			if (OnJump != null)
				OnJump.Invoke();
		}

		protected override bool IsJumpable(Vector3 vector)
		{
			if (vector.sqrMagnitude <= 0)
				return false;

			if (!groundDetector.OnGround)
				return false;

			return true;
		}

		protected void Update()
		{
			var jumpVector = GetJumpingCommand();

			if (IsJumpable(jumpVector))
			{
				Jumping(jumpVector);
			}
		}

		protected virtual void Awake()
		{
			rigidBody = GetComponent<Rigidbody2D>();
			groundDetector = GetComponent<BaseGroundDetector>();
		}
	}
}
