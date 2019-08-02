using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace TripleBladeHorse
{
    public class PlayerDeathHandler : MonoBehaviour, ICanHandleDeath
    {
        enum DeathHandle
        {
            RespawnPoint,
            ReloadLevel,
            InvokeEvent
        }

        [SerializeField] DeathHandle _onDeath;
        [SerializeField] UnityEvent _deathEvent;

        public void OnDeath(CharacterState state)
        {
            switch (_onDeath)
            {
                case DeathHandle.RespawnPoint:
                    RecoverPoint.MainRespawn(true);
                    break;

                case DeathHandle.ReloadLevel:
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    break;

                case DeathHandle.InvokeEvent:
                    _deathEvent.Invoke();
                    break;
            }
        }
    }
}

