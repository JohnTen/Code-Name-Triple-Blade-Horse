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
		}

		[Header("Formation")]
		public FormationType _type;
		public Vector3 _rotateSpeed;

		[Tooltip("This is the radius for Circle, or the total length of Line")]
		public float _distance;
		public float _transistionDuration;

		[Header("Triggering")]
		public float _keyTime;
		public string _animationName;
		public Transform _pivot;
	}
}
