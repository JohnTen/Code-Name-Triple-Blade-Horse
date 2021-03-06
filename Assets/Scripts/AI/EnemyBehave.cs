﻿using JTUtility;
using UnityEngine;

namespace TripleBladeHorse.AI
{
    public enum EnemyInput
    {
        Attack,
        Jump,
        Dodge,
        Alert,
    }

    public class EnemyBehave : MonoBehaviour, ICharacterInput<EnemyInput>
    {
        [SerializeField] float _alertArea = 5;
        [SerializeField] float _attackArea = 1.5f;
        [SerializeField] float _dodgeArea = 1f;
        [SerializeField] float _patrolArea = 5;
        [SerializeField] float _stopTime = 1;
        [SerializeField] float _error = 1;
        [SerializeField] bool _pause = false;
        [SerializeField] bool _partol;
        [SerializeField] float _chargeSpeed = 8;
        [SerializeField] CharacterState _state;

        [SerializeField] Movement.EnemyMover enemyMover;


        Vector2 _move;
        Vector2 _aim;
        float _stopPosition;
        float _stopRandomTime;
        Vector2 _bornPosition;
        Transform _character;
        Vector2 _distance;
        //Timer attackTimer = new Timer();

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

        public void Alert()
        {
            _aim = (_character.position - transform.position).normalized * 0.01f;
            OnReceivedInput?.Invoke(new InputEventArg<EnemyInput>(EnemyInput.Alert));
        }

        public void Charge()
        {
            enemyMover._baseSpeed = _chargeSpeed;
        }

        public void Patrol()
        {
            float patrolDis = this.transform.position.x - _bornPosition.x;

            if (_pause && _stopRandomTime < Time.time)
            {
                _stopPosition = Random.Range(-_patrolArea, _patrolArea);
                _pause = false;
            }
            else if (!_pause && Mathf.Abs(patrolDis - _stopPosition) < _error)
            {
                _stopRandomTime = Random.Range(0, _stopTime) + Time.time;
                _pause = true;
            }

            if (!_pause)
            {
                if (patrolDis - _stopPosition < 0)
                {
                    _move = Vector2.right;
                    _aim = _move;
                }
                else if (patrolDis - _stopPosition > 0)
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
            _aim = (_character.position - transform.position).normalized * 0.01f;
            _move = _aim;
            OnReceivedInput?.Invoke(new InputEventArg<EnemyInput>(EnemyInput.Attack));
            //attackTimer.Start(2);
        }

        public bool needDodge()
        {
            if (_distance.magnitude < _dodgeArea)
            {
                return true;
            }

            return false;

            //!attackTimer.IsReachedTime();
        }

        public void Dodge()
        {
            _move = transform.position - _character.position;
            _move.Normalize();
            _aim = -_move;
            //OnReceivedInput?.Invoke(new InputEventArg<EnemyInput>(EnemyInput.Dodge));
        }

        public bool AttackAction()
        {
            if (_distance.magnitude < _attackArea)
            {
                return true;
            }

            return false;
        }

        public bool AlertAction()
        {
            float _backAlertArea;
            _backAlertArea = _alertArea * 0.2f;
            if ((IsFacing() && Mathf.Abs(_distance.x) < _alertArea && IsSameLevel()) || Mathf.Abs(_distance.magnitude) < _backAlertArea)
            {
                return true;
            }

            return false;
        }

        bool IsFacing()
        {
            if (_distance.x > 0 && !_state._facingRight)
            {
                return true;
            }
            else if (_distance.x < 0 && _state._facingRight)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool IsSameLevel()
        {
            return (Mathf.Abs(_distance.y) < 2);
        }

        public void Awake()
        {
            _character = FindObjectOfType<PlayerCharacter>().transform;
            _stopPosition = Random.Range(-_patrolArea, _patrolArea);
            _stopRandomTime = Random.Range(0, _stopTime) + Time.time;
            _bornPosition = this.transform.position;
            //attackTimer.Start(2);
        }

        public void Update()
        {
            _distance = this.transform.position - _character.position;
        }
    }
}