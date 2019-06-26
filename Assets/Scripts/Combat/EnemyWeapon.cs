using System.Collections;
using System.Collections.Generic;
using JTUtility;
using UnityEngine;

namespace TripleBladeHorse.Combat
{
	public class EnemyWeapon : MonoBehaviour
	{
		[SerializeField] Transform _attackPoint;
		[SerializeField] GameObject _wavePrefab;
		[SerializeField] float _delay = 0.3334f;

		public void Attack()
		{
			Invoke("StartAttack", _delay);
		}

		public void StartAttack()
		{
			var obj = Instantiate(_wavePrefab).GetComponent<BaseWeapon>();
			obj.Activate(AttackPackage.CreateNewPackage(), null);
			obj.transform.position = _attackPoint.position;
		}
	}
}