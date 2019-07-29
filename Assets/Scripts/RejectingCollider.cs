using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleBladeHorse;
using TripleBladeHorse.Movement;

public class RejectingCollider : MonoBehaviour
{
	[SerializeField] float _minRejectDistance;
	[SerializeField] float _maxRejectDistance;
	[SerializeField] float _minRejectSpeed;
	[SerializeField] float _maxRejectSpeed;
	[SerializeField] string[] _filterTags;

	Collider2D _collider;

	private void Awake()
	{
		_collider = GetComponent<Collider2D>();
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (IsInsideFilter(collision.tag)) return;
		var mover = collision.GetComponent<PlayerMover>();
		if (!mover) return;
		print(collision.name);

		var bound = _collider.bounds;
		var toOther = collision.bounds.center - bound.center;
		var maxDistance = bound.extents.x + collision.bounds.extents.x;
		var force = Mathf.Lerp(_maxRejectDistance, _minRejectDistance, toOther.magnitude / maxDistance);
		var speed = Mathf.Lerp(_maxRejectSpeed, _minRejectSpeed, toOther.magnitude / maxDistance);
		var actualForce = force * (toOther.x > 0 ? Vector2.right : Vector2.left);

		mover.Knockback(actualForce, speed);
	}

	bool IsInsideFilter(string tag)
	{
		if (_filterTags == null || _filterTags.Length == 0) return false;

		foreach (var t in _filterTags)
		{
			if (t == tag) return false;
		}

		return true;
	}
}
