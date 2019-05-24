using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public interface IAttacker<T, F> : ICanProcess<T, F> where F : struct, System.IConvertible
{
	void Activate();
	void Deactivate();

	event Action<IAttackable<T>, AttackResult> OnHit;
}
