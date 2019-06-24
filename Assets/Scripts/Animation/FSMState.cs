using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TripleBladeHorse.Animation
{
	public class FSMState
	{
		public Dictionary<string, bool> _boolMap;
		public Dictionary<string, float> _floatMap;
		public Dictionary<string, int> _intMap;
		public Animation _current;
		public Animation _previous;

		public FSMState()
		{
			_boolMap = new Dictionary<string, bool>();
			_intMap = new Dictionary<string, int>();
			_floatMap = new Dictionary<string, float>();
		}
	}
}