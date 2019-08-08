using JTUtility;
using System.Collections.Generic;
using TripleBladeHorse.AI;
using TripleBladeHorse.Combat;
using UnityEngine;

namespace TripleBladeHorse.VFX
{
    public class ParticleBundleEnemy : MonoBehaviour
    {
        [System.Serializable]
        class StrParticlePair : PairedValue<string, ParticleSystem> { }

        [System.Serializable]
        class StrAudioPair : PairedValue<string, AudioClip> { }

        [SerializeField]
        private HitBox hitBox;

        [SerializeField] private AudioSource enemyAudioSource1;
        [SerializeField] private AudioSource enemyAudioSource2;
        [SerializeField] List<StrParticlePair> _particlePairs;
        [SerializeField] List<StrAudioPair> _audioPairs;
        [SerializeField] List<StrFloatPair> _volumePairs;
        [SerializeField] List<StrFloatPair> _pitchPairs;

        private Dictionary<string, ParticleSystem> _particles;
        private Dictionary<string, AudioClip> _audios;
        private Dictionary<string, float> _volume;
        private Dictionary<string, float> _pitch;
        private EnemyState _state;


        ICharacterInput<EnemyInput> _input;
        private void Awake()
        {
            _state = GetComponent<EnemyState>();
            _input = GetComponent<ICharacterInput<EnemyInput>>();
            _input.OnReceivedInput += HandleReceivedInput;
            hitBox.OnHit += OnHitEventHandler;

            _particles = new Dictionary<string, ParticleSystem>();
            _audios = new Dictionary<string, AudioClip>();
            _volume = new Dictionary<string, float>();
            _pitch = new Dictionary<string, float>();

            _audios.Add(_audioPairs);
            _pitch.Add(_pitchPairs);
            _volume.Add(_volumePairs);
            _particles.Add(_particlePairs);
        }


        private void OnHitEventHandler(AttackPackage atkPackage, AttackResult atkResult)
        {

            if (atkPackage._attackType == AttackType.Melee)
            {
                _particles["HittedByMelee"].Play();
            }
            if (atkPackage._attackType == AttackType.Range)
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
            if (_state._hitPoints < 0)
            {
                enemyAudioSource1.clip = _audios["Death"];
                enemyAudioSource1.volume = _volume["Death"];
                enemyAudioSource1.pitch = _pitch["Death"];
                enemyAudioSource1.Play();
            }
            else
            {
                enemyAudioSource1.clip = _audios["Hitted"];
                enemyAudioSource1.volume = _volume["Hitted"];
                enemyAudioSource1.pitch = _pitch["Hitted"];
                enemyAudioSource1.Play();

                if (atkPackage._attackType == AttackType.Melee || atkPackage._attackType == AttackType.ChargedMelee)
                {
                    enemyAudioSource2.clip = _audios["TakingDamageByMelee"];
                    enemyAudioSource2.volume = _volume["TakingDamageByMelee"];
                    enemyAudioSource2.pitch = _pitch["TakingDamageByMelee"];
                    enemyAudioSource2.Play();
                }

                if (atkPackage._attackType == AttackType.ChargedRange || atkPackage._attackType == AttackType.Range)
                {
                    enemyAudioSource2.clip = _audios["TakingDamageByRange"];
                    enemyAudioSource2.volume = _volume["TakingDamageByRange"];
                    enemyAudioSource2.pitch = _pitch["TakingDamageByRange"];
                    enemyAudioSource2.Play();
                }
            }
        }
        private void HandleReceivedInput(InputEventArg<EnemyInput> eventArgs)
        {
            if (eventArgs._command == EnemyInput.Attack)
            {
                enemyAudioSource1.clip = _audios["Attack"];
                enemyAudioSource1.volume = _volume["Attack"];
                enemyAudioSource1.pitch = _pitch["Attack"];
                enemyAudioSource1.Play();
            }
            if (eventArgs._command == EnemyInput.Alert)
            {
                enemyAudioSource1.clip = _audios["Alert"];
                enemyAudioSource1.volume = _volume["Alert"];
                enemyAudioSource1.pitch = _pitch["Alert"];
                enemyAudioSource1.Play();
            }
        }
    }
}

