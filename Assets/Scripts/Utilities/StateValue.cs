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

		public float ModCurrent(float value)
		{
			return Current += value;
		}
	}
}