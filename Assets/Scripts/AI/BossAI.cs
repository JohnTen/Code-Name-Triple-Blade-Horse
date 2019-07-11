﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTAI;


namespace TripleBladeHorse.AI{

    public class BossAI : MonoBehaviour
    {
        BossBehave _behave;

        Root _bossAI = BT.Root();
        private void OnEnable() {
            
            _bossAI.OpenBranch(
                BT.Selector().OpenBranch(
                    BT.Sequence().OpenBranch(
                        BT.Condition(_behave.IsLowHealth),
                        BT.Condition(_behave.NeedDodge),
                        BT.RandomSequence(_behave.lowHealthWeight).OpenBranch(
                            BT.Call(_behave.Slash),
                            BT.Call(_behave.JumpAttack),
                            BT.Call(_behave.DashAttack),
                            BT.Call(_behave.Dodge)
                        ),
                        BT.Wait(_behave.combatTemp)
                    ),
                    BT.Sequence().OpenBranch(
                        BT.Condition(_behave.NeedDodge),
                        BT.RandomSequence(_behave.weight).OpenBranch(
                            BT.Call(_behave.Slash),
                            BT.Call(_behave.JumpAttack),
                            BT.Call(_behave.DashAttack),
                            BT.Call(_behave.Dodge)
                        ),
                        BT.Wait(_behave.combatTemp)
                    ),
                    BT.Sequence().OpenBranch(
                        BT.Condition(_behave.InAttackRange),
                        BT.RandomSequence(_behave.weight).OpenBranch(
                            BT.Call(_behave.Slash),
                            BT.Call(_behave.JumpAttack),
                            BT.Call(_behave.DashAttack)
                        ),
                        BT.Wait(_behave.combatTemp)
                    ),
                    BT.Call(_behave.MoveToTarget)
                )
            );
        }

    // Update is called once per frame
        void Update()
        {
            _bossAI.Tick();
        }
        
    }
}
