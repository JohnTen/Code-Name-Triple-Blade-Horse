using UnityEngine;

namespace TripleBladeHorse
{
    public class EnemyState : CharacterState
    {
        [Header("Enemy/Combo")]
        public float _meleeComboAdditiveDamage;
        public float _rangeComboAdditiveDamage;
        public int _comboMaxTimes;
        public float _comboMaxInterval;

        [Header("Enemy/Combo/Debug")]
        public int _currentMeleeComboTimes;
        public int _currentRangeComboTimes;
        public float _currentComboInterval;
    }
}