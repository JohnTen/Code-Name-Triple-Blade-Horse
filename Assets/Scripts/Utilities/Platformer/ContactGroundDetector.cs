using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JTUtility.Platformer
{
	[RequireComponent(typeof(Collider2D))]
	public class ContactGroundDetector : BaseGroundDetector
	{
		protected new Collider2D collider;
		protected ContactPoint2D[] contactPoints = new ContactPoint2D[20];

		protected virtual void Awake()
		{
			collider = GetComponent<Collider2D>();
		}

		protected override bool IsOnGround()
		{
			var count = Physics2D.GetContacts(collider, contactPoints);

			for (int i = 0; i < count; i++)
			{
				Debug.DrawRay(contactPoints[i].point, contactPoints[i].normal * 10);
				if (Vector2.Dot(contactPoints[i].normal, Vector2.up) > 0.4f)
				{
					return true;
				}
			}

			return false;
		}

		protected override bool IsDetectedGround()
		{
			var count = Physics2D.GetContacts(collider, contactPoints);
			
			return count > 0;
		}
	}
}
