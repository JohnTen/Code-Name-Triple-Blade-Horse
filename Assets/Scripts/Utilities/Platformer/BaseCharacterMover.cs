using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JTUtility.Platformer
{
	public abstract class BaseCharacterMover : MonoBehaviour
	{
		[SerializeField] protected float moveSpeed = 10;

		protected virtual Vector3 GetMovingDirection()
		{
			return new Vector3(Input.GetAxis("Horizontal"), 0, 0);
		}

		protected virtual bool IsMovable(Vector3 vector)
		{
			return true;
		}

		protected abstract void Moving(Vector3 vector);
	}
}
