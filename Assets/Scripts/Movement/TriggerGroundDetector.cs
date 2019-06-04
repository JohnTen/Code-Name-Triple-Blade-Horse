using System.Collections;
using System.Collections.Generic;
using JTUtility;
using UnityEngine;

public class TriggerGroundDetector : MonoBehaviour, ICanDetectGround
{
	[SerializeField] protected LayerMask _groundMask;

	LinkedList<GameObject> _groundObjects = new LinkedList<GameObject>();

	public bool IsOnGround { get; protected set; }

	public event Action OnLanding;
	public event Action OnTakingOff;
	public event Action OnStayGround;

	protected void OnTriggerEnter2D(Collider2D collision)
	{
		if (!IsGround(collision.gameObject)) return;

		if (!IsOnGround)
		{
			OnLanding?.Invoke();
		}

		IsOnGround = true;
		_groundObjects.AddLast(collision.gameObject);
	}

	protected void OnTriggerStay2D(Collider2D collision)
	{
		if (!IsGround(collision.gameObject)) return;
		
		OnStayGround?.Invoke();
	}

	protected void OnTriggerExit2D(Collider2D collision)
	{
		if (!IsGround(collision.gameObject)) return;
		
		if (IsOnGround && _groundObjects.Count <= 1)
		{
			OnTakingOff?.Invoke();
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
