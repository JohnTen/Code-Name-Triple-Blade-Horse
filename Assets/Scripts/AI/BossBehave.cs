﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;
using TripleBladeHorse.Combat;

namespace TripleBladeHorse.AI
{
    public enum BossInput
    {
		Null,
        Slash,
        Dodge,
        JumpAttack,
        DashAttack,
    }
    public class BossBehave : MonoBehaviour, ICharacterInput<BossInput>
    {
        Transform _target;
        Vector2 _distance;
        CharacterState _state;
        [SerializeField] BaseWeapon _weapon;
        [SerializeField] float _attackAera;
        [SerializeField] float _dodgeAera;
        [SerializeField] float _chargeSpeed;

		int _slashCount = 0;
        int _attackCount = 0;
        float dodgePercent = 3000;
		public int _maxSlashCount=3;
		public int[] weight = new int[]{5,1,3};
		public float combatTemp = 3;
		
		bool _delayingInput;
		bool _blockingInput;
		Vector2 _move;
		Vector2 _aim;

		InputEventArg<BossInput> _delayedInput;

		public bool DelayInput
		{
			get => _delayingInput;
			set
			{
				if (_delayingInput == value)
					return;

				_delayingInput = value;
				if (!value && _delayedInput._command != BossInput.Null)
				{
					OnReceivedInput?.Invoke(_delayedInput);
				}

				if (_delayingInput)
				{
					_delayedInput._command = BossInput.Null;
					_delayedInput._additionalValue = 0;
				}
			}
		}

		public bool BlockInput
		{
			get => _blockingInput;
			set => _blockingInput = value;
		}

		public event Action<InputEventArg<BossInput>> OnReceivedInput;

        public Vector2 GetAimingDirection()
		{
			return _aim;
		}

		public Vector2 GetMovingDirection()
		{
			return _move;
		}

        public bool InAttackRange(){
            return (_distance.magnitude <= _attackAera);
        }

        public void Slash(){
            _aim = _distance.normalized;
            _move = Vector2.zero;
			InvokeInputEvent(BossInput.Slash);
            weight[0] -= 2;
            _slashCount ++;
            _attackCount ++;            
        }
        
        public void MoveToTarget(){
            _move = _distance;
            _move = _move.normalized * 0.001f;
            _aim = _move.normalized;
        }

        public void Charge(){
            _move = _distance;
            _move = _move.normalized*2f;
            _aim = _move;
        }

        public void Retreat(){
            _move = -_distance;
            _move.Normalize();
            _aim = -_move;
        }

        public bool IsLowHealth(){
            return (_state._hitPoints < 900f);
        }

        public bool NeedDodge(){
            
            if (_attackCount !=0 && _distance.magnitude <= _dodgeAera)
            {
                dodgePercent = Random.Range(0,_state._hitPoints);
                return(dodgePercent < 400f);
            }
            else return false;
        }

        public void Dodge(){
            _aim = _distance;
            _move = _aim;
			InvokeInputEvent(BossInput.Dodge);
            weight[2] += 5;
            _attackCount = 0;
        }

        public void JumpAttack(){
            _aim = _distance.normalized;
            _move = Vector2.zero;
            InvokeInputEvent(BossInput.JumpAttack);
            _attackCount ++;
        }

        public void DashAttack(){
            _aim = _distance.normalized;
            _move = Vector2.zero;
			InvokeInputEvent(BossInput.DashAttack);
            _attackCount ++;
            if(weight[2] != 3)
                weight[2] = 3;
        }

        public void CombatTempGen(){
            if(combatTemp > 1 ){
                combatTemp = Random.Range( 0.9f, _state._hitPoints*0.001f);
            }
        }

        public void WeightCalc(){
            
            if(IsLowHealth()){
                weight[1] = 3;
            };

            if(_slashCount>0 && _slashCount < _maxSlashCount && !IsLowHealth()){
                weight[0] = 1000;
            };

            if(_slashCount > _maxSlashCount){
                weight[0] = 5;
                _slashCount=0;
            };
        }

		private void InvokeInputEvent(BossInput command)
		{
			if (DelayInput)
			{
				_delayedInput._command = command;
				return;
			}
			
			OnReceivedInput?.Invoke(new InputEventArg<BossInput>(command));
		}

		private void InvokeInputEvent(BossInput command, float value)
		{
			if (DelayInput)
			{
				_delayedInput._command = command;
				_delayedInput._additionalValue = value;
				return;
			}

			OnReceivedInput?.Invoke(new InputEventArg<BossInput>(command, value));
		}

		/// Awake is called when the script instance is being loaded.
		/// </summary>
		public void Awake()
        {
            _target = FindObjectOfType<PlayerCharacter>().transform;
            _state = GetComponent<CharacterState>();
            _weapon.OnHit += HandleOnHit;
        }

        private void HandleOnHit(IAttackable attackable, AttackResult result, AttackPackage package)
        {
            if(_slashCount>0)
            {
                if(!result._attackSuccess){
                    _slashCount --;
                }
            }
        }
        // Update is called once per frame
        void Update()
        {
            _distance = _target.position - this.transform.position;

            
        }
    }
}

