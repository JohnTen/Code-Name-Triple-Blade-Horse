using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JTUtility.Platformer
{
	public class RaycastGroundDetector : BaseGroundDetector
	{
		[SerializeField] protected float landDetectOffset = 0;
		[SerializeField] protected float landDetectDepth = 0.01f;

		protected override bool IsOnGround()
		{
			var centre = transform.position;
			centre.y += landDetectOffset;

			var hit = Physics2D.Raycast(centre, Vector2.down, landDetectDepth);
			return hit.collider != null;
		}

		protected override bool IsDetectedGround()
		{
			return IsOnGround();
		}
	}
}
