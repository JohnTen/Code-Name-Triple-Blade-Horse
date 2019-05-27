using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public class CharacterState : MonoBehaviour
{
	[SerializeField] StateValue _hitPoints;
	[SerializeField] StateValue _endurance;
	[SerializeField] StateValue _stamina;

	[SerializeField] float _staminaRecoverRate;
}
