using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public interface IAttacker
{
	void Activate(AttackPackage attack, AttackMove move);
	void Deactivate();

	event Action<IAttackable, AttackResult, AttackPackage> OnHit;
}
