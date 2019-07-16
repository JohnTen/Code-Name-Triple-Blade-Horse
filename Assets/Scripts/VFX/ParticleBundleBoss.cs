using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleBladeHorse.Animation;
using TripleBladeHorse.Combat;
using TripleBladeHorse.Movement;
using TripleBladeHorse.AI;
namespace TripleBladeHorse
{
    public class ParticleBundleBoss : MonoBehaviour
    {
        [SerializeField]
        List<string> audioNames;
        [SerializeField]
        List<float> volumes;
        [SerializeField]
        List<AudioClip> audioClips;
        private Dictionary<string, AudioClip> audios =
                new Dictionary<string, AudioClip>();
        private Dictionary<string, float> audiosVolume =
                new Dictionary<string,float>();
        [SerializeField]
        HitBox[] _hitboxes;
        CharacterState _state;

        private AudioSource bossAudioSource;
        private int j = 0;
        private int k = 0;
        FSM _animator;

        private void Awake()
        {
            _animator = GetComponent<FSM>();
            bossAudioSource = GetComponent<AudioSource>();
            _state = GetComponent<CharacterState>();
            foreach (var hitbox in _hitboxes)
            {
                hitbox.OnHit += HandleOnHit;
            }
            foreach (var audioName in audioNames)
            {
                audios.Add(audioName, audioClips[j]);
                j++;
            }
            foreach (var audioName in audioNames)
            {
                audiosVolume.Add(audioName, volumes[k]);
                k++;
            }
            _animator.OnReceiveFrameEvent += HandleFrameEvent;
            _animator.Subscribe(Animation.AnimationState.FadingIn, HandleFadeInAnimation);
        }

        private void HandleFrameEvent(FrameEventEventArg eventArg)
        {
            if(eventArg._name == AnimEventNames.AttackBegin)
            {
                if (eventArg._animation.name == BossFSMData.Anim.Slash1)
                {
                    bossAudioSource.clip = audios["Combo1"];
                    bossAudioSource.volume = audiosVolume["Combo1"];
                    bossAudioSource.Play();
                }
            }
        }
        private void HandleOnHit(AttackPackage attack, AttackResult result)
        {

            if (_state._hitPoints<=0)
            {
                bossAudioSource.clip = audios["Death"];
                bossAudioSource.volume = audiosVolume["Death"];

                bossAudioSource.Play();
            }
        }
        private void HandleFadeInAnimation(AnimationEventArg eventArgs)
        {
            if(eventArgs._animation.name == BossFSMData.Anim.Combo2_1)
            {
                bossAudioSource.clip = audios["Combo2"];
                bossAudioSource.volume = audiosVolume["Combo2"];
                bossAudioSource.Play();
            }
            if (eventArgs._animation.name == BossFSMData.Anim.Combo3_1)
            {
                bossAudioSource.clip = audios["Combo3"];
                bossAudioSource.volume = audiosVolume["Combo3"];
                bossAudioSource.Play();
            }
        }
    }
}


