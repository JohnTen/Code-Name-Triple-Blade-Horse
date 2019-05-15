using System;
using UnityEngine;

namespace JTUtility
{
	public class MinMaxSlider : PropertyAttribute
	{
		public readonly float max;
		public readonly float min;

		public MinMaxSlider (float min, float max) {
			this.min = min;
			this.max = max;
		}
	}
}
