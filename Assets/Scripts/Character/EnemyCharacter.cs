using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleBladeHorse.Animation;
using TripleBladeHorse.Combat;
using TripleBladeHorse.Movement;
using TripleBladeHorse.AI;

namespace TripleBladeHorse
{
	public class EnemyCharacter : MonoBehaviour
	{
		[SerializeField] EnemyMover _mover;
		[SerializeField] EnemyState _state;
		[SerializeField] EnemyWeapon _weapon;
		[SerializeField] ProjectileLauncher _launcher;
		[SerializeField] HitFlash _flash;
		[SerializeField] HitBox _hitBox;

		bool dying;
		FSM _animator;
		ICharacterInput<EnemyInput> _input;

		private void Awake()
		{
			_mover = GetComponent<EnemyMover>();
			_state = GetComponent<EnemyState>();
			_animator = GetComponent<FSM>();
			_input = GetComponent<ICharacterInput<EnemyInput>>();
			_input.OnReceivedInput += HandleReceivedInput;
			_hitBox.OnHit += HandleOnHit;
			_launcher.Target = FindObjectOfType<PlayerCharacter>().HittingPoint;
			_animator.Subscribe(Animation.AnimationState.Completed, HandleAnimationEvent);
		}

		private void HandleAnimationEvent(AnimationEventArg eventArgs)
		{
			if (eventArgs._animation.name != "Death_Ground")
				return;

			Destroy(this.gameObject);
		}

		private void HandleOnHit(AttackPackage attack, AttackResult result)
		{
			print("Hit");
			_flash.Flash();
			_mover.Knockback(attack._fromDirection * attack._knockback);
			_state._hitPoints -= result._finalDamage;
			_state._endurance -= result._finalFatigue;
			if (_state._hitPoints < 0)
			{
				Dying();
			}
		}

		private void Update()
		{
			if (dying) return;
			_input.BlockInput = _animator.GetBool("BlockInput");
			_state._frozen = _animator.GetBool("BlockInput");

			var moveInput = _state._frozen ? Vector2.zero : _input.GetMovingDirection();
			_mover.Move(moveInput);
			_animator.SetFloat("XSpeed", moveInput.x);
			if (moveInput.x != 0)
				_animator.FlipX = moveInput.x > 0;
		}

		private void HandleReceivedInput(InputEventArg<EnemyInput> eventArgs)
		{
			if (dying) return;
			switch (eventArgs._command)
			{
				case EnemyInput.Attack:
					_animator.SetToggle("Attack", true);
					_launcher.LaunchDirection = _state._facingRight ? Vector2.right : Vector2.left;
					_launcher.Launch();
					break;
			}
		}

		private void Dying()
		{
			dying = true;
			_animator.SetBool("Death", true);

			GetComponent<Rigidbody2D>().simulated = false;
		}
	}
}
