using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTAI;

public class EnemyAI : MonoBehaviour
{
    Root enemyRoot = BT.Root();
    public EnemyBehave enemyBehave;

    private void OnEnable()
    {
        enemyRoot.OpenBranch(
            BT.Selector().OpenBranch(
               BT.If(enemyBehave.AttackAction).OpenBranch(
                   BT.Call(enemyBehave.MeleeAttack),
                   BT.Wait(2.0f)
                ),
               BT.If(enemyBehave.AlertAction).OpenBranch(
                   BT.Call(enemyBehave.MoveToPlayer)
                   ),
               BT.Call(enemyBehave.Patrol)
               )
       );
    }

    // Update is called once per frame
    void Update()
    {
        enemyRoot.Tick();
    }


}
