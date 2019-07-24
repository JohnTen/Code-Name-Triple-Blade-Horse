using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse.Combat
{
	public class ComboEventArgs : System.EventArgs
	{
		public int meleeComboTimes;
		public int rangeComboTimes;
		public float baseDamage;
		public float comboDamage;
	}

	public class EnemyHitBox : HitBox
	{
		[SerializeField] EnemyState _state;
		
		public event Action<HitBox, ComboEventArgs> OnComboRaise;
		public event Action<HitBox, ComboEventArgs> OnComboExceeded;

		protected override void ApplyDamageMultiplier(ref AttackResult result, AttackPackage attack)
		{
			base.ApplyDamageMultiplier(ref result, attack);

			if (_isWeakSpot
				&& attack._attackType != AttackType.Null
				&& attack._attackType != AttackType.StuckNDraw
				&& attack._attackType != AttackType.Float)
				ApplyComboDamage(ref result, attack);
		}

		protected virtual void ApplyComboDamage(ref AttackResult result, AttackPackage attack)
		{
			switch (attack._attackType)
			{
				case AttackType.Melee:
				case AttackType.ChargedMelee:
					_state._currentMeleeComboTimes++;
					break;
				case AttackType.Range:
				case AttackType.ChargedRange:
					_state._currentRangeComboTimes++;
					break;
			}

			var totalComboTimes = _state._currentMeleeComboTimes + _state._currentRangeComboTimes;
			var totalComboDamage =
				_state._currentMeleeComboTimes * _state._meleeComboAdditiveDamage +
				_state._currentRangeComboTimes * _state._rangeComboAdditiveDamage;

			_state._currentComboInterval = _state._comboMaxInterval;
			if (totalComboTimes > _state._comboMaxTimes)
			{
				RaiseComboExceeded(
					_state._currentMeleeComboTimes,
					_state._currentRangeComboTimes,
					result._finalDamage,
					totalComboDamage);

				_state._currentMeleeComboTimes = 0;
				_state._currentRangeComboTimes = 0;
				_state._currentComboInterval = 0;
			}
			else
			{
				RaiseComboRaise(
					_state._currentMeleeComboTimes,
					_state._currentRangeComboTimes,
					result._finalDamage,
					totalComboDamage);
			}

			result._finalDamage += totalComboDamage;
		}

		protected virtual void RaiseComboExceeded(int meleeComboTimes, int rangeComboTimes, float baseDamage, float extraDamage)
		{
			ComboEventArgs eventArgs = new ComboEventArgs
			{
				meleeComboTimes = meleeComboTimes,
				rangeComboTimes = rangeComboTimes,
				baseDamage = baseDamage,
				comboDamage = extraDamage
			};
			OnComboExceeded?.Invoke(this, eventArgs);
		}

		protected virtual void RaiseComboRaise(int meleeComboTimes, int rangeComboTimes, float baseDamage, float extraDamage)
		{
			ComboEventArgs eventArgs = new ComboEventArgs
			{
				meleeComboTimes = meleeComboTimes,
				rangeComboTimes = rangeComboTimes,
				baseDamage = baseDamage,
				comboDamage = extraDamage
			};
			OnComboRaise?.Invoke(this, eventArgs);
		}
	}
}
