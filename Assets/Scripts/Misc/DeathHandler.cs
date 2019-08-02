using UnityEngine;
using UnityEngine.Events;

namespace TripleBladeHorse
{
    public class DeathHandler : MonoBehaviour, ICanHandleDeath
    {
        [SerializeField] UnityEvent onDeath;

        public void OnDeath(CharacterState state)
        {
            onDeath.Invoke();
        }
    }
}

