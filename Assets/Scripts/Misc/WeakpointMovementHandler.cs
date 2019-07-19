using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;
using JTUtility.Bezier;
using TripleBladeHorse.Animation;

namespace TripleBladeHorse
{
	public class WeakpointMovementHandler : MonoBehaviour
	{
		[System.Serializable] class FormationNamePair : PairedValue<string, WeakpointFormation> { }

		[SerializeField] List<Transform> _weakpoints = new List<Transform>();
		[SerializeField] List<FormationNamePair> _formationList = new List<FormationNamePair>();
		[SerializeField, Range(0, 1)] float _handlePercentage = 0.5f;
		[SerializeField, Range(0, 1)] float _bezierPercentage = 0.2f;
		[SerializeField] FSM _animator;

		[Header("Debug control")]
		[SerializeField] bool _startTransiting;
		[SerializeField] string _transitionName;

		[Header("Debug")]
		[SerializeField] bool _transiting;
		[SerializeField] float _transitionTime;
		[SerializeField] float _currentRotation;
		[SerializeField] Transform _pivot;
		[SerializeField] Vector3 _pivotStartPosition;
		[SerializeField] Quaternion _pivotStartRotation;
		[SerializeField] List<Vector3> _transitionStartPoint = new List<Vector3>();
		[SerializeField] List<Transform> _trackPoints = new List<Transform>();
		[SerializeField] List<Vector2> _direction;
		[SerializeField] WeakpointFormation _lastFormation;
		[SerializeField] WeakpointFormation _currentFormation;
		[SerializeField] WeakpointFormation _targetFormation;
		[SerializeField] List<PairedValue<WeakpointFormation, Timer>> _timerPairList;

		private void Awake()
		{
			CreateBasePivot();
			_currentFormation = new WeakpointFormation(_formationList[0].Value);
			_pivot.position = _currentFormation._pivot.position;
			UpdateFormation();

			_animator.Subscribe(Animation.AnimationState.FadingIn, HandleAnimationStateEvent);
			_animator.Subscribe(Animation.AnimationState.FadingOut, HandleAnimationStateEvent);
			_animator.Subscribe(Animation.AnimationState.FadeInComplete, HandleAnimationStateEvent);
			_animator.Subscribe(Animation.AnimationState.FadeOutComplete, HandleAnimationStateEvent);
			_animator.Subscribe(Animation.AnimationState.Start, HandleAnimationStateEvent);
			_animator.Subscribe(Animation.AnimationState.Completed, HandleAnimationStateEvent);

			_timerPairList = new List<PairedValue<WeakpointFormation, Timer>>();
			_direction = new List<Vector2>();
			for (int i = 0; i < _weakpoints.Count; i++)
			{
				var direction = _trackPoints[i].position - _weakpoints[i].position;
				this._direction.Add(direction);
			}
		}
		
		private void Update()
		{
			if (_startTransiting)
			{
				_startTransiting = false;
				SwitchToFormat(_transitionName);
			}

			for (int i = 0; i < _timerPairList.Count; i++)
			{
				if (!_timerPairList[i].Value.IsReachedTime()) continue;
				SwitchToFormat(_timerPairList[i].Key);
				_timerPairList[i].Value.Dispose();
				_timerPairList.RemoveAt(i);
				i--;
			}

			if (!_transiting)
				UpdateFormation();
			else
				HandleTransition();

			FollowTrackPoints();
		}

		#region Event Handler

		private void HandleAnimationStateEvent(AnimationEventArg eventArgs)
		{
			RemoveConflictedTimerPairs(eventArgs);

			foreach (var formation in _formationList)
			{
				if (formation.Value._state != eventArgs._state
				 || eventArgs._animation.name != formation.Value._animationName)
					continue;

				Timer timer = new Timer();
				timer.Start(formation.Value._delay);
				_timerPairList.Add(new PairedValue<WeakpointFormation, Timer>(formation.Value, timer));
			}
		}

		#endregion

		private void RemoveConflictedTimerPairs(AnimationEventArg eventArgs)
		{
			var list = _timerPairList.FindAll((x) =>
			{
				return x.Key._animationName == eventArgs._animation.name;
			});

			foreach (var item in list)
			{
				var timerBegin =
					item.Key._state == Animation.AnimationState.FadingIn ||
					item.Key._state == Animation.AnimationState.FadeInComplete;
				var timerEnd =
					item.Key._state == Animation.AnimationState.FadingOut ||
					item.Key._state == Animation.AnimationState.FadeOutComplete;
				var animBegin =
					eventArgs._state == Animation.AnimationState.FadingIn ||
					eventArgs._state == Animation.AnimationState.FadeInComplete;
				var animEnd =
					eventArgs._state == Animation.AnimationState.FadingOut ||
					eventArgs._state == Animation.AnimationState.FadeOutComplete;

				if ((timerBegin && animEnd) || (timerEnd && animBegin))
				{
					_timerPairList.Remove(item);
					item.Value.Abort();
					item.Value.Dispose();
				}
			}
		}

		public void SwitchToFormat(WeakpointFormation formation)
		{
			_lastFormation.Set(_currentFormation);
			_targetFormation.Set(formation);

			_transiting = true;
			_transitionTime = 0;

			if (_targetFormation._type == _lastFormation._type)
				return;

			_transitionStartPoint.Clear();
			foreach (var point in _trackPoints)
			{
				_transitionStartPoint.Add(point.position);
			}

			Vector3[] targetPoints;
			CalculateTargetPoints(out targetPoints);
			SortTrackPointstoClosestTarget(targetPoints);
		}

		public void SwitchToFormat(string name)
		{
			var format = _formationList.Find((x) => { return x.Key == name; }).Value;
			if (format == null)
			{
				Debug.LogError("Cannot find " + name + " in formation list.");
				return;
			}

			SwitchToFormat(format);
		}

		private bool HasRepeatElement(int[] array)
		{
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = i + 1; j < array.Length; j++)
				{
					if (array[i] == array[j])
						return true;
				}
			}

			return false;
		}

		private void CalculateTargetPoints(out Vector3[] targetPoints)
		{
			targetPoints = new Vector3[_trackPoints.Count];
			var rotation =
				Mathf.Abs(_targetFormation._rotateSpeed) > float.Epsilon
				? _currentRotation : 0;

			var baseDirection = _targetFormation._pivot.up * _targetFormation._distance;
			var angularOffset = 360 / _trackPoints.Count;

			for (int target = 0; target < _trackPoints.Count; target++)
			{
				switch (_targetFormation._type)
				{
					case WeakpointFormation.FormationType.Circle:
						targetPoints[target] = baseDirection.Rotate(0, 0, rotation + angularOffset * target);
						break;

					case WeakpointFormation.FormationType.Line:
						targetPoints[target] = baseDirection.Rotate(0, 0, rotation) * target;
						break;
				}

				targetPoints[target] = _targetFormation._pivot.TransformPoint(targetPoints[target]);
			}
		}

		private void SortTrackPointstoClosestTarget(Vector3[] targetPoints)
		{
			var targetIndex = new int[_trackPoints.Count];
			var distances = new List<List<PairedValue<int, float>>>();

			// Calculate distance from each track point to each target point
			for (int track = 0; track < _trackPoints.Count; track++)
			{
				distances.Add(new List<PairedValue<int, float>>());
				for (int target = 0; target < _trackPoints.Count; target++)
				{
					print(track + " to " + target + ": " + (_trackPoints[track].position - targetPoints[target]).sqrMagnitude);
					distances[track].Add(new PairedValue<int, float>(target, (_trackPoints[track].position - targetPoints[target]).sqrMagnitude));
				}
			}

			// Sort distance between track point and target point from small to large
			for (int i = 0; i < _trackPoints.Count; i++)
			{
				distances[i].Sort((a, b) => { return a.Value > b.Value ? 1 : (a.Value < b.Value ? -1 : 0); });
				targetIndex[i] = distances[i][0].Key;
			}

			// Handle and remove repeated target index
			while (HasRepeatElement(targetIndex))
			{
				for (int track0 = 0; track0 < _trackPoints.Count; track0++)
				{
					for (int track1 = track0 + 1; track1 < _trackPoints.Count; track1++)
					{
						// Compare each points to find repeated target index;
						var target0 = targetIndex[track0];
						var target1 = targetIndex[track1];
						if (target0 != target1) continue;

						// Choose the one with larger distance
						if (distances[track0][target0].Value > distances[track1][target1].Value)
						{
							var index = distances[track1].FindIndex((x) => { return x.Key == targetIndex[track1]; });
							targetIndex[track1] = distances[track1][index + 1].Key;
						}
						else
						{
							var index = distances[track0].FindIndex((x) => { return x.Key == targetIndex[track0]; });
							targetIndex[track0] = distances[track0][index + 1].Key;
						}
					}
				}
			}

			// Sort the trackpoints by the new index 
			var tempList = new List<Transform>(_trackPoints);
			for (int i = 0; i < _trackPoints.Count; i++)
			{
				_trackPoints[targetIndex[i]] = tempList[i];
			}
		}

		private void CreateBasePivot()
		{
			_pivot = new GameObject("Pivot").transform;
			foreach (var point in _weakpoints)
			{
				var tp = new GameObject("Track Point").transform;
				_trackPoints.Add(tp);
			}
		}

		private void HandleTransition()
		{
			if (!_transiting) return;

			_transitionTime += TimeManager.DeltaTime;
			var percentage = _transitionTime / _targetFormation._transistionTime;

			_currentFormation._rotateSpeed =
				Mathf.Lerp(
					_lastFormation._rotateSpeed,
					_targetFormation._rotateSpeed,
					percentage);

			_pivot.position =
				Vector3.Lerp(
					_lastFormation._pivot.position,
					_targetFormation._pivot.position,
					percentage);

			_pivot.rotation =
				Quaternion.Lerp(
					_lastFormation._pivot.rotation,
					_targetFormation._pivot.rotation,
					percentage);

			_currentFormation._distance =
				Mathf.Lerp(
					_lastFormation._distance,
					_targetFormation._distance,
					percentage);

			_currentFormation._followSpeed =
				Mathf.Lerp(
					_lastFormation._followSpeed,
					_targetFormation._followSpeed,
					percentage);
			
			if (Mathf.Abs(_targetFormation._rotateSpeed) <= float.Epsilon)
			{
				_currentRotation = Mathf.Lerp(_currentRotation, _targetFormation._pivot.eulerAngles.z, percentage);
			}

			if (_lastFormation._type == _targetFormation._type)
			{
				UpdateFormation();
			}
			else
			{
				Vector3[] targetPoints;
				CalculateTargetPoints(out targetPoints);
				//SortTrackPointstoClosestTarget(targetPoints);

				_currentRotation += _targetFormation._rotateSpeed * TimeManager.DeltaTime;
				for (int i = 0; i < _trackPoints.Count; i++)
				{
					Debug.DrawLine(_pivot.position, targetPoints[i], Color.magenta);
					Debug.DrawLine(_trackPoints[i].position, targetPoints[i], Color.red);
					_trackPoints[i].position = Vector3.Lerp(_transitionStartPoint[i], targetPoints[i], percentage);
				}
			}

			if (_transitionTime >= _targetFormation._transistionTime)
			{
				_currentFormation.Set(_targetFormation);
				_transiting = false;
			}
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

		private void FollowTrackPoints()
		{
			for (int i = 0; i < _weakpoints.Count; i++)
			{
				var handle = GetHandlePoint(_weakpoints[i].position, _trackPoints[i].position, _direction[i], _handlePercentage);
				var nextPoint = 
					Bezier.GetPoint(
						(Vector2)_weakpoints[i].position, 
						handle, 
						(Vector2)_trackPoints[i].position, 
						_bezierPercentage * TimeManager.DeltaTime * _currentFormation._followSpeed);

				var toNextPoint = nextPoint - (Vector2)_weakpoints[i].position;

				_weakpoints[i].position = nextPoint;
				_direction[i] = toNextPoint;

				Debug.DrawLine(_weakpoints[i].position, _trackPoints[i].position, Color.white);
				Debug.DrawLine(_weakpoints[i].position, handle, Color.yellow);
				Debug.DrawLine(handle, _trackPoints[i].position, Color.yellow);
				Debug.DrawLine(_weakpoints[i].position, nextPoint, Color.blue);
				Debug.DrawLine(nextPoint, _trackPoints[i].position, Color.blue);
			}
		}

		private void UpdateFormation()
		{
			var angularOffset = 360 / _trackPoints.Count;
			var baseDirection = Vector3.up * _currentFormation._distance;
			var toPoint = baseDirection;
			_currentRotation += _currentFormation._rotateSpeed * TimeManager.DeltaTime;

			_pivot.position = _currentFormation._pivot.position;

			for (int i = 0; i < _trackPoints.Count; i++)
			{
				switch (_currentFormation._type)
				{
					case WeakpointFormation.FormationType.Circle:
						toPoint = baseDirection.Rotate(0, 0, _currentRotation + angularOffset * i);
						toPoint = _pivot.TransformPoint(toPoint);
						break;

					case WeakpointFormation.FormationType.Line:
						toPoint = baseDirection * i;
						toPoint = _pivot.TransformPoint(toPoint);
						break;
					case WeakpointFormation.FormationType.Custom:
						toPoint = _targetFormation._customTrackPoint[i].position;
						break;
				}
					
				_trackPoints[i].position = toPoint;
			}

			foreach (var point in _trackPoints)
			{
				Debug.DrawLine(point.position, _pivot.position, Color.blue);
			}
		}
	}
}
