using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JTUtility;
using JTUtility.Platformer;

public class Enemy : PhysicalMover, IAttackable
{
	[SerializeField] int hitPoint;
	[SerializeField] float hitBackForce;
	[SerializeField] float stunDuration;
	[SerializeField] float detectionRange;
	[SerializeField] Transform player;
	[SerializeField] SpriteRenderer sprite;
	[SerializeField, MinMaxSlider(0, 5)] Vector2 flashRed;
	[SerializeField, MinMaxSlider(0, 5)] Vector2 actionChange;
	[SerializeField] int state;
	[SerializeField] float stateChangeTimer;
	[SerializeField] bool stuned;

	Slider healthBar;

	public event Action<AttackPackage, AttackResult> OnHit;

	public Faction Faction { get; }
	public bool Stuned => stuned;
	public int State => state;
	public Vector2 FirstTarget { get; set; }
	public Vector2 LastTarget { get; set; }
	public bool TowardsFirstTarget;

	Dictionary<int, AttackPackage> attacks = new Dictionary<int, AttackPackage>();

	protected override void Awake()
	{
		healthBar = GetComponentInChildren<Slider>();
		healthBar.maxValue = hitPoint;
		healthBar.value = hitPoint;
		player = FindObjectOfType<PlayerMover>().transform;
		base.Awake();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, new Vector3(detectionRange * 2, 1, 1));
	}

	public void AddForce(Vector2 force)
	{
		rigidBody.AddForce(force, ForceMode2D.Impulse);
	}

	protected override void Moving(Vector3 vector)
	{
		if (stuned) return;

		if (vector.x > 0)
		{
			sprite.transform.rotation = Quaternion.Euler(0, 0, 0);
		}
		else if (vector.x < 0)
		{
			sprite.transform.rotation = Quaternion.Euler(0, 180, 0);
		}

		base.Moving(vector);
	}

	protected override Vector3 GetMovingDirection()
	{
		if (stuned)
			return Vector3.zero;

		if (DetectedPlayer())
			return Chasing();
		else
			return Patrol();
	}
	
	Vector2 Chasing()
	{
		var towardsPlayer = player.position - transform.position;
		towardsPlayer.y = 0;
		return towardsPlayer.normalized;
	}

	Vector2 Patrol()
	{
		var towardsTarget = TowardsFirstTarget ? FirstTarget - (Vector2)transform.position : LastTarget - (Vector2)transform.position;
		if (towardsTarget.sqrMagnitude < 1)
			TowardsFirstTarget = !TowardsFirstTarget;

		return towardsTarget.normalized;
	}

	bool DetectedPlayer()
	{
		var towardsPlayer = player.position - transform.position;
		Physics2D.queriesStartInColliders = false;
		var hit = Physics2D.Raycast(transform.position, towardsPlayer, detectionRange);
		return hit.transform == player;
	}

	IEnumerator BeenHit()
	{
		float time = stunDuration;

		while (time > 0)
		{
			time -= Time.deltaTime;
			if (flashRed.IsIncluded(stunDuration - time))
			{
				sprite.color = Color.red;
			}
			else
			{
				sprite.color = Color.white;
			}
			yield return null;
		}

		stuned = false;
	}

	public AttackResult ReceiveAttack(AttackPackage attack)
	{
		if (attacks.ContainsKey(attack._hashID)) return AttackResult.Failed;

		attacks.Add(attack._hashID, attack);
		DelayedRemoveAttackPackage(attack._hashID);

		hitPoint -= (int)attack._hitPointDamage;
		healthBar.value = hitPoint;
		if (hitPoint <= 0)
			Destroy(gameObject);

		var hitDirection = attack._fromDirection;
		hitDirection.x = attack._fromDirection.x > 0 ? 1 : -1;
		hitDirection.y = 0.5f;

		stuned = true;
		StartCoroutine(BeenHit());
		rigidBody.AddForce(hitDirection * hitBackForce * attack._knockback, ForceMode2D.Impulse);

		return new AttackResult(this, true, attack._hitPointDamage, attack._enduranceDamage, false);
	}

	IEnumerator DelayedRemoveAttackPackage(int hashID)
	{
		yield return new WaitForSeconds(0.2f);
		attacks.Remove(hashID);
	}
}
