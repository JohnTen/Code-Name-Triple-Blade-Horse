using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanStickKnife
{
	bool CanStick { get; }
	bool CanPullingJump { get; }
	int RestoredStamina { get; }
	float PullForceFactor { get; }

	bool TryStick(GameObject obj);
	bool TryTakeOut(GameObject obj);
}
