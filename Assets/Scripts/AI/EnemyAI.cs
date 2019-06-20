using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTAI;

public class EnemyAI : MonoBehaviour
{
    Root enemyRoot = BT.Root();
    public bool isAlert = false;
    public bool inAttackRange = false;

    // Start is called before the first frame update
    void Start()
    {
        enemyRoot.OpenBranch(
            BT.Selector().OpenBranch(
                BT.If(PlayerInSight).OpenBranch(
                    BT.Call(Alert),
                    BT.Wait(1.0f),
                    BT.If(() => { return 1 == 1; }).OpenBranch(
                        BT.Call(MeleeAttack),
                        BT.Wait(1.0F)
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

    public bool PlayerInSight()
    {

        Debug.Log("I've got you in my sight");
        return true;
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
