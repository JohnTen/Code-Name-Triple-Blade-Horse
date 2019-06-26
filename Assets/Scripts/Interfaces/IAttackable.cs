using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse.Combat
{
	public interface IAttackable
	{
		Faction Faction { get; }
		AttackResult ReceiveAttack(AttackPackage attack);

		event Action<AttackPackage, AttackResult> OnHit;
	}
}