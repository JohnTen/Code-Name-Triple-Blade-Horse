using UnityEngine;
using JTUtility;

public class PlayerState : CharacterState
{
	[Header("Player")]
	public StateValue _stamina;

	public float _staminaRecoverRate;
	public float _withdrawStaminaRecoverFactor;
}
