using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse.Combat
{
	public class WeaponSystem : MonoBehaviour
	{
		[SerializeField] float fireRate;
		[SerializeField] float allLaunchRate;
		[SerializeField] float autoPickupDistance;
		[SerializeField] float autoReturnDistance;
		[SerializeField] float pullingForce;
		[Space(30)]
		[SerializeField] Sword sword;
		[SerializeField] Sheath sheath;
		[SerializeField] CharacterState _state;
		[SerializeField] AttackMove meleeMove;
		[SerializeField] AttackMove chargedMeleeMove;
		[SerializeField] GameObject _attackEffectPrefab;
		[SerializeField] AudioSource _audio;
		[SerializeField] AudioClip[] _normalMeleeSFX;

		GameObject _currentAttackEffect;
		[SerializeField] List<ThrowingKnife> knifesInAirList;

		public bool Frozen { get; private set; }

		public event Action<Vector3> OnPull;
		public event Action<IAttacker, AttackMove> OnRaisingAttack;

		private void Awake()
		{
			knifesInAirList = new List<ThrowingKnife>();
			sheath.OnRecievedKnife += Sheath_OnRecievedKnife;
		}

		public void MeleeAttack()
		{
			var package = AttackPackage.CreateNewPackage();
			package._faction = Faction.Player;
			package._attackType = AttackType.Melee;
			package._hitPointDamage.Base = _state._hitPointDamage;
			package._enduranceDamage.Base = _state._enduranceDamage;
			OnRaisingAttack?.Invoke(sword, meleeMove);

			sword.Activate(package, meleeMove);

			if (_currentAttackEffect) return;
			_currentAttackEffect = Instantiate(_attackEffectPrefab);
			_currentAttackEffect.transform.SetParent(sword.transform);
			_currentAttackEffect.transform.localPosition = Vector3.zero;

			_audio.clip = _normalMeleeSFX.PickRandom();
			_audio.Play();
		}

		public void ChargedMeleeAttack(float chargedPercent)
		{
			var package = AttackPackage.CreateNewPackage();
			package._faction = Faction.Player;
			package._attackType = AttackType.ChargedMelee;
			package._hitPointDamage.Base = _state._hitPointDamage;
			package._enduranceDamage.Base = _state._enduranceDamage;
			package._chargedPercent.Base = chargedPercent;
			OnRaisingAttack?.Invoke(sword, chargedMeleeMove);

			sword.Activate(package, chargedMeleeMove);

			_audio.clip = _normalMeleeSFX.PickRandom();
			_audio.Play();
		}

		public void MeleeAttackEnd()
		{
			sword.Deactivate();

			if (_currentAttackEffect)
			{
				Destroy(_currentAttackEffect);
			}
		}

		public void StartRangeCharge(Func<float> chargeTimer)
		{
			sheath.StartCharge(chargeTimer);
		}

		public void RangeAttack(Vector2 direction)
		{
			var knife = sheath.TakeKnife(false);

			if (knife != null)
			{
				knifesInAirList.Add(knife);
				knife.PullingForce = pullingForce;
				knife.Launch(direction, false);
			}
			sheath.StopCharge();
		}

		public void ChargedRangeAttack(Vector2 direction)
		{
			StartCoroutine(LaunchAllKnife(direction));
		}

		public void WithdrawAll()
		{
			List<ICanHandlePullingKnife> handlers = new List<ICanHandlePullingKnife>();
			GetComponentsInChildren(handlers);

			for (int i = 0; i < knifesInAirList.Count; i++)
			{
				if (knifesInAirList[i].State == KnifeState.Flying)
					continue;

				if (knifesInAirList[i].StuckOn != null)
				{
					foreach (var handler in handlers)
					{
						handler.OnPullingKnife(knifesInAirList[i].StuckOn, knifesInAirList[i]);
					}
				}
				knifesInAirList[i].Withdraw();
			}
		}

		public void WithdrawOne()
		{
			var minDistance = float.PositiveInfinity;
			var knifeIndex = -1;
			for (int i = 0; i < knifesInAirList.Count; i++)
			{
				var distance = (knifesInAirList[i].transform.position - sheath.transform.position).sqrMagnitude;
				if (distance > minDistance
				|| knifesInAirList[i].State == KnifeState.Flying
				|| knifesInAirList[i].State == KnifeState.Returning)
					continue;

				minDistance = distance;
				knifeIndex = i;
			}
			
			if (knifeIndex >= 0)
			{
				List<ICanHandlePullingKnife> handlers = new List<ICanHandlePullingKnife>();
				GetComponentsInChildren(handlers);

				if (knifesInAirList[knifeIndex].Stuck)
					OnPull?.Invoke((knifesInAirList[knifeIndex].transform.position - transform.position).normalized);

				if (knifesInAirList[knifeIndex].StuckOn != null)
				{
					foreach (var handler in handlers)
					{
						handler.OnPullingKnife(knifesInAirList[knifeIndex].StuckOn, knifesInAirList[knifeIndex]);
					}
				}
				knifesInAirList[knifeIndex].Withdraw();
			}
		}

		public void ResetWeapon()
		{
			while (knifesInAirList.Count > 0)
			{
				knifesInAirList[0].RetractInstantly();
			}

			if (_currentAttackEffect)
			{
				Destroy(_currentAttackEffect);
			}

			sheath.StopCharge();
		}

		private void Sheath_OnRecievedKnife(ThrowingKnife knife)
		{
			knifesInAirList.Remove(knife);
		}

		private void Player_OnChangeDirection(bool right)
		{
			sheath.UpdateFacingDirection(right);
		}

		private void Update()
		{
			sheath.ReloadSpeed = fireRate;

			foreach (var knife in knifesInAirList)
			{
				var dir = knife.transform.position - sheath.transform.position;
				if (dir.sqrMagnitude > autoReturnDistance * autoReturnDistance)
				{
					knife.Withdraw();
				}

				if (knife.State == KnifeState.Stuck && !knife.Stuck && dir.sqrMagnitude < autoPickupDistance * autoPickupDistance)
				{
					knife.Withdraw();
				}
			}

			if (Input.GetKeyDown(KeyCode.P))
				ResetWeapon();
		}

		IEnumerator LaunchAllKnife(Vector3 direction)
		{
			var time = 0f;
			var launchTimes = 0;
			
			while (launchTimes < 3)
			{
				time += TimeManager.PlayerDeltaTime * allLaunchRate;
				if (time < 1)
				{
					yield return null;
					continue;
				}
				time--;
				launchTimes++;

				var knife = sheath.TakeKnife(true);
				if (knife == null) continue;

				knifesInAirList.Add(knife);
				knife.Launch(direction, true);
			}
			sheath.StopCharge();
		}
	}
}
