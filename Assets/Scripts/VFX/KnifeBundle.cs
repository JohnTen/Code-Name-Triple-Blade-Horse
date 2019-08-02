using JTUtility;
using System.Collections.Generic;
using TripleBladeHorse.Combat;
using UnityEngine;

namespace TripleBladeHorse.VFX
{
    public class KnifeBundle : MonoBehaviour
    {
        [System.Serializable]
        class StrAudioPair : PairedValue<string, AudioClip> { }

        [SerializeField] AudioSource knifeAudioSource;
        [SerializeField] List<StrAudioPair> _audioPairs;
        [SerializeField] List<StrFloatPair> _volumePairs;
        [SerializeField] List<StrFloatPair> _pitchPairs;

        [SerializeField] ThrowingKnife[] _knives;

        private Dictionary<string, AudioClip> _audios;
        private Dictionary<string, float> _volume;
        private Dictionary<string, float> _pitch;

        private void Awake()
        {
            _pitch = new Dictionary<string, float>();
            _audios = new Dictionary<string, AudioClip>();
            _volume = new Dictionary<string, float>();

            _audios.Add(_audioPairs);
            _volume.Add(_volumePairs);
            _pitch.Add(_pitchPairs);

            foreach (var knife in _knives)
            {
                knife.OnHit += HandleHitObjs;
                knife.OnStateChanged += HandleKnifeStateChange;
            }
        }

        void HandleKnifeStateChange(ThrowingKnife knife, KnifeState previousState, KnifeState currentState)
        {
            if (currentState == KnifeState.InSheath && previousState == KnifeState.Returning)
            {
                knifeAudioSource.clip = _audios["Equip"];
                knifeAudioSource.volume = _volume["Equip"];
                knifeAudioSource.pitch = _pitch["Equip"];
                knifeAudioSource.Play();
            }
            if (currentState == KnifeState.Flying)
            {
                knifeAudioSource.clip = _audios["Throw"];
                knifeAudioSource.volume = _volume["Throw"];
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

            if (climbable != null && hitbox != null)
            {
                knifeAudioSource.clip = _audios["HitMagicalElement"];
                knifeAudioSource.volume = _volume["HitMagicalElement"];
                knifeAudioSource.pitch = _pitch["HitMagicalElement"];
                knifeAudioSource.Play();
            }
            if (climbable != null && hitbox == null)
            {
                knifeAudioSource.clip = _audios["HitWood"];
                knifeAudioSource.volume = _volume["HitWood"];
                knifeAudioSource.pitch = _pitch["HitWood"];
                knifeAudioSource.Play();
            }
            if (climbable == null && hitbox != null)
            {
                knifeAudioSource.clip = _audios["HitEnemy"];
                knifeAudioSource.volume = _volume["HitEnemy"];
                knifeAudioSource.pitch = _pitch["HitEnemy"];
                knifeAudioSource.Play();
            }

        }
    }
}

