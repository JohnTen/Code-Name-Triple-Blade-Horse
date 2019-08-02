using JTUtility;
using UnityEngine;

namespace TripleBladeHorse
{
    public class PlayerState : CharacterState
    {
        [Header("Player")]
        public StateValue _stamina;

        public float _staminaRecoverRate;
        public float _withdrawStaminaRecoverFactor;
    }
}