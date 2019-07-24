using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TripleBladeHorse.Combat;
using JTUtility;
namespace TripleBladeHorse
{
    public class KnifeBundle : MonoBehaviour
    {

        [System.Serializable]
        class StrAudioPair : PairedValue<string, AudioClip> { }

        [SerializeField] AudioSource knifeAudioSource;
        [SerializeField] List<StrAudioPair> _audioPairs;
        [SerializeField] List<StrFloatPair> _volumePairs;
        [SerializeField] List<StrFloatPair> _pitchPairs;

        private Dictionary<string, AudioClip> _audios;
        private Dictionary<string, float> _audiosVolume;
        private Dictionary<string, float> _pitch;

        [SerializeField] ThrowingKnife[] knives;

        private void Awake()
        {
			_pitch = new Dictionary<string, float>();
			_audios = new Dictionary<string, AudioClip>();
            _audiosVolume = new Dictionary<string, float>();

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

        private void Update()
        {

        }

        void HandleKnifeStateChange(ThrowingKnife knife, KnifeState previousState, KnifeState currentState)
        { 
            if(currentState == KnifeState.InSheath && previousState == KnifeState.Returning)
            {
                knifeAudioSource.clip = _audios["Equip"];
                knifeAudioSource.volume = _audiosVolume["Equip"];
                knifeAudioSource.pitch = _pitch["Equip"];
                knifeAudioSource.Play();
            }
            if(currentState == KnifeState.Flying)
            {
                knifeAudioSource.clip = _audios["Throw"];
                knifeAudioSource.volume = _audiosVolume["Throw"];
                knifeAudioSource.pitch = _pitch["Throw"];
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
                knifeAudioSource.clip = _audios["HitMagicalElement"];
                knifeAudioSource.volume = _audiosVolume["HitMagicalElement"];
                knifeAudioSource.pitch = _pitch["HitMagicalElement"];
                knifeAudioSource.Play();
            }
            if (climbable != null && hitbox == null)
            {
                knifeAudioSource.clip = _audios["HitWood"];
                knifeAudioSource.volume = _audiosVolume["HitWood"];
                knifeAudioSource.pitch = _pitch["HitWood"];
                knifeAudioSource.Play();
            }
            if (climbable == null && hitbox != null)
            {
                knifeAudioSource.clip = _audios["HitEnemy"];
                knifeAudioSource.volume = _audiosVolume["HitEnemy"];
                knifeAudioSource.pitch = _pitch["HitEnemy"];
                knifeAudioSource.Play();
            }

        }
    }
}

