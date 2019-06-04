using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public interface IAttackable
{
	AttackResult ReceiveAttack(ref AttackPackage attack);

	event Action<AttackPackage> OnHit;
}
