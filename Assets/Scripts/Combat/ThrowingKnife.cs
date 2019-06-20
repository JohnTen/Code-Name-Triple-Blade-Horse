using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KnifeState
{
	InSheath,
	Flying,
	Returning,
	Hover,
	Stuck,
}

public class ThrowingKnife : BaseWeapon
{
	[SerializeField] float speed;
	[SerializeField] float bladeLength;
	[SerializeField] float backDistance;
	[SerializeField] float sinkingLength;
	[SerializeField] float hoveringDistance;
	[SerializeField] float hoveringDuration;
	[SerializeField] float hoveringRotateSpeed;
	[Space(30)]
	[SerializeField] float traveledDistance;
	[SerializeField] float hoverTimer;
	[SerializeField] KnifeState state;
	[SerializeField] bool piercing;
	[SerializeField] AttackMove normalAttack;
	[SerializeField] AttackMove chargedAttack;
	[SerializeField] AttackMove floatAttack;

	bool activated;
	Sheath sheath;
	AttackPackage attackPackage;

	public KnifeState State
	{
		get { return state; }
		set { state = value; }
	}

	public bool StuckedOnClimbable { get; private set; }

	private void Update()
	{
		switch (state)
		{
			case KnifeState.Flying: Flying(); break;
			case KnifeState.Returning: Returning(); break;
			case KnifeState.Hover: Hovering(); break;
		}
	}

	public void SetSheath(Sheath sheath)
	{
		this.sheath = sheath;
	}

	public bool Launch(Vector3 direction, bool isPiercing = false)
	{
		if (state != KnifeState.InSheath) return false;

		piercing = isPiercing;
		state = KnifeState.Flying;

		transform.position = sheath.LaunchPosition.position;
		transform.right = direction;

		if (isPiercing)
		{
			attackPackage = CreateNewPackage(AttackType.ChargedRange, chargedAttack);
		}
		else
		{
			attackPackage = CreateNewPackage(AttackType.Range, normalAttack);
		}

		return true;
	}

	public bool Hover()
	{
		if (state != KnifeState.Flying) return false;

		piercing = false;
		state = KnifeState.Hover;

		attackPackage = CreateNewPackage(AttackType.Float, floatAttack);
		attackPackage._attackRate = 0.2f;

		return true;
	}

	public bool Withdraw()
	{
		if (state == KnifeState.InSheath || state == KnifeState.Returning)
			return false;

		transform.parent = null;
		state = KnifeState.Returning;

		if (piercing)
		{
			attackPackage = CreateNewPackage(AttackType.ChargedRange, chargedAttack);
		}
		else
		{
			attackPackage = CreateNewPackage(AttackType.Range, normalAttack);
		}

		Returning();

		return true;
	}

	private void Flying()
	{
		Physics2D.queriesHitTriggers = false;
		RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, speed * Time.deltaTime + bladeLength);
		transform.position += transform.right * speed * Time.deltaTime;
		traveledDistance += speed * Time.deltaTime;

		if (hit.collider != null)
		{
			var attackable = hit.collider.GetComponent<IAttackable>();

			if (hit.collider.tag == "Climbable")
			{
				StuckedOnClimbable = true;
				state = KnifeState.Stuck;
				transform.position = (Vector3)hit.point + transform.right * sinkingLength;
				return;
			}
			else if (hit.collider.tag == "Player") { }
			else if (attackable != null)
			{
				print("Enemy fly");
				var attack = attackPackage;
				attack._fromDirection = hit.collider.transform.position - transform.position;
				RaiseOnHitEvent(attackable, attackable.ReceiveAttack(attack), attack);
			}
			else
			{
				print(hit.collider.name);
				transform.position = hit.point;
				Hover();
				return;
			}
		}

		if (traveledDistance >= hoveringDistance)
			Hover();
	}

	private void Returning()
	{
		var dir = sheath.transform.position - transform.position;

		if (dir.sqrMagnitude <= backDistance * backDistance)
		{
			sheath.PutBackKnife(this);
			state = KnifeState.InSheath;

			hoverTimer = 0;
			StuckedOnClimbable = false;
			traveledDistance = 0;
			return;
		}

		transform.right = dir;

		Debug.DrawRay(transform.position, transform.right * (speed * Time.deltaTime + bladeLength), Color.red);

		RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, speed * Time.deltaTime + bladeLength);
		if (hit.collider != null)
		{
			var attackable = hit.collider.GetComponent<IAttackable>();

			if (attackable != null)
			{
				print("Enemy return");
				var attack = attackPackage;
				attack._fromDirection = hit.collider.transform.position - transform.position;
				RaiseOnHitEvent(attackable, attackable.ReceiveAttack(attack), attack);
			}
		}

		transform.position += transform.right * speed * Time.deltaTime;
	}

	private void Hovering()
	{
		hoverTimer += Time.deltaTime;
		transform.Rotate(0, 0, hoveringRotateSpeed * Time.deltaTime);

		if (hoverTimer > hoveringDuration)
			Withdraw();

		Debug.DrawRay(transform.position, transform.right * bladeLength, Color.red);
		RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, bladeLength);
		if (hit.collider != null)
		{
			var attackable = hit.collider.GetComponent<IAttackable>();

			if (attackable != null)
			{
				print("Enemy hover");
				var attack = attackPackage;
				attack._fromDirection = hit.collider.transform.position - transform.position;
				RaiseOnHitEvent(attackable, attackable.ReceiveAttack(attack), attack);
			}
		}
	}

	public override void Activate(AttackPackage attack, AttackMove move)
	{
		attackPackage = attack;
		activated = true;
	}

	public override void Deactivate()
	{
		activated = false;
	}

	private AttackPackage CreateNewPackage(AttackType type, AttackMove move)
	{
		var package = AttackPackage.CreateNewPackage();
		package._hitPointDamage.Base = _baseHitPointDamage;
		package._enduranceDamage.Base = _baseEnduranceDamage;
		package._attackType = type;
		package._faction = Faction.Player;
		package = move.Process(package);

		return package;
	}
}
