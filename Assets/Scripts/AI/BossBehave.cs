using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse.AI
{
    public enum BossInput
    {
        Slash,
        Dodge,
        JumpAttack,
        DashAttack,

    }
    public class BossBehave : MonoBehaviour, ICharacterInput<BossInput>
    {
        Transform _target;
        Vector2 _distance;
        [SerializeField] float _attackAera;
        [SerializeField] float _dodgeAera;
        [SerializeField] float _chargeSpeed;

        int _slashCount=0;
        [SerializeField] int _maxSlashCount=3;
        public int[] weight = new int[]{5,3,3,1};
        public int[] lowHealthWeight = new int[] {5,3,3,3};   

        public bool DelayInput { get; set; }
		public bool BlockInput { get; set; }

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
            OnReceivedInput?.Invoke(new InputEventArg<BossInput>(BossInput.Slash));
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
            return true;
        }

        public bool NeedDodge(){
            return (_distance.magnitude <= _dodgeAera);
        }

        public void Dodge(){
            _aim = _target.position - transform.position;
            _move = -_aim;
            OnReceivedInput?.Invoke(new InputEventArg<BossInput>(BossInput.Dodge));
            weight[2] += 3;
        }

        public void JumpAttack(){
            _aim = -_distance.normalized;
            OnReceivedInput?.Invoke(new InputEventArg<BossInput>(BossInput.JumpAttack));
        }

        public void DashAttack(){
            _aim = _target.position - transform.position;
            _move = _aim;
            OnReceivedInput?.Invoke(new InputEventArg<BossInput>(BossInput.DashAttack));
            if(weight[2] != 3)
                weight[2] = 3;
        }



        /// Awake is called when the script instance is being loaded.
        /// </summary>
        public void Awake()
        {
            _target = FindObjectOfType<PlayerCharacter>().transform;
        }

        // Update is called once per frame
        void Update()
        {
            _distance = this.transform.position - _target.position;
        }
    }
}

