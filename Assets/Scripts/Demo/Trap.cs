using UnityEngine;

namespace TripleBladeHorse
{
    public class Trap : MonoBehaviour
    {
        [SerializeField] float _damage;

        CharacterState _player;

        private void Start()
        {
            _player = TripleBladeHorse.GameManager.PlayerInstance.GetComponent<CharacterState>();
        }

        public void TriggerTrap()
        {
            _player._hitPoints -= _damage;
        }
    }
}