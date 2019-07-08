using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTAI;


namespace TripleBladeHorse.AI{

    public class BossAI : MonoBehaviour
    {
        Root _bossAI = BT.Root();
        private void OnEnable() {
            
            _bossAI.OpenBranch(
                
            );
        }

    // Update is called once per frame
        void Update()
        {
            _bossAI.Tick();
        }
    }
}
