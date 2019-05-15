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
		private float baseValue;
		public float BaseValue
		{
			get { return baseValue; }
			set { baseValue = value; }
		}

		[SerializeField]
		private float currentValue;
		public float CurrentValue
		{
			get { return currentValue; }
			set { currentValue = value; }
		}

		public void ResetCurrentValue()
		{
			CurrentValue = BaseValue;
		}

		public float ModCurrent(float value)
		{
			return CurrentValue += value;
		}
	}
}