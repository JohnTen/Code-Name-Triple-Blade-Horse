using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyCharacter : MonoBehaviour
{
	[SerializeField] EnemyMover _mover;
	[SerializeField] EnemyState _state;
	[SerializeField] EnemyAnimation _animation;
	[SerializeField] EnemyWeapon _weapon;
	[SerializeField] ProjectileLauncher _launcher;
	[SerializeField] HitFlash _flash;
	[SerializeField] HitBox _hitBox;

	ICharacterInput<EnemyInput> _input;

	private void Awake()
	{
		_mover = GetComponent<EnemyMover>();
		_state = GetComponent<EnemyState>();
		_input = GetComponent<ICharacterInput<EnemyInput>>();
		_input.OnReceivedInput += HandleReceivedInput;
		_hitBox.OnHit += HandleOnHit;
		_launcher.Target = FindObjectOfType<PlayerCharacter>().HittingPoint;
	}

	private void HandleOnHit(AttackPackage attack, AttackResult result)
	{
		print("Hit");
		_flash.Flash();
		_mover.Knockback(attack._fromDirection * attack._knockback);
		_state._hitPoints -= result._finalDamage;
		_state._endurance -= result._finalFatigue;
		if (_state._hitPoints < 0)
			Destroy(this.gameObject);
	}

	private void Update()
	{
		_input.BlockInput = _animation._frozen;

		var moveInput = _state._frozen ? Vector2.zero : _input.GetMovingDirection();
		_mover.Move(moveInput);
		_animation._speed = moveInput;
	}

	private void HandleReceivedInput(InputEventArg<EnemyInput> eventArgs)
	{
		switch (eventArgs._command)
		{
			case EnemyInput.Attack:
				_animation.Attack();
				//_weapon.Attack();
				_launcher.LaunchDirection = _state._facingRight ? Vector2.right : Vector2.left;
				_launcher.Launch();
				break;
		}
	}

	
}
