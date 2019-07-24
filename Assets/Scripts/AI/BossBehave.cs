using System.Collections;
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
        [SerializeField] Vector2 _distance;
        CharacterState _state;
        [SerializeField] BaseWeapon _weapon;
        [SerializeField] float _attackAera;
        [SerializeField] float _movementAera;
        [SerializeField] float _dodgeAera;
        [SerializeField] float _chargeSpeed;

        bool _opening = true;
        bool _isCharging;
        bool _isWlaking;
		[SerializeField] int _slashCount = 0;
        [SerializeField] bool _dodged = false;
        float dodgePercent = 3000;
		[SerializeField] int _maxSlashCount=3;
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
            if(DelayInput || BlockInput){
                return Vector2.zero;
            }
			return _aim;
		}

		public Vector2 GetMovingDirection()
		{
            if(DelayInput || BlockInput){
                return Vector2.zero;
            }
			return _move;
		}

        public void Initialization(){
            _isCharging = false;
            _dodged = false;
        }
        public bool Moving(){
            return (DelayInput || BlockInput)&& !_isWlaking;
        }
        public bool Opening(){
            return _opening;
        }
        public void AfterOpening(){
            _opening = false;
        }
        public bool InAttackRange(){
            if(_distance.magnitude <= _attackAera){
                _isCharging = false;
                _isWlaking = false;
                return true;
            }else
            {
                return false;
            }
            
        }

        public void Slash(){
            _aim = _distance.normalized;
            _move = Vector2.zero;
            if(!BlockInput && !DelayInput){
                _slashCount ++;
            };            
			InvokeInputEvent(BossInput.Slash);
            _dodged = false;            
        }

        public void MoveToTarget(){
            _move = _distance;
            _move = _move.normalized * 0.01f;
            _aim = _move.normalized;
            _isWlaking = true;
        }

        public void Charge(){
            _move = _distance;
            _move = _move.normalized*2f;
            _aim = _move;
            _isCharging = true;
        }

        public bool IsNotCharging(){
            
            if(! _isCharging){
                _aim = Vector2.zero;
                _move = Vector2.zero;
            }
            return !_isCharging;
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
            
            if (!_dodged && _distance.magnitude <= _dodgeAera)
            {
                dodgePercent = Random.Range(0,_state._hitPoints);
                return(dodgePercent < 600f);
            }
            else return false;
        }

        public void Dodge(){
            _aim = _distance;
            _move = _aim;
			InvokeInputEvent(BossInput.Dodge);
            _dodged = true;
            weight[2] += 5;
            
        }

        public bool TooFar(){
            return (_distance.magnitude > 16f);
        }
        public void JumpAttack(){
            _aim = _distance.normalized;
            _move = Vector2.zero;

            InvokeInputEvent(BossInput.JumpAttack);
            _dodged = false;
        }

        public void DashAttack(){
            _aim = _distance.normalized;
            _move = Vector2.zero;
			InvokeInputEvent(BossInput.DashAttack);
            if(weight[2] != 1){
                    weight[2] = 1;
            }
            _dodged = false;
        }

        public void CombatTempGen(){
            combatTemp = Mathf.Sqrt(Random.Range( 0.25f, _state._hitPoints*0.001f));
            
            if(IsLowHealth()){
                combatTemp = 0.4f;
            }
        }

        public void WeightCalc(){
            

            if(IsLowHealth() && weight[1]!=3){
                weight[1] = 3;
            };

            if(_slashCount > 0 && _slashCount <= _maxSlashCount){
                weight[0] -= 2;
            };

            if(_slashCount > _maxSlashCount){
                weight[0] = 3;
                _slashCount = 0;
            };

            if(_distance.magnitude <= _dodgeAera){
                weight[2] = -1;
            }else{
                weight[2] = 3;
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

        }
        // Update is called once per frame
        void Update()
        {
            
            _distance = _target.position - this.transform.position;

        }
    }
}

