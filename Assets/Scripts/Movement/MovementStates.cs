using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TripleBladeHorse.Movement
{
	public enum MovingState
	{
		Idle,
		Move,
		AttackStep,
		Airborne,
		Passive,
		Dash,
	}

	public enum LandingState
	{
		OnGround,
		Airborne,
	}
}