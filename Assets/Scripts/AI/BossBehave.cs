using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

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
        [SerializeField] float _attackAera;
        [SerializeField] float _dodgeAera;
        [SerializeField] float _chargeSpeed;

		int _slashCount=0;
        [SerializeField] int _maxSlashCount=3;
        public int[] weight = new int[]{5,3,3,1};
        public int[] lowHealthWeight = new int[] {5,3,3,3};
        public float combatTemp = 3;
		
		bool _delayingInput;
		bool _blockingInput;
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
        Vector2 _move;
		Vector2 _aim;
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
            _aim = -_distance;
            _move = _aim;
			InvokeInputEvent(BossInput.Slash);
            weight[0]--;
            _slashCount++;
            if(_slashCount > _maxSlashCount){
                weight[0] = 5;
                _slashCount=0;
            }
        }
        public void MoveToTarget(){
            _move = _target.position - transform.position;
            _move.Normalize();
            _aim = _move;
        }

        public bool IsLowHealth(){
            return (_state._hitPoints < 900f);
        }

        public bool NeedDodge(){
            return (_distance.magnitude <= _dodgeAera);
        }

        public void Dodge(){
            _aim = _target.position - transform.position;
            _move = -_aim;
			InvokeInputEvent(BossInput.Dodge);
            weight[2] += 3;
        }

        public void JumpAttack(){
            _aim = -_distance.normalized;
			InvokeInputEvent(BossInput.JumpAttack);
        }

        public void DashAttack(){
            _aim = _target.position - transform.position;
            _move = _aim;
			InvokeInputEvent(BossInput.DashAttack);
            if(weight[2] != 3)
                weight[2] = 3;
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
        }

        // Update is called once per frame
        void Update()
        {
            _distance = this.transform.position - _target.position;
            if(combatTemp > 1 ){
                combatTemp=_state._hitPoints*0.001f;
            }
        }
    }
}

