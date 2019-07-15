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

        private int i = 0;
        private void Awake()
        {
            foreach (var particleName in particleNames)
            {
                particles.Add(particleName, particleObjs[i]);
                i++;
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
        }
    }
}

