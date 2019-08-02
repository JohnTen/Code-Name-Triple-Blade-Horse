using System.Collections.Generic;
using TripleBladeHorse.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace TripleBladeHorse.UI
{
    public class ShadowyHpBar : MonoBehaviour
    {
        [SerializeField] float _disappearDelay;
        [SerializeField] float _decayDelay;
        [SerializeField] float _decaySpeed;
        [SerializeField] float _deathDecaySpeed;
        [Header("References")]
        [SerializeField] Slider _baseHealth;
        [SerializeField] Slider _baseDamage;
        [SerializeField] Slider _extraDamage;
        [SerializeField] Text _damageNumber;
        [SerializeField] EnemyState _state;
        [SerializeField] List<EnemyHitBox> _hitsboxes;
        [Header("Debug")]
        [SerializeField] bool _decaying;
        [SerializeField] int _lastComboTime;
        [SerializeField] float _currentBaseDamage;
        [SerializeField] float _currentExtraDamage;
        [SerializeField] float _disappearTimer;
        [SerializeField] float _decayTimer;

        private void Awake()
        {
            _baseHealth.maxValue = _state._hitPoints.Base;
            _baseDamage.maxValue = _state._hitPoints.Base;
            _extraDamage.maxValue = _state._hitPoints.Base;
            _baseHealth.value = _state._hitPoints.Base;
            _baseDamage.value = _state._hitPoints.Base;
            _extraDamage.value = _state._hitPoints.Base;

            foreach (var hitbox in _hitsboxes)
            {
                hitbox.OnHit += HandleHittingEvent;
            }
        }

        private void Update()
        {
            if (_disappearTimer > 0)
                _disappearTimer -= TimeManager.DeltaTime;

            if (_decayTimer > 0)
                _decayTimer -= TimeManager.DeltaTime;

            if (_decayTimer <= 0 && (_currentBaseDamage > 0 || _currentExtraDamage > 0))
            {
                _decaying = true;
            }

            if (_decaying)
            {
                _currentBaseDamage -= _decaySpeed * TimeManager.DeltaTime * _baseDamage.maxValue;
                _currentExtraDamage -= _decaySpeed * TimeManager.DeltaTime * _extraDamage.maxValue;

                if (_currentBaseDamage < 0) _currentBaseDamage = 0;
                if (_currentExtraDamage < 0) _currentExtraDamage = 0;

                UpdateSlider();
                if (_currentBaseDamage == 0 && _currentExtraDamage == 0)
                {
                    _decaying = false;
                }
            }

            if (_disappearDelay > 0 && _disappearTimer <= 0 && !_decaying && _decayTimer <= 0)
            {
                //print("Start disappear");
            }
        }

        private void UpdateSlider()
        {
            _baseHealth.value = _state._hitPoints;
            _baseDamage.value = _baseHealth.value + _currentBaseDamage;
            _extraDamage.value = _baseDamage.value + _currentExtraDamage;

            if (_damageNumber != null)
            {
                _damageNumber.text = ((int)(_currentBaseDamage + _currentExtraDamage)).ToString();
                _damageNumber.enabled = _currentBaseDamage + _currentExtraDamage > 0;
            }
        }

        private void HandleHittingEvent(AttackPackage attack, AttackResult result)
        {
            _decayTimer = _decayDelay;
            _disappearTimer = _disappearDelay;
            var totalComboTimes = _state._currentMeleeComboTimes + _state._currentRangeComboTimes;
            var comboDamage =
                _state._currentMeleeComboTimes * _state._meleeComboAdditiveDamage +
                _state._currentRangeComboTimes * _state._rangeComboAdditiveDamage;

            if (totalComboTimes == _lastComboTime)
            {
                _currentBaseDamage += result._finalDamage;
            }
            else
            {
                if (_lastComboTime == _state._comboMaxTimes)
                {
                    _decaying = true;
                }

                _lastComboTime = totalComboTimes;
                var baseDamage = result._finalDamage - comboDamage;

                if (baseDamage > _state._hitPoints)
                {
                    baseDamage = _state._hitPoints;
                }

                if (_currentExtraDamage > _state._hitPoints)
                {
                    _currentExtraDamage = _state._hitPoints;
                }

                _currentBaseDamage += baseDamage;
                _currentExtraDamage += comboDamage;
            }

            UpdateSlider();
        }
    }
}
