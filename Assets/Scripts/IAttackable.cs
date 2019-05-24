using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public interface IAttackable<T>
{
	AttackResult ReceiveAttack(T attack);

	event Action<T> OnHit;
}
