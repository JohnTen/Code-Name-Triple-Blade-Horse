using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse.Combat
{
	public class ComboEventArgs : System.EventArgs
	{
		public int comboTimes;
		public float baseDamage;
		public float comboDamage;
	}

	public class EnemyHitBox : HitBox
	{
		[SerializeField] EnemyState _state;

		public event Action<HitBox, int> OnComboCancel;
		public event Action<HitBox, ComboEventArgs> OnComboRaise;
		public event Action<HitBox, ComboEventArgs> OnComboExceeded;

		protected override void ApplyDamageMultiplier(ref AttackResult result, AttackPackage attack)
		{
			base.ApplyDamageMultiplier(ref result, attack);

			if (_isWeakSpot
				&& attack._attackType != AttackType.StuckNDraw
				&& attack._attackType != AttackType.Float)
				ApplyComboDamage(ref result, attack);
		}

		protected virtual void ApplyComboDamage(ref AttackResult result, AttackPackage attack)
		{
			_state._currentComboTimes++;
			_state._currentComboInterval = _state._comboMaxInterval;
			if (_state._currentComboTimes > _state._comboMaxTimes)
			{
				RaiseComboExceeded(
					_state._currentComboTimes, 
					result._finalDamage, 
					_state._currentComboTimes * _state._comboAdditiveDamage);

				_state._currentComboTimes = 0;
				_state._currentComboInterval = 0;
			}
			else
			{
				RaiseComboRaise(
					_state._currentComboTimes,
					result._finalDamage,
					_state._currentComboTimes * _state._comboAdditiveDamage);
			}

			result._finalDamage += _state._currentComboTimes * _state._comboAdditiveDamage;
		}

		protected virtual void RaiseComboCancel(int comboTimes)
		{
			OnComboCancel?.Invoke(this, comboTimes);
		}

		protected virtual void RaiseComboExceeded(int comboTimes, float extraDamage, float baseDamage)
		{
			ComboEventArgs eventArgs = new ComboEventArgs
			{
				comboTimes = comboTimes,
				baseDamage = baseDamage,
				comboDamage = extraDamage
			};
			OnComboExceeded?.Invoke(this, eventArgs);
		}

		protected virtual void RaiseComboRaise(int comboTimes, float extraDamage, float baseDamage)
		{
			ComboEventArgs eventArgs = new ComboEventArgs
			{
				comboTimes = comboTimes,
				baseDamage = baseDamage,
				comboDamage = extraDamage
			};
			OnComboRaise?.Invoke(this, eventArgs);
		}

		protected virtual void Update()
		{
			if (_state._currentComboInterval > 0)
			{
				_state._currentComboInterval -= TimeManager.DeltaTime;
				if (_state._currentComboInterval <= 0)
				{
					RaiseComboCancel(_state._currentComboTimes);
					_state._currentComboTimes = 0;
				}
			}
		}
	}
}
