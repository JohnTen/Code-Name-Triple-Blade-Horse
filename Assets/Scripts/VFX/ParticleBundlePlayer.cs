using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleBladeHorse.Animation;
using TripleBladeHorse.Movement;
using TripleBladeHorse.Combat;

namespace TripleBladeHorse
{
    public class ParticleBundlePlayer : MonoBehaviour, IDeathHandler
    {
        [SerializeField]
        private HitBox hitBox;
        [SerializeField]
        GameObject sheath;
        [SerializeField] AudioSource playerAudioSource;
        [SerializeField]
        List<string> particleNames;
        [SerializeField]
        List<ParticleSystem> particleObjs;
        private Dictionary<string, ParticleSystem> particles = 
                new Dictionary<string,ParticleSystem>();
        [SerializeField]
        List<string> audioNames;
        [SerializeField]
        List<AudioClip> audioClips;
        private Dictionary<string, AudioClip> audios =
                        new Dictionary<string, AudioClip>();
        private FSM fSM;
        private PlayerMover playerMover;
        private ICharacterInput<PlayerInputCommand> characterInput;
        private ICanDetectGround groundDetect;
        private IAttackable _hitBox;
        private int i = 0;
        private int j = 0;
        private void Start()
        {
            fSM = this.GetComponent<FSM>();
            playerMover = this.GetComponent<PlayerMover>();
            groundDetect = this.GetComponent<ICanDetectGround>();
            characterInput = this.GetComponent<ICharacterInput<PlayerInputCommand>>();
            _hitBox = GetComponentInChildren<IAttackable>();
            fSM.OnReceiveFrameEvent += FrameEventHandler;
            playerMover.OnMovingStateChanged += MoveStateChangeHandler;
            groundDetect.OnLandingStateChanged += HandleLanding;
            characterInput.OnReceivedInput += HandleChargingATK;
            TimeManager.Instance.OnBulletTimeBegin += TimeManagerHandler;
            _hitBox.OnHit += OnHittedHandler;
            foreach (var particleName in particleNames)
            {
                particles.Add(particleName, particleObjs[i]);
                i++;
            }
            foreach(var audioName in audioNames)
            {
                audios.Add(audioName, audioClips[j]);
                j++;
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
                if (fSM.GetCurrentAnimation().name == PlayerFSMData.Anim.ATK_Melee_Ground_1)
                {
                    //particleObjs[0].Play();
                    particles["ATK_Melee_Ground_1"].Play();
                }
                if (fSM.GetCurrentAnimation().name == PlayerFSMData.Anim.ATK_Melee_Ground_2)
                {
                    particles["ATK_Melee_Ground_2"].Play();
                }
                if (fSM.GetCurrentAnimation().name == PlayerFSMData.Anim.ATK_Melee_Ground_3)
                {
                    particles["ATK_Melee_Ground_1"].Play();
                }
               
            }
            if(eventArgs._name == AnimEventNames.Regenerate)
            {
                particles["RegenerateCompleted"].Play();
                particles["Regenerate"].Stop();

            }
         
        }
        private void MoveStateChangeHandler(ICanChangeMoveState moveState, MovingEventArgs eventArgs)
        {
            if(eventArgs.currentMovingState == MovingState.Move
                &&fSM.GetCurrentAnimation().name == "Run_Ground_old"|| fSM.GetCurrentAnimation().name == "Run_Ground_New")
            {
                particles["Run_Ground"].Play();
            }
            else
            {
                particles["Run_Ground"].Stop();
            }
            if(eventArgs.currentMovingState == MovingState.Airborne)
            {
                particles["Jump_In_Air"].Play();
                playerAudioSource.clip = audios["Jump"];
                playerAudioSource.Play();
            }
            if(eventArgs.currentMovingState == MovingState.Dash)
            {
                playerAudioSource.clip = audios["Dash"];
                playerAudioSource.Play();
            }
        }
        private void HandleLanding(ICanDetectGround detector, LandingEventArgs eventArgs)
        {
            if (eventArgs.lastLandingState != eventArgs.currentLandingState &&
                eventArgs.currentLandingState == LandingState.OnGround)
            {
                particles["Land_In_Ground"].Play();
            }

        }

        private void HandleChargingATK(InputEventArg<PlayerInputCommand> eventArg)
        {
            if (eventArg._command == PlayerInputCommand.MeleeChargeBegin)
            {
                particles["ATK_Charge_Ground_Charging"].Play();
            }
            if (eventArg._command == PlayerInputCommand.MeleeChargeAttack)
            {
                particles["ATK_Charge_Ground_Charging"].Stop();
                particles["ATK_Charge_Ground_ATK"].Play();
            }
            if(eventArg._command == PlayerInputCommand.Regenerate)
            {
                particles["Regenerate"].Play();
                playerAudioSource.clip = audios["Regenerate"];
                playerAudioSource.Play();
            }
        }
        private void TimeManagerHandler()
        {
            particles["BulletTime"].Play();
        }

        public void HandleDeath(CharacterState state)
        {
            playerAudioSource.clip = audios["Death"];
            playerAudioSource.Play();
        }

        private void OnHittedHandler(AttackPackage attack, AttackResult result)
        {
            playerAudioSource.clip = audios["Pain"];
            playerAudioSource.Play();
        }
    }

}
