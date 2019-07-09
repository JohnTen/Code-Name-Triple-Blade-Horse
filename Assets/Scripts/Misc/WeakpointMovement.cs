using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;
using JTUtility.Bezier;

namespace TripleBladeHorse
{
	public class WeakpointMovement : MonoBehaviour
	{
		[SerializeField] List<Transform> weakpoints = new List<Transform>();
		[SerializeField] List<Transform> alignment1 = new List<Transform>();
		[SerializeField] List<Transform> alignment2 = new List<Transform>();
		[SerializeField] List<Transform> alignment3 = new List<Transform>();
		[SerializeField] float speed = 5;
		[SerializeField, Range(0, 2)] int currentAlignment = 0;
		[SerializeField, Range(0, 1)] float handlePercentage = 0.5f;
		[SerializeField, Range(0, 1)] float bezierPercentage = 0.2f;

		[Header("Debug")]
		[SerializeField] List<Vector2> direction;

		private void Awake()
		{
			var currentAlignment = GetCurrentAlignment();
			direction.Clear();
			for (int i = 0; i < weakpoints.Count; i++)
			{
				var direction = currentAlignment[i].position - weakpoints[i].position;
				this.direction.Add(direction);
			}
		}

		private void Update()
		{
			var currentAlignment = GetCurrentAlignment();

			for (int i = 0; i < weakpoints.Count; i++)
			{
				var handle = GetHandlePoint(weakpoints[i].position, currentAlignment[i].position, direction[i], handlePercentage);
				var nextPoint = Bezier.GetPoint((Vector2)weakpoints[i].position, handle, (Vector2)currentAlignment[i].position, bezierPercentage);
				var toNextPoint = nextPoint - (Vector2)weakpoints[i].position;
				var distance = speed * Time.deltaTime;
				if (toNextPoint.sqrMagnitude < distance * distance)
					distance = toNextPoint.magnitude;
				weakpoints[i].position += (Vector3)(toNextPoint.normalized * distance);
				direction[i] = toNextPoint;
				Debug.DrawLine(weakpoints[i].position, currentAlignment[i].position, Color.white);
				Debug.DrawLine(weakpoints[i].position, handle, Color.yellow);
				Debug.DrawLine(handle, currentAlignment[i].position, Color.yellow);
				Debug.DrawLine(weakpoints[i].position, nextPoint, Color.blue);
				Debug.DrawLine(nextPoint, currentAlignment[i].position, Color.blue);
			}
		}

		List<Transform> GetCurrentAlignment()
		{
			List<Transform> alignment;
			switch (currentAlignment)
			{
				case 0: alignment = alignment1; break;
				case 1: alignment = alignment2; break;
				case 2: alignment = alignment3; break;
				default:
					alignment = alignment1; break;
			}

			return alignment;
		}

		Vector2 GetHandlePoint(Vector2 currentPoint, Vector2 target, Vector2 lastDirection, float percentage)
		{
			var toTarget = target - currentPoint;
			var length = toTarget.magnitude;
			var angle = Vector2.SignedAngle(toTarget, lastDirection);

			toTarget /= length;
			if (angle > 45 || angle < -45)
			{
				angle = Mathf.Clamp(angle, -45, 45);
				lastDirection = toTarget.Rotate(angle);
			}
			else
			{
				lastDirection.Normalize();
			}

			angle *= Mathf.Deg2Rad;
			length = percentage * length;
			length = length / Mathf.Cos(angle);
			return lastDirection * length + currentPoint;
		}
	}
}
