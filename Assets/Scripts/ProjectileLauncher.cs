using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
	[SerializeField] Transform _attackPoint;
	[SerializeField] Projectile _prefab;
	[SerializeField] float _delay = 0.3334f;
	[SerializeField] ParticleSystem _launchEffect;

	public Transform Target { get; set; }
	public Vector2 LaunchDirection { get; set; } = Vector2.right;

	public void Launch()
	{
		_launchEffect.Play();
		Invoke("StartAttack", _delay);
	}

	public void StartAttack()
	{
		var projectile = Instantiate(_prefab.gameObject, _attackPoint.position, Quaternion.identity).GetComponent<Projectile>();
		projectile.Target = Target;
		projectile.InitializeDirection(LaunchDirection);
		projectile.Activate(AttackPackage.CreateNewPackage(), null);
	}
}
