using System.Collections;
using System.Collections.Generic;
using JTUtility;
using UnityEngine;

namespace TripleBladeHorse.AI
{
	public enum EnemyInput
	{
		Attack,
		Jump,
	}

	public class EnemyBehave : MonoBehaviour, ICharacterInput<EnemyInput>
	{
		[SerializeField] float _alertArea = 5;
		[SerializeField] float _attackArea = 1.5f;
		[SerializeField] float _patrolArea = 5;
		[SerializeField] float _stopTime = 1;
		[SerializeField] float _error = 1;
		[SerializeField] bool _pause = false;
		[SerializeField] bool _partol;
		[SerializeField] CharacterState _state;

		Vector2 _move;
		Vector2 _aim;
		float _stopPosition;
		float _stopRandomTime;
		Vector2 _bornPosition;
		Transform _character;

		public bool DelayInput { get; set; }
		public bool BlockInput { get; set; }

		public event Action<InputEventArg<EnemyInput>> OnReceivedInput;

		public Vector2 GetAimingDirection()
		{
			return _aim;
		}

		public Vector2 GetMovingDirection()
		{
			return _move;
		}

		public void MoveToPlayer()
		{
			_move = _character.position - transform.position;
			_move.Normalize();
			_aim = _move;
		}

		public void Patrol()
		{
			float distance = this.transform.position.x - _bornPosition.x;

			if (_pause && _stopRandomTime < Time.time)
			{
				_stopPosition = Random.Range(-_patrolArea, _patrolArea);
				_pause = false;
			}
			else if (!_pause && Mathf.Abs(distance - _stopPosition) < _error)
			{
				_stopRandomTime = Random.Range(0, _stopTime) + Time.time;
				_pause = true;
			}

			if (!_pause)
			{
				if (distance - _stopPosition < 0)
				{
					_move = Vector2.right;
					_aim = _move;
				}
				else if (distance - _stopPosition > 0)
				{
					_move = Vector2.left;
					_aim = _move;
				}
			}

			if (_pause)
			{
				_move = Vector2.zero;
			}
		}

		public void Attack()
		{
			_move = (_character.position - transform.position).normalized * 0.01f;
			OnReceivedInput?.Invoke(new InputEventArg<EnemyInput>(EnemyInput.Attack));
		}

		public bool AttackAction()
		{
			float distance;
			distance = Mathf.Abs(this.transform.position.x - _character.position.x);
			if (distance < _attackArea)
			{
				return true;
			}

			return false;
		}

		public bool AlertAction()
		{
			float distance;
			distance = this.transform.position.x - _character.position.x;
			if (Mathf.Abs(distance) < _alertArea && (distance > 0) != _state._facingRight)
			{
				return true;
			}

			return false;
		}

		public void Awake()
		{
			_character = FindObjectOfType<PlayerCharacter>().transform;
			_stopPosition = Random.Range(-_patrolArea, _patrolArea);
			_stopRandomTime = Random.Range(0, _stopTime) + Time.time;
			_bornPosition = this.transform.position;
		}
	}
}