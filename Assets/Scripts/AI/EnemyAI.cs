﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTAI;

namespace TripleBladeHorse.AI
{
	public class EnemyAI : MonoBehaviour
	{
		[SerializeField] float _attackInterval = 2f;

		Root enemyRoot = BT.Root();
		public EnemyBehave enemyBehave;

		private void OnEnable()
		{
			enemyRoot.OpenBranch(
				BT.Selector().OpenBranch(
				   BT.If(enemyBehave.AttackAction).OpenBranch(
					   BT.Call(enemyBehave.Attack),
					   BT.Wait(_attackInterval)
					),
				   BT.If(enemyBehave.AlertAction).OpenBranch(
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
