using JTUtility;
using System.Collections.Generic;
using TripleBladeHorse.Animation;
using TripleBladeHorse.Combat;
using UnityEngine;

namespace TripleBladeHorse.VFX
{
    public class ParticleBundleBoss : MonoBehaviour
    {

        [System.Serializable]
        class StrAudioPair : PairedValue<string, AudioClip> { }

        [SerializeField] private AudioSource bossAudioSource;
        [SerializeField] List<StrAudioPair> _audioPairs;
        [SerializeField] List<StrFloatPair> _volumePairs;
        [SerializeField] List<StrFloatPair> _pitchPairs;

        private Dictionary<string, AudioClip> _audios;
        private Dictionary<string, float> _audiosVolume;
        private Dictionary<string, float> _pitch;

        FSM _animator;
        [SerializeField] HitBox[] _hitboxes;
        CharacterState _state;

        private void Awake()
        {
            _animator = GetComponent<FSM>();
            _state = GetComponent<CharacterState>();
            _animator.OnReceiveFrameEvent += HandleFrameEvent;
            _animator.Subscribe(Animation.AnimationState.FadingIn, HandleFadeInAnimation);

            bossAudioSource = GetComponent<AudioSource>();
            _audiosVolume = new Dictionary<string, float>();
            _audios = new Dictionary<string, AudioClip>();
            _pitch = new Dictionary<string, float>();

            _audios.Add(_audioPairs);
            _pitch.Add(_pitchPairs);
            _audiosVolume.Add(_volumePairs);
        }

        private void HandleFrameEvent(FrameEventEventArg eventArg)
        {
            if (eventArg._name == AnimEventNames.AttackBegin)
            {
                if (eventArg._animation.name == BossFSMData.Anim.Slash1)
                {
                    bossAudioSource.clip = _audios["Combo1"];
                    bossAudioSource.volume = _audiosVolume["Combo1"];
                    bossAudioSource.pitch = _pitch["Combo1"];
                    bossAudioSource.Play();
                }
            }
        }
        private void HandleOnHit(AttackPackage attack, AttackResult result)
        {

            if (_state._hitPoints <= 0)
            {
                bossAudioSource.clip = _audios["Death"];
                bossAudioSource.volume = _audiosVolume["Death"];
                bossAudioSource.pitch = _pitch["Death"];
                bossAudioSource.Play();
            }
        }
        private void HandleFadeInAnimation(AnimationEventArg eventArgs)
        {
            if (eventArgs._animation.name == BossFSMData.Anim.Combo2_1)
            {
                bossAudioSource.clip = _audios["Combo2"];
                bossAudioSource.volume = _audiosVolume["Combo2"];
                bossAudioSource.pitch = _pitch["Combo2"];
                bossAudioSource.Play();
            }
            if (eventArgs._animation.name == BossFSMData.Anim.Combo3_1)
            {
                bossAudioSource.clip = _audios["Combo3"];
                bossAudioSource.volume = _audiosVolume["Combo3"];
                bossAudioSource.pitch = _pitch["Combo3"];
                bossAudioSource.Play();
            }
        }
    }
}


