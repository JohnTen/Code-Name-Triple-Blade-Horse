using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse.Animation
{
	public abstract class FSMData : ScriptableObject
	{
		public Animation _defaultAnimation;
		public List<Animation> _animationDatas;
		public List<StrBoolPair> _boolState;
		public List<StrFloatPair> _floatState;
		public List<StrIntPair> _intState;
		public List<Transition> _transitions;

		public abstract void InitalizeStates();
		public abstract void InitalizeAnimations();
		public abstract void InitalizeTransitions();
	}
}
