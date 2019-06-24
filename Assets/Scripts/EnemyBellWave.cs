using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBellWave : BaseWeapon
{
	[SerializeField] float _attackDuration;
	[SerializeField] float _maxSize;
	[SerializeField] SpriteRenderer _attackShape;
	[SerializeField] Gradient _color;
	
	float _timer;

	public override void Activate(AttackPackage attack, AttackMove move)
	{
		base.Activate(attack, move);
		this.transform.localScale = Vector3.zero;
	}

	public override void Deactivate()
	{
		base.Deactivate();
		Destroy(this.gameObject);
	}

	private void Update()
	{
		_timer += Time.deltaTime;
		this.transform.localScale = Vector3.one * (Mathf.Lerp(0, _maxSize, _timer / _attackDuration));
		_attackShape.color = _color.Evaluate(_timer / _attackDuration);

		if (_timer >= _attackDuration)
			Deactivate();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		var attackable = collision.GetComponent<IAttackable>();
		var toTarget = collision.transform.position.x - this.transform.position.x;
		var direction = toTarget > 0 ? Vector2.right : Vector2.left;
		Attack(attackable, direction);
	}
}
