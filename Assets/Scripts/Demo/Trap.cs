using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
	[SerializeField] float _damage;

	PlayerState _player;

	public void Start()
	{
		_player = FindObjectOfType<PlayerState>();
	}

	public void TriggerTrap()
	{
		_player._hitPoints -= _damage;
	}
}
