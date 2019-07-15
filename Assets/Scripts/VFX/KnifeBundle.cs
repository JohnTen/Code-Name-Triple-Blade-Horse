using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TripleBladeHorse.Combat;

namespace TripleBladeHorse
{
    public class KnifeBundle : MonoBehaviour
    {
        [SerializeField] AudioSource knifeAudioSource;
        [SerializeField] ThrowingKnife[] knives;
        [SerializeField]
        List<string> audioNames;
        [SerializeField]
        List<AudioClip> audioClips;
        private Dictionary<string, AudioClip> audios =
                new Dictionary<string, AudioClip>();
        private int i = 0;
        private void Awake()
        {

            foreach(var knife in knives)
            {
                knife.OnStateChanged += HandleKnifeStateChange;
                knife.OnHit += HandleHitObjs;
            }
            foreach (var audioName in audioNames)
            {
                audios.Add(audioName, audioClips[i]);
                i++;
            }
        }

        private void Update()
        {

        }

        void HandleKnifeStateChange(ThrowingKnife knife, KnifeState previousState, KnifeState currentState)
        { 
            if(currentState == KnifeState.InSheath && previousState == KnifeState.Returning)
            {
                knifeAudioSource.clip = audios["Equip"];
                knifeAudioSource.Play();
            }
            if(currentState == KnifeState.Flying)
            {
                knifeAudioSource.clip = audios["Throw"];
                knifeAudioSource.Play();
            }
        }
        void HandleHitObjs(IAttackable attackable, AttackResult attackResult, AttackPackage attackPackage)
        {
            var component = attackable as Component;
            if (component == null || !attackResult._attackSuccess) return;

            var climbable = component.GetComponent<Climbable>();
            var hitbox = component.GetComponent<HitBox>();

            if(climbable!=null && hitbox!= null)
            {
                knifeAudioSource.clip = audios["HitMagicalElement"];
                knifeAudioSource.Play();
            }
            if (climbable != null && hitbox == null)
            {
                knifeAudioSource.clip = audios["HitWood"];
                knifeAudioSource.Play();
            }
            if (climbable == null && hitbox != null)
            {
                knifeAudioSource.clip = audios["HitEnemy"];
                knifeAudioSource.Play();
            }

        }
    }
}

