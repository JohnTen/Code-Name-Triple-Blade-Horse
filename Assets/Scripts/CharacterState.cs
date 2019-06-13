using UnityEngine;
using JTUtility;

public class CharacterState : MonoBehaviour
{
	[Header("Basic")]
	public StateValue _hitPoints;
	public StateValue _endurance;

	public float _enduranceRecoverRate;
	public float _enduranceRefreshDelay;

	public float _hitPointDamage;
	public float _enduranceDamage;

	public bool _facingRight;
	public bool _frozen;
}
