using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public class EnemyHitBox : HitBox
{
	[SerializeField] EnemyState _state;

	public event Action<int> OnComboCancel;
	public event Action<int> OnComboRaise;

	protected override void ApplyDamageMultiplier(ref AttackResult result, AttackPackage attack)
	{
		base.ApplyDamageMultiplier(ref result, attack);

		if (_isWeakSpot)
			ApplyComboDamage(ref result, attack);
	}

	protected virtual void ApplyComboDamage(ref AttackResult result, AttackPackage attack)
	{
		result._finalDamage += _state._currentComboTimes * _state._comboAdditiveDamage;
		_state._currentComboTimes++;
		RaiseComboRaise(_state._currentComboTimes);
		_state._currentComboInterval = _state._comboMaxInterval;
	}

	protected virtual void RaiseComboCancel(int comboTimes)
	{
		OnComboCancel?.Invoke(comboTimes);
	}

	protected virtual void RaiseComboRaise(int comboTimes)
	{
		OnComboRaise?.Invoke(comboTimes);
	}

	protected virtual void Update()
	{
		if (_state._currentComboInterval > 0)
		{
			_state._currentComboInterval -= Time.deltaTime;
			if (_state._currentComboInterval <= 0)
			{
				RaiseComboCancel(_state._currentComboTimes);
				_state._currentComboTimes = 0;
			}
		}
	}
}
