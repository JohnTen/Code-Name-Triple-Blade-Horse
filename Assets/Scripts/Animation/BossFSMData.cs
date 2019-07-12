using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse.Animation
{
	[CreateAssetMenu(menuName = "AnimationFSMData/BossFSMData")]
	public class BossFSMData : FSMData
	{
		public class Anim
		{
			public const string Idle = "Boss1_Idle";
			public const string QuickMove = "Boss1_Move_Quick_Forward";
			public const string SlowMove = "Boss1_Move_Slow_Forward";
			public const string Backward = "Boss1_Move_Backward";
			public const string Retreat = "Boss1_ATK_Comb3_1_1";
			public const string Slash1 = "Boss1_ATK_Comb1_2";
			public const string Slash2 = "Boss1_ATK_Comb1_3";
			public const string Combo2_1 = "Boss1_ATK_Comb2_1";
			public const string Combo2_2 = "Boss1_ATK_Comb2_2";
			public const string Combo2_3 = "Boss1_ATK_Comb2_3";
			public const string Combo3_1 = "Boss1_ATK_Comb3_1_2";
			public const string Combo3_2 = "Boss1_ATK_Comb3_2";
			public const string Combo3_3 = "Boss1_ATK_Comb3_3";
			public const string Death = "Boss1_Death";
		}

		public class Stat
		{
			public const string Slash = "Slash";
			public const string Thrust = "Thrust";
			public const string Combo2 = "Combo2";
			public const string Death = "Death";
			public const string Retreat = "Retreat";
			public const string QucikMove = "QuickMove";
			public const string Backward = "Backward";
			public const string Frozen = "Frozen";
			public const string DelayInput = "DelayInput";
			public const string BlockInput = "BlockInput";
			public const string XSpeed = "XSpeed";
		}

		class GeneralFunction
		{
			public static Func<bool, FSMState> Idle =
				(sd) =>
				{
					return Mathf.Abs(sd._floatMap[Stat.XSpeed]) <= float.Epsilon;
				};
			public static Func<bool, FSMState> QucikMove =
				(sd) =>
				{
					return Mathf.Abs(sd._floatMap[Stat.XSpeed]) > float.Epsilon && sd._boolMap[Stat.QucikMove];
				};
			public static Func<bool, FSMState> SlowMove =
				(sd) =>
				{
					return Mathf.Abs(sd._floatMap[Stat.XSpeed]) > float.Epsilon && !sd._boolMap[Stat.QucikMove];
				};
			public static Func<bool, FSMState> Backward =
				(sd) =>
				{
					return Mathf.Abs(sd._floatMap[Stat.XSpeed]) > float.Epsilon && sd._boolMap[Stat.Backward];
				};
		}

		public BossFSMData()
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
				new Animation(Anim.Idle, 1f, 0, 0),
				new Animation(Anim.QuickMove, 1f, 0, 0),
				new Animation(Anim.SlowMove, 1f, 0, 0),
				new Animation(Anim.Backward, 1f, 0, 0),
				new Animation(Anim.Retreat, 1f, 0, 0, true, true, false, false),
				new Animation(Anim.Slash1, 1f, 1, 0, true, true, false, false),
				new Animation(Anim.Slash2, 1f, 1, 0, true, true, false, false),
				new Animation(Anim.Combo2_1, 1f, 1, 0f, true, true, false, false),
				new Animation(Anim.Combo2_2, 1f, 1, 0f, true, true, false, false),
				new Animation(Anim.Combo2_3, 1f, 1, 0f, true, true, false, false),
				new Animation(Anim.Combo3_1, 1f, 1, 0f, true, true, false, false),
				new Animation(Anim.Combo3_2, 1f, 1, 0f, true, true, false, false),
				new Animation(Anim.Combo3_3, 1f, 1, 0f, true, true, false, false),
				new Animation(Anim.Death, 1f, 1, 0f, true, true, true, false),
			};
		}

		public override void InitalizeStates()
		{
			_toggleState = new List<StrBoolPair>()
			{
				new StrBoolPair() {Key = Stat.Slash, Value = false},
				new StrBoolPair() {Key = Stat.Thrust, Value = false},
				new StrBoolPair() {Key = Stat.Combo2, Value = false},
			};

			_boolState = new List<StrBoolPair>()
			{
				new StrBoolPair() {Key = Stat.Death, Value = false},
				new StrBoolPair() {Key = Stat.Retreat, Value = false},
				new StrBoolPair() {Key = Stat.QucikMove, Value = false},
				new StrBoolPair() {Key = Stat.Backward, Value = false},
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
				//// Death
				new Transition(
					Transition.Any, Anim.Death, 0.05f,
					(sd) => {
						return sd._boolMap[Stat.Death] && sd._current.name != Anim.Death;
					}),

				//// Idle
				new Transition(
					Anim.Idle, Anim.Retreat, 0.05f,
					(sd) => {
						return sd._boolMap[Stat.Retreat];
					}),
				new Transition(
					Anim.Idle, Anim.Slash1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Slash];
					}),
				new Transition(
					Anim.Idle, Anim.Combo2_1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Combo2];
					}),
				new Transition(
					Anim.Idle, Anim.Combo3_1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Thrust];
					}),
				new Transition(
					Anim.Idle, Anim.Backward, 0.05f,
					GeneralFunction.Backward),
				new Transition(
					Anim.Idle, Anim.QuickMove, 0.05f,
					GeneralFunction.QucikMove),
				new Transition(
					Anim.Idle, Anim.SlowMove, 0.05f,
					GeneralFunction.SlowMove),

				//// Slow Move
				new Transition(
					Anim.SlowMove, Anim.Retreat, 0.05f,
					(sd) => {
						return sd._boolMap[Stat.Retreat];
					}),
				new Transition(
					Anim.SlowMove, Anim.Slash1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Slash];
					}),
				new Transition(
					Anim.SlowMove, Anim.Combo2_1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Combo2];
					}),
				new Transition(
					Anim.SlowMove, Anim.Combo3_1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Thrust];
					}),
				new Transition(
					Anim.SlowMove, Anim.Backward, 0.05f,
					GeneralFunction.Backward),
				new Transition(
					Anim.SlowMove, Anim.QuickMove, 0.05f,
					GeneralFunction.QucikMove),
				new Transition(
					Anim.SlowMove, Anim.Idle, 0.05f,
					GeneralFunction.Idle),

				//// Quick Move
				new Transition(
					Anim.QuickMove, Anim.Retreat, 0.05f,
					(sd) => {
						return sd._boolMap[Stat.Retreat];
					}),
				new Transition(
					Anim.QuickMove, Anim.Slash1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Slash];
					}),
				new Transition(
					Anim.QuickMove, Anim.Combo2_1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Combo2];
					}),
				new Transition(
					Anim.QuickMove, Anim.Combo3_1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Thrust];
					}),
				new Transition(
					Anim.QuickMove, Anim.Backward, 0.05f,
					GeneralFunction.Backward),
				new Transition(
					Anim.QuickMove, Anim.SlowMove, 0.05f,
					GeneralFunction.SlowMove),
				new Transition(
					Anim.QuickMove, Anim.Idle, 0.05f,
					GeneralFunction.Idle),

				//// Backward
				new Transition(
					Anim.Backward, Anim.Retreat, 0.05f,
					(sd) => {
						return sd._boolMap[Stat.Retreat];
					}),
				new Transition(
					Anim.Backward, Anim.Slash1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Slash];
					}),
				new Transition(
					Anim.Backward, Anim.Combo2_1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Combo2];
					}),
				new Transition(
					Anim.Backward, Anim.Combo3_1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Thrust];
					}),
				new Transition(
					Anim.Backward, Anim.QuickMove, 0.05f,
					(sd) => {
						return !sd._boolMap[Stat.Backward] && GeneralFunction.QucikMove(sd);
					}),
				new Transition(
					Anim.Backward, Anim.SlowMove, 0.05f,
					(sd) => {
						return !sd._boolMap[Stat.Backward] && GeneralFunction.SlowMove(sd);
					}),
				new Transition(
					Anim.Backward, Anim.Idle, 0.05f,
					GeneralFunction.Idle),

				//// Retreat
				new Transition(
					Anim.Retreat, Anim.Slash1, 0.05f,
					(sd) => {
						return !sd._boolMap[Stat.Retreat] && sd._toggleMap[Stat.Slash];
					}),
				new Transition(
					Anim.Retreat, Anim.Combo2_1, 0.05f,
					(sd) => {
						return !sd._boolMap[Stat.Retreat] && sd._toggleMap[Stat.Combo2];
					}),
				new Transition(
					Anim.Retreat, Anim.Combo3_1, 0.05f,
					(sd) => {
						return !sd._boolMap[Stat.Retreat] && sd._toggleMap[Stat.Thrust];
					}),
				new Transition(
					Anim.Retreat, Anim.Backward, 0.05f,
					(sd) => {
						return !sd._boolMap[Stat.Retreat] && GeneralFunction.Backward(sd);
					}),
				new Transition(
					Anim.Retreat, Anim.QuickMove, 0.05f,
					(sd) => {
						return !sd._boolMap[Stat.Retreat] && GeneralFunction.QucikMove(sd);
					}),
				new Transition(
					Anim.Retreat, Anim.SlowMove, 0.05f,
					(sd) => {
						return !sd._boolMap[Stat.Retreat] && GeneralFunction.SlowMove(sd);
					}),
				new Transition(
					Anim.Retreat, Anim.Idle, 0.05f,
					(sd) => {
						return !sd._boolMap[Stat.Retreat] && GeneralFunction.Idle(sd);
					}),
				
				//// Slash1
				new Transition(
					Anim.Slash1, Anim.Retreat, 0.05f,
					(sd) => {
						return sd._boolMap[Stat.Retreat];
					}),
				new Transition(
					Anim.Slash1, Anim.Slash2, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Slash];
					}),
				new Transition(
					Anim.Slash1, Anim.Combo2_1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Combo2];
					}),
				new Transition(
					Anim.Slash1, Anim.Combo3_1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Thrust];
					}),
				new Transition(
					Anim.Slash1, Anim.Backward, 0.05f,
					(sd) => {
						return sd._current.completed && GeneralFunction.Backward(sd);
					}),
				new Transition(
					Anim.Slash1, Anim.QuickMove, 0.05f,
					(sd) => {
						return sd._current.completed && GeneralFunction.QucikMove(sd);
					}),
				new Transition(
					Anim.Slash1, Anim.SlowMove, 0.05f,
					(sd) => {
						return sd._current.completed && GeneralFunction.SlowMove(sd);
					}),
				new Transition(
					Anim.Slash1, Anim.Idle, 0.05f,
					(sd) => {
						return sd._current.completed && GeneralFunction.Idle(sd);
					}),
				
				//// Slash2
				new Transition(
					Anim.Slash2, Anim.Retreat, 0.05f,
					(sd) => {
						return sd._boolMap[Stat.Retreat];
					}),
				new Transition(
					Anim.Slash2, Anim.Slash1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Slash];
					}),
				new Transition(
					Anim.Slash2, Anim.Combo2_1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Combo2];
					}),
				new Transition(
					Anim.Slash2, Anim.Combo3_1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Thrust];
					}),
				new Transition(
					Anim.Slash2, Anim.Backward, 0.05f,
					(sd) => {
						return sd._current.completed && GeneralFunction.Backward(sd);
					}),
				new Transition(
					Anim.Slash2, Anim.QuickMove, 0.05f,
					(sd) => {
						return sd._current.completed && GeneralFunction.QucikMove(sd);
					}),
				new Transition(
					Anim.Slash2, Anim.SlowMove, 0.05f,
					(sd) => {
						return sd._current.completed && GeneralFunction.SlowMove(sd);
					}),
				new Transition(
					Anim.Slash2, Anim.Idle, 0.05f,
					(sd) => {
						return sd._current.completed && GeneralFunction.Idle(sd);
					}),
				
				//// Combo2_1
				new Transition(
					Anim.Combo2_1, Anim.Combo2_2, 0f,
					(sd) => {
						return sd._current.completed;
					}),

				//// Combo2_2
				new Transition(
					Anim.Combo2_2, Anim.Combo2_3, 0f,
					(sd) => {
						return sd._current.completed;
					}),
				
				//// Combo2_3
				new Transition(
					Anim.Combo2_3, Anim.Retreat, 0.05f,
					(sd) => {
						return sd._boolMap[Stat.Retreat];
					}),
				new Transition(
					Anim.Combo2_3, Anim.Slash1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Slash];
					}),
				new Transition(
					Anim.Combo2_3, Anim.Combo2_1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Combo2];
					}),
				new Transition(
					Anim.Combo2_3, Anim.Combo3_1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Thrust];
					}),
				new Transition(
					Anim.Combo2_3, Anim.Backward, 0.05f,
					(sd) => {
						return sd._current.completed && GeneralFunction.Backward(sd);
					}),
				new Transition(
					Anim.Combo2_3, Anim.QuickMove, 0.05f,
					(sd) => {
						return sd._current.completed && GeneralFunction.QucikMove(sd);
					}),
				new Transition(
					Anim.Combo2_3, Anim.SlowMove, 0.05f,
					(sd) => {
						return sd._current.completed && GeneralFunction.SlowMove(sd);
					}),
				new Transition(
					Anim.Combo2_3, Anim.Idle, 0.05f,
					(sd) => {
						return sd._current.completed && GeneralFunction.Idle(sd);
					}),

				//// Combo3_1
				new Transition(
					Anim.Combo3_1, Anim.Combo3_2, 0f,
					(sd) => {
						return sd._current.completed;
					}),

				//// Combo3_2
				new Transition(
					Anim.Combo3_2, Anim.Combo3_3, 0f,
					(sd) => {
						return sd._current.completed;
					}),
				
				//// Combo3_3
				new Transition(
					Anim.Combo3_3, Anim.Retreat, 0.05f,
					(sd) => {
						return sd._boolMap[Stat.Retreat];
					}),
				new Transition(
					Anim.Combo3_3, Anim.Slash1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Slash];
					}),
				new Transition(
					Anim.Combo3_3, Anim.Combo2_1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Combo2];
					}),
				new Transition(
					Anim.Combo3_3, Anim.Combo3_1, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Thrust];
					}),
				new Transition(
					Anim.Combo3_3, Anim.Backward, 0.05f,
					(sd) => {
						return sd._current.completed && GeneralFunction.Backward(sd);
					}),
				new Transition(
					Anim.Combo3_3, Anim.QuickMove, 0.05f,
					(sd) => {
						return sd._current.completed && GeneralFunction.QucikMove(sd);
					}),
				new Transition(
					Anim.Combo3_3, Anim.SlowMove, 0.05f,
					(sd) => {
						return sd._current.completed && GeneralFunction.SlowMove(sd);
					}),
				new Transition(
					Anim.Combo3_3, Anim.Idle, 0.05f,
					(sd) => {
						return sd._current.completed && GeneralFunction.Idle(sd);
					}),
			};
		}
	}
}
