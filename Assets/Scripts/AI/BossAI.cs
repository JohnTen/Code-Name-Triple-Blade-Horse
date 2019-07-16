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

            _behave = GetComponent<BossBehave>();
            
            _bossAI.OpenBranch(
                BT.Selector().OpenBranch(
                    BT.Sequence().OpenBranch(
                        BT.Condition(_behave.Opening),
                        BT.Call(_behave.AfterOpening),
                        BT.Wait(2f)
                    ),
                    BT.Sequence().OpenBranch(
                        BT.Condition(_behave.TooFar),
                        BT.Call(_behave.Slash),
                        BT.Wait(0.2f),
                        BT.Call(_behave.Slash),
                        BT.Wait(0.2f),
                        BT.Call(_behave.DashAttack)
                    ),
                    BT.Sequence().OpenBranch(
                        BT.Condition(_behave.NeedDodge),
                        BT.Call(_behave.Dodge),
                        BT.Wait(0.5f)
                    ),
                    BT.Sequence().OpenBranch(
                        BT.Condition(_behave.IsLowHealth),
                        BT.Condition(_behave.InAttackRange),
                        BT.Call(_behave.WeightCalc),
                        BT.RandomSequence(_behave.weight).OpenBranch(
                            BT.Call(_behave.Slash),
                            BT.Call(_behave.JumpAttack),
                            BT.Call(_behave.DashAttack)
                        ),
                        BT.Call(_behave.CombatTempGen),
                        BT.Wait(_behave.combatTemp)
                    ),
                    BT.Sequence().OpenBranch(
                        BT.Condition(_behave.InAttackRange),
                        BT.Call(_behave.WeightCalc),
                        BT.RandomSequence(_behave.weight).OpenBranch(
                            BT.Call(_behave.Slash),
                            BT.Call(_behave.JumpAttack),
                            BT.Call(_behave.DashAttack)
                        ),
                        BT.Call(_behave.CombatTempGen),
                        BT.Wait(_behave.combatTemp)
                        
                    ),
                    BT.RandomSequence(new int[] {3,1}).OpenBranch(
                        BT.Call(_behave.MoveToTarget),
                        BT.Call(_behave.Charge)
                    )
                    
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
