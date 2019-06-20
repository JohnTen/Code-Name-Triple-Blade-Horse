using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;
using System;

public class EnemyAnimation : MonoBehaviour
{
	[SerializeField] CharacterState _state;
	[SerializeField] UnityArmatureComponent _armatureComponent;

	DragonBones.Armature _armature;
	DragonBones.AnimationState _walkState;

	public bool _frozen;
	public Vector2 _speed;

	public void Attack()
	{
		print("Attack");
		_walkState = null;
		var anim = _armature.animation.FadeIn("ATK_Ground", 0.1f, 1);
		anim.resetToPose = false;
		anim.timeScale = 1f;
		_frozen = true;
	}

	void UpdateAnimation()
	{
		if (_state._facingRight != _armature.flipX)
		{
			_armature.flipX = !_armature.flipX;
		}

		if (_speed.x == 0)
		{
			if (_walkState != null)
			{
				_walkState = _armature.animation.FadeIn("Idle_Ground", 0.1f, 0, 0, null);
				_walkState.resetToPose = false;
				_walkState = null;
			}
		}
		else
		{
			if (_walkState == null)
			{
				_walkState = _armature.animation.FadeIn("Walk_Ground", 0.1f, 0, 0, null);
				_walkState.resetToPose = false;
			}
		}
	}

	private void Start()
	{
		_armature = this._armatureComponent.armature;
		_armatureComponent.AddDBEventListener(EventObject.COMPLETE, OnAnimationEventHandler);
	}

	private void OnAnimationEventHandler(string type, EventObject eventObject)
	{
		if (eventObject.animationState.name == "ATK_Ground")
		{
			_frozen = false;
			_armature.animation.FadeIn("Idle_Ground", 0.1f, 0, 0, null);
		}
	}

	private void Update()
	{
		UpdateAnimation();
	}
}
