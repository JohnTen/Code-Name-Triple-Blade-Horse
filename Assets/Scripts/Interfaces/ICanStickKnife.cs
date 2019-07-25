﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanStickKnife
{
	bool CanStick { get; }
	bool CanPullingJump { get; }
	int RestoredStamina { get; }
	float PullForceFactor { get; }

	bool TryStick(GameObject obj);
	bool TryPullOut(GameObject obj, ref Vector2 pullingVelocity);
	void Remove(GameObject obj);
}
