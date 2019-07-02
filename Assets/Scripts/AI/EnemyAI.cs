using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTAI;

namespace TripleBladeHorse.AI
{
	public class EnemyAI : MonoBehaviour
	{
		[SerializeField] float _attackInterval;

		Root enemyRoot = BT.Root();
		public EnemyBehave enemyBehave;

		private void OnEnable()
		{
            enemyRoot.OpenBranch(
                BT.Selector().OpenBranch(
                    BT.Sequence().OpenBranch(
                        BT.Condition(enemyBehave.AttackAction),
						BT.Call(enemyBehave.Attack),
						BT.Wait(_attackInterval)
                    ),
                    BT.Sequence().OpenBranch(
                        BT.Condition(enemyBehave.AlertAction),
					    BT.Call(enemyBehave.MoveToPlayer)
					   ),
				    BT.Call(enemyBehave.Patrol)
				   )
		   );
		}
		
		void Update()
		{
			enemyRoot.Tick();
		}
	}
}
