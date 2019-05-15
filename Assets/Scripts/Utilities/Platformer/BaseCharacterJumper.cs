using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JTUtility.Platformer
{
	public abstract class BaseCharacterJumper : MonoBehaviour
	{
		[SerializeField] protected float jumpHeight = 3;

		public abstract event Action OnJump;

		protected virtual Vector3 GetJumpingCommand()
		{
			return Input.GetButtonDown("Jump") ? Vector3.up : Vector3.zero;
		}

		protected virtual bool IsJumpable(Vector3 vector)
		{
			return true;
		}

		protected abstract void Jumping(Vector3 direction);
	}
}
