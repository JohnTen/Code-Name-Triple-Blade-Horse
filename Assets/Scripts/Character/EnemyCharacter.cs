using TripleBladeHorse.AI;
using TripleBladeHorse.Animation;
using TripleBladeHorse.Combat;
using TripleBladeHorse.Movement;
using UnityEngine;

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
        float _enduranceRefreshTimer;
        float _enduranceRecoverTimer;
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
            if (eventArgs._animation.name != "Monster1_Death_Ground")
                return;

            Destroy(this.gameObject);
        }

        private void HandleOnHit(AttackPackage attack, AttackResult result)
        {
            if (_animator.GetCurrentAnimation().name == GhoulAnimationData.Anim.Death_Ground) return;

            _flash.Flash();
            _mover.Knockback(attack._fromDirection * attack._knockback);
            _state._hitPoints -= result._finalDamage;
            _state._endurance -= result._finalFatigue;

            if (_state._hitPoints < 0)
            {
                Dying();
            }

            if (_state._endurance <= 0)
            {
                _state._endurance.Current = 0;
                _animator.SetToggle(attack._staggerAnimation, true);
                _launcher.Interrupt();
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

            var aimInput = _state._frozen ? Vector2.zero : _input.GetAimingDirection();
            if (aimInput.x != 0)
            {
                _animator.FlipX = aimInput.x > 0;
                _state._facingRight = aimInput.x > 0;
            }

            var backward = _state._facingRight != aimInput.x > 0;
            _animator.SetBool("Backward", backward);
        }

        private void HandleReceivedInput(InputEventArg<EnemyInput> eventArgs)
        {
            if (dying) return;
            switch (eventArgs._command)
            {
                case EnemyInput.Attack:
                    _animator.SetToggle("Attack", true);
                    _launcher.LaunchDirection = _input.GetMovingDirection().x > 0 ? Vector2.right : Vector2.left;
                    _launcher.Launch();
                    break;
            }

            HandleEndurance();
        }

        private void Dying()
        {
            dying = true;
            _animator.SetBool("Death", true);
            _launcher.Interrupt();

            GetComponent<Rigidbody2D>().simulated = false;

            foreach (var handler in GetComponents<ICanHandleDeath>())
            {
                handler.OnDeath(_state);
            }
        }

        private void HandleEndurance()
        {
            if (_state._endurance < _state._enduranceSafeThreshlod)
            {
                _enduranceRefreshTimer += TimeManager.DeltaTime;
            }
            else
            {
                _enduranceRefreshTimer = 0;
            }

            if (_enduranceRefreshTimer >= _state._enduranceRefreshDelay)
            {
                _state._endurance.ResetCurrentValue();
            }

            if (_enduranceRecoverTimer < _state._enduranceRecoverDelay)
            {
                _enduranceRecoverTimer += TimeManager.DeltaTime;
            }
            else if (!_state._endurance.IsFull())
            {
                _state._endurance += _state._enduranceRecoverRate * TimeManager.DeltaTime;
            }
        }
    }
}
