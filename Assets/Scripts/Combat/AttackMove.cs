using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleBladeHorse.VFX;

namespace TripleBladeHorse.Combat
{
	[CreateAssetMenu]
	public class AttackMove : ScriptableObject, ICanProcess<AttackPackage>
	{
		[SerializeField] float hitPointDamageFactor;
		[SerializeField] float enduranceDamageFactor;
		[SerializeField] ParticleSystem.MinMaxCurve damageCurve;
		[SerializeField] float staminaCost;
		[SerializeField] float movement;
		[SerializeField] float knockbackDistance;

		[SerializeField] float cancelRange;
		[SerializeField] bool canTriggerGapStagger;
		[SerializeField] string gapStaggerAnimation;
		[SerializeField] string staggerAnimation;

		[Header("Responsive effects")]
		[SerializeField] float frameFrozenDuration;
		[SerializeField] ScreenShakeParams screenShakeParam;

		public float HitPointDamageFactor => hitPointDamageFactor;
		public float EnduranceDamageFactor => enduranceDamageFactor;
		public ParticleSystem.MinMaxCurve DamageCurve => damageCurve;
		public float StaminaCost => staminaCost;
		public float Movement => movement;
		public float Knockback => knockbackDistance;
		public float CancelRange => cancelRange;
		public bool CanTriggerGapStagger => canTriggerGapStagger;
		public string GapStaggerAnimation => gapStaggerAnimation;
		public string StaggerAnimation => staggerAnimation;

		public float FrameFrozen => frameFrozenDuration;
		public ScreenShakeParams ScreenShakeParam => screenShakeParam;


		public AttackPackage Process(AttackPackage target)
		{
			target._hitPointDamage *= hitPointDamageFactor * damageCurve.Evaluate(target._chargedPercent);
			target._enduranceDamage *= enduranceDamageFactor;
			target._knockback.Base += knockbackDistance;
			target._triggerGapStagger = canTriggerGapStagger;
			target._gapStaggerAnimation = gapStaggerAnimation;
			target._staggerAnimation = staggerAnimation;
			target._move = this;

			return target;
		}
	}
}