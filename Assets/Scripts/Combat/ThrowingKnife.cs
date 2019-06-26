using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TripleBladeHorse.Combat
{
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
		[SerializeField] float normalAttackRate = 1;
		[SerializeField] float floatAttackRate = 0.2f;
		[SerializeField] LayerMask rayCastMask;
		[Header("Debug")]
		[SerializeField] float traveledDistance;
		[SerializeField] float hoverTimer;
		[SerializeField] KnifeState state;
		[SerializeField] bool piercing;
		[SerializeField] AttackMove normalAttack;
		[SerializeField] AttackMove chargedAttack;
		[SerializeField] AttackMove floatAttack;
		[SerializeField] AttackMove drawStuckAttack;


		bool activated;
		Sheath sheath;
		ICanStickKnife stuckOn;
		IAttackable stuckAttack;

		public KnifeState State
		{
			get { return state; }
			set { state = value; }
		}

		public ICanStickKnife StuckOn => stuckOn;
		public IAttackable StuckAttack => stuckAttack;

		public bool Stuck { get; private set; }

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

			Activate(AttackPackage.CreateNewPackage(), null);
			_attackMove = isPiercing ? chargedAttack : normalAttack;
			_defaultType = isPiercing ? AttackType.ChargedRange : AttackType.Range;
			_baseAttackRate = normalAttackRate;


			return true;
		}

		public bool Hover()
		{
			if (state != KnifeState.Flying) return false;

			piercing = false;
			state = KnifeState.Hover;

			_attackMove = floatAttack;
			_defaultType = AttackType.Float;
			_baseAttackRate = floatAttackRate;

			return true;
		}

		public bool Withdraw()
		{
			if (state == KnifeState.InSheath || state == KnifeState.Returning)
				return false;

			transform.parent = null;
			state = KnifeState.Returning;

			_attackMove = piercing ? chargedAttack : normalAttack;
			_defaultType = piercing ? AttackType.ChargedRange : AttackType.Range;
			_baseAttackRate = normalAttackRate;

			if (stuckOn != null)
			{
				if (stuckOn.TryTakeOut(this.gameObject))
				{
					var direction = sheath.transform.position - this.transform.position;
					direction = DirectionalHelper.NormalizeHorizonalDirection(direction);
					TryAttack(stuckAttack, direction);
				}

				stuckOn = null;
			}

			Returning();

			return true;
		}

		public void RetractInstantly()
		{
			Deactivate();
			sheath.PutBackKnife(this);
			if (stuckOn != null)
			{
				stuckOn.TryTakeOut(this.gameObject);
			}
			stuckOn = null;
			stuckAttack = null;

			Stuck = false;
			hoverTimer = 0;
			traveledDistance = 0;
			state = KnifeState.InSheath;
		}

		private void Flying()
		{
			Physics2D.queriesHitTriggers = false;
			RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, speed * Time.deltaTime + bladeLength, rayCastMask);
			transform.position += transform.right * speed * Time.deltaTime;
			traveledDistance += speed * Time.deltaTime;

			if (hit.collider != null)
			{
				HandleFlyingCollision(hit);
			}

			if (traveledDistance >= hoveringDistance)
				Hover();
		}

		private void Returning()
		{
			var dir = sheath.transform.position - transform.position;

			if (dir.sqrMagnitude <= backDistance * backDistance)
			{
				Deactivate();
				sheath.PutBackKnife(this);
				state = KnifeState.InSheath;

				hoverTimer = 0;
				Stuck = false;
				traveledDistance = 0;
				return;
			}

			transform.right = dir;

			Debug.DrawRay(transform.position, transform.right * (speed * Time.deltaTime + bladeLength), Color.red);

			RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, speed * Time.deltaTime + bladeLength, rayCastMask);
			if (hit.collider != null)
			{
				TryAttack(hit.collider.transform);
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
			RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, bladeLength, rayCastMask);
			if (hit.collider != null)
			{
				TryAttack(hit.collider.transform);
			}
		}

		private bool TryAttack(Transform target)
		{
			var attackable = target.GetComponent<IAttackable>();
			if (IsAttackable(attackable))
			{
				var direction = target.position - transform.position;
				direction = DirectionalHelper.NormalizeHorizonalDirection(direction);
				return TryAttack(attackable, direction);
			}

			return false;
		}

		private void HandleFlyingCollision(RaycastHit2D hit)
		{
			var stickable = hit.collider.GetComponent<ICanStickKnife>();
			var attackable = hit.collider.GetComponent<IAttackable>();

			if (stickable != null && stickable.TryStick(this.gameObject))
			{
				Stuck = true;
				state = KnifeState.Stuck;
				transform.position = (Vector3)hit.point + transform.right * sinkingLength;
				transform.SetParent(hit.collider.transform);
				stuckOn = stickable;
			}

			if (IsAttackable(attackable))
			{
				print(hit.collider.name);
				var direction = hit.collider.transform.position - transform.position;
				direction = DirectionalHelper.NormalizeHorizonalDirection(direction);

				if (Stuck)
				{
					_defaultType = AttackType.StuckNDraw;
					_attackMove = drawStuckAttack;
				}

				if (TryAttack(attackable, direction) && Stuck)
				{
					stuckAttack = attackable;
				}
			}

			if (attackable == null && !Stuck)
			{
				print(hit.collider.name);
				Hover();
			}
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
}