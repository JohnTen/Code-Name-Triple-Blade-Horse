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
        float _distance;

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

        public void Slash(){
            
        }
        public void MoveToTarget(){
            _move = _target.position - transform.position;
            _move.Normalize();
            _aim = _move;
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
            _distance = this.transform.position.x - _target.position.x;
        }
    }
}

