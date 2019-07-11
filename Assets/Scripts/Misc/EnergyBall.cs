using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleBladeHorse.Combat;

namespace TripleBladeHorse
{
	public class EnergyBall : MonoBehaviour
	{
		[SerializeField] int _replenishAmount = 1;
		[SerializeField] Projectile _projectile;

		public int ReplenishAmount
		{
			get => _replenishAmount;
			set => _replenishAmount = value;
		}

		private void Awake()
		{
			_projectile.OnDestorying += OnDestroying;
			_projectile.Target = GameManager.PlayerInstance.transform;
			_projectile.InitializeDirection(_projectile.Target.position - _projectile.transform.position);
		}

		private void OnDestroying(Projectile projectile, Collider2D collider)
		{
			if (collider == null) return;

			var player = collider.GetComponentInParent<PlayerCharacter>();
			player.ReplenishStamina(ReplenishAmount);
		}
	}
}
