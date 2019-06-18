﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTAI;

public class EnemyAI : MonoBehaviour
{
    Root enemyRoot = BT.Root();

    // Start is called before the first frame update
    void Start()
    {
        enemyRoot.OpenBranch(
            BT.Selector().OpenBranch(
                BT.If(() => { return 1 == 1; }).OpenBranch(
                    BT.Call(Alert),
                    BT.Wait(1.0f),
                    BT.If(() => { return 1 == 1; }).OpenBranch(
                        BT.Call(MeleeAttack)
                     )
                ),
                BT.If(() => { return 2 > 1; }).OpenBranch(
                    BT.Call(MoveToPosition)
                    )
            )

        );
    }

    // Update is called once per frame
    void Update()
    {

        enemyRoot.Tick();
    }

    void PlayerInSight()
    {
        Debug.Log("I've got you in my sight");
        return;
    }
    void Alert()
    {
        Debug.Log("didididididi~");
    }

    void MeleeAttack()
    {
        Debug.Log("hasaki!!!");
    }

    void MoveToPosition()
    {
        Debug.Log("where r u?");
    }
}
