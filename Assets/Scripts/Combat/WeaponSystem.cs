using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerWeaponState
{

}
public enum PlayerMoveState
{

}

public class WeaponSystem : MonoBehaviour
{
	IAttacker _weapon;
	IAttackMove<AttackPackage> _moves;

	public void Activate(AttackPackage attack)
	{

	}

	public void Deactivate()
	{

	}

	public bool IsAvailable()
	{
		return true;
	}
}
