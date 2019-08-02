using JTUtility;
using UnityEngine;

namespace TripleBladeHorse
{
    public class CharacterState : MonoBehaviour
    {
        [Header("Basic")]
        public StateValue _hitPoints;
        public StateValue _endurance;

        public float _enduranceRecoverRate;
        public float _enduranceRefreshDelay;
        public float _enduranceRecoverDelay;
        public float _enduranceSafeThreshlod;

        public float _hitPointDamage;
        public float _enduranceDamage;

        public bool _facingRight = true;
        public bool _frozen;
        public bool _airborne;
        public bool _attacking;
    }
}