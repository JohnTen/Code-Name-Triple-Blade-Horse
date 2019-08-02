using BTAI;
using UnityEngine;

namespace TripleBladeHorse.AI
{
    public class MeleeAI : MonoBehaviour
    {
        public EnemyBehave meleeBehave;
        [SerializeField] float _attackIntervalMelee;
        private void OnEnable()
        {
            meleeRoot.OpenBranch(
                BT.Selector().OpenBranch(
                    BT.Sequence().OpenBranch(
                        BT.Condition(meleeBehave.AttackAction),
                        BT.Call(meleeBehave.Attack),
                        BT.Wait(_attackIntervalMelee)
                    ),
                    BT.Sequence().OpenBranch(
                        BT.Condition(meleeBehave.AlertAction),
                        BT.Call(meleeBehave.Alert),
                        BT.Call(meleeBehave.Charge),
                        BT.Call(meleeBehave.MoveToPlayer)
                    ),
                    BT.Call(meleeBehave.Patrol)
                )
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

