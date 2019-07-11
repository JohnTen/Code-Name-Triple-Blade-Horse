using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleBladeHorse.Animation;
using TripleBladeHorse.Movement;

namespace TripleBladeHorse
{
    public class ParticleBundle : MonoBehaviour
    {
        [SerializeField]
        List<string> particleNames;
        [SerializeField]
        List<ParticleSystem> particleObjs;
        private Dictionary<string, ParticleSystem> particles = 
                new Dictionary<string,ParticleSystem>();
        private FSM fSM;
        private PlayerMover playerMover;
        private ICanDetectGround groundDetect;
        private int i = 0;
        private void Start()
        {
            fSM = this.GetComponent<FSM>();
            playerMover = this.GetComponent<PlayerMover>();
            groundDetect = this.GetComponent<ICanDetectGround>();
            fSM.OnReceiveFrameEvent += FrameEventHandler;
            playerMover.OnMovingStateChanged += MoveStateChangeHandler;
            groundDetect.OnLandingStateChanged += HandleLanding;
            foreach(var particleName in particleNames)
            {
                particles.Add(particleName, particleObjs[i]);
                i++;
            }
        }

        private void FrameEventHandler(FrameEventEventArg eventArgs)
        {
            if (eventArgs._name == "AttackBegin")
            {
                if (fSM.GetCurrentAnimation().name == "ATK_Melee_Ground_1")
                {
                    //particleObjs[0].Play();
                    particles["ATK_Melee_Ground_1"].Play();
                }
                if (fSM.GetCurrentAnimation().name == "ATK_Melee_Ground_2")
                {
                    particles["ATK_Melee_Ground_2"].Play();
                }
                if (fSM.GetCurrentAnimation().name == "ATK_Melee_Ground_3")
                {
                    particles["ATK_Melee_Ground_1"].Play();
                }
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
    }

}
