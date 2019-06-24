using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;
using DragonBones;

public class PlayerAnimation : MonoBehaviour
{
	DragonBones.Armature _armature;
	DragonBones.AnimationState _walkState;
	DragonBones.AnimationState _dashState;
	Dictionary<string, bool> _boolValueMap;
	Dictionary<string, float> _floatValueMap;
	Dictionary<string, int> _intValueMap;

	readonly string[] _boolValueKeys = { "Jump", "Dash", "Airborne", "Landing", "Falling", "Frozen", "MeleeAttack", "CanBreak", "DelayInput" };
	readonly string[] _floatValueKeys = { "XSpeed", "YSpeed", "AttackStepSpeed", "AttackStepDistance"};
	
	int _currentAttack = 0;
	string[] _meleeAttacks = { "ATK_Melee_Ground_1", "ATK_Melee_Ground_2", "ATK_Melee_Ground_3" };

	[SerializeField] UnityArmatureComponent _armatureComponent;

	[SerializeField] float attackTimer;

	[SerializeField] CharacterState _state;

	public event Action<string, float> OnRecievedFrameEvent;

	public PlayerAnimation()
	{
		_boolValueMap = new Dictionary<string, bool>();
		_floatValueMap = new Dictionary<string, float>();
		_intValueMap = new Dictionary<string, int>();

		foreach (var key in _boolValueKeys)
		{
			_boolValueMap.Add(key, false);
		}

		foreach (var key in _floatValueKeys)
		{
			_floatValueMap.Add(key, 0);
		}
	}

	public string GetCurrentAnimation()
	{
		return _armature.animation.lastAnimationName;
	}

	public bool GetBool(string key)
	{
		return _boolValueMap[key];
	}

	public float GetFloat(string key)
	{
		return _floatValueMap[key];
	}

	public float GetInt(string key)
	{
		return _intValueMap[key];
	}

	public void SetBool(string key, bool value)
	{
		_boolValueMap[key] = value;
	}

	public void SetFloat(string key, float value)
	{
		_floatValueMap[key] = value;
	}

	public void SetInt(string key, int value)
	{
		_intValueMap[key] = value;
	}


	private void Start()
	{
		_armature = this._armatureComponent.armature;
		_armatureComponent.AddDBEventListener(EventObject.COMPLETE, OnAnimationEventHandler);
		_armatureComponent.AddDBEventListener(EventObject.FRAME_EVENT, OnFrameEventHandler);
	}

	private void Update()
	{
		if (!IsPlayingMeleeAnimation() && attackTimer > 0)
		{
			attackTimer -= Time.deltaTime;
			if (attackTimer < 0)
				_currentAttack = 0;
		}

		UpdateAnimation();
	}

	public void SetSpeed(float x, float y)
	{
		_floatValueMap["XSpeed"] = x;
		_floatValueMap["YSpeed"] = y;
	}

	public void Attack()
	{
		print("Attack");
		_walkState = null;
		var anim = _armature.animation.FadeIn(_meleeAttacks[_currentAttack], 0, 1);
		anim.resetToPose = false;
		anim.timeScale = 2.5f;
		_currentAttack++;
		_currentAttack %= _meleeAttacks.Length;
		_boolValueMap["Frozen"] = true;
	}

	private void OnAnimationEventHandler(string type, EventObject eventObject)
	{
		if (eventObject.animationState.name == "Droping_Buffering")
		{
			_armature.animation.FadeIn("Idle_Ground", 0.2f, 0, 0, null).resetToPose = false;

			_boolValueMap["Frozen"] = false;
			_boolValueMap["Landing"] = false;
		}

		foreach (var attack in _meleeAttacks)
		{
			if (attack != eventObject.animationState.name) continue;
			_boolValueMap["Frozen"] = false;
			attackTimer = 0.3f;
			_armature.animation.FadeIn("Idle_Ground", 0.2f, 0, 0, null).resetToPose = false;
		}
	}

	private void OnFrameEventHandler(string type, EventObject eventObject)
	{
		if (eventObject.name == "DelayInput")
		{
			_boolValueMap["DelayInput"] = true;
			OnRecievedFrameEvent("DelayInput", 1);
		}

		else if (eventObject.name == "ReleaseInput")
		{
			_boolValueMap["DelayInput"] = false;
			OnRecievedFrameEvent("DelayInput", 0);
		}

		else if (eventObject.name == "AttackBegin")
		{
			_boolValueMap["MeleeAttack"] = true;
			OnRecievedFrameEvent("AttackBegin", 1);
		}

		else if (eventObject.name == "DelayInput")
		{
			_boolValueMap["DelayInput"] = true;
			OnRecievedFrameEvent("DelayInput", 0);
		}

		else if (eventObject.name == "AttackEnd")
		{
			_boolValueMap["MeleeAttack"] = false;
			OnRecievedFrameEvent("AttackEnd", 0);
		}

		else if (eventObject.name == "AttackStepDistance")
		{
			_floatValueMap["AttackStepDistance"] = eventObject.data.floats[0];
			OnRecievedFrameEvent("AttackStepDistance", eventObject.data.floats[0]);
		}

		else if (eventObject.name == "AttackStepSpeed")
		{
			_floatValueMap["AttackStepSpeed"] = eventObject.data.floats[0];
			OnRecievedFrameEvent("AttackStepSpeed", eventObject.data.floats[0]);
		}
	}

	private void UpdateAnimation()
	{
		if (_state._facingRight == _armature.flipX)
		{
			_armature.flipX = !_armature.flipX;
		}

		if (_boolValueMap["Dash"])
		{
			_walkState = null;
			if (_dashState == null)
			{
				_dashState = _armature.animation.FadeIn("Dash_Ground", 0.1f, 0);
				_dashState.resetToPose = false;
			}

			return;
		}
		else if (_dashState != null)
		{
			_armature.animation.FadeIn("Idle_Ground", 0, 0);
			_dashState = null;
		}

		if (_boolValueMap["Jump"] && _boolValueMap["Airborne"] == false)
		{
			var anim = _armature.animation.FadeIn("Jump_Ground", 0.1f, 1, 0, null);
			anim.timeScale = 5f;
			anim.resetToPose = false;
			
			_walkState = null;
			_boolValueMap["Jump"] = false;
			_boolValueMap["Airborne"] = true;
			_boolValueMap["Landing"] = false;
			_boolValueMap["Frozen"] = false;

			return;
		}
		_boolValueMap["Jump"] = false;

		if (_boolValueMap["Airborne"])
		{
			if (_boolValueMap["Landing"])
			{
				var anim = _armature.animation.FadeIn("Droping_Buffering", 0f, 1, 0, null);
				anim.timeScale = 3f;
				anim.resetToPose = false;
				_boolValueMap["Airborne"] = false;
				_boolValueMap["Falling"] = false;
				_boolValueMap["Frozen"] = true;
				return;
			}

			if (_floatValueMap["YSpeed"] >= 0)
			{
				if (_boolValueMap["Falling"])
				{
					var anim = _armature.animation.FadeIn("Jump_Air", 0.2f, 1, 0, null);
					anim.timeScale = 2f;
					anim.resetToPose = false;
				}
				_boolValueMap["Falling"] = false;
			}
			else if (!_boolValueMap["Falling"])
			{
				_armature.animation.FadeIn("Droping", 0.2f, 0, 0, null).resetToPose = false;
				_boolValueMap["Falling"] = true;
			}

			return;
		}

		if (_armature.animation.lastAnimationName == "Droping_Buffering") return;

		if (_floatValueMap["XSpeed"] == 0)
		{
			if (_walkState != null)
			{
				_walkState = _armature.animation.FadeIn("Idle_Ground", 0.1f, 0, 0, null);
				_walkState.resetToPose = false;
				_walkState.timeScale = 0.5f;
				_walkState = null;
			}
		}
		else
		{
			if (_walkState == null)
			{
				_walkState = _armature.animation.FadeIn("Run_Ground", 0.1f, 0, 0, null);
				_walkState.resetToPose = false;
			}
		}
	}

	private bool IsPlayingMeleeAnimation()
	{
		foreach (var attack in _meleeAttacks)
		{
			if (attack == _armature.animation.lastAnimationName)
				return true;
		}

		return false;
	}
}
