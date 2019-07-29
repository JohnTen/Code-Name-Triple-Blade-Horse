using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleBladeHorse.Animation;
using TripleBladeHorse.Combat;
using TripleBladeHorse.Movement;
using TripleBladeHorse.AI;
using JTUtility;
namespace TripleBladeHorse
{
    public class ParticleBundleEnemy : MonoBehaviour
    {
        [System.Serializable]
        class StrParticlePair : PairedValue<string, ParticleSystem> { }

        [System.Serializable]
        class StrAudioPair : PairedValue<string, AudioClip> { }

        [SerializeField]
        private HitBox hitBox;
        [SerializeField]
        private EnemyState _state;

        [SerializeField] private AudioSource enemyAudioSource1;
        [SerializeField] private AudioSource enemyAudioSource2;
        [SerializeField] List<StrParticlePair> _particlePairs;
        [SerializeField] List<StrAudioPair> _audioPairs;
        [SerializeField] List<StrFloatPair> _volumePairs;
        [SerializeField] List<StrFloatPair> _pitchPairs;

        private Dictionary<string, ParticleSystem> _particles;
        private Dictionary<string, AudioClip> _audios;
        private Dictionary<string, float> _audiosVolume;
        private Dictionary<string, float> _pitch;


        ICharacterInput<EnemyInput> _input;
        private void Awake()
        {
            _state = GetComponent<EnemyState>();
            _input = GetComponent<ICharacterInput<EnemyInput>>();
            _input.OnReceivedInput += HandleReceivedInput;
            hitBox.OnHit += OnHitEventHandler;

            _particles = new Dictionary<string, ParticleSystem>();
            _audios = new Dictionary<string, AudioClip>();
            _audiosVolume = new Dictionary<string, float>();
            _pitch = new Dictionary<string, float>();

            foreach (var pair in _particlePairs)
            {
                _particles.Add(pair.Key, pair.Value);
            }

            foreach (var pair in _audioPairs)
            {
                _audios.Add(pair.Key, pair.Value);
            }

            foreach (var pair in _volumePairs)
            {
                _audiosVolume.Add(pair.Key, pair.Value);
            }

            foreach (var pitch in _pitchPairs)
            {
                _pitch.Add(pitch.Key, pitch.Value);
            }
        }


        private void OnHitEventHandler(AttackPackage atkPackage, AttackResult atkResult)
        {

            if(atkPackage._attackType == AttackType.Melee)
            {
                _particles["HittedByMelee"].Play();
            }
            if(atkPackage._attackType == AttackType.Range)
            {
                _particles["HittedByRange"].Play();
            }
            if (atkPackage._attackType == AttackType.ChargedMelee)
            {
                _particles["HittedByChargedMelee"].Play();
            }
            if (atkPackage._attackType == AttackType.ChargedRange)
            {
                _particles["HittedByChargedRange"].Play();
            }
            if (atkPackage._attackType == AttackType.Float)
            {
                _particles["HittedByFloatRange"].Play();
            }
            if(_state._hitPoints<0)
            {
                enemyAudioSource1.clip = _audios["Death"];
                enemyAudioSource1.volume = _audiosVolume["Death"];
                enemyAudioSource1.pitch = _pitch["Death"];
                enemyAudioSource1.Play();
            }
            else
            {
                enemyAudioSource1.clip = _audios["Hitted"];
                enemyAudioSource1.volume = _audiosVolume["Hitted"];
                enemyAudioSource1.pitch = _pitch["Hitted"];
                enemyAudioSource1.Play();

                if(atkPackage._attackType == AttackType.Melee || atkPackage._attackType == AttackType.ChargedMelee)
                {
                    enemyAudioSource2.clip = _audios["TakingDamageByMelee"];
                    enemyAudioSource2.volume = _audiosVolume["TakingDamageByMelee"];
                    enemyAudioSource2.pitch = _pitch["TakingDamageByMelee"];
                    enemyAudioSource2.Play();
                }

                if (atkPackage._attackType == AttackType.ChargedRange || atkPackage._attackType == AttackType.Range)
                {
                    enemyAudioSource2.clip = _audios["TakingDamageByRange"];
                    enemyAudioSource2.volume = _audiosVolume["TakingDamageByRange"];
                    enemyAudioSource2.pitch = _pitch["TakingDamageByRange"];
                    enemyAudioSource2.Play();
                }
            }
        }
        private void HandleReceivedInput(InputEventArg<EnemyInput> eventArgs)
        {
            if(eventArgs._command == EnemyInput.Attack)
            {
                enemyAudioSource1.clip = _audios["Attack"];
                enemyAudioSource1.volume = _audiosVolume["Attack"];
                enemyAudioSource1.pitch = _pitch["Attack"];
                enemyAudioSource1.Play();
            }
            if (eventArgs._command == EnemyInput.Alert)
            {
                enemyAudioSource1.clip = _audios["Alert"];
                enemyAudioSource1.volume = _audiosVolume["Alert"];
                enemyAudioSource1.pitch = _pitch["Alert"];
                enemyAudioSource1.Play();
            }
        }
    }
}

