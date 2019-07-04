using System;
using System.Collections.Generic;
using UnityEngine;

namespace TripleBladeHorse.Combat
{
	public class BaseProjectileLauncher : MonoBehaviour
	{
		[SerializeField] protected Transform _attackPoint;
		[SerializeField] protected Projectile _prefab;
		[SerializeField] protected ParticleSystem _launchEffect;
		[SerializeField] protected AttackMove _move;

		public virtual Transform Target { get; set; }
		public virtual Vector2 LaunchDirection { get; set; } = Vector2.right;

		public virtual void Launch()
		{
			if (_launchEffect != null)
				_launchEffect.Play();

			var projectile = GetProjectile();
			projectile.Activate(AttackPackage.CreateNewPackage(), _move);
		}

		public virtual Projectile GetProjectile()
		{
			var projectile = Instantiate(_prefab.gameObject, _attackPoint.position, Quaternion.identity).GetComponent<Projectile>();
			projectile.Target = Target;
			projectile.InitializeDirection(LaunchDirection);

			return projectile;
		}

		public virtual void Interrupt()
		{
			if (_launchEffect != null)
				_launchEffect.Stop();
		}
	}
}
