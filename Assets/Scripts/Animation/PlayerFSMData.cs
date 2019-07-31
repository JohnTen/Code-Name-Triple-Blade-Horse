using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse.Animation
{
	[CreateAssetMenu(menuName = "AnimationFSMData/PlayerFSMData")]
	public class PlayerFSMData : FSMData
	{
		public class Anim
		{
			public const string Idle_Ground = "Idle_Ground";
			public const string Run_Ground = "Run_Ground";
			public const string Dash_Up = "Dash_Air_Up";
			public const string Dash_Down = "Dash_Air_Down";
			public const string Dash_Horizontal = "Dash_Ground";
			public const string Dash_Diagonal_Up = "Dash_Ground_Slant_Up";
			public const string Dash_Diagonal_Down = "Dash_Ground_Slant_Down";
			public const string Jump_Air = "Jump_Air";
			public const string Jump_Ground = "Jump_Ground";
			public const string Dropping = "Droping";
			public const string Dropping_Buffering = "Droping_Buffering";
			public const string ATK_Melee_Air_1 = "ATK_Melee_Air_1";
			public const string ATK_Melee_Air_2 = "ATK_Melee_Air_2";
			public const string ATK_Melee_Air_3 = "ATK_Melee_Air_3";
			public const string ATK_Melee_Ground_1 = "ATK_Melee_Ground_1";
			public const string ATK_Melee_Ground_2 = "ATK_Melee_Ground_2";
			public const string ATK_Melee_Ground_3 = "ATK_Melee_Ground_3";
			public const string ATK_Charge_Ground_Charging = "ATK_Charge_Ground_Charging";
			public const string ATK_Charge_Ground_ATK = "ATK_Charge_Ground_ATK";
			public const string Stagger_Weak = "Hitten_Front_Ground_Small";
			public const string Stagger_Med = "Hitten_Front_Ground_Normal";
			public const string Stagger_Strong = "Hitten_Front_Ground_Big";
			public const string Healing = "Healing";
			public const string Death = "Death_Ground";
		}

		public class Stat
		{
			public const string Jump = "Jump";
			public const string Healing = "Healing";
			public const string MeleeAttack = "MeleeAttack";
			public const string DashBegin = "DashBegin";
			public const string DashEnd = "DashEnd";
			public const string Death = "Death";
			public const string WeakStagger = "Stagger_Weak";
			public const string MidStagger = "Stagger_Mediocre";
			public const string StrongStagger = "Stagger_Strong";
			public const string Charge = "Charge";
			public const string Airborne = "Airborne";
			public const string Frozen = "Frozen";
			public const string DelayInput = "DelayInput";
			public const string BlockInput = "BlockInput";
			public const string XSpeed = "XSpeed";
			public const string YSpeed = "YSpeed";
		}

		class GeneralFunction
		{
			public static Func<bool, FSMState> Jump = 
				(sd) => 
				{
					return sd._toggleMap[Stat.Jump];
				};

			public static Func<bool, FSMState> GroundCharging =
				(sd) =>
				{
					return sd._boolMap[Stat.Charge] && !sd._boolMap[Stat.Airborne];
				};

			public static Func<bool, FSMState> GroundAttack =
				(sd) =>
				{
					return !sd._boolMap[Stat.Charge] && sd._toggleMap[Stat.MeleeAttack] && !sd._boolMap[Stat.Airborne];
				};

			public static Func<bool, FSMState> AirAttack =
				(sd) =>
				{
					return sd._toggleMap[Stat.MeleeAttack] && sd._boolMap[Stat.Airborne];
				};

			public static Func<bool, FSMState> Dash_Horizontal =
				(sd) =>
				{
					return sd._toggleMap[Stat.DashBegin]
					&& (Mathf.Abs(sd._floatMap[Stat.YSpeed]) <= float.Epsilon
					&& Mathf.Abs(sd._floatMap[Stat.XSpeed]) > float.Epsilon);
				};

			public static Func<bool, FSMState> Dash_Diagonal_Up =
				(sd) =>
				{
					return sd._toggleMap[Stat.DashBegin]
					&& (sd._floatMap[Stat.YSpeed] > float.Epsilon
					&& Mathf.Abs(sd._floatMap[Stat.XSpeed]) > float.Epsilon);
				};

			public static Func<bool, FSMState> Dash_Diagonal_Down =
				(sd) =>
				{
					return sd._toggleMap[Stat.DashBegin]
					&& (sd._floatMap[Stat.YSpeed] < -float.Epsilon
					&& Mathf.Abs(sd._floatMap[Stat.XSpeed]) > float.Epsilon);
				};

			public static Func<bool, FSMState> Dash_Up =
				(sd) =>
				{
					return sd._toggleMap[Stat.DashBegin]
					&& (sd._floatMap[Stat.YSpeed] > float.Epsilon
					&& Mathf.Abs(sd._floatMap[Stat.XSpeed]) <= float.Epsilon);
				};

			public static Func<bool, FSMState> Dash_Down =
				(sd) =>
				{
					return sd._toggleMap[Stat.DashBegin]
					&& (sd._floatMap[Stat.YSpeed] < float.Epsilon
					&& Mathf.Abs(sd._floatMap[Stat.XSpeed]) <= float.Epsilon);
				};
		}

		public PlayerFSMData()
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
				new Animation(Anim.Run_Ground, 1.5f, 0, 0),
				new Animation(Anim.Dash_Horizontal, 1, 0, 0, true, false, false, false),
				new Animation(Anim.Dash_Up, 1, 0, 0, true, false, false, false),
				new Animation(Anim.Dash_Down, 1, 0, 0, true, false, false, false),
				new Animation(Anim.Dash_Diagonal_Up, 1, 0, 0, true, false, false, false),
				new Animation(Anim.Dash_Diagonal_Down, 1, 0, 0, true, false, false, false),
				new Animation(Anim.Jump_Ground, 5, 1, 0.2f),
				new Animation(Anim.Jump_Air, 1, 1, 0.2f),
				new Animation(Anim.Dropping, 1, 1, 0f),
				new Animation(Anim.Dropping_Buffering, 3, 1, 0.3f, true, false, true, false),
				new Animation(Anim.ATK_Melee_Air_1, 1.8f, 1, 0.7f, true, true, false, true),
				new Animation(Anim.ATK_Melee_Air_2, 1.8f, 1, 0.7f, true, true, false, true),
				new Animation(Anim.ATK_Melee_Air_3, 1.8f, 1, 0.7f, true, true, false, true),
				new Animation(Anim.ATK_Melee_Ground_1, 1.8f, 1, 0.7f, true, true, false, false),
				new Animation(Anim.ATK_Melee_Ground_2, 1.8f, 1, 0.7f, true, true, false, false),
				new Animation(Anim.ATK_Melee_Ground_3, 1.8f, 1, 0.7f, true, true, false, false),
				new Animation(Anim.ATK_Charge_Ground_Charging, 1f, 1, 0.7f, true, false, false, false),
				new Animation(Anim.ATK_Charge_Ground_ATK, 1f, 1, 0.7f, true, true, false, false),
				new Animation(Anim.Stagger_Weak, 1f, 1, 0.7f, true, false, true, false),
				new Animation(Anim.Stagger_Med, 1f, 1, 0.7f, true, false, true, false),
				new Animation(Anim.Stagger_Strong, 1f, 1, 0.7f, true, false, true, false),
				new Animation(Anim.Healing, 1f, 1, 0.7f, true, false, true, false),
				new Animation(Anim.Death, 1f, 1, 0.7f, true, false, true, false),
			};
		}

		public override void InitalizeStates()
		{
			_toggleState = new List<StrBoolPair>()
			{
				new StrBoolPair() {Key = Stat.Jump, Value = false},
				new StrBoolPair() {Key = Stat.Healing, Value = false},
				new StrBoolPair() {Key = Stat.DashBegin, Value = false},
				new StrBoolPair() {Key = Stat.DashEnd, Value = false},
				new StrBoolPair() {Key = Stat.MeleeAttack, Value = false},
				new StrBoolPair() {Key = Stat.Death, Value = false},
				new StrBoolPair() {Key = Stat.StrongStagger, Value = false},
				new StrBoolPair() {Key = Stat.MidStagger, Value = false},
				new StrBoolPair() {Key = Stat.WeakStagger, Value = false},
			};

			_boolState = new List<StrBoolPair>()
			{
				new StrBoolPair() {Key = Stat.Charge, Value = false},
				new StrBoolPair() {Key = Stat.Airborne, Value = true},
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
				//// Death
				new Transition(
					Transition.Any, Anim.Death, 0.1f,
					(sd) =>
					{
						return sd._toggleMap[Stat.Death];
					}),

				//// Stagger_Strong
				new Transition(
					Transition.Any, Anim.Stagger_Strong, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.StrongStagger];
					}),
				new Transition(
					Anim.Stagger_Strong, Anim.Idle_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && sd._floatMap[Stat.XSpeed] <= float.Epsilon;
					}),
				new Transition(
					Anim.Stagger_Strong, Anim.Run_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && sd._floatMap[Stat.XSpeed] > float.Epsilon;
					}),
				new Transition(
					Anim.Stagger_Strong, Anim.Jump_Ground, 0.1f,
					GeneralFunction.Jump),
				new Transition(
					Anim.Stagger_Strong, Anim.ATK_Melee_Ground_1, 0.05f,
					GeneralFunction.GroundAttack),
				new Transition(
					Anim.Stagger_Strong, Anim.ATK_Charge_Ground_Charging, 0.05f,
					GeneralFunction.GroundCharging),
				new Transition(
					Anim.Stagger_Strong, Anim.Dash_Up, 0.05f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.Stagger_Strong, Anim.Dash_Horizontal, 0.05f,
					GeneralFunction.Dash_Horizontal),
				
				//// Stagger_Mediocre
				
				new Transition(
					Transition.Any, Anim.Stagger_Med, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.MidStagger] && sd._current.name != Anim.Stagger_Strong;
					}),
				new Transition(
					Anim.Stagger_Med, Anim.Idle_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && sd._floatMap[Stat.XSpeed] <= float.Epsilon;
					}),
				new Transition(
					Anim.Stagger_Med, Anim.Run_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && sd._floatMap[Stat.XSpeed] > float.Epsilon;
					}),
				new Transition(
					Anim.Stagger_Med, Anim.Jump_Ground, 0.1f,
					GeneralFunction.Jump),
				new Transition(
					Anim.Stagger_Med, Anim.ATK_Melee_Ground_1, 0.05f,
					GeneralFunction.GroundAttack),
				new Transition(
					Anim.Stagger_Med, Anim.ATK_Charge_Ground_Charging, 0.05f,
					GeneralFunction.GroundCharging),
				new Transition(
					Anim.Stagger_Med, Anim.Dash_Up, 0.05f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.Stagger_Med, Anim.Dash_Horizontal, 0.05f,
					GeneralFunction.Dash_Horizontal),
				
				//// Stagger_Weak
				new Transition(
					Transition.Any, Anim.Stagger_Weak, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.WeakStagger] && (sd._current.name != Anim.Stagger_Med || sd._current.name != Anim.Stagger_Strong);
					}),
				new Transition(
					Anim.Stagger_Weak, Anim.Idle_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && sd._floatMap[Stat.XSpeed] <= float.Epsilon;
					}),
				new Transition(
					Anim.Stagger_Weak, Anim.Run_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && sd._floatMap[Stat.XSpeed] > float.Epsilon;
					}),
				new Transition(
					Anim.Stagger_Weak, Anim.Jump_Ground, 0.1f,
					GeneralFunction.Jump),
				new Transition(
					Anim.Stagger_Weak, Anim.ATK_Melee_Ground_1, 0.05f,
					GeneralFunction.GroundAttack),
				new Transition(
					Anim.Stagger_Weak, Anim.ATK_Charge_Ground_Charging, 0.05f,
					GeneralFunction.GroundCharging),
				new Transition(
					Anim.Stagger_Weak, Anim.Dash_Up, 0.05f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.Stagger_Weak, Anim.Dash_Horizontal, 0.05f,
					GeneralFunction.Dash_Horizontal),

				//// Idle_Ground
				new Transition(
					Anim.Idle_Ground, Anim.Dash_Up, 0.05f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.Idle_Ground, Anim.Dash_Down, 0.05f,
					GeneralFunction.Dash_Down),
				new Transition(
					Anim.Idle_Ground, Anim.Dash_Diagonal_Up, 0.05f,
					GeneralFunction.Dash_Diagonal_Up),
				new Transition(
					Anim.Idle_Ground, Anim.Dash_Diagonal_Down, 0.05f,
					GeneralFunction.Dash_Diagonal_Down),
				new Transition(
					Anim.Idle_Ground, Anim.Dash_Horizontal, 0.05f,
					GeneralFunction.Dash_Horizontal),

				new Transition(
					Anim.Idle_Ground, Anim.Jump_Ground, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.Jump] && !sd._boolMap[Stat.Airborne];
					}),

				new Transition(
					Anim.Idle_Ground, Anim.Dropping, 0.1f,
					(sd) => {
						return sd._floatMap[Stat.YSpeed] < 0f && sd._boolMap[Stat.Airborne];
					}),

				new Transition(
					Anim.Idle_Ground, Anim.Run_Ground, 0.1f,
					(sd) => {
						return Mathf.Abs(sd._floatMap[Stat.XSpeed]) > float.Epsilon;
					}),

				new Transition(
					Anim.Idle_Ground, Anim.Jump_Ground, 0.1f,
					(sd) => {
						return sd._floatMap[Stat.YSpeed] > float.Epsilon;
					}),

				new Transition(
					Anim.Idle_Ground, Anim.ATK_Melee_Ground_1, 0.1f,
					GeneralFunction.GroundAttack),

				new Transition(
					Anim.Idle_Ground, Anim.ATK_Charge_Ground_Charging, 0.05f,
					GeneralFunction.GroundCharging),

				new Transition(
					Anim.Idle_Ground, Anim.Healing, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.Healing];
					}),
				
				//// Run_Ground
				new Transition(
					Anim.Run_Ground, Anim.Dash_Up, 0.05f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.Run_Ground, Anim.Dash_Down, 0.05f,
					GeneralFunction.Dash_Down),
				new Transition(
					Anim.Run_Ground, Anim.Dash_Diagonal_Up, 0.05f,
					GeneralFunction.Dash_Diagonal_Up),
				new Transition(
					Anim.Run_Ground, Anim.Dash_Diagonal_Down, 0.05f,
					GeneralFunction.Dash_Diagonal_Down),
				new Transition(
					Anim.Run_Ground, Anim.Dash_Horizontal, 0.05f,
					GeneralFunction.Dash_Horizontal),

				new Transition(
					Anim.Run_Ground, Anim.Jump_Ground, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.Jump];
					}),
				new Transition(
					Anim.Run_Ground, Anim.Dropping, 0.1f,
					(sd) => {
						return sd._floatMap[Stat.YSpeed] < 0.1f && sd._boolMap[Stat.Airborne];
					}),
				new Transition(
					Anim.Run_Ground, Anim.Jump_Ground, 0.1f,
					(sd) => {
						return sd._floatMap[Stat.YSpeed] > float.Epsilon;
					}),

				new Transition(
					Anim.Run_Ground, Anim.ATK_Melee_Ground_1, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.MeleeAttack];
					}),

				new Transition(
					Anim.Run_Ground, Anim.ATK_Charge_Ground_Charging, 0.05f,
					GeneralFunction.GroundCharging),

				new Transition(
					Anim.Run_Ground, Anim.Healing, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.Healing];
					}),
				new Transition(
					Anim.Run_Ground, Anim.Idle_Ground, 0.1f,
					(sd) => {
						return Mathf.Abs(sd._floatMap[Stat.XSpeed]) <= float.Epsilon;
					}),

				//// Jump_Ground
				new Transition(
					Anim.Jump_Ground, Anim.Dash_Up, 0.05f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.Jump_Ground, Anim.Dash_Down, 0.05f,
					GeneralFunction.Dash_Down),
				new Transition(
					Anim.Jump_Ground, Anim.Dash_Diagonal_Up, 0.05f,
					GeneralFunction.Dash_Diagonal_Up),
				new Transition(
					Anim.Jump_Ground, Anim.Dash_Diagonal_Down, 0.05f,
					GeneralFunction.Dash_Diagonal_Down),
				new Transition(
					Anim.Jump_Ground, Anim.Dash_Horizontal, 0.05f,
					GeneralFunction.Dash_Horizontal),
				new Transition(
					Anim.Jump_Ground, Anim.ATK_Melee_Air_1, 0.1f,
					GeneralFunction.AirAttack),
				new Transition(
					Anim.Jump_Ground, Anim.Jump_Air, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Jump];
					}),
				new Transition(
					Anim.Jump_Ground, Anim.Dropping, 0.1f,
					(sd) => {
						return sd._floatMap[Stat.YSpeed] < 0;
					}),
				new Transition(
					Anim.Jump_Ground, Anim.Dropping_Buffering, 0.1f,
					(sd) => {
						return !sd._boolMap[Stat.Airborne] && sd._floatMap[Stat.YSpeed] <= float.Epsilon;
					}),
				
				//// Jump_Air
				new Transition(
					Anim.Jump_Air, Anim.Dash_Up, 0.05f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.Jump_Air, Anim.Dash_Down, 0.05f,
					GeneralFunction.Dash_Down),
				new Transition(
					Anim.Jump_Air, Anim.Dash_Diagonal_Up, 0.05f,
					GeneralFunction.Dash_Diagonal_Up),
				new Transition(
					Anim.Jump_Air, Anim.Dash_Diagonal_Down, 0.05f,
					GeneralFunction.Dash_Diagonal_Down),
				new Transition(
					Anim.Jump_Air, Anim.Dash_Horizontal, 0.05f,
					GeneralFunction.Dash_Horizontal),
				new Transition(
					Anim.Jump_Air, Anim.ATK_Melee_Air_1, 0.1f,
					GeneralFunction.AirAttack),
				new Transition(
					Anim.Jump_Air, Anim.Dropping, 0.05f,
					(sd) => {
						return sd._current.completed;
					}),
				new Transition(
					Anim.Jump_Air, Anim.Dropping_Buffering, 0.1f,
					(sd) => {
						return !sd._boolMap[Stat.Airborne];
					}),

				//// Droping
				new Transition(
					Anim.Dropping, Anim.Dash_Up, 0.05f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.Dropping, Anim.Dash_Down, 0.05f,
					GeneralFunction.Dash_Down),
				new Transition(
					Anim.Dropping, Anim.Dash_Diagonal_Up, 0.05f,
					GeneralFunction.Dash_Diagonal_Up),
				new Transition(
					Anim.Dropping, Anim.Dash_Diagonal_Down, 0.05f,
					GeneralFunction.Dash_Diagonal_Down),
				new Transition(
					Anim.Dropping, Anim.Dash_Horizontal, 0.05f,
					GeneralFunction.Dash_Horizontal),
				new Transition(
					Anim.Dropping, Anim.ATK_Melee_Air_1, 0.1f,
					GeneralFunction.AirAttack),
				new Transition(
					Anim.Dropping, Anim.Jump_Air, 0.05f,
					(sd) => {
						return sd._toggleMap[Stat.Jump];
					}),
				new Transition(
					Anim.Dropping, Anim.Dropping_Buffering, 0f,
					(sd) => {
						return !sd._boolMap[Stat.Airborne];
					}),
				
				//// Droping_Buffering
				new Transition(
					Anim.Dropping_Buffering, Anim.Dash_Up, 0.05f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.Dropping_Buffering, Anim.Dash_Down, 0.05f,
					GeneralFunction.Dash_Down),
				new Transition(
					Anim.Dropping_Buffering, Anim.Dash_Diagonal_Up, 0.05f,
					GeneralFunction.Dash_Diagonal_Up),
				new Transition(
					Anim.Dropping_Buffering, Anim.Dash_Diagonal_Down, 0.05f,
					GeneralFunction.Dash_Diagonal_Down),
				new Transition(
					Anim.Dropping_Buffering, Anim.Dash_Horizontal, 0.05f,
					GeneralFunction.Dash_Horizontal),
				new Transition(
					Anim.Dropping_Buffering, Anim.Jump_Ground, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.Jump];
					}),
				new Transition(
					Anim.Dropping_Buffering, Anim.Idle_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && sd._floatMap[Stat.XSpeed] <= float.Epsilon;
					}),
				new Transition(
					Anim.Dropping_Buffering, Anim.Run_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && sd._floatMap[Stat.XSpeed] > float.Epsilon;
					}),
				new Transition(
					Anim.Dropping_Buffering, Anim.ATK_Melee_Ground_1, 0.1f,
					GeneralFunction.GroundAttack),

				new Transition(
					Anim.Dropping_Buffering, Anim.ATK_Charge_Ground_Charging, 0.05f,
					GeneralFunction.GroundCharging),

				new Transition(
					Anim.Dropping_Buffering, Anim.Healing, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.Healing];
					}),
				
				//// Dash_Horizontal
				new Transition(
					Anim.Dash_Horizontal, Anim.Dash_Up, 0f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.Dash_Horizontal, Anim.Dash_Down, 0f,
					GeneralFunction.Dash_Down),
				new Transition(
					Anim.Dash_Horizontal, Anim.Dash_Diagonal_Up, 0f,
					GeneralFunction.Dash_Diagonal_Up),
				new Transition(
					Anim.Dash_Horizontal, Anim.Dash_Diagonal_Down, 0f,
					GeneralFunction.Dash_Diagonal_Down),
				new Transition(
					Anim.Dash_Horizontal, Anim.Dash_Horizontal, 0f,
					GeneralFunction.Dash_Horizontal),
				new Transition(
					Anim.Dash_Horizontal, Anim.ATK_Melee_Ground_1, 0.1f,
					GeneralFunction.GroundAttack),

				new Transition(
					Anim.Dash_Horizontal, Anim.ATK_Charge_Ground_Charging, 0.05f,
					GeneralFunction.GroundCharging),
				new Transition(
					Anim.Dash_Horizontal, Anim.ATK_Melee_Air_1, 0.1f,
					GeneralFunction.AirAttack),
				new Transition(
					Anim.Dash_Horizontal, Anim.Jump_Ground, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.Jump] && !sd._boolMap[Stat.Airborne];
					}),
				new Transition(
					Anim.Dash_Horizontal, Anim.Jump_Air, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.Jump] && sd._boolMap[Stat.Airborne];
					}),
				new Transition(
					Anim.Dash_Horizontal, Anim.Dropping, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.DashEnd] && sd._boolMap[Stat.Airborne];
					}),
				new Transition(
					Anim.Dash_Horizontal, Anim.Idle_Ground, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.DashEnd] && Mathf.Abs(sd._floatMap[Stat.XSpeed]) <= float.Epsilon;
					}),
				new Transition(
					Anim.Dash_Horizontal, Anim.Run_Ground, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.DashEnd] && Mathf.Abs(sd._floatMap[Stat.XSpeed]) > float.Epsilon;
					}),
				
				//// Dash_Up
				new Transition(
					Anim.Dash_Up, Anim.Dash_Up, 0f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.Dash_Up, Anim.Dash_Down, 0f,
					GeneralFunction.Dash_Down),
				new Transition(
					Anim.Dash_Up, Anim.Dash_Diagonal_Up, 0f,
					GeneralFunction.Dash_Diagonal_Up),
				new Transition(
					Anim.Dash_Up, Anim.Dash_Diagonal_Down, 0f,
					GeneralFunction.Dash_Diagonal_Down),
				new Transition(
					Anim.Dash_Up, Anim.Dash_Horizontal, 0f,
					GeneralFunction.Dash_Horizontal),
				new Transition(
					Anim.Dash_Up, Anim.Dropping, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.DashEnd] && sd._boolMap[Stat.Airborne];
					}),
				new Transition(
					Anim.Dash_Up, Anim.Idle_Ground, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.DashEnd] && Mathf.Abs(sd._floatMap[Stat.XSpeed]) <= float.Epsilon;
					}),
				new Transition(
					Anim.Dash_Up, Anim.Run_Ground, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.DashEnd] && Mathf.Abs(sd._floatMap[Stat.XSpeed]) > float.Epsilon;
					}),
				
				//// Dash_Down
				new Transition(
					Anim.Dash_Down, Anim.Dash_Up, 0f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.Dash_Down, Anim.Dash_Down, 0f,
					GeneralFunction.Dash_Down),
				new Transition(
					Anim.Dash_Down, Anim.Dash_Diagonal_Up, 0f,
					GeneralFunction.Dash_Diagonal_Up),
				new Transition(
					Anim.Dash_Down, Anim.Dash_Diagonal_Down, 0f,
					GeneralFunction.Dash_Diagonal_Down),
				new Transition(
					Anim.Dash_Down, Anim.Dash_Horizontal, 0f,
					GeneralFunction.Dash_Horizontal),
				new Transition(
					Anim.Dash_Down, Anim.Dropping, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.DashEnd] && sd._boolMap[Stat.Airborne];
					}),
				new Transition(
					Anim.Dash_Down, Anim.Idle_Ground, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.DashEnd] && Mathf.Abs(sd._floatMap[Stat.XSpeed]) <= float.Epsilon;
					}),
				new Transition(
					Anim.Dash_Down, Anim.Run_Ground, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.DashEnd] && Mathf.Abs(sd._floatMap[Stat.XSpeed]) > float.Epsilon;
					}),
				
				//// Dash_Diagonal_Up
				new Transition(
					Anim.Dash_Diagonal_Up, Anim.Dash_Up, 0f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.Dash_Diagonal_Up, Anim.Dash_Down, 0f,
					GeneralFunction.Dash_Down),
				new Transition(
					Anim.Dash_Diagonal_Up, Anim.Dash_Diagonal_Up, 0f,
					GeneralFunction.Dash_Diagonal_Up),
				new Transition(
					Anim.Dash_Diagonal_Up, Anim.Dash_Diagonal_Down, 0f,
					GeneralFunction.Dash_Diagonal_Down),
				new Transition(
					Anim.Dash_Diagonal_Up, Anim.Dash_Horizontal, 0f,
					GeneralFunction.Dash_Horizontal),
				new Transition(
					Anim.Dash_Diagonal_Up, Anim.Dropping, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.DashEnd] && sd._boolMap[Stat.Airborne];
					}),
				new Transition(
					Anim.Dash_Diagonal_Up, Anim.Idle_Ground, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.DashEnd] && Mathf.Abs(sd._floatMap[Stat.XSpeed]) <= float.Epsilon;
					}),
				new Transition(
					Anim.Dash_Diagonal_Up, Anim.Run_Ground, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.DashEnd] && Mathf.Abs(sd._floatMap[Stat.XSpeed]) > float.Epsilon;
					}),
				
				//// Dash_Diagonal_Down
				new Transition(
					Anim.Dash_Diagonal_Down, Anim.Dash_Up, 0f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.Dash_Diagonal_Down, Anim.Dash_Down, 0f,
					GeneralFunction.Dash_Down),
				new Transition(
					Anim.Dash_Diagonal_Down, Anim.Dash_Diagonal_Up, 0f,
					GeneralFunction.Dash_Diagonal_Up),
				new Transition(
					Anim.Dash_Diagonal_Down, Anim.Dash_Diagonal_Down, 0f,
					GeneralFunction.Dash_Diagonal_Down),
				new Transition(
					Anim.Dash_Diagonal_Down, Anim.Dash_Horizontal, 0f,
					GeneralFunction.Dash_Horizontal),
				new Transition(
					Anim.Dash_Diagonal_Down, Anim.Dropping, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.DashEnd] && sd._boolMap[Stat.Airborne];
					}),
				new Transition(
					Anim.Dash_Diagonal_Down, Anim.Idle_Ground, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.DashEnd] && Mathf.Abs(sd._floatMap[Stat.XSpeed]) <= float.Epsilon;
					}),
				new Transition(
					Anim.Dash_Diagonal_Down, Anim.Run_Ground, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.DashEnd] && Mathf.Abs(sd._floatMap[Stat.XSpeed]) > float.Epsilon;
					}),

				//// ATK_Charge_Ground_Charging
				new Transition(
					Anim.ATK_Charge_Ground_Charging, Anim.Dash_Up, 0.05f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.ATK_Charge_Ground_Charging, Anim.Dash_Down, 0.05f,
					GeneralFunction.Dash_Down),
				new Transition(
					Anim.ATK_Charge_Ground_Charging, Anim.Dash_Diagonal_Up, 0.05f,
					GeneralFunction.Dash_Diagonal_Up),
				new Transition(
					Anim.ATK_Charge_Ground_Charging, Anim.Dash_Diagonal_Down, 0.05f,
					GeneralFunction.Dash_Diagonal_Down),
				new Transition(
					Anim.ATK_Charge_Ground_Charging, Anim.Dash_Horizontal, 0.05f,
					GeneralFunction.Dash_Horizontal),
				new Transition(
					Anim.ATK_Charge_Ground_Charging, Anim.Jump_Ground, 0.05f,
					GeneralFunction.Jump),
				new Transition(
					Anim.ATK_Charge_Ground_Charging, Anim.Idle_Ground, 0.1f,
					(sd) => {
						return 
						!sd._toggleMap[Stat.MeleeAttack] &&
						!sd._boolMap[Stat.Charge];
					}),
				new Transition(
					Anim.ATK_Charge_Ground_Charging, Anim.ATK_Charge_Ground_ATK, 0.05f,
					(sd) => {
						return 
						sd._toggleMap[Stat.MeleeAttack] && 
						!sd._boolMap[Stat.Airborne] && 
						sd._boolMap[Stat.Charge];
					}),

				new Transition(
					Anim.ATK_Charge_Ground_Charging, Anim.Healing, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.Healing];
					}),

				//// ATK_Charge_Ground_ATK
				new Transition(
					Anim.ATK_Charge_Ground_ATK, Anim.Dash_Up, 0.05f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.ATK_Charge_Ground_ATK, Anim.Dash_Down, 0.05f,
					GeneralFunction.Dash_Down),
				new Transition(
					Anim.ATK_Charge_Ground_ATK, Anim.Dash_Diagonal_Up, 0.05f,
					GeneralFunction.Dash_Diagonal_Up),
				new Transition(
					Anim.ATK_Charge_Ground_ATK, Anim.Dash_Diagonal_Down, 0.05f,
					GeneralFunction.Dash_Diagonal_Down),
				new Transition(
					Anim.ATK_Charge_Ground_ATK, Anim.Dash_Horizontal, 0.05f,
					GeneralFunction.Dash_Horizontal),
				new Transition(
					Anim.ATK_Charge_Ground_ATK, Anim.Jump_Ground, 0.05f,
					GeneralFunction.Jump),
				new Transition(
					Anim.ATK_Charge_Ground_ATK, Anim.ATK_Melee_Ground_1, 0f,
					GeneralFunction.GroundAttack),
				new Transition(
					Anim.ATK_Charge_Ground_ATK, Anim.Healing, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.Healing];
					}),
				new Transition(
					Anim.ATK_Charge_Ground_ATK, Anim.Idle_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && Mathf.Abs(sd._floatMap[Stat.XSpeed]) <= float.Epsilon;
					}),
				new Transition(
					Anim.ATK_Charge_Ground_ATK, Anim.Run_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && Mathf.Abs(sd._floatMap[Stat.XSpeed]) > float.Epsilon;
					}),


				//// ATK_Melee_Air_1
				new Transition(
					Anim.ATK_Melee_Air_1, Anim.Dash_Up, 0.05f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.ATK_Melee_Air_1, Anim.Dash_Down, 0.05f,
					GeneralFunction.Dash_Down),
				new Transition(
					Anim.ATK_Melee_Air_1, Anim.Dash_Diagonal_Up, 0.05f,
					GeneralFunction.Dash_Diagonal_Up),
				new Transition(
					Anim.ATK_Melee_Air_1, Anim.Dash_Diagonal_Down, 0.05f,
					GeneralFunction.Dash_Diagonal_Down),
				new Transition(
					Anim.ATK_Melee_Air_1, Anim.Dash_Horizontal, 0.05f,
					GeneralFunction.Dash_Horizontal),
				new Transition(
					Anim.ATK_Melee_Air_1, Anim.Dropping_Buffering, 0.05f,
					(sd) => {
						return !sd._boolMap[Stat.Airborne] && !sd._boolMap[Stat.DelayInput];
					}),
				new Transition(
					Anim.ATK_Melee_Air_1, Anim.ATK_Melee_Air_2, 0.05f,
					GeneralFunction.AirAttack),
				new Transition(
					Anim.ATK_Melee_Air_1, Anim.Dropping, 0.1f,
					(sd) => {
						return sd._current.completed;
					}),
				new Transition(
					Anim.ATK_Melee_Air_1, Anim.Jump_Air, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.Jump];
					}),

				//// ATK_Melee_Air_2
				new Transition(
					Anim.ATK_Melee_Air_2, Anim.Dash_Up, 0.05f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.ATK_Melee_Air_2, Anim.Dash_Down, 0.05f,
					GeneralFunction.Dash_Down),
				new Transition(
					Anim.ATK_Melee_Air_2, Anim.Dash_Diagonal_Up, 0.05f,
					GeneralFunction.Dash_Diagonal_Up),
				new Transition(
					Anim.ATK_Melee_Air_2, Anim.Dash_Diagonal_Down, 0.05f,
					GeneralFunction.Dash_Diagonal_Down),
				new Transition(
					Anim.ATK_Melee_Air_2, Anim.Dash_Horizontal, 0.05f,
					GeneralFunction.Dash_Horizontal),
				new Transition(
					Anim.ATK_Melee_Air_2, Anim.Dropping_Buffering, 0.05f,
					(sd) => {
						return !sd._boolMap[Stat.Airborne] && !sd._boolMap[Stat.DelayInput];
					}),
				new Transition(
					Anim.ATK_Melee_Air_2, Anim.ATK_Melee_Air_3, 0.05f,
					GeneralFunction.AirAttack),
				new Transition(
					Anim.ATK_Melee_Air_2, Anim.Dropping, 0.1f,
					(sd) => {
						return sd._current.completed;
					}),
				new Transition(
					Anim.ATK_Melee_Air_2, Anim.Jump_Air, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.Jump];
					}),

				//// ATK_Melee_Air_3
				new Transition(
					Anim.ATK_Melee_Air_3, Anim.Dash_Up, 0.05f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.ATK_Melee_Air_3, Anim.Dash_Down, 0.05f,
					GeneralFunction.Dash_Down),
				new Transition(
					Anim.ATK_Melee_Air_3, Anim.Dash_Diagonal_Up, 0.05f,
					GeneralFunction.Dash_Diagonal_Up),
				new Transition(
					Anim.ATK_Melee_Air_3, Anim.Dash_Diagonal_Down, 0.05f,
					GeneralFunction.Dash_Diagonal_Down),
				new Transition(
					Anim.ATK_Melee_Air_3, Anim.Dash_Horizontal, 0.05f,
					GeneralFunction.Dash_Horizontal),
				new Transition(
					Anim.ATK_Melee_Air_3, Anim.Dropping_Buffering, 0.05f,
					(sd) => {
						return !sd._boolMap[Stat.Airborne] && !sd._boolMap[Stat.DelayInput];
					}),
				new Transition(
					Anim.ATK_Melee_Air_3, Anim.ATK_Melee_Air_1, 0.05f,
					GeneralFunction.AirAttack),
				new Transition(
					Anim.ATK_Melee_Air_3, Anim.Dropping, 0.1f,
					(sd) => {
						return sd._current.completed;
					}),
				new Transition(
					Anim.ATK_Melee_Air_3, Anim.Jump_Air, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.Jump];
					}),
				
				//// ATK_Melee_Ground_1
				new Transition(
					Anim.ATK_Melee_Ground_1, Anim.ATK_Melee_Ground_2, 0.05f,
					GeneralFunction.GroundAttack),
				new Transition(
					Anim.ATK_Melee_Ground_1, Anim.Dash_Up, 0.05f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.ATK_Melee_Ground_1, Anim.Dash_Down, 0.05f,
					GeneralFunction.Dash_Down),
				new Transition(
					Anim.ATK_Melee_Ground_1, Anim.Dash_Diagonal_Up, 0.05f,
					GeneralFunction.Dash_Diagonal_Up),
				new Transition(
					Anim.ATK_Melee_Ground_1, Anim.Dash_Diagonal_Down, 0.05f,
					GeneralFunction.Dash_Diagonal_Down),
				new Transition(
					Anim.ATK_Melee_Ground_1, Anim.Dash_Horizontal, 0.05f,
					GeneralFunction.Dash_Horizontal),
				new Transition(
					Anim.ATK_Melee_Ground_1, Anim.Jump_Ground, 0.1f,
					GeneralFunction.Jump),
				new Transition(
					Anim.ATK_Melee_Ground_1, Anim.ATK_Charge_Ground_Charging, 0.05f,
					GeneralFunction.GroundCharging),
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
				new Transition(
					Anim.ATK_Melee_Ground_1, Anim.Healing, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.Healing];
					}),
				
				//// ATK_Melee_Ground_2
				
				new Transition(
					Anim.ATK_Melee_Ground_2, Anim.ATK_Melee_Ground_3, 0.05f,
					GeneralFunction.GroundAttack),
				new Transition(
					Anim.ATK_Melee_Ground_2, Anim.Jump_Ground, 0.1f,
					GeneralFunction.Jump),
				new Transition(
					Anim.ATK_Melee_Ground_2, Anim.Dash_Up, 0.05f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.ATK_Melee_Ground_2, Anim.Dash_Down, 0.05f,
					GeneralFunction.Dash_Down),
				new Transition(
					Anim.ATK_Melee_Ground_2, Anim.Dash_Diagonal_Up, 0.05f,
					GeneralFunction.Dash_Diagonal_Up),
				new Transition(
					Anim.ATK_Melee_Ground_2, Anim.Dash_Diagonal_Down, 0.05f,
					GeneralFunction.Dash_Diagonal_Down),
				new Transition(
					Anim.ATK_Melee_Ground_2, Anim.Dash_Horizontal, 0.05f,
					GeneralFunction.Dash_Horizontal),
				new Transition(
					Anim.ATK_Melee_Ground_2, Anim.ATK_Charge_Ground_Charging, 0.05f,
					GeneralFunction.GroundCharging),
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
				new Transition(
					Anim.ATK_Melee_Ground_2, Anim.Healing, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.Healing];
					}),
				
				//// ATK_Melee_Ground_3
				new Transition(
					Anim.ATK_Melee_Ground_3, Anim.ATK_Melee_Ground_1, 0.05f,
					GeneralFunction.GroundAttack),
				new Transition(
					Anim.ATK_Melee_Ground_3, Anim.Jump_Ground, 0.1f,
					GeneralFunction.Jump),
				new Transition(
					Anim.ATK_Melee_Ground_3, Anim.Dash_Up, 0.05f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.ATK_Melee_Ground_3, Anim.Dash_Down, 0.05f,
					GeneralFunction.Dash_Down),
				new Transition(
					Anim.ATK_Melee_Ground_3, Anim.Dash_Diagonal_Up, 0.05f,
					GeneralFunction.Dash_Diagonal_Up),
				new Transition(
					Anim.ATK_Melee_Ground_3, Anim.Dash_Diagonal_Down, 0.05f,
					GeneralFunction.Dash_Diagonal_Down),
				new Transition(
					Anim.ATK_Melee_Ground_3, Anim.Dash_Horizontal, 0.05f,
					GeneralFunction.Dash_Horizontal),
				new Transition(
					Anim.ATK_Melee_Ground_3, Anim.ATK_Charge_Ground_Charging, 0.05f,
					GeneralFunction.GroundCharging),
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
				new Transition(
					Anim.ATK_Melee_Ground_3, Anim.Healing, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.Healing];
					}),

				//// Healing
				
				new Transition(
					Anim.Healing, Anim.Dash_Up, 0.05f,
					GeneralFunction.Dash_Up),
				new Transition(
					Anim.Healing, Anim.Dash_Down, 0.05f,
					GeneralFunction.Dash_Down),
				new Transition(
					Anim.Healing, Anim.Dash_Diagonal_Up, 0.05f,
					GeneralFunction.Dash_Diagonal_Up),
				new Transition(
					Anim.Healing, Anim.Dash_Diagonal_Down, 0.05f,
					GeneralFunction.Dash_Diagonal_Down),
				new Transition(
					Anim.Healing, Anim.Dash_Horizontal, 0.05f,
					GeneralFunction.Dash_Horizontal),

				new Transition(
					Anim.Healing, Anim.Jump_Ground, 0.1f,
					(sd) => {
						return sd._toggleMap[Stat.Jump] && !sd._boolMap[Stat.Airborne];
					}),

				new Transition(
					Anim.Healing, Anim.Idle_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && Mathf.Abs(sd._floatMap[Stat.XSpeed]) <= float.Epsilon;
					}),

				new Transition(
					Anim.Healing, Anim.Run_Ground, 0.1f,
					(sd) => {
						return sd._current.completed && Mathf.Abs(sd._floatMap[Stat.XSpeed]) > float.Epsilon;
					}),

				new Transition(
					Anim.Healing, Anim.Jump_Ground, 0.1f,
					(sd) => {
						return sd._floatMap[Stat.YSpeed] > float.Epsilon;
					}),

				new Transition(
					Anim.Healing, Anim.ATK_Melee_Ground_1, 0.1f,
					GeneralFunction.GroundAttack),

				new Transition(
					Anim.Healing, Anim.ATK_Charge_Ground_Charging, 0.05f,
					GeneralFunction.GroundCharging),
			};
		}
	}
}