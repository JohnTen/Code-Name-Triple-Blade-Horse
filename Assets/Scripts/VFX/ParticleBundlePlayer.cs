using JTUtility;
using System.Collections.Generic;
using TripleBladeHorse.Animation;
using TripleBladeHorse.Combat;
using TripleBladeHorse.Movement;
using UnityEngine;

namespace TripleBladeHorse.VFX
{
    public class ParticleBundlePlayer : MonoBehaviour, IDeathHandler
    {
        [System.Serializable]
        class StrParticlePair : PairedValue<string, ParticleSystem> { }

        [System.Serializable]
        class StrAudioPair : PairedValue<string, AudioClip> { }


        [SerializeField] AudioSource _playerAudioSource;
        [SerializeField] List<StrParticlePair> _particlePairs;
        [SerializeField] List<StrAudioPair> _audioPairs;
        [SerializeField] List<StrFloatPair> _volumePairs;
        [SerializeField] List<StrFloatPair> _pitchPairs;
        [SerializeField] float _footstepFreq = 2;

        private Dictionary<string, ParticleSystem> _particles;
        private Dictionary<string, AudioClip> _audios;
        private Dictionary<string, float> _volume;
        private Dictionary<string, float> _pitch;

        private FSM _fSM;
        private PlayerMover _playerMover;
        private ICharacterInput<PlayerInputCommand> _characterInput;
        private ICanDetectGround _groundDetect;
        private IAttackable _hitBox;

        private bool _repeatFootstepSFX;
        private float _footstepSFXTimer;

        private void Start()
        {
            _fSM = this.GetComponent<FSM>();
            _playerMover = this.GetComponent<PlayerMover>();
            _groundDetect = this.GetComponent<ICanDetectGround>();
            _characterInput = this.GetComponent<ICharacterInput<PlayerInputCommand>>();
            _hitBox = GetComponentInChildren<IAttackable>();
            _fSM.OnReceiveFrameEvent += FrameEventHandler;
            _playerMover.OnMovingStateChanged += MoveStateChangeHandler;
            _groundDetect.OnLandingStateChanged += HandleLanding;
            _characterInput.OnReceivedInput += HandleChargingATK;
            TimeManager.Instance.OnBulletTimeBegin += TimeManagerHandler;
            _hitBox.OnHit += OnHittedHandler;
            _fSM.Subscribe(Animation.AnimationState.FadingIn, HandleAnimationFadeInEvent);

            _particles = new Dictionary<string, ParticleSystem>();
            _audios = new Dictionary<string, AudioClip>();
            _volume = new Dictionary<string, float>();
            _pitch = new Dictionary<string, float>();

            _particles.Add(_particlePairs);
            _audios.Add(_audioPairs);
            _volume.Add(_volumePairs);
            _pitch.Add(_pitchPairs);
        }

        private void Update()
        {
            if (_repeatFootstepSFX && TimeManager.Time > _footstepSFXTimer)
            {
                int i = Random.Range(0, 6);
                string j = i.ToString();
                _playerAudioSource.clip = _audios["Run_Ground" + j];
                _playerAudioSource.volume = _volume["Run_Ground" + j];
                _playerAudioSource.pitch = _pitch["Run_Ground" + j];
                _playerAudioSource.Play();
                _footstepSFXTimer = TimeManager.Time + 1 / _footstepFreq;
            }
        }

        private void OnDestroy()
        {
            TimeManager.Instance.OnBulletTimeBegin -= TimeManagerHandler;
        }

        private void FrameEventHandler(FrameEventEventArg eventArgs)
        {
            if (eventArgs._name == AnimEventNames.AttackBegin)
            {
                if (_fSM.GetCurrentAnimation().name == PlayerFSMData.Anim.ATK_Melee_Ground_1)
                {
                    _particles["ATK_Melee_Ground_1"].Play();
                }
                if (_fSM.GetCurrentAnimation().name == PlayerFSMData.Anim.ATK_Melee_Ground_2)
                {
                    _particles["ATK_Melee_Ground_2"].Play();
                }
                if (_fSM.GetCurrentAnimation().name == PlayerFSMData.Anim.ATK_Melee_Ground_3)
                {
                    _particles["ATK_Melee_Ground_1"].Play();
                }

                if (_fSM.GetCurrentAnimation().name == PlayerFSMData.Anim.ATK_Charge_Ground_ATK)
                {
                    _particles["ATK_Charge_Ground_Charging"].Stop();
                    _particles["ATK_Charge_Ground_ATK"].Play();
                }
            }

            if (eventArgs._name == AnimEventNames.Regenerate)
            {
                _particles["RegenerateCompleted"].Play();
                _particles["Regenerate"].Stop();
            }


        }

        private void HandleAnimationFadeInEvent(AnimationEventArg eventArgs)
        {
            if (eventArgs._animation.name == PlayerFSMData.Anim.Healing)
            {
                _particles["Regenerate"].Play();
                _playerAudioSource.clip = _audios["Regenerate"];
                _playerAudioSource.volume = _volume["Regenerate"];
                _playerAudioSource.pitch = _pitch["Regenerate"];
                _playerAudioSource.Play();
            }
        }

        private void MoveStateChangeHandler(ICanChangeMoveState moveState, MovingEventArgs eventArgs)
        {
            if (eventArgs.currentMovingState == MovingState.Move
                && _fSM.GetCurrentAnimation().name == PlayerFSMData.Anim.Run_Ground)
            {
                _particles["Run_Ground"].Play();
                _repeatFootstepSFX = true;
            }
            else
            {
                _particles["Run_Ground"].Stop();
                _repeatFootstepSFX = false;
            }
            if (eventArgs.currentMovingState == MovingState.Airborne)
            {
                _particles["Jump_In_Air"].Play();
                int i = Random.Range(0, 4);
                string j = i.ToString();
                _playerAudioSource.clip = _audios["Jump" + j];
                _playerAudioSource.volume = _volume["Jump" + j];
                _playerAudioSource.pitch = _pitch["Jump" + j];
                _playerAudioSource.Play();
            }
            if (eventArgs.currentMovingState == MovingState.Dash)
            {
                _playerAudioSource.clip = _audios["Dash"];
                _playerAudioSource.volume = _volume["Dash"];
                _playerAudioSource.pitch = _pitch["Dash"];
                _playerAudioSource.Play();
            }
        }

        private void HandleLanding(ICanDetectGround detector, LandingEventArgs eventArgs)
        {
            if (eventArgs.lastLandingState != eventArgs.currentLandingState &&
                eventArgs.currentLandingState == LandingState.OnGround)
            {
                _particles["Land_In_Ground"].Play();
            }
        }

        private void HandleChargingATK(InputEventArg<PlayerInputCommand> eventArg)
        {
            if (eventArg._command == PlayerInputCommand.RangeChargeAttack)
            {
                _particles["Range_Charge_Ground_Charging"].Stop();
                _particles["Range_Charge_Ground_ATK"].Play();
            }
            if (eventArg._command == PlayerInputCommand.RangeChargeBegin)
            {
                _particles["Range_Charge_Ground_Charging"].Play();
            }
            if (eventArg._command == PlayerInputCommand.RangeChargeBreak)
            {
                _particles["Range_Charge_Ground_Charging"].Stop();
                _particles["Range_Charge_Ground_ATK"].Stop();
            }
        }

        private void TimeManagerHandler()
        {
            _particles["BulletTime"].Play();
        }

        public void HandleDeath(CharacterState state)
        {
            _playerAudioSource.clip = _audios["Death"];
            _playerAudioSource.volume = _volume["Death"];
            _playerAudioSource.pitch = _pitch["Death"];
            _playerAudioSource.Play();
        }

        private void OnHittedHandler(AttackPackage attack, AttackResult result)
        {
            _playerAudioSource.clip = _audios["Pain"];
            _playerAudioSource.volume = _volume["Pain"];
            _playerAudioSource.pitch = _pitch["Pain"];
            _playerAudioSource.Play();
        }

    }

}
