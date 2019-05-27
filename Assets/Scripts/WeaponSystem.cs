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
	IAttacker<AttackPackage, PlayerWeaponState> _weapon;
	IAttackMove<AttackPackage, PlayerMoveState> _moves;

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
