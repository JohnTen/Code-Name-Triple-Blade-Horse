using System.Collections.Generic;
using JTUtility;

namespace TripleBladeHorse.Animation
{
	public class Transition
	{
		public const string Any = "_ANY";

		public List<string> currentAnims;
		public string nextAnim;

		Func<bool, FSMState> rule;
		public float transitionTime;

		public Transition(Transition transition)
		{
			this.currentAnims = transition.currentAnims;
			this.nextAnim = transition.nextAnim;
			this.transitionTime = transition.transitionTime;
			this.rule = transition.rule;
		}

		public Transition(string currentAnim, string nextAnim, float transitionTime, Func<bool, FSMState> rule)
		{
			this.currentAnims = new List<string>();
			this.currentAnims.Add(currentAnim);
			this.nextAnim = nextAnim;
			this.transitionTime = transitionTime;
			this.rule = rule;
		}

		public Transition(string[] currentAnims, string nextAnim, float transitionTime, Func<bool, FSMState> rule)
		{
			this.currentAnims = new List<string>(currentAnims);
			this.nextAnim = nextAnim;
			this.transitionTime = transitionTime;
			this.rule = rule;
		}

		public bool Test(string currentAnimation, FSMState stateData)
		{
			if (!currentAnims.Contains(currentAnimation) && !currentAnims.Contains(Any)) return false;
			return rule(stateData);
		}
	}
}
