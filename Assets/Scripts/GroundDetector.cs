using System.Collections;
using System.Collections.Generic;
using JTUtility;
using UnityEngine;

public class GroundDetector : MonoBehaviour, ICanDetectGround
{
	[SerializeField] LayerMask _groundMask;
	[SerializeField] UpdateType _updateType;

	List<Collider2D> touchedObjects;
	
	private bool _onGround;

	public bool IsOnGround => touchedObjects.Count > 0;
	public event Action OnLanding;
	public event Action OnTakingOff;
	public event Action OnStayGround;

	private void Awake()
	{
		touchedObjects = new List<Collider2D>();
	}

	private void Update()
	{
		if (_updateType != UpdateType.Update) return;
		ManualUpdate();
	}

	private void LateUpdate()
	{
		if (_updateType != UpdateType.LateUpdate) return;
		ManualUpdate();
	}

	private void FixedUpdate()
	{
		if (_updateType != UpdateType.FixedUpdate) return;
		ManualUpdate();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if ((1 << collision.gameObject.layer & _groundMask) == 0) return;

		touchedObjects.Add(collision);
		if (touchedObjects.Count == 1)
			OnLanding?.Invoke();
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if ((1 << collision.gameObject.layer & _groundMask) == 0) return;

		touchedObjects.Remove(collision);
		if (touchedObjects.Count == 0)
			OnTakingOff?.Invoke();
	}

	private void ManualUpdate()
	{
		if (touchedObjects.Count > 0)
			OnStayGround?.Invoke();
	}
}
