using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public class WeaponSystem : MonoBehaviour
{
	[SerializeField] float fireRate;
	[SerializeField] float allLaunchRate;
	[SerializeField] float autoPickupDistance;
	[SerializeField] float autoReturnDistance;
	[Space(30)]
	[SerializeField] Sword sword;
	[SerializeField] Sheath sheath;
	[SerializeField] List<ThrowingKnife> knifesInAirList;

	public bool Frozen { get; private set; }

	public event Action<Vector3> OnPull;

	private void Awake()
	{
		knifesInAirList = new List<ThrowingKnife>();
		sheath.OnRecievedKnife += Sheath_OnRecievedKnife;
	}

	public void MeleeAttack()
	{
		sword.Activate(AttackPackage.CreateNewPackage());
	}

	public void ChargedMeleeAttack(float chargedPercent)
	{
		sword.Charge(chargedPercent);
		sword.Activate(AttackPackage.CreateNewPackage());
	}

	public void MeleeAttackEnd()
	{
		sword.Deactivate();
	}

	public void RangeAttack(Vector2 direction)
	{
		var knife = sheath.TakeKnife(false);

		if (knife != null)
		{
			knifesInAirList.Add(knife);
			knife.Launch(direction, true);
		}
	}

	public void ChargedRangeAttack(Vector2 direction)
	{
		StartCoroutine(LaunchAllKnife(direction));
	}

	public void WithdrawAll()
	{
		for (int i = 0; i < knifesInAirList.Count; i++)
		{
			if (knifesInAirList[i].State == KnifeState.Flying)
				continue;

			if (knifesInAirList[i].StuckedOnClimbable)
				OnPull?.Invoke((knifesInAirList[i].transform.position - transform.position).normalized);
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
			|| knifesInAirList[i].State == KnifeState.Flying)
				continue;

			minDistance = distance;
			knifeIndex = i;
		}

		if (knifeIndex >= 0)
		{
			if (knifesInAirList[knifeIndex].StuckedOnClimbable)
				OnPull?.Invoke((knifesInAirList[knifeIndex].transform.position - transform.position).normalized);
			knifesInAirList[knifeIndex].Withdraw();
		}
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

			if (knife.State == KnifeState.Stuck && !knife.StuckedOnClimbable && dir.sqrMagnitude < autoPickupDistance * autoPickupDistance)
			{
				knife.Withdraw();
			}
		}
	}

	IEnumerator LaunchAllKnife(Vector3 direction)
	{
		var time = 0f;

		Frozen = true;
		while (sheath.knifeCount > 0)
		{
			time += Time.deltaTime * allLaunchRate;
			if (time < 1)
			{
				yield return null;
				continue;
			}
			time--;

			var knife = sheath.TakeKnife(true);
			knifesInAirList.Add(knife);
			knife.Launch(direction, true);
		}
		Frozen = false;
	}
}
