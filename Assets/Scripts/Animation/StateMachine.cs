﻿using System.Collections.Generic;
using UnityEngine;
using DragonBones;
using JTUtility;

namespace TripleBladeHorse.Animation
{
	public enum AnimationState
	{
		FadingIn,
		FadeInComplete,
		FadingOut,
		FadeOutComplete,
		Start,
		Completed,
	}

	public class AnimationEventArg : System.EventArgs
	{
		public AnimationState _state;
		public Animation _animation;

		public AnimationEventArg() { }
		public AnimationEventArg(AnimationState state, Animation animation)
		{
			_state = state;
			_animation = animation;
		}
	}

	public class FrameEventEventArg : System.EventArgs
	{
		public string _name;
		public float _floatData;
		public bool _boolData;
		public int _intData;
		public Animation _animation;


		public FrameEventEventArg() { }
		public FrameEventEventArg(string name, float floatData, bool boolData, int intData, Animation animation)
		{
			_name = name;
			_floatData = floatData;
			_boolData = boolData;
			_intData = intData;
			_animation = animation;
		}
	}

	public class StateMachine : MonoBehaviour
	{
		#region Fields
		[SerializeField] FSMData _data;
		[SerializeField] UnityArmatureComponent _armature;

		private FSMState stateData;
		private List<Transition> transitions;
		private List<Animation> animationDatas;
		#endregion

		#region Properties
		public bool FlipX
		{
			get => _armature.armature.flipX;
			set => _armature.armature.flipX = value;
		}
		#endregion

		#region Events
		public event Action<AnimationEventArg> OnAnimationStateChange;
		public event Action<FrameEventEventArg> OnReceiveFrameEvent;
		#endregion

		#region Unity Messages
		private void Start()
		{
			Initialization();
			_armature.AddDBEventListener(EventObject.FADE_IN, HandleAnimationEvent);
			_armature.AddDBEventListener(EventObject.FADE_IN_COMPLETE, HandleAnimationEvent);
			_armature.AddDBEventListener(EventObject.FADE_OUT, HandleAnimationEvent);
			_armature.AddDBEventListener(EventObject.FADE_OUT_COMPLETE, HandleAnimationEvent);
			_armature.AddDBEventListener(EventObject.START, HandleAnimationEvent);
			_armature.AddDBEventListener(EventObject.COMPLETE, HandleAnimationEvent);
			_armature.AddDBEventListener(EventObject.FRAME_EVENT, HandleFrameEvent);
		}

		private void Update()
		{
			UpdateAnimation();
			ResetToggleState();
		}
		#endregion

		#region Public Methods
		public Animation GetCurrentAnimation()
		{
			return stateData._current;
		}

		public bool GetBool(string stateName)
		{
			return stateData._boolMap[stateName];
		}

		public float GetFloat(string stateName)
		{
			return stateData._floatMap[stateName];
		}

		public int GetInt(string stateName)
		{
			return stateData._intMap[stateName];
		}

		public bool GetToggle(string stateName)
		{
			return stateData._toggleMap[stateName];
		}

		public void SetBool(string stateName, bool state)
		{
			stateData._boolMap[stateName] = state;
		}

		public void SetFloat(string stateName, float state)
		{
			stateData._floatMap[stateName] = state;
		}

		public void SetInt(string stateName, int state)
		{
			stateData._intMap[stateName] = state;
		}

		public void SetToggle(string stateName, bool state)
		{
			stateData._toggleMap[stateName] = state;
		}
		#endregion

		#region Private/Protected Methods
		private void Initialization()
		{
			_data.InitalizeStates();
			_data.InitalizeAnimations();
			_data.InitalizeTransitions();
			animationDatas = new List<Animation>(_data._animationDatas);
			transitions = new List<Transition>(_data._transitions);

			stateData = new FSMState();
			foreach (var pair in _data._floatState)
			{
				stateData._floatMap.Add(pair);
			}

			foreach (var pair in _data._boolState)
			{
				stateData._boolMap.Add(pair);
			}

			foreach (var pair in _data._intState)
			{
				stateData._intMap.Add(pair);
			}

			foreach (var pair in _data._toggleState)
			{
				stateData._toggleMap.Add(pair);
			}

			UpdateCurrentAnimation();
		}

		private void UpdateAnimation()
		{
			var transition = GetTransition(stateData._current.name);
			if (transition != null)
			{
				print("nextAnim: " + transition.nextAnim);
				var animation = GetAnimation(transition.nextAnim);

				var anim = _armature.animation.FadeIn(
					transition.nextAnim,
					transition.transitionTime,
					animation.playTimes);

				anim.timeScale = animation.timeScale;
				anim.resetToPose = false;
			}
		}

		private void ResetToggleState()
		{
			var keys = new List<string>(stateData._toggleMap.Keys);
			foreach (var key in keys)
			{
				stateData._toggleMap[key] = false;
			}
		}

		private Animation GetAnimation(string animName)
		{
			foreach (var animationData in animationDatas)
			{
				if (animationData.name == animName)
				{
					return animationData;
				}
			}
			return new Animation();
		}

		private Transition GetTransition(string currentAnimaName)
		{
			foreach (var transition in transitions)
			{
				if (transition.Test(currentAnimaName, stateData))
				{
					return transition;
				}
			}
			return null;
		}

		private void UpdateCurrentAnimation()
		{
			if (stateData._current.name == _armature.animation.lastAnimationName)
				return;

			stateData._current = GetAnimation(_armature.animation.lastAnimationName);
		}

		private void HandleAnimationEvent(string type, EventObject eventObject)
		{
			if (type == EventObject.FADE_OUT)
			{
				stateData._previous = GetAnimation(eventObject.animationState._animationData.name);
				stateData._previous.fadingOut = true;
				stateData._previous.fadeOutComplete = false;
				stateData._previous.playing = false;
				stateData._previous.fadingIn = false;

				OnAnimationStateChange?.Invoke(new AnimationEventArg(AnimationState.FadingOut, stateData._previous));
			}
			else if (type == EventObject.FADE_IN)
			{
				stateData._current = GetAnimation(eventObject.animationState._animationData.name);
				stateData._current.fadingIn = true;
				stateData._current.fadeInComplete = false;
				stateData._current.playing = false;
				stateData._current.completed = false;
				OnAnimationStateChange?.Invoke(new AnimationEventArg(AnimationState.FadingIn, stateData._current));
			}
			else if (type == EventObject.FADE_OUT_COMPLETE)
			{
				stateData._previous.fadingOut = false;
				stateData._previous.fadeOutComplete = true;
				OnAnimationStateChange?.Invoke(new AnimationEventArg(AnimationState.FadeOutComplete, stateData._previous));
			}
			else if (type == EventObject.FADE_IN_COMPLETE)
			{
				stateData._current.fadeInComplete = true;
				stateData._current.fadingIn = false;
				OnAnimationStateChange?.Invoke(new AnimationEventArg(AnimationState.FadeInComplete, stateData._current));
			}
			else if (type == EventObject.START)
			{
				stateData._current.playing = true;
				OnAnimationStateChange?.Invoke(new AnimationEventArg(AnimationState.Start, stateData._current));
			}
			else if (type == EventObject.COMPLETE)
			{
				stateData._current.completed = true;
				stateData._current.playing = false;
				OnAnimationStateChange?.Invoke(new AnimationEventArg(AnimationState.Completed, stateData._current));
			}
		}

		private void HandleFrameEvent(string type, EventObject eventObject)
		{
			foreach (var pair in stateData._boolMap)
			{
				if (eventObject.name == pair.Key)
				{
					stateData._boolMap[pair.Key] = eventObject.data.GetInt(0) != 0;
					return;
				}
			}
			foreach (var pair in _data._intState)
			{
				if (eventObject.name == pair.Key)
				{
					stateData._intMap[pair.Key] = eventObject.data.GetInt(0);
					return;
				}
			}
			foreach (var pair in _data._floatState)
			{
				if (eventObject.name == pair.Key)
				{
					stateData._floatMap[pair.Key] = eventObject.data.GetFloat(0);
					return;
				}
			}

			var eventArgs = new FrameEventEventArg();
			eventArgs._name = eventObject.name;
			eventArgs._animation = stateData._current;

			if (eventObject.data != null)
			{
				eventArgs._floatData = eventObject.data.GetFloat(0);
				eventArgs._boolData = eventObject.data.GetInt(0) != 0;
				eventArgs._intData = eventObject.data.GetInt(0);
			}

			OnReceiveFrameEvent?.Invoke(eventArgs);
		}
		#endregion
	}
}