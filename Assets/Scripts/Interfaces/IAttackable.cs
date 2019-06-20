using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public interface IAttackable
{
	Faction Faction { get; }
	AttackResult ReceiveAttack(AttackPackage attack);

	event Action<AttackPackage, AttackResult> OnHit;
}
