using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TripleBladeHorse
{
	[System.Serializable]
	public class WeakpointFormation
	{
		public enum FormationType
		{
			Circle,
			Line,
			Custom,
		}

		[Header("Formation")]
		public FormationType _type;
		public float _rotateSpeed;

		[Tooltip("This is the radius for Circle, or the distance between each point on a Line")]
		public float _distance;

		public float _followSpeed;
		public float _transistionTime;

		[Header("Triggering")]
		public float _delay;
		public Animation.AnimationState _state;
		public string _animationName;
		public Transform _pivot;
		public Transform[] _customTrackPoint;

		public WeakpointFormation() { }
		public WeakpointFormation(WeakpointFormation other)
		{
			_type = other._type;
			_rotateSpeed = other._rotateSpeed;
			_distance = other._distance;
			_followSpeed = other._followSpeed;
			_transistionTime = other._transistionTime;
			_delay = other._delay;
			_animationName = other._animationName;
			_pivot = other._pivot;

			_customTrackPoint = new Transform[other._customTrackPoint.Length];
			for (int i = 0; i < _customTrackPoint.Length; i++)
			{
				_customTrackPoint[i] = other._customTrackPoint[i];
			}
		}

		public void Set(WeakpointFormation other)
		{
			_type = other._type;
			_rotateSpeed = other._rotateSpeed;
			_distance = other._distance;
			_followSpeed = other._followSpeed;
			_transistionTime = other._transistionTime;
			_delay = other._delay;
			_animationName = other._animationName;
			_pivot = other._pivot;

			_customTrackPoint = new Transform[other._customTrackPoint.Length];
			for (int i = 0; i < _customTrackPoint.Length; i++)
			{
				_customTrackPoint[i] = other._customTrackPoint[i];
			}
		}
	}
}
