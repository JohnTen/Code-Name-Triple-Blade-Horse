using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TripleBladeHorse.Combat
{
	public class ProjectileLauncher : MonoBehaviour
	{
		[SerializeField] Transform _attackPoint;
		[SerializeField] Projectile _prefab;
		[SerializeField] float _delay = 0.3334f;
		[SerializeField] ParticleSystem _launchEffect;
		[SerializeField] AttackMove _move;
		[SerializeField] Vector2 _launchDirection = Vector2.right;

		public Transform Target { get; set; }
		public Vector2 LaunchDirection
		{
			get => _launchDirection;
			set => _launchDirection = value;
		}

		public void Launch()
		{
			if (_launchEffect != null)
				_launchEffect.Play();
			Invoke("StartAttack", _delay);
		}

		public void StartAttack()
		{
			var projectile = Instantiate(_prefab.gameObject, _attackPoint.position, Quaternion.identity).GetComponent<Projectile>();
			projectile.Target = Target;
			projectile.InitializeDirection(LaunchDirection);
			projectile.Activate(AttackPackage.CreateNewPackage(), _move);
		}

		public void Interrupt()
		{
			CancelInvoke();
			_launchEffect.Stop();
		}
	}
}
