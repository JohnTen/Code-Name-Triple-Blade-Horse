﻿using BTAI;
using UnityEngine;


namespace TripleBladeHorse.AI
{

    public class BossAI : MonoBehaviour
    {
        BossBehave _behave;

        Root _bossAI = BT.Root();
        private void OnEnable()
        {

            _behave = GetComponent<BossBehave>();

            _bossAI.OpenBranch(
                BT.Selector().OpenBranch(
                    BT.If(_behave.Moving).OpenBranch(
                        BT.Wait(Time.deltaTime),
                        BT.Call(_behave.Initialization)
                    ),

                    BT.Sequence().OpenBranch(
                        BT.Condition(_behave.Opening),
                        BT.Call(_behave.AfterOpening),
                        BT.Wait(2f),
                        BT.Call(_behave.DashAttack),
                        BT.Call(_behave.TurnToTarget)
                    ),
                    BT.Sequence().OpenBranch(
                        BT.Condition(_behave.TooFar),
                        BT.Call(_behave.Charge)
                    ),
                    BT.Sequence().OpenBranch(
                        BT.Condition(_behave.NeedDodge),
                        BT.Call(_behave.Dodge),
                        BT.Wait(0.5f),
                        BT.RandomSequence().OpenBranch(
                            BT.Call(_behave.Slash),
                            BT.Call(_behave.JumpAttack),
                            BT.Call(_behave.DashAttack)
                        ),
                        BT.Call(_behave.TurnToTarget)
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
                        BT.Call(_behave.TurnToTarget)
                    //BT.Wait(1f)                        
                    ),

                    BT.Sequence().OpenBranch(
                        BT.Condition(_behave.InAttackRange),
                        BT.Call(_behave.WeightCalc),
                        BT.RandomSequence(_behave.weight).OpenBranch(
                            BT.Call(_behave.Slash),
                            BT.Call(_behave.JumpAttack),
                            BT.Call(_behave.DashAttack)
                        ),
                        BT.Call(_behave.TurnToTarget)
                    //BT.Wait(1f)                                               
                    ),

                    BT.Sequence().OpenBranch(
                        BT.Condition(_behave.IsNotCharging),
                        BT.Call(_behave.MoveToTarget),
                        BT.Wait(1f)
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
