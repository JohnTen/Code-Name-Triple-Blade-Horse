using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse.Animation
{
	[CreateAssetMenu(menuName = "AnimationFSMData/PlayerFSMData")]
	public class PlayerAnimationData : FSMData
	{
		class Anim
		{
			public const string Idle_Ground = "Idle_Ground_New";
			public const string Run_Ground = "Run_Ground";
			public const string Dash_Ground = "Dash_Ground";
			public const string Jump_Ground = "Jump_Ground";
			public const string Jump_Air = "Jump_Air";
			public const string Droping = "Droping";
			public const string Droping_Buffering = "Droping_Buffering";
			public const string ATK_Melee_Ground_1 = "ATK_Melee_Ground_1";
			public const string ATK_Melee_Ground_2 = "ATK_Melee_Ground_2";
			public const string ATK_Melee_Ground_3 = "ATK_Melee_Ground_3";
			public const string Hitten_Ground = "Hitten_Ground";
		}
		
		class Stat
		{
			public const string Jump = "Jump";
			public const string MeleeAttack = "MeleeAttack";
			public const string Stagger = "Stagger";
			public const string Dash = "Dash";
			public const string Airborne = "Airborne";
			public const string Frozen = "Frozen";
			public const string DelayInput = "DelayInput";
			public const string BlockInput = "BlockInput";
			public const string XSpeed = "XSpeed";
			public const string YSpeed = "YSpeed";
		}

		public PlayerAnimationData()
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
				new Animation(Anim.Idle_Ground, 0.5f, 0, 0),
				new Animation(Anim.Run_Ground, 1, 0, 0),
				new Animation(Anim.Dash_Ground, 1, 0, 0),
				new Animation(Anim.Jump_Ground, 5, 1, 0.2f),
				new Animation(Anim.Jump_Air, 2, 1, 0.2f),
				new Animation(Anim.Droping, 1, 1, 0f),
				new Animation(Anim.Droping_Buffering, 3, 1, 0.3f),
				new Animation(Anim.ATK_Melee_Ground_1, 1f, 1, 0.7f),
				new Animation(Anim.ATK_Melee_Ground_2, 1f, 1, 0.7f),
				new Animation(Anim.ATK_Melee_Ground_3, 1f, 1, 0.7f),
				new Animation(Anim.Hitten_Ground, 0.5f, 1, 0.7f),
			};
		}

		public override void InitalizeStates()
		{
			_toggleState = new List<StrBoolPair>()
			{
				new StrBoolPair() {Key = Stat.Jump, Value = false},
				new StrBoolPair() {Key = Stat.MeleeAttack, Value = false},
				new StrBoolPair() {Key = Stat.Stagger, Value = false},
			};

			_boolState = new List<StrBoolPair>()
			{
				new StrBoolPair() {Key = Stat.Dash, Value = false},
				new StrBoolPair() {Key = Stat.Airborne, Value = false},
				new StrBoolPair() {Key = Stat.Frozen, Value = false},
				new StrBoolPair() {Key = Stat.DelayInput, Value = false},
				new StrBoolPair() {Key = Stat.BlockInput, Value = false},
			};

			_floatState = new List<StrFloatPair>()
			{
				new StrFloatPair() {Key = Stat.XSpeed, Value = 0},
				new StrFloatPair() {Key = Stat.YSpeed, Value = 0},
			};

			_intState = new List<StrIntPair>();
		}

		public override void InitalizeTransitions()
		{
			_transitions = new List<Transition>()
			{

				//// Hitten_Ground
				new Transition(
					Transition.Any, Anim.Hitten_Ground, 0f,
					(sd) => {
						return sd._toggleMap[Stat.Stagger];
					}),
				new Transition(
					Anim.Hitten_Ground, Anim.Idle_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && sd._floatMap[Stat.XSpeed] <= float.Epsilon;
					}),
				new Transition(
					Anim.Hitten_Ground, Anim.Run_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && sd._floatMap[Stat.XSpeed] > float.Epsilon;
					}),
				new Transition(
					Anim.Hitten_Ground, Anim.ATK_Melee_Ground_1, 0f,
					(sd) => {
						return sd._toggleMap[Stat.MeleeAttack];
					}),

				//// Idle_Ground
				new Transition(
					Anim.Idle_Ground, Anim.Jump_Ground, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.Jump] && !sd._boolMap[Stat.Airborne];
					}),

				new Transition(
					Anim.Idle_Ground, Anim.Jump_Ground, 0.1f,
					(sd) => {
						return sd._floatMap[Stat.YSpeed] > float.Epsilon;
					}),

				new Transition(
					Anim.Idle_Ground, Anim.Run_Ground, 0.1f,
					(sd) => {
						return Mathf.Abs(sd._floatMap[Stat.XSpeed]) > float.Epsilon;
					}),

				new Transition(
					Anim.Idle_Ground, Anim.Dash_Ground, 0.05f,
					(sd) => {
						return sd._boolMap[Stat.Dash];
					}),
				new Transition(
					Anim.Idle_Ground, Anim.ATK_Melee_Ground_1, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.MeleeAttack];
					}),
				
				//// Run_Ground
				new Transition(
					Anim.Run_Ground, Anim.Jump_Ground, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.Jump] && !sd._boolMap[Stat.Airborne];
					}),
				new Transition(
					Anim.Run_Ground, Anim.Jump_Ground, 0.1f,
					(sd) => {
						return sd._floatMap[Stat.YSpeed] > float.Epsilon;
					}),
				new Transition(
					Anim.Run_Ground, Anim.Idle_Ground, 0.1f,
					(sd) => {
						return Mathf.Abs(sd._floatMap[Stat.XSpeed]) <= float.Epsilon;
					}),
				new Transition(
					Anim.Run_Ground, Anim.Dash_Ground, 0.05f,
					(sd) => {
						return sd._boolMap[Stat.Dash];
					}),
				new Transition(
					Anim.Run_Ground, Anim.ATK_Melee_Ground_1, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.MeleeAttack];
					}),

				//// Dash_Ground
				new Transition(
					Anim.Dash_Ground, Anim.Droping, 0.1f,
					(sd) => {
						return !sd._boolMap[Stat.Dash] && sd._boolMap[Stat.Airborne];
					}),
				new Transition(
					Anim.Dash_Ground, Anim.Idle_Ground, 0.1f,
					(sd) => {
						return !sd._boolMap[Stat.Dash] && Mathf.Abs(sd._floatMap[Stat.XSpeed]) <= float.Epsilon;
					}),
				new Transition(
					Anim.Dash_Ground, Anim.Run_Ground, 0.1f,
					(sd) => {
						return !sd._boolMap[Stat.Dash] && Mathf.Abs(sd._floatMap[Stat.XSpeed]) > float.Epsilon;
					}),

				//// Jump_Ground
				new Transition(
					Anim.Jump_Ground, Anim.Dash_Ground, 0.05f,
					(sd) => {
						return sd._boolMap[Stat.Dash];
					}),
				new Transition(
					Anim.Jump_Ground, Anim.Droping, 0.1f,
					(sd) => {
						return sd._floatMap[Stat.YSpeed] < 0;
					}),
				
				//// Jump_Air
				new Transition(
					Anim.Jump_Air, Anim.Droping, 0.05f,
					(sd) => {
						return sd._floatMap[Stat.YSpeed] < 0;
					}),
				new Transition(
					Anim.Jump_Air, Anim.Dash_Ground, 0.05f,
					(sd) => {
						return sd._boolMap[Stat.Dash];
					}),

				//// Droping
				new Transition(
					Anim.Droping, Anim.Jump_Air, 0.05f,
					(sd) => {
						return sd._floatMap[Stat.YSpeed] > 0 || sd._toggleMap[Stat.Jump];
					}),
				new Transition(
					Anim.Droping, Anim.Dash_Ground, 0.05f,
					(sd) => {
						return sd._boolMap[Stat.Dash];
					}),
				new Transition(
					Anim.Droping, Anim.Droping_Buffering, 0f,
					(sd) => {
						return !sd._boolMap[Stat.Airborne];
					}),
				
				//// Droping_Buffering
				new Transition(
					Anim.Droping_Buffering, Anim.Jump_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && sd._toggleMap[Stat.Jump];
					}),
				new Transition(
					Anim.Droping_Buffering, Anim.Idle_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && sd._floatMap[Stat.XSpeed] <= float.Epsilon;
					}),
				new Transition(
					Anim.Droping_Buffering, Anim.Run_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && sd._floatMap[Stat.XSpeed] > float.Epsilon;
					}),
				
				//// ATK_Melee_Ground_1
				new Transition(
					Anim.ATK_Melee_Ground_1, Anim.ATK_Melee_Ground_2, 0f,
					(sd) => {
						return sd._toggleMap[Stat.MeleeAttack];
					}),
				new Transition(
					Anim.ATK_Melee_Ground_1, Anim.Jump_Ground, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.Jump];
					}),
				new Transition(
					Anim.ATK_Melee_Ground_1, Anim.Idle_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && Mathf.Abs(sd._floatMap[Stat.XSpeed]) <= float.Epsilon;
					}),
				new Transition(
					Anim.ATK_Melee_Ground_1, Anim.Run_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && Mathf.Abs(sd._floatMap[Stat.XSpeed]) > float.Epsilon;
					}),
				
				//// ATK_Melee_Ground_2
				new Transition(
					Anim.ATK_Melee_Ground_2, Anim.ATK_Melee_Ground_3, 0f,
					(sd) => {
						return sd._toggleMap[Stat.MeleeAttack];
					}),
				new Transition(
					Anim.ATK_Melee_Ground_2, Anim.Jump_Ground, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.Jump];
					}),
				new Transition(
					Anim.ATK_Melee_Ground_2, Anim.Idle_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && Mathf.Abs(sd._floatMap[Stat.XSpeed]) <= float.Epsilon;
					}),
				new Transition(
					Anim.ATK_Melee_Ground_2, Anim.Run_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && Mathf.Abs(sd._floatMap[Stat.XSpeed]) > float.Epsilon;
					}),
				
				//// ATK_Melee_Ground_3
				new Transition(
					Anim.ATK_Melee_Ground_3, Anim.ATK_Melee_Ground_1, 0f,
					(sd) => {
						return sd._toggleMap[Stat.MeleeAttack];
					}),
				new Transition(
					Anim.ATK_Melee_Ground_3, Anim.Jump_Ground, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.Jump];
					}),
				new Transition(
					Anim.ATK_Melee_Ground_3, Anim.Idle_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && Mathf.Abs(sd._floatMap[Stat.XSpeed]) <= float.Epsilon;
					}),
				new Transition(
					Anim.ATK_Melee_Ground_3, Anim.Run_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && Mathf.Abs(sd._floatMap[Stat.XSpeed]) > float.Epsilon;
					}),
			};
		}
	}
}