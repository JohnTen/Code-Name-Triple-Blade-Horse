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
        private Dictionary<string, ParticleSystem> particles;
        private FSM fSM;
        private PlayerMover playerMover;
        private ICanDetectGround groundDetect;
        private void Awake()
        {
            fSM = this.GetComponent<FSM>();
            playerMover = this.GetComponent<PlayerMover>();
            groundDetect = this.GetComponent<ICanDetectGround>();
            fSM.OnReceiveFrameEvent += FrameEventHandler;
            playerMover.OnMovingStateChanged += MoveStateChangeHandler;
            groundDetect.OnLandingStateChanged += HandleLanding;
        }

        private void FrameEventHandler(FrameEventEventArg eventArgs)
        {
            if (eventArgs._name == "AttackBegin")
            {
                if (fSM.GetCurrentAnimation().name == "ATK_Melee_Ground_1")
                {
                    particleObjs[0].Play();
                   //particles["ATK_Melee_Ground_1"].loop = false;
                }
                if (fSM.GetCurrentAnimation().name == "ATK_Melee_Ground_2")
                {
                    particleObjs[1].Play();
                    //particles["ATK_Melee_Ground_2"].loop = false;
                }
                if (fSM.GetCurrentAnimation().name == "ATK_Melee_Ground_3")
                {
                    particleObjs[0].Play();
                    //particles["ATK_Melee_Ground_2"].loop = false;
                }
            }
        }
        private void MoveStateChangeHandler(ICanChangeMoveState moveState, MovingEventArgs eventArgs)
        {
            if(eventArgs.currentMovingState == MovingState.Move
                &&fSM.GetCurrentAnimation().name == "Run_Ground_old")
            {
                particleObjs[2].Play();
            }
            else
            {
                particleObjs[2].Stop();
            }
            if(eventArgs.currentMovingState == MovingState.Airborne)
            {

                particleObjs[3].Play();
            }
        }
        private void HandleLanding(ICanDetectGround detector, LandingEventArgs eventArgs)
        {
            if (eventArgs.lastLandingState != eventArgs.currentLandingState &&
                eventArgs.currentLandingState == LandingState.OnGround)
            {
                particleObjs[4].Play();
            }

        }
    }

}
