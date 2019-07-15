using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleBladeHorse.Animation;
using TripleBladeHorse.Combat;
using TripleBladeHorse.Movement;
using TripleBladeHorse.AI;
namespace TripleBladeHorse
{
    public class ParticleBundleEnemy : MonoBehaviour
    {
        [SerializeField]
        private HitBox hitBox;
        [SerializeField]
        private EnemyState _state;
        [SerializeField]
        List<string> particleNames;
        [SerializeField]
        List<ParticleSystem> particleObjs;
        private Dictionary<string, ParticleSystem> particles =
                new Dictionary<string, ParticleSystem>();

        [SerializeField]
        List<string> audioNames;
        [SerializeField]
        List<AudioClip> audioClips;
        private Dictionary<string, AudioClip> audios =
                new Dictionary<string, AudioClip>();
        private AudioSource enemyAudioSource;
        private int i = 0;
        private int j = 0;
        ICharacterInput<EnemyInput> _input;
        private void Awake()
        {
            _state = GetComponent<EnemyState>();
            _input = GetComponent<ICharacterInput<EnemyInput>>();
            _input.OnReceivedInput += HandleReceivedInput;
            enemyAudioSource = GetComponent<AudioSource>();
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
            hitBox.OnHit += OnHitEventHandler;
        }


        private void OnHitEventHandler(AttackPackage atkPackage, AttackResult atkResult)
        {

            if(atkPackage._attackType == AttackType.Melee)
            {
                particles["HittedByMelee"].Play();
            }
            if(atkPackage._attackType == AttackType.Range)
            {
                particles["HittedByRange"].Play();
            }
            if (atkPackage._attackType == AttackType.ChargedMelee)
            {
                particles["HittedByChargedMelee"].Play();
            }
            if (atkPackage._attackType == AttackType.ChargedRange)
            {
                particles["HittedByChargedRange"].Play();
            }
            if (atkPackage._attackType == AttackType.Float)
            {
                particles["HittedByFloatRange"].Play();
            }
            if(_state._hitPoints<0)
            {
                enemyAudioSource.clip = audios["Death"];
                enemyAudioSource.Play();
            }
            else
            {
                enemyAudioSource.clip = audios["Hitted"];
                enemyAudioSource.Play();
            }
        }
        private void HandleReceivedInput(InputEventArg<EnemyInput> eventArgs)
        {
            if(eventArgs._command == EnemyInput.Attack)
            {
                enemyAudioSource.clip = audios["Attack"];
                enemyAudioSource.Play();
            }
            if (eventArgs._command == EnemyInput.Alert)
            {
                enemyAudioSource.clip = audios["Alert"];
                enemyAudioSource.Play();
            }
        }
    }
}

