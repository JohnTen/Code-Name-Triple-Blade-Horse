using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTAI;

namespace TripleBladeHorse.AI{
    public class MeleeAI : MonoBehaviour
    {
        private void OnEnable() {
            meleeRoot.OpenBranch(

            );    
        }
        Root meleeRoot = BT.Root();
    // Update is called once per frame
        void Update()
        {
            meleeRoot.Tick();
        }
    }
}

