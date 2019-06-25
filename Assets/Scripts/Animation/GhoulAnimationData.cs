using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse.Animation
{
	[CreateAssetMenu(menuName = "AnimationFSMData/GhoulFSMData")]
	public class GhoulAnimationData : FSMData
	{
		class Anim
		{
			public const string Idle_Ground = "Idle_Ground";
			public const string Walk_Ground = "Walk_Ground";
			public const string ATK_Ground = "ATK_Ground";
			public const string Death_Ground = "Death_Ground";
		}

		class Stat
		{
			public const string Attack = "Attack";
			public const string Stagger = "Stagger";
			public const string Death = "Death";
			public const string Frozen = "Frozen";
			public const string DelayInput = "DelayInput";
			public const string BlockInput = "BlockInput";
			public const string XSpeed = "XSpeed";
		}

		public GhoulAnimationData()
		{
			InitalizeStates();
			InitalizeAnimations();
			InitalizeTransitions();
			_defaultAnimation = _animationDatas[0];
		}

		public override void InitalizeAnimations()
		{
			_animationDatas = new List<Animation>()
			{
				new Animation(Anim.Idle_Ground, 1, 0, 0),
				new Animation(Anim.Walk_Ground, 1, 0, 0),
				new Animation(Anim.ATK_Ground, 1, 1, 1),
				new Animation(Anim.Death_Ground, 1, 1, 1),
			};
		}

		public override void InitalizeStates()
		{
			_toggleState = new List<StrBoolPair>()
			{
				new StrBoolPair() {Key = Stat.Attack, Value = false},
				new StrBoolPair() {Key = Stat.Stagger, Value = false},
			};

			_boolState = new List<StrBoolPair>()
			{
				new StrBoolPair() {Key = Stat.Death, Value = false},
				new StrBoolPair() {Key = Stat.Frozen, Value = false},
				new StrBoolPair() {Key = Stat.DelayInput, Value = false},
				new StrBoolPair() {Key = Stat.BlockInput, Value = false},
			};

			_floatState = new List<StrFloatPair>()
			{
				new StrFloatPair() {Key = Stat.XSpeed, Value = 0},
			};

			_intState = new List<StrIntPair>();
		}

		public override void InitalizeTransitions()
		{
			_transitions = new List<Transition>()
			{
				//// Hitten_Ground
				//// Idle_Ground
				new Transition(
					Anim.Idle_Ground, Anim.Death_Ground, 0.05f,
					(sd) => {
						return sd._boolMap[Stat.Death];
					}),

				new Transition(
					Anim.Idle_Ground, Anim.ATK_Ground, 0.2f,
					(sd) => {
						return sd._toggleMap[Stat.Attack];
					}),

				new Transition(
					Anim.Idle_Ground, Anim.Walk_Ground, 0.1f,
					(sd) => {
						return Mathf.Abs(sd._floatMap[Stat.XSpeed]) > float.Epsilon;
					}),

				//// Run_Ground
				new Transition(
					Anim.Walk_Ground, Anim.Death_Ground, 0.05f,
					(sd) => {
						return sd._boolMap[Stat.Death];
					}),

				new Transition(
					Anim.Walk_Ground, Anim.ATK_Ground, 0.2f,
					(sd) => {
						return sd._toggleMap[Stat.Attack];
					}),

				new Transition(
					Anim.Walk_Ground, Anim.Idle_Ground, 0.1f,
					(sd) => {
						return Mathf.Abs(sd._floatMap[Stat.XSpeed]) <= float.Epsilon;
					}),

				//// ATK_Ground
				new Transition(
					Anim.ATK_Ground, Anim.Death_Ground, 0.05f,
					(sd) => {
						return sd._boolMap[Stat.Death];
					}),
				new Transition(
					Anim.ATK_Ground, Anim.Idle_Ground, 0.05f,
					(sd) => {
						return sd._current.completed;
					}),
			};
		}
	}
}
