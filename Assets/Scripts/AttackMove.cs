using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AttackMove : ScriptableObject
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

}
