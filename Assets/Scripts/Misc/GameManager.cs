using JTUtility;
using UnityEngine;

namespace TripleBladeHorse
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] PlayerCharacter _player;

        public static PlayerCharacter PlayerInstance
        {
            get
            {
                if (Instance._player)
                    return Instance._player;

                Instance._player = FindObjectOfType<PlayerCharacter>();
                return Instance._player;
            }
        }
    }
}
