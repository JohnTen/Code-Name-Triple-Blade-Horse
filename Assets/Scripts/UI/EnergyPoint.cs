using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TripleBladeHorse
{
    public class EnergyPoint : MonoBehaviour
    {
        [SerializeField] float  i;
        [SerializeField]
        GameObject player;
        GameObject energy1, energy2, energy3, energy4, energy5, energy6, energy7
            , energy8, energy9, energy10;
        // Start is called before the first frame update
        void Start()
        {
            energy1 = GameObject.Find("Energy1");
            energy2 = GameObject.Find("Energy2");
            energy3 = GameObject.Find("Energy3");
            energy4 = GameObject.Find("Energy4");
            energy5 = GameObject.Find("Energy5");
            energy6 = GameObject.Find("Energy6");
            energy7 = GameObject.Find("Energy7");
            energy8 = GameObject.Find("Energy8");
            energy9 = GameObject.Find("Energy9");
            energy10 = GameObject.Find("Energy10");

        }

        // Update is called once per frame
        void Update()
        {
            i = player.GetComponent<PlayerState>()._stamina.Current;

            switch (i)
            {
                case 0:
                    energy1.SetActive(false);
                    energy2.SetActive(false);
                    energy3.SetActive(false);
                    energy4.SetActive(false);
                    energy5.SetActive(false);
                    energy6.SetActive(false);
                    energy7.SetActive(false);
                    energy8.SetActive(false);
                    energy9.SetActive(false);
                    energy10.SetActive(false);
                    break;
                case 1:
                    energy1.SetActive(true);
                    energy2.SetActive(false);
                    energy3.SetActive(false);
                    energy4.SetActive(false);
                    energy5.SetActive(false);
                    energy6.SetActive(false);
                    energy7.SetActive(false);
                    energy8.SetActive(false);
                    energy9.SetActive(false);
                    energy10.SetActive(false);
                    break;
                case 2:
                    energy1.SetActive(true);
                    energy2.SetActive(true);
                    energy3.SetActive(false);
                    energy4.SetActive(false);
                    energy5.SetActive(false);
                    energy6.SetActive(false);
                    energy7.SetActive(false);
                    energy8.SetActive(false);
                    energy9.SetActive(false);
                    energy10.SetActive(false);
                    break;
                case 3:
                    energy1.SetActive(true);
                    energy2.SetActive(true);
                    energy3.SetActive(true);
                    energy4.SetActive(false);
                    energy5.SetActive(false);
                    energy6.SetActive(false);
                    energy7.SetActive(false);
                    energy8.SetActive(false);
                    energy9.SetActive(false);
                    energy10.SetActive(false);
                    break;
                case 4:
                    energy1.SetActive(true);
                    energy2.SetActive(true);
                    energy3.SetActive(true);
                    energy4.SetActive(true);
                    energy5.SetActive(false);
                    energy6.SetActive(false);
                    energy7.SetActive(false);
                    energy8.SetActive(false);
                    energy9.SetActive(false);
                    energy10.SetActive(false);
                    break;
                case 5:
                    energy1.SetActive(true);
                    energy2.SetActive(true);
                    energy3.SetActive(true);
                    energy4.SetActive(true);
                    energy5.SetActive(true);
                    energy6.SetActive(false);
                    energy7.SetActive(false);
                    energy8.SetActive(false);
                    energy9.SetActive(false);
                    energy10.SetActive(false);
                    break;
                case 6:
                    energy1.SetActive(true);
                    energy2.SetActive(true);
                    energy3.SetActive(true);
                    energy4.SetActive(true);
                    energy5.SetActive(true);
                    energy6.SetActive(true);
                    energy7.SetActive(false);
                    energy8.SetActive(false);
                    energy9.SetActive(false);
                    energy10.SetActive(false);
                    break;
                case 7:
                    energy1.SetActive(true);
                    energy2.SetActive(true);
                    energy3.SetActive(true);
                    energy4.SetActive(true);
                    energy5.SetActive(true);
                    energy6.SetActive(true);
                    energy7.SetActive(true);
                    energy8.SetActive(false);
                    energy9.SetActive(false);
                    energy10.SetActive(false);
                    break;
                case 8:
                    energy1.SetActive(true);
                    energy2.SetActive(true);
                    energy3.SetActive(true);
                    energy4.SetActive(true);
                    energy5.SetActive(true);
                    energy6.SetActive(true);
                    energy7.SetActive(true);
                    energy8.SetActive(true);
                    energy9.SetActive(false);
                    energy10.SetActive(false);
                    break;
                case 9:
                    energy1.SetActive(true);
                    energy2.SetActive(true);
                    energy3.SetActive(true);
                    energy4.SetActive(true);
                    energy5.SetActive(true);
                    energy6.SetActive(true);
                    energy7.SetActive(true);
                    energy8.SetActive(true);
                    energy9.SetActive(true);
                    energy10.SetActive(false);
                    break;
                case 10:
                    energy1.SetActive(true);
                    energy2.SetActive(true);
                    energy3.SetActive(true);
                    energy4.SetActive(true);
                    energy5.SetActive(true);
                    energy6.SetActive(true);
                    energy7.SetActive(true);
                    energy8.SetActive(true);
                    energy9.SetActive(true);
                    energy10.SetActive(true);
                    break;


            }


        }
    }
}

