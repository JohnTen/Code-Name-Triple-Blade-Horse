using UnityEngine;
using JTUtility;

public class CharacterState : MonoBehaviour
{
	[Header("Basic")]
	public StateValue _hitPoints;
	public StateValue _endurance;

	public float _enduranceRecoverRate;
	public float _enduranceRefreshDelay;

	public float _basicDamage;
	public float _enduranceDamage;
}
