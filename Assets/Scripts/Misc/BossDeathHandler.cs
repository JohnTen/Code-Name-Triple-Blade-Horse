using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TripleBladeHorse
{
	public class BossDeathHandler : MonoBehaviour, ICanHandleDeath
	{
		[SerializeField] Transform[] _weakpoints;

		public void OnDeath(CharacterState state)
		{
			foreach (var points in _weakpoints)
			{
				points.gameObject.SetActive(false);
			}
		}
	}
}
