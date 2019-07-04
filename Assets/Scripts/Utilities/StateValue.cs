using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JTUtility
{
	[Serializable]
	public struct StateValue
	{
		[SerializeField]
		private float @base;
		public float Base
		{
			get { return @base; }
			set { @base = value; }
		}

		[SerializeField]
		private float current;
		public float Current
		{
			get { return current; }
			set { current = value; }
		}

		public void ResetCurrentValue()
		{
			Current = Base;
		}

		public bool IsFull()
		{
			return Base - Current <= float.Epsilon;
		}

		public static implicit operator float(StateValue value)
		{
			return value.current;
		}
		
		public static StateValue operator +(StateValue v1, float v2)
		{
			v1.current += v2;
			return v1;
		}

		public static StateValue operator -(StateValue v1, float v2)
		{
			v1.current -= v2;
			return v1;
		}

		public static StateValue operator *(StateValue v1, float v2)
		{
			v1.current *= v2;
			return v1;
		}

		public static StateValue operator /(StateValue v1, float v2)
		{
			v1.current /= v2;
			return v1;
		}
	}
}