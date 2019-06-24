using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TripleBladeHorse.Animation
{
	[CreateAssetMenu(menuName = "AnimationFSMData/PlayerFSMData")]
	public class PlayerFSMData : FSMData
	{
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
				new Animation("Idle_Ground", 1, 0),
				new Animation("Run_Ground", 1, 0),
				new Animation("Dash_Ground", 1, 0),
				new Animation("Jump_Ground", 1, 1),
				new Animation("Jump_Air", 1, 1),
				new Animation("Droping", 1, 1),
				new Animation("Droping_Buffering", 1, 1),
				new Animation("ATK_Melee_Ground_1", 1, 1),
				new Animation("ATK_Melee_Ground_2", 1, 1),
				new Animation("ATK_Melee_Ground_3", 1, 1),
			};
		}

		public override void InitalizeStates()
		{
			_boolState = new List<StrBoolPair>()
			{
				new StrBoolPair() {Key = "Jump", Value = false},
				new StrBoolPair() {Key = "Dash", Value = false},
				new StrBoolPair() {Key = "Airborne", Value = false},
				new StrBoolPair() {Key = "Landing", Value = false},
				new StrBoolPair() {Key = "Frozen", Value = false},
				new StrBoolPair() {Key = "MeleeAttack", Value = false},
				new StrBoolPair() {Key = "DelayInput", Value = false},
				new StrBoolPair() {Key = "BlockInput", Value = false},
			};

			_floatState = new List<StrFloatPair>()
			{
				new StrFloatPair() {Key = "XSpeed", Value = 0},
				new StrFloatPair() {Key = "YSpeed", Value = 0},
				new StrFloatPair() {Key = "AttackStepSpeed", Value = 0},
				new StrFloatPair() {Key = "AttackStepDistance", Value = 0},
			};

			_intState = new List<StrIntPair>();
		}

		public override void InitalizeTransitions()
		{
			_transitions = new List<Transition>()
			{
				new Transition(
					"Idle_Ground", "Run_Ground", 0.2f,
					(sd) => {
						return Mathf.Abs(sd._floatMap["XSpeed"]) > float.Epsilon;
					}),
				new Transition(
					"Idle_Ground", "Dash_Ground", 0.05f,
					(sd) => {
						return sd._boolMap["Dash"];
					}),
				new Transition(
					"Idle_Ground", "Jump_Ground", 0.1f,
					(sd) => {
						return sd._boolMap["Jump"] && !sd._boolMap["Airborne"];
					}),
				new Transition(
					"Idle_Ground", "ATK_Melee_Ground_1", 0.1f,
					(sd) => {
						return sd._boolMap["MeleeAttack"];
					}),
				new Transition(
					"Run_Ground", "Idle_Ground", 0.2f,
					(sd) => {
						return Mathf.Abs(sd._floatMap["XSpeed"]) <= float.Epsilon;
					}),
				new Transition(
					"Run_Ground", "Jump_Ground", 0.1f,
					(sd) => {
						return sd._boolMap["Jump"] && !sd._boolMap["Airborne"];
					}),
				new Transition(
					"Run_Ground", "ATK_Melee_Ground_1", 0.1f,
					(sd) => {
						return sd._boolMap["MeleeAttack"];
					}),
				new Transition(
					"Jump_Ground", "Droping", 0.1f,
					(sd) => {
						return sd._floatMap["YSpeed"] <= 0;
					}),
				new Transition(
					"Droping", "Droping_Buffering", 0f,
					(sd) => {
						return sd._boolMap["Landing"];
					}),
				new Transition(
					"ATK_Melee_Ground_1", "Idle_Ground", 0.1f,
					(sd) => {
						return sd._current.completed && Mathf.Abs(sd._floatMap["XSpeed"]) <= float.Epsilon;
					}),
				new Transition(
					"ATK_Melee_Ground_1", "Run_Ground", 0.1f,
					(sd) => {
						return sd._current.completed && Mathf.Abs(sd._floatMap["XSpeed"]) > float.Epsilon;
					}),
			};
		}
	}
}