using UnityEngine;
using UnityEngine.UI;

namespace TripleBladeHorse.UI
{
    public class EnergyPoint : MonoBehaviour
    {

        [SerializeField] Image _emputyEnergy;
        [SerializeField] Image _fullEnergy;
        GameObject[] _fullEnergys;
        PlayerState _playerState;
        //Image[] Energypoints;
        // Start is called before the first frame update
        void Start()
        {
            _playerState = GameManager.PlayerInstance.GetComponent<PlayerState>();
            CreateEnergy();
        }

        // Update is called once per frame
        void Update()
        {
            int currentEnergy = (int)_playerState._stamina.Current;
            int baseEnergy = (int)_playerState._stamina.Base;
            for (int i = 0; i < currentEnergy; i++)
            {
                _fullEnergys[i].SetActive(true);
            }
            for (int i = currentEnergy; i < baseEnergy; i++)
            {
                _fullEnergys[i].SetActive(false);
            }

        }

        void CreateEnergy()
        {
            int baseEnergy = (int)_playerState._stamina.Base;
            _fullEnergys = new GameObject[baseEnergy];

            for (int i = 0; i < baseEnergy; i++)
            {

                GameObject emputy = Instantiate(_emputyEnergy.gameObject);
                GameObject full = Instantiate(_fullEnergy.gameObject);
                full.transform.SetParent(emputy.transform);
                emputy.transform.SetParent(transform);
                _fullEnergys[i] = full;
                emputy.transform.localScale = Vector3.one;

            }

        }
    }
}

