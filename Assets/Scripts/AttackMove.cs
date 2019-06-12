using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AttackMove : ScriptableObject, ICanProcess<AttackPackage>
{
	[SerializeField] float hitPointDamageFactor;
	[SerializeField] float enduranceDamageFactor;
	[SerializeField] AnimationCurve damageCurve;
	[SerializeField] float staminaCost;
	[SerializeField] float movement;
	[SerializeField] float knockbackDistance;

	[SerializeField] float cancelRange;
	[SerializeField] bool canTriggerGapStagger;
	[SerializeField] string gapStaggerAnimation;
	[SerializeField] string staggerAnimation;

	public float HitPointDamageFactor => hitPointDamageFactor;
	public float EnduranceDamageFactor => enduranceDamageFactor;
	public AnimationCurve DamageCurve => damageCurve;
	public float StaminaCost => staminaCost;
	public float Movement => movement;
	public float Knockback => knockbackDistance;
	public float CancelRange => cancelRange;
	public bool CanTriggerGapStagger => canTriggerGapStagger;
	public string GapStaggerAnimation => gapStaggerAnimation;
	public string StaggerAnimation => staggerAnimation;


	public AttackPackage Process(AttackPackage target)
	{
		target._hitPointDamage *= hitPointDamageFactor;
		target._enduranceDamage *= enduranceDamageFactor;
		target._knockback += knockbackDistance;

		return target;
	}
}
