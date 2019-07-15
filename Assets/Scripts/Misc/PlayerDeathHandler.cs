using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TripleBladeHorse
{
	public class PlayerDeathHandler : MonoBehaviour, ICanHandleDeath
	{
		enum DeathHandle
		{
			RespawnPoint,
			ReloadLevel,
		}

		[SerializeField] DeathHandle _onDeath;

		public void OnDeath(CharacterState state)
		{
			switch (_onDeath)
			{
				case DeathHandle.RespawnPoint:
					RecoverPoint.MainRespawn(true);
					break;

				case DeathHandle.ReloadLevel:
					SceneManager.LoadScene(SceneManager.GetActiveScene().name);
					break;
			}
		}
	}
}

