using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse
{
	public class EnemyState : CharacterState
	{
		[Header("Enemy/Combo")]
		public float _comboAdditiveDamage;
		public int _comboMaxTimes;
		public float _comboMaxInterval;

		[Header("Enemy/Combo/Debug")]
		public int _currentComboTimes;
		public float _currentComboInterval;
	}
}