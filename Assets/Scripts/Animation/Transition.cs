using System.Collections.Generic;
using JTUtility;

namespace TripleBladeHorse.Animation
{
	public class Transition
	{
		public const string Any = "_ANY";

		public string currentAnim;
		public string nextAnim;

		Func<bool, FSMState> rule;
		public float transitionTime;

		public Transition(Transition transition)
		{
			this.currentAnim = transition.currentAnim;
			this.nextAnim = transition.nextAnim;
			this.transitionTime = transition.transitionTime;
			this.rule = transition.rule;
		}

		public Transition(string currentAnim, string nextAnim, float transitionTime, Func<bool, FSMState> rule)
		{
			this.currentAnim = currentAnim;
			this.nextAnim = nextAnim;
			this.transitionTime = transitionTime;
			this.rule = rule;
		}

		public bool Test(string currentAnimation, FSMState stateData)
		{
			if (currentAnim != currentAnimation && currentAnim != Any) return false;
			return rule(stateData);
		}
	}
}
