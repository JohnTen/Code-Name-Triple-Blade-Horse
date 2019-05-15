using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JTUtility.Platformer
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class PhysicalMover : BaseCharacterMover
	{
		[SerializeField] Bounds characterBound;

		protected Rigidbody2D rigidBody;

		protected override bool IsMovable(Vector3 vector)
		{
			if (vector == Vector3.zero) return true;

			var centre = transform.position + characterBound.center;
			centre.y -= characterBound.extents.y;
			Vector2 detectDir = Vector2.zero;

			if (vector.x > 0)
			{
				detectDir = Vector2.right;
			}
			else if (vector.x < 0)
			{
				detectDir = Vector2.left;
			}

			for (int i = 0; i < 3; i++)
			{
				if (Physics2D.Raycast(centre, detectDir, characterBound.extents.x).collider != null)
				{
					return false;
				}
				centre.y += characterBound.extents.y;
			}

			return base.IsMovable(vector);
		}

		protected override void Moving(Vector3 vector)
		{
			var vel = rigidBody.velocity;

			vel.x = vector.x;
			vel.y += vector.y;
			rigidBody.velocity = vel;
		}

		protected virtual void Awake()
		{
			Physics2D.queriesStartInColliders = false;
			Physics2D.queriesHitTriggers = false;

			rigidBody = GetComponent<Rigidbody2D>();
		}

		protected virtual void FixedUpdate()
		{
			var dir = GetMovingDirection();
			dir *= Time.deltaTime * moveSpeed;

			if (IsMovable(dir))
				Moving(dir);
		}
	}
}
