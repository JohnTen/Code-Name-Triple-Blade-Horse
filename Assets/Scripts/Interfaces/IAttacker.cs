using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse.Combat
{
	public interface IAttacker
	{
		void Activate(AttackPackage attack, AttackMove move);
		void Deactivate();

		event Action<IAttackable, AttackResult, AttackPackage> OnHit;
	}
}