using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public interface IAttacker : ICanProcess<AttackPackage>
{
	void Activate(AttackPackage attack);
	void Deactivate();

	event Action<IAttackable, AttackResult, AttackPackage> OnHit;
}
