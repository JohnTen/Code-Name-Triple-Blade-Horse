using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleBladeHorse.Combat;

namespace TripleBladeHorse
{
	public class AimAssistant : MonoBehaviour
	{
		[SerializeField] float _maxAimingRange;
		[SerializeField] float _assistanceAngle = 15f;
		[SerializeField] float _projectileSpeed = 15f;
		[SerializeField] Transform _launchPoint;
		[SerializeField] List<string> _aimingTags;
		[SerializeField] LayerMask _obstacleLayer;

		[Header("Debug")]
		[SerializeField] bool _trackingObjects;
		[SerializeField] List<Transform> _objects;
		[SerializeField] List<Vector2> _velocities;
		[SerializeField] List<Vector2> _lastPosition;
		[SerializeField] float _lastFrameDeltaTime;

		private void Awake()
		{
			_objects = new List<Transform>();
			_velocities = new List<Vector2>();
			_lastPosition = new List<Vector2>();
			_lastFrameDeltaTime = TimeManager.DeltaTime;
		}

		private void Update()
		{
			if (_trackingObjects)
			{
				GatherTarget();
				UpdateObjects();
			}
		}

		public void StartAimingAssistant()
		{
			_objects.Clear();
			_velocities.Clear();
			_lastPosition.Clear();
			_lastFrameDeltaTime = TimeManager.DeltaTime;
			_trackingObjects = true;
		}

		public void StopAimingAssistant()
		{
			_trackingObjects = false;
		}

		public Vector2 ToNearestTarget(bool facingRight)
		{
			var maxDistance = _maxAimingRange * _maxAimingRange;
			var currentPosition = (Vector2)_launchPoint.position;
			var minDistance = float.PositiveInfinity;
			var aimDirection = facingRight ? Vector2.right : Vector2.left;

			for (int i = 0; i < _objects.Count; i++)
			{
				//var predictPoint = PredictHittingPoint(_objects[i].position, _velocities[i], currentPosition, _projectileSpeed);
				//if (float.IsInfinity(predictPoint.x)) continue;


				var toPoint = (Vector2)_objects[i].position - currentPosition;
				if (Mathf.Sign(toPoint.x) != Mathf.Sign(aimDirection.x)) continue;

				var hit = Physics2D.Raycast(currentPosition, toPoint, _maxAimingRange, _obstacleLayer);
				if (hit.collider != null) continue;

				if (toPoint.sqrMagnitude > minDistance || toPoint.sqrMagnitude > maxDistance) continue;

				minDistance = toPoint.sqrMagnitude;
				aimDirection = toPoint;
			}

			return aimDirection.normalized;
		}

		public Vector2 ExcuteAimingAssistantance(Vector2 aimingDirection)
		{
			var currentPosition = (Vector2)_launchPoint.position;
			var minAngle = float.PositiveInfinity;

			Debug.DrawRay(currentPosition, aimingDirection * 10);
			for (int i = 0; i < _objects.Count; i++)
			{
				var predictPoint = PredictHittingPoint(_objects[i].position, _velocities[i], currentPosition, _projectileSpeed);
				print(predictPoint);
				if (float.IsInfinity(predictPoint.x) || !IsWithinRange(aimingDirection, predictPoint)) continue;

				var toPoint = predictPoint - currentPosition;
				var angle = Vector2.Angle(aimingDirection, predictPoint);

				var hit = Physics2D.Raycast(currentPosition, toPoint, _maxAimingRange, _obstacleLayer);
				if (hit.collider != null) continue;

				if (angle > minAngle) continue;
				minAngle = angle;
				aimingDirection = toPoint;
			}

			Debug.DrawRay(currentPosition, aimingDirection * 10, Color.yellow);
			return aimingDirection.normalized;
		}

		void GatherTarget()
		{
			foreach (var tag in _aimingTags)
			{
				var objects = GameObject.FindGameObjectsWithTag(tag);
				foreach (var obj in objects)
				{
					if (_objects.Contains(obj.transform)) continue;

					_objects.Add(obj.transform);
					_lastPosition.Add(obj.transform.position);
					_velocities.Add(Vector2.zero);
				}
			}
		}

		bool IsWithinRange(Vector2 aimDirection, Vector2 position)
		{
			var maxDistance = _maxAimingRange * _maxAimingRange;
			var toPosition = position - (Vector2)_launchPoint.position;

			if (toPosition.sqrMagnitude > maxDistance) return false;

			if (Vector2.Angle(aimDirection, toPosition) > _assistanceAngle)
				return false;

			return true;
		}

		Vector2 PredictHittingPoint(Vector2 targetPosition, Vector2 targetVelocity, Vector2 shooterPosition, float projectileSpeed)
		{
			var displacement = targetPosition - shooterPosition;
			float targetMoveAngle = Vector3.Angle(-displacement, targetVelocity) * Mathf.Deg2Rad;

			//if the target is stopping or if it is impossible for the projectile to catch up with the target (Sine Formula)
			if (targetVelocity.magnitude == 0 || 
				targetVelocity.magnitude > projectileSpeed && 
				Mathf.Sin(targetMoveAngle) / projectileSpeed > Mathf.Cos(targetMoveAngle) / targetVelocity.magnitude)
			{
				return targetPosition;
			}
			//also Sine Formula
			float shootAngle = Mathf.Asin(Mathf.Sin(targetMoveAngle) * targetVelocity.magnitude / projectileSpeed);
			return targetPosition + targetVelocity * displacement.magnitude / Mathf.Sin(Mathf.PI - targetMoveAngle - shootAngle) * Mathf.Sin(shootAngle) / targetVelocity.magnitude;
		}

		void RemoveUnexistedObject(int index)
		{
			_objects.RemoveAt(index);
			_lastPosition.RemoveAt(index);
			_velocities.RemoveAt(index);
		}

		void UpdateObjects()
		{
			_lastFrameDeltaTime = 1 / _lastFrameDeltaTime;
			for (int i = 0; i < _objects.Count; i++)
			{
				if (_objects[i] == null)
				{
					RemoveUnexistedObject(i);
					i--;
					continue;
				}

				var posDiff = (Vector2)_objects[i].position - _lastPosition[i];
				_velocities[i] = posDiff * _lastFrameDeltaTime;
			}

			_lastFrameDeltaTime = TimeManager.PlayerDeltaTime;
		}
	}
}

