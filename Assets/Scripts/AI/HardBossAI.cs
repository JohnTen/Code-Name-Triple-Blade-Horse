using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTAI;

namespace TripleBladeHorse.AI{
    public class HardBossAI : MonoBehaviour
    {
        BossBehave _behave;

        Root _AI = BT.Root();

        private void OnEnable() {

            _behave = GetComponent<BossBehave>();

            _AI.OpenBranch(
                BT.Selector().OpenBranch(
                    BT.While(_behave.Opening).OpenBranch(
                        BT.Call(_behave.MoveToTarget),
                        BT.Wait(0.5f),
                        BT.Call(_behave.TurnToTarget),
                        BT.Wait(0.5f),
                        BT.Call(_behave.DashAttack),
                        BT.Wait(2f),
                        BT.Call(_behave.TurnToTarget),
                        BT.Call(_behave.AfterOpening)
                    ),
                    
                    BT.If(_behave.Moving).OpenBranch(
                        BT.Wait(Time.deltaTime),
                        BT.Call(_behave.Initialization)
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
                            BT.Call(_behave.DashAttack)
                        ),
                        BT.Call(_behave.TurnToTarget)
                    ),

                    BT.Sequence().OpenBranch(
                        BT.Condition(_behave.InAttackRange),
                        BT.RandomSequence().OpenBranch(
                            BT.Call(_behave.Slash),
                            BT.Call(_behave.JumpAttack),
                            BT.Call(_behave.DashAttack)
                        ),
                        BT.Call(_behave.TurnToTarget)                                              
                    ),

                    BT.Sequence().OpenBranch(
                        BT.Call(_behave.Charge),
                        BT.Wait(0.5f)                        
                    )
                )
            );
        }

        // Update is called once per frame
        void Update()
        {
            _AI.Tick();
        }
    }
}
