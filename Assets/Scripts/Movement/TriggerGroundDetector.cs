using System.Collections;
using System.Collections.Generic;
using JTUtility;
using UnityEngine;

public class TriggerGroundDetector : MonoBehaviour, ICanDetectGround
{
	[SerializeField] protected LayerMask _groundMask;

	LinkedList<GameObject> _groundObjects = new LinkedList<GameObject>();

	public bool IsOnGround { get; protected set; }

	public event Action<ICanDetectGround, LandingEventArgs> OnLandingStateChanged;

	protected void OnTriggerEnter2D(Collider2D collision)
	{
		if (!IsGround(collision.gameObject)) return;

		if (!IsOnGround)
		{
			OnLandingStateChanged?.Invoke(this, 
				new LandingEventArgs(LandingState.Airborne, LandingState.OnGround));
		}

		IsOnGround = true;
		_groundObjects.AddLast(collision.gameObject);
	}

	protected void OnTriggerExit2D(Collider2D collision)
	{
		if (!IsGround(collision.gameObject)) return;
		
		if (IsOnGround && _groundObjects.Count <= 1)
		{
			OnLandingStateChanged?.Invoke(this, 
				new LandingEventArgs(LandingState.OnGround, LandingState.Airborne));

			IsOnGround = false;
		}

		_groundObjects.Remove(collision.gameObject);
	}

	protected bool IsGround(GameObject obj)
	{
		if (!obj) return false;
		return (1 << obj.layer & _groundMask.value) != 0;
	}
}
