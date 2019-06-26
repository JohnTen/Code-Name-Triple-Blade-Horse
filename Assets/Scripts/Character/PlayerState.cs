using UnityEngine;
using JTUtility;

namespace TripleBladeHorse
{
	public class PlayerState : CharacterState
	{
		[Header("Player")]
		public StateValue _stamina;

		public float _staminaRecoverRate;
		public float _withdrawStaminaRecoverFactor;
	}
}