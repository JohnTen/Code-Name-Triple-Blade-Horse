using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleBladeHorse.Animation;
using TripleBladeHorse.Combat;
namespace TripleBladeHorse
{
    public class ParticleBundleEnemy : MonoBehaviour
    {
        [SerializeField]
        private HitBox hitBox;
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
        private void Awake()
        {
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
            enemyAudioSource.clip = audios["Hitted"];
            enemyAudioSource.Play();
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
        }
    }
}

