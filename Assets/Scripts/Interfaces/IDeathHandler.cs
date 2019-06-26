using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TripleBladeHorse
{
	public interface IDeathHandler
	{
		void HandleDeath(CharacterState state);
	}
}
